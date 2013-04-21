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
        private Dictionary<string, Object> mLoadedAssets = new Dictionary<string, Object>();
        private List<ContentReaderEntry> mContentReaders = new List<ContentReaderEntry>();

        /// <summary>
        ///  Constructor.
        /// </summary>
        public ContentManagerX( IServiceProvider provider )
            : base( provider )
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
            // We will always use backslash / when separating paths in an asset name.
            assetName = assetName.Replace( '\\', '/' );

            // Was the asset already loaded into our item cache? If so return a duplicated version
            // of the original.
            return LoadAssetFromDisk<T>( assetName );
        }

        private T LoadAssetFromDisk<T>( string assetName )
        {
            // Find the asset path in our list of assets.
            string assetPath = null;

            if (! mAssetFiles.TryGetValue( assetName, out assetPath ) )
            {
                throw new GameContentException( "Could not locate requested asset name", assetName );
            }

            // Find the content reader entry responsible for this asset type.
            Type assetReaderType = null;

            foreach ( ContentReaderEntry entry in mContentReaders )
            {
                if ( assetPath.EndsWith( entry.Extension ) )
                {
                    // Sanity - ensure the types match.
                    if ( typeof ( T ) != entry.ContentType )
                    {
                        throw new GameContentException( "Registered content reader does not match requested asset type", assetPath );
                    }

                    // Get the reader type.
                    assetReaderType = entry.ReaderType;
                    break;
                }
            }

            // Ensure we actually got an asset reader.
            if ( assetReaderType == null )
            {
                throw new GameContentException( "Could not locate a content reader for the asset", assetName );
            }

            // Open up a file stream to the asset and put it into the object cache.
            T asset = default(T);

            using ( FileStream stream = File.OpenRead( assetPath ) )
            {
                // Get the content reader responsible for this asset type, and instantiate a new copy
                // to read it in.
                Scott.Game.Content.ContentReader<T> reader =
                    (Scott.Game.Content.ContentReader<T>) Activator.CreateInstance( assetReaderType );

                // What content directory is this asset located in?
                string contentDir = Path.GetDirectoryName( assetName );

                // Now slurp the asset into memory.
                asset = reader.Read( stream, assetName, contentDir, assetPath, this );

                // Don't forget to cache it!
                mLoadedAssets.Add( assetName, asset );
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
                // Load assets.
                if ( IsValidExtension( path ) )
                {
                    string baseName  = Path.GetFileNameWithoutExtension( path );
                    string assetName = Path.Combine( contentDir, baseName );

                    // TODO: Make sure it is not already in asset list.

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
