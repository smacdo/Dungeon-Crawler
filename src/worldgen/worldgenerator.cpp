/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
