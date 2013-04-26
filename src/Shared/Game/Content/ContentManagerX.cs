using ICSharpCode.SharpZipLib.Zip;
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

        // Flag that lets us know if the content directory was scanned for asset entries.
        // Normally this isn't required, but we're trying to act like the builtin XNA content
        // manager so we support scanning at first asset load.
        private bool mAssetDirectoryScanned = false;

        /// <summary>
        ///  Tracks assets.
        ///   asset name => asset info
        /// </summary>
        private Dictionary<string, AssetInfo> mAssetFiles = new Dictionary<string, AssetInfo>();

        private List<ZipFile> mBundleArchives = new List<ZipFile>();

        // List of valid file extensions.
        private List<string> mValidExtensions = new List<string>();
        
        // List of content readers, which process a file into a loadable instance.
        private List<ContentReaderInfo> mContentReaders = new List<ContentReaderInfo>();

        // Cached assets (asset name => instance).
        private Dictionary<string, Object> mAssetCache = new Dictionary<string, Object>();

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
            // Make sure the asset name is normalized so we don't get funky path issues.
            assetName = NormalizeAssetName( assetName );

            // Was the asset already loaded into our item cache? If so return a duplicated version
            // of the original.
            Object cachedObject = null;

            if ( mAssetCache.TryGetValue( assetName, out cachedObject ) )
            {
                return (T) cachedObject;
            }

            // Get information on the requested asset.
            AssetInfo assetInfo;

            if ( !mAssetFiles.TryGetValue( assetName, out assetInfo ) )
            {
                throw new GameContentException(
                    "Could not locate asset '{0}', it was not found in the content directory."
                    .With( assetName )
                );
            }

            // Check if this is a precompiled XNB asset or a normal asset file. If it is an XNB
            // asset then we will need to use XNA's legacy loader.
            T asset = default( T );

            if ( assetInfo.IsXnb )
            {
                asset = base.Load<T>( assetName );
            }
            else
            {
                asset = LoadAssetFromDisk<T>( assetInfo );
            }

            // Cache the asset in memory for subsequent loads.
            mAssetCache.Add( assetName, asset );

            // Now return our loaded asset.
            return asset;
        }

        private T LoadAssetFromDisk<T>( AssetInfo assetInfo )
        {
            string assetName = assetInfo.AssetName;

            // Locate the content reader responsible for loading this asset type.
            ContentReader<T> contentReader = CreateContentReader<T>( assetInfo );

            // Obtain a stream capable of reading the asset, and then slurp it into memory.
            T asset = default(T);

            using ( Stream stream = OpenStream( assetName ) )
            {
                string contentDir = Path.GetDirectoryName( assetName );
                asset = contentReader.Read( stream, assetName, contentDir, this );
            }

            return asset;
        }

        /// <summary>
        ///  Overloaded from base.
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        protected override Stream OpenStream( string assetName )
        {
            // Make sure the asset name is normalized so we don't get funky path issues.
            assetName = NormalizeAssetName( assetName );

            // Locate the asset's file information.
            AssetInfo asset;

            if ( !mAssetFiles.TryGetValue( assetName, out asset ) )
            {
                throw new GameContentException(
                    "Could not locate asset '{0}', it was not found in the content directory."
                    .With( assetName )
                );
            }

            // What kind of asset file was it? Is it located on disk, or is it located in a bundle
            // archive?
            if ( asset.IsInBundle )
            {
                // Zip files stream are not streamable... so we have to load everything into memory
                // and then return a stream to that.
                Stream zipStream = asset.BundleFile.GetInputStream( asset.BundleEntry );
                MemoryStream memStream = new MemoryStream();

                zipStream.CopyTo( memStream );
                memStream.Seek( 0, SeekOrigin.Begin );

                return memStream;
            }
            else
            {
                return File.OpenRead( asset.FilePath );
            }
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
                                          Dictionary<string, AssetInfo> items )
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

                // Normalize asset names to use backslash as the path separator.
                assetName = NormalizeAssetName( assetName );

                // Was this asset already loaded? If so throw an error and bail out.
                if ( mAssetFiles.ContainsKey( assetName ) )
                {
                    throw new GameContentException(
                        "Asset name '{0}' was already loaded. Is this a duplicate?"
                        .With( assetName )
                    );
                }

                // What kind of file did we find? There are several different types of files that the
                // content manager can support, we'll need to deal with them differently.
                AssetInfo asset = new AssetInfo( assetName, path );

                if ( path.EndsWith( ".xnb" ) )
                {
                    // A precompiled XNA asset...
                    asset.IsXnb = true;
                    items.Add( assetName, asset );
                }
                else if ( path.EndsWith( ".bundle" ) )
                {
                    // A compressed bundle of assets.
                    SearchBundleForAssets( path, items );
                }
                else if ( IsValidAssetExtension( path ) )
                {
                    // A valid raw asset file that can be loaded by the content manager.
                    ContentReaderInfo readerInfo = GetContentReaderInfo( asset );
                    asset.ContentType = readerInfo.ContentType;
                    asset.ReaderType = readerInfo.ReaderType;

                    items.Add( assetName, asset );
                }
            }
        }

        /// <summary>
        ///  Searches a zip bundle for game assets.
        /// </summary>
        /// <param name="bundleFileName"></param>
        private void SearchBundleForAssets( string bundleFileName,
                                            Dictionary<string, AssetInfo> items )
        {
            FileStream fs   = File.OpenRead( bundleFileName );
            ZipFile zipFile = new ZipFile( fs );

            foreach ( ZipEntry entry in zipFile )
            {
                // Only record file entries that are valid asset types.
                if ( !entry.IsFile )
                {
                    continue;
                }

                // Get the asset name, which is taken from the zip entry's file name.
                string assetPath = entry.Name;
                string assetName = ExtractAssetName( assetPath, null );

                // What kind of file did we find? There are several different types of files that the
                // content manager can support, we'll need to deal with them differently.
                AssetInfo asset   = new AssetInfo( assetName, assetPath );

                asset.BundleFile  = zipFile;
                asset.BundleEntry = entry;

                if ( IsXnbFile( assetPath ) )
                {
                    // A precompiled XNA asset...
                    asset.IsXnb = true;
                    items.Add( assetName, asset );
                }
                else if ( IsValidAssetExtension( assetPath ) )
                {
                    // A valid raw asset file that can be loaded by the content manager.
                    ContentReaderInfo readerInfo = GetContentReaderInfo( asset );
                    asset.ContentType = readerInfo.ContentType;
                    asset.ReaderType  = readerInfo.ReaderType;

                    items.Add( assetName, asset );
                }
            }
        }

        private ContentReader<T> CreateContentReader<T>( AssetInfo asset )
        {
            string assetName = asset.AssetName;

            // Was the content reader type located? Freak out if it wasn't.
            if ( asset.ContentType == null )
            {
                throw new GameContentException(
                    "Asset '{0}' does not have a backing System.Type".With( assetName ) );
            }
            else if ( asset.ReaderType == null )
            {
                throw new GameContentException(
                    "Asset '{0}' does not have content reader type".With( assetName ) );
            }

            // Instantiate a copy of the content reader.
            ContentReader<T> reader =
                Activator.CreateInstance( asset.ReaderType ) as ContentReader<T>;

            if ( reader == null )
            {
                throw new GameContentException(
                    "Failed to instantiate ContentReader<T> for asset '{0}'".With( assetName ) );
            }

            return reader;
        }

        private ContentReaderInfo GetContentReaderInfo( AssetInfo asset )
        {
            // Locate the content reader type that is responsible for reading this file's asset
            // type.
            ContentReaderInfo readerInfo = null;

            foreach ( ContentReaderInfo entry in mContentReaders )
            {
                if ( asset.FilePath.EndsWith( entry.Extension ) )
                {
                    readerInfo = entry;
                    break;
                }
            }

            // Did we locate it?
            if ( readerInfo == null )
            {
                throw new GameContentException(
                    "Failed to locate content reader for asset '{0}'".With( asset.AssetName ) );
            }

            return readerInfo;
        }

        private AssetInfo FindAssetInfoFor( string assetName )
        {
            AssetInfo asset;

            if ( !mAssetFiles.TryGetValue( assetName, out asset ) )
            {
                throw new GameContentException(
                    "Could not locate asset '{0}', it was not found in the content directory."
                    .With( assetName )
                );
            }

            return asset;
        }

        /// <summary>
        ///  Checks if the asset filename ends in a valid asset extension.
        /// </summary>
        /// <param name="filename">Asset filename.</param>
        /// <returns>True if filename has a valid file extension.</returns>
        private bool IsValidAssetExtension( string filename )
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

        private static bool IsXnbFile( string assetPath )
        {
            return assetPath.EndsWith( ".xnb" );
        }

        private static string ExtractAssetName( string assetPath )
        {
            return ExtractAssetName( assetPath, null );
        }

        private static string ExtractAssetName( string assetPath, string contentDir )
        {
            string baseName  = Path.GetFileNameWithoutExtension( assetPath );
            string assetName = "";

            if ( !String.IsNullOrEmpty( contentDir ) )
            {
                assetName = Path.Combine( contentDir, baseName );
            }
            else
            {
                assetName = Path.Combine( Path.GetDirectoryName( assetPath ), baseName );
            }

            return NormalizeAssetName( assetName );
        }

        /// <summary>
        ///  Normalize asset name so it is always the same name across all platforms.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <returns>Normalized asset name</returns>
        private static string NormalizeAssetName( string assetName )
        {
            return assetName.Replace( '\\', '/' );
        }

        private class ContentReaderInfo
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
