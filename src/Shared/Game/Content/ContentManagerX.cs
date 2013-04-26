using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        /// <summary>
        ///  List of content readers, which process an asset file into a loadable instance.
        /// </summary>
        private static List<ContentReaderInfo> mContentReaders = new List<ContentReaderInfo>();

        /// <summary>
        /// Flag that lets us know if the content directory was scanned for asset entries.
        /// Normally this isn't required, but we're trying to act like the builtin XNA content
        /// manager so we support scanning at first asset load.
        /// </summary>
        private bool mAssetDirectoryScanned = false;

        /// <summary>
        ///  Relative path to a directory where the game's content is stored.
        /// </summary>
        private string mContentDir = String.Empty;

        /// <summary>
        ///  True if the instance has been disposed, false otherwise.
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        ///  List of loaded assets. Each asset name is mapped to a class describing information
        ///  on how to load the asset from disk.
        /// </summary>
        private Dictionary<string, AssetInfo> mAssetFiles = new Dictionary<string, AssetInfo>();

        /// <summary>
        ///  A list of zipped bundle files that have been loaded. They should only be added once
        ///  otherwise weird errors could happen.
        /// </summary>
        private static List<ZipFile> mBundleArchives = new List<ZipFile>();

        /// <summary>
        ///  Asset cache. Content items that are loaded are cached into this dictionary, so they
        ///  can be quickly returned on subsequent loads.
        /// </summary>
        private Dictionary<string, Object> mAssetCache = new Dictionary<string, Object>();

        /// <summary>
        ///  Static constructor. Set up the content readers and their extension information.
        /// </summary>
        static ContentManagerX()
        {
            InitContentReaderList();
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        public ContentManagerX( IServiceProvider provider, string rootDirectory )
            : base( provider, rootDirectory )
        {
            mContentDir = rootDirectory;
        }

        /// <summary>
        ///  Dispose of the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                Unload();
                mDisposed = true;
            }

            base.Dispose( disposing );
        }

        /// <summary>
        ///  Check if the requested asset was loaded into the content manager.
        /// </summary>
        /// <param name="assetName">Name of the asset to check.</param>
        /// <returns>True if it was loaded, false otherwise.</returns>
        public bool WasLoaded( string assetName )
        {
            return mAssetFiles.ContainsKey( NormalizeAssetName( assetName ) );
        }

        /// <summary>
        ///  Load requested asset name into memory, and return a reference to the loaded instance.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>Instance of the loaded asset.</returns>
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
            AssetInfo assetInfo = FindAssetInfoFor( assetName );

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

        /// <summary>
        ///  Load the requested asset from disk
        /// </summary>
        /// <typeparam name="T">The asset type to load.</typeparam>
        /// <param name="assetInfo">Information on how to load the asset.</param>
        /// <returns>Instance of the asset.</returns>
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
        ///  Returns a stream capable of loading the named asset.
        /// </summary>
        /// <param name="assetName">Name of the asset to load.</param>
        /// <returns>Stream containing the asset data.</returns>
        protected override Stream OpenStream( string assetName )
        {
            // Make sure the asset name is normalized so we don't get funky path issues and then
            // load information for the asset.
            AssetInfo asset = FindAssetInfoFor( NormalizeAssetName( assetName ) );

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
        ///  Search the content manager's content directory for a list of loadable assets, and
        ///  optionally preload them all into memory.
        /// </summary>
        /// <param name="preload">True to preload all found assets into the cache, false to
        /// wait until requested.</param>
        public void SearchContentDirForAssets( bool preload )
        {
            // Make sure the content path exists before proceeding.
            if ( String.IsNullOrEmpty( mContentDir ) || !Directory.Exists( mContentDir ) )
            {
                throw new GameContentException(
                    "Could not locate game content folder {0}".With( mContentDir) );
            }

            // Now scan the directory for content.
            SearchDirForContent( mContentDir, mContentDir, mAssetFiles );
            mAssetDirectoryScanned = true;

            // Did the caller want us to preload all the assets we found?
            if ( preload )
            {
                foreach ( string assetName in mAssetFiles.Keys )
                {
                    // do SOMETHING
                }
            }
        }

        /// <summary>
        ///  Recursively search this directory for a list of assets.
        /// </summary>
        /// <param name="dirpath">Path to the content directory being searched.</param>
        /// <param name="items">
        ///  List of assets to add assets found in this content directory.
        /// </param>
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
                if ( WasLoaded( assetName ) )
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
        /// <param name="bundleFileName">Path to the asset bundle.</param>
        /// <param name="items">
        ///  List of assets to add assets found in this bundle.
        /// </param>
        private void SearchBundleForAssets( string bundleFileName,
                                            Dictionary<string, AssetInfo> items )
        {
            FileStream fs   = File.OpenRead( bundleFileName );
            ZipFile zipFile = new ZipFile( fs );

            // Track the loaded zip files.
            mBundleArchives.Add( zipFile );

            // Iterate through the bundle and make note of the asset files contained within.
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

                // Was this asset already loaded? If so throw an error and bail out.
                if ( WasLoaded( assetName ) )
                {
                    throw new GameContentException(
                        "Asset name '{0}' was already loaded. Is this a duplicate?"
                        .With( assetName )
                    );
                }

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

        /// <summary>
        ///  Returns a content reader capable of loading this asset into memory.
        /// </summary>
        /// <typeparam name="T">Content type to load.</typeparam>
        /// <param name="asset">Information on the asset.</param>
        /// <returns>Content reader capable of loading this asset.</returns>
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

        /// <summary>
        ///  Gets the content reader type capable of loading this asset into memory.
        /// </summary>
        /// <param name="asset">Information on this asset.</param>
        /// <returns>Content reader type capable of loading this asset.</returns>
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

        /// <summary>
        ///  Search the content manager's list of assets, and return information about the
        ///  requested asset name.
        /// </summary>
        /// <param name="assetName">Name of the aset to search for.</param>
        /// <returns>Information on the asset.</returns>
        private AssetInfo FindAssetInfoFor( string assetName )
        {
            // If we haven't scanned the content directory yet, do it right now.
            if ( !mAssetDirectoryScanned )
            {
                SearchContentDirForAssets( false );
            }

            // Locate the asset in our list of loaded assets.
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
        ///  Checks if the asset file extension is a valid and loadable content type.
        /// </summary>
        /// <param name="filename">Asset filename.</param>
        /// <returns>True if this asset file name can be loaded.</returns>
        private static bool IsValidAssetExtension( string filename )
        {
            foreach ( ContentReaderInfo readerInfo in mContentReaders )
            {
                if ( filename.EndsWith( readerInfo.Extension ) )
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
        ///  Extracts the asset name from an asset's filepath.
        /// </summary>
        /// <param name="assetPath">Path to the asset.</param>
        /// <returns>Name of the asset.</returns>
        private static string ExtractAssetName( string assetPath )
        {
            return ExtractAssetName( assetPath, null );
        }

        /// <summary>
        ///  Extracts the asset name from an asset's filepath.
        /// </summary>
        /// <param name="assetPath">Path to the asset.</param>
        /// <param name="contentDir">Path to the game's content directory.</param>
        /// <returns>Name of the asset.</returns>
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

        /// <summary>
        ///  Information on a content reader.
        /// </summary>
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
