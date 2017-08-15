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

namespace Scott.Forge.Content
{
    // TODO: This class is mostly built around XNA content manager which mixed the concepts of loading from disk
    // (Which is primarily what this class does), and asset lifetime management with unloading. Redesign this class
    // to the central content loader (that calls out to content readers) and refactor CachedContentManager to be
    // the "front door" content manager.
    // TODO: Also take advantage of the refactor to add async await support to the game.
    // TODO: Once the refactor is complete write unit tests. Don't bother before then.

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
    ///  TODO: Add async await. Consider using threaded resource model (where resources track state internally).
    ///  TODO: Add ability to load and unload groups of items.
    ///  TODO: Add telemetry to track how many objects are loaded, how often, how much time is spent in loading and
    ///        the average time to load an asset.
    /// </remarks>
    public class ForgeContentManager : IContentManager
    {
        private bool mDisposed = false;

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
            if (!mDisposed)
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
                ContentContainers.Clear();
                ContentContainers = null;

                ContentHandlerDirectories.Clear();
                ContentHandlerDirectories = null;

                // Only dispose once.
                mDisposed = true;
            }
        }

        /// <summary>
        ///  Load an asset by path name.
        /// </summary>
        /// <remarks>
        ///  All content items should be addressed by their relative path from the content directory. For example,
        ///  if content items are located at "C:\Game\Content\Images\Cat.png" then the path is "Images\Cat.png".
        /// </remarks>
        /// <typeparam name="T">Runtime asset class type.</typeparam>
        /// <param name="assetPath">Relative path of the asset.</param>
        /// <returns>An instance of the loaded asset.</returns>
        public T Load<T>(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new InvalidAssetNameException(assetPath);
            }

            // Use consistent directory separator characters.
            assetPath = NormalizeFilePath(assetPath);

            // Check type of file being loaded. XNB files have a special codepath to take, all other content items are
            // loaded using this class.
            if (IsXnbFile(assetPath))
            {
                if (XnaContentManager == null)
                {
                    throw new InvalidOperationException("XNA content manager required to load .XNB asset");
                }

                return XnaContentManager.Load<T>(GetAssetPathWithoutExtension(assetPath));
            }
            else
            {
                return LoadForgeContent<T>(assetPath);
            }
        }

        /// <summary>
        ///  Load an asset using a content reader.
        /// </summary>
        /// <remarks>
        ///  This will use the content reader systm to read an asset from a stream and turn it into the requested
        ///  runtime form. It can use built-in readers for runtime types such as Sprite or Texture, or it can (if
        ///  configured) use readers from the content pipeline to read raw uncooked assets.
        /// </remarks>
        /// <typeparam name="T">The asset type to load.</typeparam>
        /// <param name="assetInfo">Asset file name to load.</param>
        /// <returns>Instance of the asset.</returns>
        private T LoadForgeContent<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new InvalidAssetNameException(assetName);
            }

            // Get the correct content reader for the asset.
            IContentReader<T> contentReader = null;
            var fileExtension = Path.GetExtension(assetName);

            foreach (var directory in ContentHandlerDirectories)
            {
                if (directory.TryGetContentReaderFor<T>(fileExtension, ref contentReader))
                {
                    break;
                }
            }

            // Use the content reader system to convert the asset into its runtime form.
            var asset = default(T);

            using (var stream = OpenStream(assetName))
            {
                asset = contentReader.Read(stream, assetName, this);
            }

            return asset;
        }
        
        /// <summary>
        ///  Returns an open stream for reading the given asset name.
        /// </summary>
        /// <remarks>
        ///  This is called both by the underlying class implementation (for .XNB files) and by the LoadForgeContent
        ///  method. It will search the content containers and return a stream from the first valid container holding
        ///  the asset.
        /// </remarks>
        /// <param name="assetName">Name of the asset to open.</param>
        /// <returns>Open stream for reading the asset.</returns>
        protected Stream OpenStream(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new ArgumentNullException(nameof(assetName));
            }

            // Search content containers for the first container that has the asset, and return a stream to the item.
            Stream readStream = null;

            foreach (var container in ContentContainers)
            {
                if (container.TryReadItem(assetName, ref readStream))
                {
                    return readStream;
                }
            }

            // None of the content containers had the item listed so throw an exception.
            throw new AssetNotFoundException(assetName);
        }

        /// <summary>
        ///  Check if the asset filepath is a valid precompiled .xnb asset.
        /// </summary>
        /// <param name="assetPath">Path to the asset.</param>
        /// <returns>True if it is a precompiled xnb asset, false otherwise.</returns>
        private static bool IsXnbFile(string assetPath)
        {
            return assetPath.EndsWith(".xnb");
        }

        /// <summary>
        ///  Get the asset path without the asset extension.
        /// </summary>
        /// <param name="assetPath">Asset path relative to content root.</param>
        /// <returns>Asset path without file extension.</returns>
        private static string GetAssetPathWithoutExtension(string assetPath)
        {
            return
                Path.GetDirectoryName(assetPath) +
                Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(assetPath);
        }

        /// <summary>
        ///  Normalize asset name so it is always the same name across all platforms.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <returns>Normalized asset name</returns>
        private static string NormalizeFilePath( string assetName )
        {
            return assetName.Replace( '\\', '/' );
        }
    }
}
