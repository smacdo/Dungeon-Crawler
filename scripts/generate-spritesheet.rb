#!/usr/bin/ruby
#########################################################################
### Author: Scott MacDonald
### Takes a directory of .png files and generates a spritesheet along
### with a XML file describing the sprite sheet layout
#########################################################################
require 'RMagick'
require 'find'
require 'builder'

include Magick

###
### Grab arguments from the command line
###
if ARGV.count < 3
    puts "./generate-spritesheet.rb [inputdir] [outputdir] [outputname]"
    exit
end

inputpath  = ARGV[0]
outputpath = ARGV[1]
outputname = ARGV[2]

inputFileName = nil

###
### Output spritesheet dimensions
###
maxHeight    = 1024
maxWidth     = 1024
spriteHeight = 32
spriteWidth  = 32
padWidth     = 0
padHeight    = 0

###
### Use ruby's find command to locate all potential sprite sheet images in the directory
### that the user gave us.
###
### Once we've found a sprite file, add it both to image magick's list of sprites and an
### in-memory XML database containing information on how to look up the sprite in the
### sprite sheet
###
puts "Loading sprite images..."
inputs  = ImageList.new
index   = 0
data    = Array.new

Find.find( inputpath ) do |path|
    if FileTest.directory?( path )
        next    # Allow subdirectories when searching for sprites
    else
        # Only allow .png files to be included, ignore everything else
        if /([A-Za-z0-9_-]+)\.png$/.match( path )
            # Use image magick to query information about this image
            name  = $1
            image = Image.read( path )

            if ( image.length != 1 )
                puts "Failed to read image #{path}"
                exit
            end

            width  = image[0].columns
            height = image[0].rows

            # Make sure dimensions are valid
            if ( width != spriteWidth || height != spriteHeight )
                puts "Invalid width/height for #{path}"
                exit
            end

            # Generate data the XML writer will need
            row = index / spriteWidth
            col = index % spriteWidth
            
            data <<
            {
                :name => name,
                :row  => row,
                :col  => col,
                :x    => col * spriteWidth,
                :y    => row * spriteHeight
            }

            # Also add it to image magick's list of images
            inputs.read path

            # Make sure file counter is updated
            index += 1
        else
            Find.prune
        end
    end
end

###
### Calculate number of rows and columns required
###
numRows = maxHeight / spriteHeight
numCols = maxWidth / spriteWidth

###
### Merge all of the images together into a sprite sheet
###
puts "Generating sprite sheet..."
outputIL = inputs.montage {
    self.geometry = Magick::Geometry.new( spriteWidth,
                                          spriteHeight,
                                          padWidth,
                                          padHeight )
                                        
    self.tile     = Magick::Geometry.new( numCols, numRows )
}

### Verify we only have one output image
if outputIL.length > 1
    puts "Too many sprites, can't fit onto one sprite sheet."
    exit
end

###
### Write the sprite sheet out to disk
###
outputIL.write "#{outputpath}/#{outputname}.png"
puts "Created spritesheet: #{outputpath}/#{outputname}.png"

###
### Write an XML file describing the sprite sheet out to disk
###
xmldoc = Builder::XmlMarkup.new( :indent => 4 )
xmldoc.instruct! :xml, :encoding => "UTF-8"

xmldoc.spritesheet( "file" => "#{outputname}.png" ) do
    data.each do |value|
        xmldoc.sprite( "name" => value[:name],
                       "x"    => value[:x],
                       "y"    => value[:y],
                       "w"    => spriteWidth,
                       "h"    => spriteHeight )
    end
end

# Write it to disk
xmlfile = File.new( "#{outputpath}/#{outputname}.xml", "w" )
xmlfile.print( xmldoc.target! )
xmlfile.close

puts "Created spritetable: #{outputpath}/#{outputname}.xml"

