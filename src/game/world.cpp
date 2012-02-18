/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
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
#include "game/world.h"
#include "game/dungeon.h"
#include "game/level.h"
#include "game/actor.h"

#include "common/utils.h"

#include <memory>
#include <cassert>

/**
 * World constructor. Create a new world
 */
World::World( Dungeon* mainDungeon )
    : mpPlayer(),
      mpMainDungeon( mainDungeon ),
      mpSpawnLevel( mainDungeon->getLevel( 0 ) )
{
    assert( mpMainDungeon != NULL );
    assert( mpSpawnLevel  != NULL );

    // Get dat spawn point
    mSpawnPoint = mpSpawnLevel->stairsUp();
}

/**
 * Destructor. Blows up the world (oh noes)
 */
World::~World()
{
}

/**
 * Adds a player to the game
 */
void World::addPlayer( std::shared_ptr<Actor> pActor )
{
    mpPlayer = pActor;
}

/**
 * Simulates zero or more time slices
 */
void World::simulate( size_t sliceCount )
{

}

/**
 * Returns a shared pointer to the main dungeon in the world
 */
std::shared_ptr<Dungeon> World::mainDungeon()
{
    return mpMainDungeon;
}

/**
 * Returns a shared pointer to the level that players initially spawn in when
 * they start playing
 */
std::shared_ptr<Level> World::spawnLevel()
{
    return mpSpawnLevel;
}

/**
 * Returns the point at which players should spawn into the default spawn levle
 * for this world
 */
Point World::spawnPoint() const
{
    return mSpawnPoint;
}

/**
 * Returns a shared pointer to the active player
 */
std::shared_ptr<Actor> World::player()
{
    return mpPlayer;
}
