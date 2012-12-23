# python
from PIL import Image, ImageDraw

# Atlas layouts
DirectionNames = [ 'North', 'West', 'South', 'East' ]
SheetName = 'MalePlayer'
SpriteWidth = 64
SpriteHeight = 64
AtlasImages = [
	{
		'action': 'Idle',
		'start_col': 0,
		'last_col': 0,
		'file': 'walk.png'
	},
	{
		'action': 'Walk',
		'start_col': 1,
		'last_col': 8,
		'file': 'walk.png'
	},
	{
		'action': 'Thrust',
		'start_col': 0,
		'last_col': 7,
		'file': 'thrust.png'
	},
	{
		'action': 'Slash',
		'start_col': 0,
		'last_col': 5,
		'file': 'slash.png'
	},
	{
		'action': 'Spell',
		'start_col': 0,
		'last_col': 6,
		'file': 'spell.png'
	},
	{
		'action': 'Bow',
		'start_col': 0,
		'last_col': 12,
		'file': 'bow.png'
	}
#	{
#		'file': 'hurt.png'
#	}
]

ManualAnimations = [
	{
		'file': 'hurt.png',
		'name': 'Hurt',
		'frames': [ [ 0 * 64, 0 ],
		            [ 1 * 64, 0 ],
					[ 2 * 64, 0 ],
					[ 3 * 64, 0 ],
					[ 4 * 64, 0 ],
					[ 5 * 64, 0 ] ]
	}
]

# XML file
xml = """
<?xml version="1.0"? encoding="UTF-8"?>
<sprite name="{name}" image="{name}.png" spriteWidth="{swidth}" spriteHeight="{sheight}">
""".format( name = SheetName, swidth = SpriteWidth, sheight = SpriteHeight )

# Generate an output atlas
atlasMaxSize = 1024
atlasX = 0
atlasY = 0

atlas = Image.new( 'RGBA', ( atlasMaxSize, atlasMaxSize ), ( 255, 0, 255, 0 ) )
draw  = ImageDraw.Draw( atlas )
draw.rectangle( ( 0, 0, atlasMaxSize, atlasMaxSize ), fill=( 255, 0, 255 ) )

# Start extracting images from the atlas
for actionType in AtlasImages:
	action = actionType['action']
	sourceFileName = SheetName + "/" + actionType['file']
	sourceAtlas = Image.open( sourceFileName )
	
	# Go through each direction
	for directionIndex in range(4):
		directionName = DirectionNames[ directionIndex ]
		offsetY = directionIndex * SpriteHeight
		
		# Write the animation header
		xml += "  <animation name=\"{name}{dir}\">\n".format( name = action, dir = directionName )
	
		# Write out each frame in the animation
		for col in range( actionType['start_col'], actionType['last_col'] + 1 ):
			# Coordinates of the sprite in the source atlas
			offsetX = col * SpriteWidth
			
			# Extract the sprite from it's source atlas
			sprite = sourceAtlas.crop( ( offsetX,
			                             offsetY,
			                             offsetX + SpriteWidth,
										 offsetY + SpriteHeight ) )
			
			# Pack it the sprite into the output atlas, and keep track of the coordinates
			if ( atlasX + SpriteWidth > atlasMaxSize ):
				atlasX  = 0
				atlasY += SpriteHeight

			if ( atlasY + SpriteHeight > atlasMaxSize ):
				raise Exception( "Exceed sprite atlas height" )
				
			atlas.paste( sprite, ( atlasX,
			                       atlasY,
								   atlasX + SpriteWidth,
								   atlasY + SpriteHeight ) )
								   
			atlasX += SpriteWidth
			
			# Write the XML
			xml += "   <frame x=\"{x}\" y=\"{y}\" />\n".format( x = offsetX, y = offsetY )
	
		# Write animation footer
		xml += "  </animation>\n"
		
# Now extract any manually defined animations
for animation in ManualAnimations:
	# Open the sprite atlas
	sourceFileName = SheetName + "/" + animation['file']
	sourceAtlas    = Image.open( sourceFileName )
	
	# XML animation ehader
	xml += "  <animation name=\"{name}\">\n".format( name = animation['name'] )
	
	# Iterate through all the animation frames
	for frame in animation['frames']:
		# Coordinates of the sprite in the source atlas
		x = frame[0]
		y = frame[1]
		offsetX = col * SpriteWidth
		
		# Extract the sprite from it's source atlas
		sprite = sourceAtlas.crop( ( offsetX,
									 offsetY,
									 offsetX + SpriteWidth,
									 offsetY + SpriteHeight ) )
		
		# Pack it the sprite into the output atlas, and keep track of the coordinates
		if ( atlasX + SpriteWidth > atlasMaxSize ):
			atlasX  = 0
			atlasY += SpriteHeight

		if ( atlasY + SpriteHeight > atlasMaxSize ):
			raise Exception( "Exceed sprite atlas height" )
			
		atlas.paste( sprite, ( atlasX,
							   atlasY,
							   atlasX + SpriteWidth,
							   atlasY + SpriteHeight ) )
							   
		atlasX += SpriteWidth
		
		# Write the XML
		xml += "   <frame x=\"{x}\" y=\"{y}\" />\n".format( x = offsetX, y = offsetY )
	
	# XML animation footer
	xml += "  </animation>\n"
	
# XML sprite footer
xml += "</sprite>"

atlas.save( SheetName + ".png" )
print( xml )