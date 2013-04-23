using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Scott.Game.Content
{
    /// <summary>
    ///  Its so awesome, its XTRA awesome.
    ///  
    ///  TODO: Act more like the XNA Content Manager. Use attributes decorated on the content
    ///        reader classes to discover readers, rather than manually configuring them.
    ///        
    ///  NOTE: THIS CLASS SHOULD BE THREADSAFE + CACHE, THREAD AND ASYNC HAPPY.
    /// </summary>
    public class ContentManagerX : Microsoft.Xna.Framework.Content.ContentManager
    {
        private bool mDisposed = false;
        private Dictionary<string, string> mAssetFiles = new Dictionary<string, string>();
        private List<string> mValidExtensions = new List<string>();
        
        private List<ContentReaderEntry> mContentReaders = new List<ContentReaderEntry>();

        // Cached assets (asset name => instance).
        private Dictionary<string, Object> mAssetCache = new Dictionary<string, Object>();

        // List of precompiled XNB files that have been found.
        //  (asset name => asset path)
        private Dictionary<string, string> mXnbAssets = new Dictionary<string, string>();

        /// <summary>
        ///  Constructor.
        /// </summary>
        public ContentManagerX( IServiceProvider provider, string rootDirectory )
            : base( provider, rootDirectory )
        {
            InitContentReaderList();
        }

        /// <summary>
        ///  Dispose of the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                // Do clean up work here...
                mDisposed = true;
            }

            base.Dispose( disposing );
        }

        /// <summary>
        ///  Searches the given directory for game content.
        /// </summary>
        /// <param name="contentPath">Content path to search.</param>
        public void AddContentDir( string contentPath, bool preload )
        {
            // Make sure the content path exists.
            if ( !Directory.Exists( contentPath ) )
            {
                throw new GameContentException( "Could not locate root game content folder " + contentPath );
            }

            // Now search the directory recursively, looking for game content files.
            SearchDirForContent( contentPath, contentPath, mAssetFiles );

            // Should we start preloading the assets we discovered?
            if ( preload )
            {
                foreach ( string assetName in mAssetFiles.Keys )
                {
                    // do SOMETHING
                }
            }
        }

        /// <summary>
        ///  Load requested asset name. This is an immediate request for the named asset, and
        ///  this method will block until the content can be returned to teh caller.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override T Load<T>( string assetName )
        {
            // Was the asset already loaded into our item cache? If so return a duplicated version
            // of the original.
            Object cachedObject = null;

            if ( mAssetCache.TryGetValue( assetName, out cachedObject ) )
            {
                return (T) cachedObject;
            }

            // Check if this is a precompiled XNB asset or a normal asset file. If it is an XNB
            // asset then we will need to use XNA's legacy loader.
            T asset = default( T );

            if ( mXnbAssets.ContainsKey( assetName ) )
            {
                asset = base.Load<T>( assetName );
            }
            else
            {
                asset = LoadAssetFromDisk<T>( assetName );
            }

            // Cache the asset in memory for subsequent loads.
            mAssetCache.Add( assetName, asset );

            // Now return our loaded asset.
            return asset;
        }

        /// <summary>
        ///  Loads a game content file from disk.
        /// </summary>
        /// <typeparam name="T">The content type to load.</typeparam>
        /// <param name="assetName">Name of the asset to load.</param>
        /// <returns>Instance of the asset loaded from disk.</returns>
        private T LoadAssetFromDisk<T>( string assetName )
        {
            // Find the asset path in our list of assets.
            string assetPath = null;

            if (! mAssetFiles.TryGetValue( assetName, out assetPath ) )
            {
                throw new GameContentException( "Could not locate requested asset name", assetName );
            }

            // Locate the content reader responsible for loading this asset type.
            ContentReader<T> contentReader = FindContentReader<T>( assetPath );

            // Open up a file stream to the asset and put it into the object cache.
            T asset = default(T);

            using ( FileStream stream = File.OpenRead( assetPath ) )
            {
                // What content directory is this asset located in?
                string contentDir = Path.GetDirectoryName( assetName );

                // Now slurp the asset into memory.
                asset = contentReader.Read( stream, assetName, contentDir, assetPath, this );
            }

            return asset;
        }

        /// <summary>
        ///  Overloaded from base.
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        protected override System.IO.Stream OpenStream( string assetName )
        {
            return base.OpenStream( assetName );
        }

        /// <summary>
        ///  Forcibly unload all content. I don't actually think this works, since the game will
        ///  hold hard refs...
        /// </summary>
        public override void Unload()
        {
            base.Unload();
        }

        /// <summary>
        ///  Locates the correct content reader for a given asset filename.
        /// </summary>
        /// <typeparam name="T">The asset's content type.</typeparam>
        /// <param name="assetFileName">Asset's file name.</param>
        /// <param name="contentReader">(Out) Content reader that can load this asset.</param>
        /// <returns>True if the content reader was located, false otherwise.</returns>
        private ContentReader<T> FindContentReader<T>( string assetFileName )
        {
            Type contentReaderType = null;

            // Locate the content reader type that is responsible for reading this file's asset
            // type.
            foreach ( ContentReaderEntry entry in mContentReaders )
            {
                if ( assetFileName.EndsWith( entry.Extension ) )
                {
                    // Ensure the content reader's file extension and the file name extension match
                    // up. There's probably no harm if they do not, but it is an additional sanity
                    // check.
                    if ( typeof( T ) != entry.ContentType )
                    {
                        throw new GameContentException(
                            "Registered content reader file extension is not valid for {0}"
                            .With( assetFileName ) );
                    }

                    // Pass the located content reader instance out of this function.
                    contentReaderType = entry.ReaderType;
                    break;
                }
            }

            // Was the content reader type located? Freak out if it wasn't.
            if ( contentReaderType == null )
            {
                throw new GameContentException(
                    "Could not locate the content reader for {0}".With( assetFileName ) );
            }

            // Instantiate a copy of the content reader.
            ContentReader<T> reader =
                Activator.CreateInstance( contentReaderType ) as ContentReader<T>;

            if ( reader == null )
            {
                throw new GameContentException(
                    "Failed to instantiate ContentReader<T> - cast failed." );
            }

            return reader;
        }

        /// <summary>
        ///  Searches for content reader classes, and installs them into this content manager.
        /// </summary>
        private void InitContentReaderList()
        {
            mContentReaders = new List<ContentReaderEntry>();

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
                        ContentReaderEntry entry    = new ContentReaderEntry();

                        entry.ContentType = attr.ContentType;
                        entry.ReaderType = type;
                        entry.Extension  = attr.Extension;

                        // XXX: verify no other assets have same name

                        mContentReaders.Add( entry );
                        mValidExtensions.Add( attr.Extension );
                    }
                }
            }

        }

        /// <summary>
        ///  Recursively search this directory for assets.
        /// </summary>
        /// <param name="dirpath"></param>
        /// <param name="items"></param>
        private void SearchDirForContent( string rootDir,
                                          string currentDir,
                                          Dictionary<string, string> items )
        {
            // Remove the root content dir to get our asset's base directory that we use for its
            // asset name.
            int index = currentDir.IndexOf( rootDir + Path.DirectorySeparatorChar );
            string contentDir = ( index < 0
                                    ? currentDir
                                    : currentDir.Remove( index, rootDir.Length + 1 ) );

            // First recursively search the directories.
            foreach ( string path in Directory.GetDirectories( currentDir ) )
            {
                SearchDirForContent( rootDir, path, items );
            }

            // Now get a list of files in this directory, and add files that match asset
            // extensions to the list of asset items.
            foreach ( string path in Directory.GetFiles( currentDir ) )
            {
                string baseName  = Path.GetFileNameWithoutExtension( path );
                string assetName = Path.Combine( contentDir, baseName );

                // Is this a precompiled XNA resource? If so, we mark it in a special fashion...
                if ( path.EndsWith( ".xnb" ) )
                {
                    // Make sure the asset was not already loaded.
                    if ( mXnbAssets.ContainsKey( assetName ) || items.ContainsKey( assetName ) )
                    {
                        throw new GameContentException(
                            "Asset name '{0}' already loaded, duplicate?".With( assetName ) );
                    }

                    // Track the asset in the xnb list not the normal asset list.
                    mXnbAssets.Add( assetName, path );
                }
                else if ( IsValidExtension( path ) )
                {
                    // Make sure the asset was not already loaded.
                    if ( mXnbAssets.ContainsKey( assetName ) || items.ContainsKey( assetName ) )
                    {
                        throw new GameContentException(
                            "Asset name '{0}' already loaded, duplicate?".With( assetName ) );
                    }

                    // We will always use backslash / when separating paths in an asset name.
                    assetName = assetName.Replace( '\\', '/' );

                    items.Add( assetName, path );
                }
            }
        }

        private bool IsValidExtension( string filename )
        {
            foreach ( string extension in mValidExtensions )
            {
                if ( filename.EndsWith( extension ) )
                {
                    return true;
                }
            }

            return false;
        }

        private class ContentReaderEntry
        {
            public Type ContentType;
            public Type ReaderType;
            public string Extension;
        }
    }
}
