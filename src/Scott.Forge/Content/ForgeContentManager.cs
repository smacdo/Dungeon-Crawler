/*
 * Copyright 2012-2017 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Scott.Forge.Content
{
    /// <summary>
    ///  The content manager is responsible for the loading and unloading of game resources.
    /// </summary>
    /// <remarks>
    ///  The Forge content manager is responsible for loading game assets from storage and returning them as usable
    ///  runtime objects. It is capable of loading both Forge content classes and legacy XNA .XNB objects.
    ///  
    ///  Assets are referenced by their full name and path relative to the content folder. Note that XNA requires
    ///  callers to remove the file extension, this manager does not. An example of this would be an image that is
    ///  stored on disk as "Path\\To\\Your\\Game\\Content\\Foo\\bar.txt". The expected asset path is "Foo\\bar.txt".
    ///
    ///  Object loading is controlled using content readers. When constructing a content manager, you must provide it
    ///  with one or more sets of custom content readers. At run time the manaer will query this set of readers to
    ///  find which one is capable of handling a given file extension for the requested object type. The manager will
    ///  instantiate a copy of this reader, load it into memory and return it to the caller.
    ///  
    ///  Each content manager can be configured with one or more content containers. These containers represent
    ///  storage locations such as a game's content directory, or a zip file holding game data. The content manager
    ///  will query each content container until it finds one that can return the requested asset file name.
    ///  
    ///  A game can add multiple content containers to allow for DLC and modding support. Because the loader queries
    ///  each container in the order it was added, a game can configure the last container to be the game data that
    ///  the game shipped with, and the other containers "override" content in the first. Or you can use the packs
    ///  to split up content into multiple locations.
    ///
    ///  TODO: Add ability to load and unload groups of items.
    ///  TODO: Add telemetry to track how many objects are loaded, how often, how much time is spent in loading and
    ///        the average time to load an asset.
    /// </remarks>
    public class ForgeContentManager : IContentManager
    {
        private bool _disposed = false;
        private Dictionary<string, object> _cache = new Dictionary<string, object>();

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="contentContainers">List of content containers to load assets from.</param>
        /// <param name="contentHandlerDirectories">List of content handler directories.</param>
        public ForgeContentManager(
            IList<IContentContainer> contentContainers,
            IList<IContentHandlerDirectory> contentHandlerDirectories)
        {
            if (contentContainers == null)
            {
                throw new ArgumentNullException(nameof(contentContainers));
            }

            ContentContainers = contentContainers;

            if (contentHandlerDirectories == null)
            {
                throw new ArgumentNullException(nameof(contentHandlerDirectories));
            }

            ContentHandlerDirectories = contentHandlerDirectories;
        }

        /// <summary>
        ///  Get the list of content containers used by this content manager.
        /// </summary>  
        public IList<IContentContainer> ContentContainers { get; private set; }

        /// <summary>
        ///  Get the list of content handler directories used by this content manager.
        /// </summary>
        public IList<IContentHandlerDirectory> ContentHandlerDirectories { get; private set; }

        /// <summary>
        ///  Get or set legacy XNA content manager. DEPRECATED!
        /// </summary>
        /// <remarks>
        ///  Avoid using this property if possible because XNA support will eventually be removed from the system.
        /// </remarks>
        public Microsoft.Xna.Framework.Content.ContentManager XnaContentManager { get; set; }

        /// <summary>
        ///  Load an asset by path name.
        /// </summary>
        /// <remarks>
        ///  All content items should be addressed by their relative path from the content directory. For example,
        ///  if content items are located at "C:\Game\Content\Images\Cat.png" then the path is "Images\Cat.png".
        ///  
        ///  If the item has been loaded once already a cached copy will be returned instead.
        /// </remarks>
        /// <typeparam name="TContent">Runtime asset class type.</typeparam>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>An instance of the loaded asset.</returns>
        public async Task<TContent> Load<TContent>(string assetPath)
        {
            // Do not load after the content manager is disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException("this");
            }

            // Make sure asset path was provided.
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new InvalidAssetNameException(assetPath);
            }

            // Use consistent directory separator characters.
            assetPath = NormalizeFilePath(assetPath);

            // If the asset was already loaded and stored in the cache, skip the loading and just return whats in the
            // cache.
            if (_cache.ContainsKey(assetPath))
            {
                // Check that the cached object type is the same as the requested type. If not, then there are two
                // different asset types with the same name in the cache.
                var cachedObject = _cache[assetPath];

                if (cachedObject.GetType() != typeof(TContent))
                {
                    throw new AssetWrongTypeException(assetPath, typeof(TContent), cachedObject.GetType());
                }

                return (TContent)cachedObject;
            }

            // Check type of file being loaded. XNB files have a special codepath to take, all other content items are
            // loaded using this class.
            var asset = default(TContent);

            if (IsXnbFile(assetPath))
            {
                if (XnaContentManager == null)
                {
                    throw new XnbLoadingSupportMissingException(assetPath);
                }

                // TODO: Test if we can load XNA assets on a background thread and if so adjust code to use
                //       async library. Also might want to add an option to disable this if needed.
                asset = XnaContentManager.Load<TContent>(GetAssetPathWithoutExtension(assetPath));
            }
            else
            {
                asset = await LoadForgeAsset<TContent>(assetPath);
            }

            // Place the object in the cache before returning it to the caller.
            _cache[assetPath] = asset;
            return asset;
        }

        /// <summary>
        ///  Load an asset using the Forge content container and reader system.
        /// </summary>
        /// <remarks>
        ///  This method will query the content manager for a content reader capable of reading the asset's file
        ///  extension, and the content containers for a stream to the asset path. Once both are located the stream
        ///  is passed to the converter and the resultant object is returned to the caller.
        /// </remarks>
        /// <typeparam name="TContent">The asset type to load.</typeparam>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>Instance of the asset.</returns>
        private async Task<TContent> LoadForgeAsset<TContent>(string assetPath)
        {
            // Get the correct content reader for the asset.
            IContentReader<TContent> contentReader = null;
            var fileExtension = Path.GetExtension(assetPath);

            foreach (var directory in ContentHandlerDirectories)
            {
                if (directory.TryGetContentReaderFor(fileExtension, ref contentReader))
                {
                    break;
                }
            }

            // Ensure a content reader was located.
            if (contentReader == null)
            {
                throw new ContentReaderNotFoundException(fileExtension);
            }

            // Instantiate a content reader, get a stream for the asset on disk and then return the converted
            // object.
            var asset = default(TContent);

            using (var stream = await OpenStream(assetPath))
            {
                asset = await contentReader.Read(stream, assetPath, this);
            }

            return asset;
        }

        /// <summary>
        ///  Returns an open stream for reading the given asset name.
        /// </summary>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>Open stream for reading the asset.</returns>
        private async Task<Stream> OpenStream(string assetPath)
        {
            // Search content containers for the first container that has the asset, and return a stream to the item.
            foreach (var container in ContentContainers)
            {
                Stream readStream;

                if (await container.TryReadItem(assetPath, out readStream))
                {
                    return readStream;
                }
            }

            // None of the content containers had the item listed so throw an exception.
            throw new AssetNotFoundException(assetPath);
        }

        /// <summary>
        ///  Check if an asset is loaded.
        /// </summary>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>True if the asset is loaded and false otherwise.</returns>
        public bool IsLoaded(string assetPath)
        {
            // Do not unload after the content manager is disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException("this");
            }

            return _cache.ContainsKey(assetPath);
        }

        /// <summary>
        ///  Unload all assets loaded by this content manager.
        /// </summary>
        public void Unload()
        {
            // Do not unload after the content manager is disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException("this");
            }

            // Unload each object from the cache.
            var allKeys = new List<string>(_cache.Keys);

            foreach (var k in allKeys)
            {
                Unload(k);
            }
        }

        /// <summary>
        ///  Unload an asset by path name.
        /// </summary>
        /// <remarks>
        ///  Unload will remove an asset from the asset cache, and if the object implements IDisposable it will
        ///  dispose of this object. Any live reference to the object will remain but be will become usuable.
        /// </remarks>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>True if the object was unloaded, false if it was never loaded.</returns>
        public bool Unload(string assetPath)
        {
            // Do not unload after the content manager is disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException("this");
            }

            // Try retrieving the object from the asset cache.
            object asset = null;
            var exists = _cache.TryGetValue(assetPath, out asset);

            if (exists)
            {
                // Dipose the item prior to removal if it supports IDisposable.
                var disposable = asset as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }

                // Remove entry from cache.
                _cache.Remove(assetPath);
            }

            return exists;
        }

        /// <summary>
        ///  Dispose this content manager and all of its assets.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Dispose of assets loaded by this content manager.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (XnaContentManager != null)
                    {
                        XnaContentManager.Dispose();
                        XnaContentManager = null;
                    }
                }

                // NOTE: Dispose unmanaged resources here.
                // This content manager returns loaded assets to the caller without caching so there are no assets
                // to dispose of here.
                ContentContainers = null;
                ContentHandlerDirectories = null;

                // Only dispose once.
                _disposed = true;
            }
        }

        /// <summary>
        ///  Check if the asset filepath is a valid precompiled .xnb asset.
        /// </summary>
        /// <param name="assetPath">Path to the asset.</param>
        /// <returns>True if it is a precompiled xnb asset, false otherwise.</returns>
        internal static bool IsXnbFile(string assetPath)
        {
            return assetPath?.EndsWith(".xnb") ?? false;
        }

        /// <summary>
        ///  Get the asset path without the asset extension.
        /// </summary>
        /// <param name="assetPath">Asset path relative to content root.</param>
        /// <returns>Asset path without file extension.</returns>
        internal static string GetAssetPathWithoutExtension(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return assetPath;
            }

            var directoryName = Path.GetDirectoryName(assetPath);
            var baseName = Path.GetFileNameWithoutExtension(assetPath);

            if (!string.IsNullOrEmpty(directoryName))
            {
                // TODO: Use platform specific directory separator.
                return directoryName + "\\" + baseName;
            }
            else
            {
                return baseName;
            }
        }

        /// <summary>
        ///  Normalize asset name so it is always the same name across all platforms.
        /// </summary>
        /// <param name="assetPath">Path to the asset relative to the content folder.</param>
        /// <returns>Normalized asset name</returns>
        internal static string NormalizeFilePath(string assetPath)
        {
            // TODO: Use platform specific directory separator.
            return assetPath?.Replace('/', '\\');
        }
    }
}
