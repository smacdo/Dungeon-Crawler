# python
from funcs import *

# Atlas definitions go here
Everything = [
	{
		'sheetname': 'MalePlayer',
		'spritewidth': 64,
		'spriteheight': 64,
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
	{
		'sheetname': 'Skeleton',
		'spritewidth': 64,
		'spriteheight': 64,
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
	}
]

# Go through every atlas
for o in Everything:
	CreateAtlas( o['sheetname'], o['spritewidth'], o['spriteheight'], o['directional'], o['manual'] )