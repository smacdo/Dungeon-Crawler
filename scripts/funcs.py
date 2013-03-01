from PIL import Image, ImageDraw
DirectionNames = [ 'North', 'West', 'South', 'East' ]
SpriteDir = "sprites"

def CreateAtlas( sheetName, default, sprite_size, atlas_size, directionalAnimations, manualAnimations ):
	print( "Building: " + sheetName )
	
	maxWidth = atlas_size[0]
	maxHeight = atlas_size[1]
	spriteWidth = sprite_size[0]
	spriteHeight = sprite_size[1]

	xml = """<?xml version="1.0" encoding="UTF-8"?>
<sprite name="{name}" image="{dir}/{name}.png" spriteWidth="{swidth}" spriteHeight="{sheight}">
""".format( name = sheetName, dir = SpriteDir, swidth = spriteWidth, sheight = spriteHeight )

	# Generate an output atlas
	atlasX = 0
	atlasY = 0

	atlas = Image.new( 'RGBA', ( maxWidth, maxHeight ), ( 255, 0, 255, 0 ) )
	draw  = ImageDraw.Draw( atlas )
	draw.rectangle( ( 0, 0, maxWidth, maxHeight ), fill=( 255, 0, 255 ) )

	# Start extracting images from the atlas
	for actionType in directionalAnimations:
		action = actionType['action']
		sourceFileName = sheetName + "/" + actionType['file']
		sourceAtlas = Image.open( sourceFileName )
		
		# Go through each direction
		for directionIndex in range(4):
			directionName = DirectionNames[ directionIndex ]
			offsetY = directionIndex * spriteHeight
			
			# Write the animation header
			animationName = action + directionName
			
			if ( animationName == default ):
				xml += "  <animation name=\"{name}\" default=\"true\">\n".format( name = animationName )
			else:
				xml += "  <animation name=\"{name}\">\n".format( name = animationName )
		
			# Write out each frame in the animation
			for col in range( actionType['start_col'], actionType['last_col'] + 1 ):
				# Coordinates of the sprite in the source atlas
				offsetX = col * spriteWidth
				
				# Extract the sprite from it's source atlas
				sprite = sourceAtlas.crop( ( offsetX,
											 offsetY,
											 offsetX + spriteWidth,
											 offsetY + spriteHeight ) )
				
				# Pack it the sprite into the output atlas, and keep track of the coordinates
				if ( atlasX + spriteWidth > maxWidth ):
					atlasX  = 0
					atlasY += spriteHeight

				if ( atlasY + spriteHeight > maxHeight ):
					raise Exception( "Exceed sprite atlas height" )
					
				atlas.paste( sprite, ( atlasX,
									   atlasY,
									   atlasX + spriteWidth,
									   atlasY + spriteHeight ) )
				
				# Write the XML
				xml += "   <frame x=\"{x}\" y=\"{y}\" />\n".format( x = atlasX, y = atlasY )
													   
				atlasX += spriteWidth
		
			# Write animation footer
			xml += "  </animation>\n"
			
	# Now extract any manually defined animations
	for animation in manualAnimations:
		# Open the sprite atlas
		sourceFileName = sheetName + "/" + animation['file']
		sourceAtlas    = Image.open( sourceFileName )
		
		# Write the animation header
		animationName = animation['name']
		
		if ( animationName == default ):
			xml += "  <animation name=\"{name}\" default=\"true\">\n".format( name = animationName )
		else:
			xml += "  <animation name=\"{name}\">\n".format( name = animationName )
		
		# Iterate through all the animation frames
		for frame in animation['frames']:
			# Coordinates of the sprite in the source atlas
			x = frame[0]
			y = frame[1]
			offsetX = col * spriteWidth
			
			# Extract the sprite from it's source atlas
			sprite = sourceAtlas.crop( ( offsetX,
										 offsetY,
										 offsetX + spriteWidth,
										 offsetY + spriteHeight ) )
			
			# Pack it the sprite into the output atlas, and keep track of the coordinates
			if ( atlasX + spriteWidth > maxWidth ):
				atlasX  = 0
				atlasY += spriteHeight

			if ( atlasY + spriteHeight > maxHeight ):
				raise Exception( "Exceed sprite atlas height" )
				
			atlas.paste( sprite, ( atlasX,
								   atlasY,
								   atlasX + spriteWidth,
								   atlasY + spriteHeight ) )
			
			# Write the XML
			xml += "   <frame x=\"{x}\" y=\"{y}\" />\n".format( x = atlasX, y = atlasY )
						   
			atlasX += spriteWidth
		
		# XML animation footer
		xml += "  </animation>\n"
		
	# XML sprite footer
	xml += "</sprite>"

	atlas.save( sheetName + ".png" )

	xmlfile = open( sheetName + ".sprite", 'w' )
	xmlfile.write( xml )
	xmlfile.close()