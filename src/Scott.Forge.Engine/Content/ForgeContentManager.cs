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
using Microsoft.Xna.Framework.Content;
using Scott.Forge.Content;

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  The content manager is responsible for the loading and unloading of game resources.
    /// </summary>
    /// 
    ///  Custom XNA content manager for the Forge game engine. This content manager implementation adds support for
    ///  compressed zip archives ("bundles"), asset preloading, name based references and the ability to use raw asset
    ///  files instead of compiled .xnb blobs.
    /// </summary>
    /// <remarks>
    ///  The content manager is capable of loading several types of content items. It uses the XNA loader to load any
    ///  .XNB files, built-in loaders to load Forge cooked resources and (if configured) it will use asset loaders to
    ///  load uncooked resources at run time.
    ///  
    ///  Each content manager can be configured with multiple content containers that hold content. These containers
    ///  serve two purposes. The first is to abstract how resources are stored - one container could be a physical file
    ///  system directory, and another one could be the contents of a zip archive. The other benefit to containers is
    ///  to provide a sorted list of places to search for content such that one container could provide an override to
    ///  another container's resource. This allows for developers and players to support mod packs.
    ///  
    ///  TODO: Ensure that the content reader system is compatable with XNA.
    ///  TODO: Add async / threaded loading support.
    ///  TODO: Add ability to load and unload groups of items.
    ///  
    /// </remarks>
    public class ForgeContentManager : ContentManager, IContentManager
    {
        /// <summary>
        ///  A list of content readers that are capable of converting raw assets into usable game resources objects.
        /// </summary>
        private static List<ContentReaderInfo> mContentReaders = new List<ContentReaderInfo>();

        /// <summary>
        ///  True if the instance has been disposed, false otherwise.
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        ///  List of content containers.
        /// </summary>  
        private List<IContentContainer> mContentContainers = new List<IContentContainer>();

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

            mContentContainers.Add(new DirectoryContentContainer(rootDirectory));
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
        /// <typeparam name="T">Runtime content type to load.</typeparam>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>An instance of the loaded asset.</returns>
        public override T Load<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new InvalidAssetNameException(assetName);
            }

            // Use consistent directory separator characters.
            assetName = NormalizeFilePath(assetName);

            // Check type of file being loaded. XNB files have a special codepath to take, all other content items are
            // loaded using this class.
            if (IsXnbFile(assetName))
            {
                return base.Load<T>(assetName);
            }
            else
            {
                return LoadForgeContent<T>(assetName);
            }
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
        protected override Stream OpenStream(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new ArgumentNullException(nameof(assetName));
            }

            // Search content containers for the first container that has the asset, and return a stream to the item.
            Stream readStream = null;
            
            foreach (var container in mContentContainers)
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

            // Use the content reader system to convert the asset into its runtime form.
            var contentReader = CreateContentReader<T>(assetName);
            var asset = default(T);

            using (var stream = OpenStream(assetName))
            {
                var contentDir = Path.GetDirectoryName(assetName);
                asset = contentReader.Read(stream, assetName, contentDir, this);
            }

            return asset;
        }
        
        /// <summary>
        ///  Creates a content reader that can load an asset as the requested content type.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="assetName">Asset file name.</param>
        /// <returns>Content reader capable of loading this asset.</returns>
        private ContentReader<T> CreateContentReader<T>(string assetName)
        {
            if (assetName == null)
            {
                throw new ArgumentNullException(nameof(assetName));
            }

            // Find the right content reader type for this asset, instantiate and return the content reader.
            var readerType = FindContentReaderTypeFor<T>(assetName);
            var readerInstance = Activator.CreateInstance(readerType) as ContentReader<T>;

            Debug.Assert(readerInstance != null);

            return readerInstance;
        }

        /// <summary>
        ///  Find the content reader type capable of loading this asset.
        /// </summary>
        /// <param name="asset">Information on this asset.</param>
        /// <returns>Content reader type capable of loading this asset.</returns>
        private Type FindContentReaderTypeFor<T>(string assetName)
        {
            foreach (var entry in mContentReaders)
            {
                if (assetName.EndsWith(entry.Extension))
                {
                    if (entry.ContentType == null || entry.ContentType != typeof(T))
                    {
                        throw new ContentReaderWrongOutputTypeException(
                            assetName,
                            contentReaderType: entry.ReaderType,
                            expectedOutputType: typeof(T),
                            actualOutputType: entry.ContentType);
                    }

                    return entry.ReaderType;
                }
            }

            // Failed to locate matching content reader.
            throw new ContentReaderMissingException(assetName);
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
                        var attr = attribs[0] as ContentReaderAttribute;
                        var entry = new ContentReaderInfo();

                        Debug.Assert(attr.ContentType != null);

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
            return assetName.Replace( '\\', '/' );
        }

        /// <summary>
        ///  Information on a content reader.
        /// </summary>
        private class ContentReaderInfo
        {
            public Type ContentType;
            public Type ReaderType;
            public string Extension;
        }
    }
}
