# Build all content in the content/ directory and stage it into the game directory.
#########################################################################################
import os
import shutil
import zipfile

# 0) Script customization
PotentialContentDirs = [ "content", "../content" ]
PotentialBuildDirs = [ "build", "../build" ]
BuildTargets = [ "Debug", "Release" ]

AssetExtensions = [ ".sprite", ".png", ".xnb" ]
BuildTypesRaw = [ "Debug" ]
BuildTypesZip = [ "Release" ]

# 1) Locate the content/ directory
ActualContentDir = ""

for dir in PotentialContentDirs:
	dirpath = os.path.join( os.getcwd(), dir )
	
	if os.path.isdir( dirpath ):
		print( "Content dir: " + dirpath )
		ActualContentDir = dirpath
		
if ActualContentDir == "":
	exit( "Failed to locate game content directory" )

# 2) Locate the build directories
ActualBuildDir = ""

for dir in PotentialBuildDirs:
	dirpath = os.path.join( os.getcwd(), dir )
	
	if os.path.isdir( dirpath ):
		print( "Build dir: " + dirpath )
		ActualBuildDir = dirpath
			
if ActualBuildDir == "":
	exit( "Failed to locate build directory" )
	
# 3) Build a file list of all the game's content
RawAssetFiles = []

for root, subFolders, files in os.walk( ActualContentDir ):
	for file in files:
		# Does it have the correct file extension?		
		for ext in AssetExtensions:
			if file.endswith( ext ):
				RawAssetFiles.append( os.path.join( root, file ) )
				
# 4) Trim the asset file paths by converting them into paths relative to the game content dir
AssetFiles = []
ContentRoot = os.path.join( ActualContentDir, "" )

for file in RawAssetFiles:
	AssetFiles.append( file.replace( ContentRoot, "" ) )

#5) Copy the assets to the requested targets
for target in BuildTypesRaw:
	# Content dir
	targetdir = os.path.join( ActualBuildDir, target )
	contentdir = os.path.join( targetdir, "Content" )
	
	print( "Copying assets to: " + contentdir )
	
	if not os.path.isdir( targetdir ):
		exit( "Could not locate build directory for " + target )
		
	if not os.path.isdir( contentdir ):
		print( "Creating: " + contentdir )
		os.mkdir( contentdir )
		
	# Copy all of our content files
	for file in AssetFiles:
		fromFile = os.path.join( ActualContentDir, file )
		toFile = os.path.join( contentdir, file )
		toDir = os.path.dirname( toFile )
		
		if not os.path.isdir( toDir ):
			os.makedirs( toDir )
			
		shutil.copy( fromFile, toFile )

#6) Copy the assets in compressed form to the requested targets
for target in BuildTypesZip:
	# Content dir
	targetdir = os.path.join( ActualBuildDir, target )
	contentdir = os.path.join( targetdir, "Content" )
	
	print("Copying compressed bundle to: " + contentdir )
	
	if not os.path.isdir( targetdir ):
		exit( "Could not locate build directory for " + target )
		
	if not os.path.isdir( contentdir ):
		print( "Creating: " + contentdir )
		os.mkdir( contentdir )
		
	# Create zip
	zip = zipfile.ZipFile( os.path.join( contentdir, "content.bundle" ), "w", zipfile.ZIP_DEFLATED, True )
	zip.comment = "Dungeon Crawler Content Bundle"
	
	for file in AssetFiles:
		absolutePath = os.path.join( ActualContentDir, file )
		zip.write( absolutePath, file, zipfile.ZIP_DEFLATED )
			
	zip.close()