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
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Xna.Framework.Content;
using Scott.Forge.Content;

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  Custom XNA content manager for the Forge game engine. This content manager implementation adds support for
    ///  compressed zip archives ("bundles"), asset preloading, name based references and the ability to use raw asset
    ///  files instead of compiled .xnb blobs.
    /// </summary>
    /// <remarks>
    ///  Content objects are loaded when first requested, and then cached in memory. This means that every request to
    ///  load for a named asset will return the same instance and care should be taken not to modify this object once
    ///  loaded.
    /// 
    ///  TODO: Switch from ICSharpCode zip library to the built in zip library (if possible).
    ///  TODO: Ensure that the content reader system is compatable with XNA.
    ///  TODO: Add async / threaded loading support.
    ///  TODO: Add ability to load and unload groups of items.
    ///  TODO: Use weak references to allow items to expire from the cache (if desired).
    /// </remarks>
    public class ForgeContentManager : ContentManager, IContentManager
    {
        /// <summary>
        ///  A list of content readers that are capable of converting raw assets into usable game resources objects.
        /// </summary>
        private static List<ContentReaderInfo> mContentReaders = new List<ContentReaderInfo>();

        /// <summary>
        /// A flag that is set to true when the content directory is scanned for asset entries. Callers can either
        /// prescan at start up, or when the first content item is requested.
        /// </summary>
        private bool mContentDirectoryScanned = false;

        /// <summary>
        ///  The directory path to the game's content directory, relative to the path containing the game's binary.
        /// </summary>
        private string mContentDir = String.Empty;

        /// <summary>
        ///  True if the instance has been disposed, false otherwise.
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        ///  Holds a list of open zip handles required for reading resources.
        /// </summary>  
        private static List<ZipFile> mBundleArchives = new List<ZipFile>();

        /// <summary>
        ///  Table of loaded assets indexed by asset name. Acts as a cache so only one instance of each item is loaded
        ///  in memory.
        /// </summary>
        private Dictionary<string, System.Object> mAssetCache = new Dictionary<string, Object>();

        /// <summary>
        ///  This static constructor will scan the game assembly for classes with a content reader attribute, and
        ///  register them in a static table.
        /// </summary>
        static ForgeContentManager()
        {
            InitContentReaderList();
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="provider">The service provider that should be used to locate services.</param>
        /// <param name="rootDirectory">The directory path that contains content.</param>
        public ForgeContentManager(IServiceProvider provider, string rootDirectory)
            : base(provider, rootDirectory)
        {
            Debug.Assert(mContentReaders != null);

            if (rootDirectory == null)
            {
                throw new ArgumentNullException(nameof(rootDirectory));
            }

            mContentDir = rootDirectory;
        }

        /// <summary>
        ///  Manually unload content when the content manager is disposed.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if (!mDisposed)
            {
                Unload();
                mDisposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Load an asset.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="assetPath">Name of the asset.</param>
        /// <returns>Instance of the loaded asset.</returns>
        public override T Load<T>(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new ArgumentException("Asset name is null or empty", nameof(assetPath));
            }

            assetPath = NormalizeFilePath(assetPath);
            
            // Check the asset cache if this item was already loaded. If so then return a reference to the cached
            // item rather than loading it again.
            System.Object cachedObject = null;

            if (mAssetCache.TryGetValue(assetPath, out cachedObject))
            {
                return (T) cachedObject;
            }

            // Check the type of content file being loaded. XNB and raw content files both have special code paths to execute.
            // TODO: Switch to stream loading for XNB so they can be in bundles.
            // TODO: Add native content types in addition to raw content files.
            var assetContentDirPath = Path.Combine(mContentDir, assetPath);
            var instance = default(T);

            if (IsXnbFile(assetContentDirPath))
            {
                // Remove content directory from asset path.

                var contentBaseDir = mContentDir + "\\";
                if (assetContentDirPath.StartsWith(contentBaseDir))
                {
                    assetContentDirPath = assetContentDirPath.Substring(
                        contentBaseDir.Length,
                        assetContentDirPath.Length - contentBaseDir.Length);
                }

                // Remove .xnb from asset name.
                var xnbExtension = ".xnb";
                assetContentDirPath = assetContentDirPath.Substring(
                    0,
                    assetContentDirPath.Length - xnbExtension.Length);

                instance = base.Load<T>(assetContentDirPath);
            }
            else
            {
                // Search the content directory for the asset before trying content bundles.
                if (File.Exists(assetContentDirPath))
                {
                    instance = LoadAssetFromDisk<T>(assetContentDirPath);
                }
                else
                {
                    // TODO: Search content bundles.
                    throw new NotImplementedException("Add support for bundle loading or file does not exist");
                }
            }

            // Cache the asset instance before returning.
            mAssetCache.Add(assetPath, instance);
            return instance;
        }

        /// <summary>
        ///  Load an asset using a content reader.
        /// </summary>
        /// <remarks>
        ///  This method uses a content reader to read a raw (uncompiled) asset and convert it into its compiled
        ///  runtime form.
        /// </remarks>
        /// <typeparam name="T">The asset type to load.</typeparam>
        /// <param name="assetInfo">File path of the asset.</param>
        /// <returns>Instance of the asset.</returns>
        private T LoadAssetFromDisk<T>(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new ArgumentNullException(nameof(assetPath));
            }
            
            // Use the content reader system to convert the asset into its runtime form.
            var contentReader = CreateContentReader<T>(assetPath);
            var asset = default(T);

            using (var stream = File.OpenRead(assetPath))
            {
                var contentDir = Path.GetDirectoryName(assetPath);
                asset = contentReader.Read(stream, assetPath, contentDir, this);
            }

            return asset;
        }

        /// <summary>
        ///  Unload all content loaded by this content manager.
        /// </summary>
        /// <remarks>
        ///  TODO: Make this work.
        /// </remarks>
        public override void Unload()
        {
            base.Unload();
        }
        
        /// <summary>
        ///  Search the given directory recursively and add any game content files to the provided dictionary.
        /// </summary>
        /// <param name="contentDir">The path to the game's content directory.</param>
        /// <param name="currentDir">The directory path to be searched.</param>
        /// <param name="items">Dictionary holding a list of discovered assets.</param>
        private void SearchDirectoryForContent(
            string contentDir,
            string currentDir,
            ref Dictionary<string, AssetInfo> items)
        {
            // Extract a relative path for the asset to the game's content directory.
            // Ex: "C:\game\content\sprite\a.sprite" ==> "sprite\a.sprite".
            var index = currentDir.IndexOf(contentDir + Path.DirectorySeparatorChar);
            var relativeDirectory = (index < 0 ? currentDir : currentDir.Remove(index, contentDir.Length + 1));

            // First recursively search the directories.
            foreach (var childDirPath in Directory.GetDirectories(currentDir))
            {
                SearchDirectoryForContent(contentDir, childDirPath, ref items);
            }

            // Now get a list of files in this directory, and add files that match asset extensions to the list of
            // asset items.
            var contentBaseDir = mContentDir + "/";

            foreach (var filePath in Directory.GetFiles(currentDir))
            {
                var assetPath = NormalizeFilePath(filePath);

                // Remove content directory from asset path.
                if (assetPath.StartsWith(contentBaseDir))
                {
                    assetPath = assetPath.Substring(contentBaseDir.Length, assetPath.Length - contentBaseDir.Length);
                }

                // Check the type of file that was encountered and read it appropriately.
                var asset = new AssetInfo(assetPath, filePath);

                if (filePath.EndsWith(".xnb"))
                {
                    // This is a precompiled XNA asset. Set a flag indicating this and let the legacy XNA loader handle
                    // loading the file.
                    asset.IsXnb = true;
                    items.Add(assetPath, asset);
                }
                else if (filePath.EndsWith(".bundle"))
                {
                    // A compressed bundle of assets.
                    SearchBundleForAssets(filePath, items);
                }
                else if (IsValidAssetExtension(filePath))
                {
                    // A valid raw asset file that can be loaded by the content manager.
                    var readerInfo = GetContentReaderInfo( filePath );
                    asset.ContentType = readerInfo.ContentType;
                    asset.ReaderType = readerInfo.ReaderType;

                    items.Add(assetPath, asset);
                }
            }
        }

        /// <summary>
        ///  Searches a compressed bundle for game assets.
        /// </summary>
        /// <param name="bundleFileName">Path to the asset bundle.</param>
        /// <param name="items">Dictionary holding a list of discovered assets.</param>
        private void SearchBundleForAssets(string bundleFileName, Dictionary<string, AssetInfo> items)
        {
            var fs = File.OpenRead(bundleFileName);
            var zipFile = new ZipFile(fs);

            // Cache reference to the zip file so it remains open through the lifetime of this content manager.
            mBundleArchives.Add(zipFile);

            // Iterate through the archive and add any resources contained within.
            foreach (ZipEntry entry in zipFile)
            {
                // Skip entries that are not files.
                if (!entry.IsFile)
                {
                    continue;
                }

                // Convert the resource's zip path into an asset name.
                var assetPath = entry.Name;
                var assetName = assetPath;

                // Check the type of file that was encountered and read it appropriately. Skip any bundles inside this
                // bundle.
                var asset = new AssetInfo(assetName, assetPath);

                asset.BundleFile = zipFile;
                asset.BundleEntry = entry;

                if (IsXnbFile(assetPath))
                {
                    // This is a precompiled XNA asset. Set a flag indicating this and let the legacy XNA loader handle
                    // loading the file.
                    asset.IsXnb = true;
                    items.Add(assetName, asset);
                }
                else if (IsValidAssetExtension(assetPath))
                {
                    // A valid raw asset file that can be loaded by the content manager.
                    var readerInfo = GetContentReaderInfo(assetPath);
                    asset.ContentType = readerInfo.ContentType;
                    asset.ReaderType  = readerInfo.ReaderType;

                    items.Add(assetName, asset);
                }
            }
        }

        /// <summary>
        ///  Create a content reader that is capable of loading the given asset into memory.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="asset">Information on the asset.</param>
        /// <returns>Content reader capable of loading this asset.</returns>
        private ContentReader<T> CreateContentReader<T>(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new ArgumentNullException(nameof(assetPath));
            }

            // Double check that the asset info is correct.
            var contentReaderInfo = GetContentReaderInfo(assetPath);

            if (contentReaderInfo.ContentType == null)
            {
                throw new ContentManagerException(
                    "Asset '{0}' does not have a backing System.Type".With(assetPath));
            }
            else if (contentReaderInfo.ReaderType == null)
            {
                throw new ContentManagerException(
                    "Asset '{0}' does not have content reader type".With(contentReaderInfo));
            }

            // Instantiate a copy of the content reader.
            var reader = Activator.CreateInstance(contentReaderInfo.ReaderType) as ContentReader<T>;

            if (reader == null)
            {
                throw new ContentManagerException(
                    "Failed to instantiate ContentReader<T> for asset '{0}'".With(assetPath));
            }

            return reader;
        }

        /// <summary>
        ///  Gets the content reader type capable of loading this asset into memory.
        ///  
        /// </summary>
        /// <param name="asset">Information on this asset.</param>
        /// <returns>Content reader type capable of loading this asset.</returns>
        private ContentReaderInfo GetContentReaderInfo(string assetPath)
        {
            foreach (var entry in mContentReaders)
            {
                if (assetPath.EndsWith(entry.Extension))
                {
                    return entry;
                }
            }

            // Failed to locate matching content reader.
            throw new ContentReaderMissingException(assetPath);
        }

        /// <summary>
        ///  Checks if the asset file extension is a valid and loadable content type.
        /// </summary>
        /// <param name="filename">Asset filename.</param>
        /// <returns>True if this asset file name can be loaded.</returns>
        private static bool IsValidAssetExtension( string filename )
        {
            foreach (var readerInfo in mContentReaders)
            {
                if (filename.EndsWith(readerInfo.Extension))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Searches the currently loaded assemblies for asset content readers, and then records
        ///  them into a list of content readers for later use in asset loading.
        /// </summary>
        private static void InitContentReaderList()
        {
            mContentReaders = new List<ContentReaderInfo>();

            // Search for all classes with ContentReaderAttribute.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type attribute = typeof( ContentReaderAttribute );

            foreach ( Assembly assembly in assemblies )
            {
                foreach ( Type type in assembly.GetTypes() )
                {
                    object[] attribs = type.GetCustomAttributes( attribute, true );

                    if ( attribs.Length > 0 )
                    {
                        ContentReaderAttribute attr = attribs[0] as ContentReaderAttribute;
                        ContentReaderInfo entry    = new ContentReaderInfo();

                        entry.ContentType = attr.ContentType;
                        entry.ReaderType = type;
                        entry.Extension = attr.Extension;

                        mContentReaders.Add( entry );
                    }
                }
            }
        }

        /// <summary>
        ///  Check if the asset filepath is a valid precompiled .xnb asset.
        /// </summary>
        /// <param name="assetPath">Path to the asset.</param>
        /// <returns>True if it is a precompiled xnb asset, false otherwise.</returns>
        private static bool IsXnbFile( string assetPath )
        {
            return assetPath.EndsWith( ".xnb" );
        }


        /// <summary>
        ///  Normalize asset name so it is always the same name across all platforms.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <returns>Normalized asset name</returns>
        private static string NormalizeFilePath( string assetName )
        {
            return assetName.Replace( '/', '\\' );
        }

        /// <summary>
        ///  Information on a content reader.
        /// </summary>
        private struct ContentReaderInfo
        {
            public Type ContentType;
            public Type ReaderType;
            public string Extension;
        }

        /// <summary>
        ///  Represents asset information.
        /// </summary>
        private class AssetInfo
        {
            public string AssetName { get; set; }
            public string FilePath { get; set; }
            public ZipEntry BundleEntry { get; set; }
            public ZipFile BundleFile { get; set; }
            public bool IsXnb { get; set; }
            public Type ContentType { get; set; }
            public Type ReaderType { get; set; }

            public AssetInfo( string name, string path )
            {
                AssetName = name;
                FilePath = path;
                BundleEntry = null;
                IsXnb = false;
            }

            // Check if asset is in a bundle.
            public bool IsInBundle { get { return BundleEntry != null; } }
        }
    }
}
