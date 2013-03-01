from os import listdir
from os.path import isfile, join

text = ""

contentpath="DungeonCrawler\\DungeonCrawler\\bin\\x86\\Debug\\Content"
source="../DungeonCrawler/DungeonCrawler/bin/x86/$(var.build)/Content"

for file in listdir( contentpath ):
	if isfile( join( contentpath, file ) ):
		text += "<File Id=\"{f}\" Name=\"{f}\" Source=\"{s}/{f}\" />\n".format( f = file, s = source )
		
text += "\n\n\n"

contentpath="DungeonCrawler\\DungeonCrawler\\bin\\x86\\Debug\\Content\\sprites"
source="../DungeonCrawler/DungeonCrawler/bin/x86/$(var.build)/Content/sprites"

for file in listdir( contentpath ):
	if isfile( join( contentpath, file ) ):
		text += "<File Id=\"{f}\" Name=\"{f}\" Source=\"{s}/{f}\" />\n".format( f = file, s = source )

		
xmlfile = open( "content_files_TEMP.txt", 'w' )
xmlfile.write( text )
xmlfile.close()

print("Output written to content_files_TEMP.txt")