/*
 * Copyright (C) 2012 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#include "worldgen/worldgenerator.h"
#include "worldgen/dungeongenerator.h"

#include "worldgen/worldgenerator.h"
#include "worldgen/dungeongenerator.h"
#include "game/world.h"
#include "common/point.h"

/**
 * Constructor
 */
WorldGenerator::WorldGenerator( size_t maxWidth,
                                size_t maxHeight,
                                unsigned int seed )
    : LEVEL_WIDTH( maxWidth ),
      LEVEL_HEIGHT( maxHeight ),
      RANDOM_SEED( seed )
{
}

/**
 * Destructor
 */
WorldGenerator::~WorldGenerator()
{
}

/**
 * Generates a new world and returns it
 */
World * WorldGenerator::generate( TileFactory& tileFactory ) const
{
    DungeonGenerator generator( tileFactory,
                                LEVEL_WIDTH,
                                LEVEL_HEIGHT,
                                RANDOM_SEED );
    Dungeon *pDungeon = generator.generate();

    // Create a spawn point for the game
//    Point spawnAt  = pDungeon->getLevel( 0 )->stairsUp();

    return new World( pDungeon );
}
