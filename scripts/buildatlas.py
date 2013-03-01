# python
from funcs import *

# Atlas definitions go here
Everything = [
	#-----------------------------------------------------------------------------------------#
	# Male player
	#-----------------------------------------------------------------------------------------#
	{
		'sheetname': 'MalePlayer',
		'default': 'IdleSouth',
		'sprite_size': [ 64, 64 ],
		'atlas_size': [ 1024, 1024 ],
		'directional': [
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
		],
		'manual': [
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
	},
	#-----------------------------------------------------------------------------------------#
	# Skeleton
	#-----------------------------------------------------------------------------------------#
	{
		'sheetname': 'Skeleton',
		'default': 'IdleSouth',
		'sprite_size': [ 64, 64 ],
		'atlas_size': [ 1024, 1024 ],
		'directional': [
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
		],
		'manual': [
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
	},
	#-----------------------------------------------------------------------------------------#
	# Longsword
	#-----------------------------------------------------------------------------------------#
	{
		'sheetname': 'Longsword',
		'default': 'SlashSouth',
		'sprite_size': [ 192, 192 ],
		'atlas_size': [ 2048, 1024 ],
		'directional': [
			{
				'action': 'Slash',
				'start_col': 0,
				'last_col': 6,
				'file': 'slash.png'
			},
		],
		'manual': [
		]
	}
]

# Go through every atlas
for o in Everything:
	CreateAtlas( o['sheetname'], o['default'], o['sprite_size'], o['atlas_size'], o['directional'], o['manual'] )