#!/usr/bin/ruby
#########################################################################
### Author: Scott MacDonald
### Takes a directory of .png files and generates a spritesheet along
### with a XML file describing the sprite sheet layout
#########################################################################
require 'RMagick'
require 'find'
include Magick

###
### Grab arguments from the command line
###
inputpath  = "tiles/dungeon"
outputpath = "output"
outputname = "dungeon"

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
### Find all the images that will comprise the spritesheet
###
puts "Loading sprite images..."
inputs = ImageList.new

Find.find( inputpath ) do |path|
    if FileTest.directory?( path )
        # Allow sub directories when searching
        next
    else
        # Only allow .png files to be included, ignore everything else
        if /[A-Za-z0-9_-]+\.png$/.match( path )
            # Add it to the image list
            inputs.read path
        else
            puts "IGNORING: #{path}"
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

### Write the sprite sheet out to disk
outputIL.write "#{outputpath}/#{outputname}.png"
