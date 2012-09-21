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
#include "game/world.h"
#include "game/dungeon.h"
#include "game/level.h"
#include "engine/actor.h"
#include "engine/actordata.h"

#include "common/utils.h"

#include <memory>
#include <cassert>

/**
 * World constructor. Create a new world
 */
World::World( Dungeon* mainDungeon )
    : mpPlayer(),
      mpMainDungeon( std::shared_ptr<Dungeon>( mainDungeon ) ),
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
    // Sanity checks
    assert( mpPlayer.use_count() == 1 && "No one should still have a ptr" );
}

/**
 * Adds a player to the game
 */
std::shared_ptr<Actor> World::spawnNewPlayer()
{
    mpPlayer =
        std::shared_ptr<Actor>(
            new Actor( new ActorData( mpSpawnLevel, mSpawnPoint ) ) );

    return mpPlayer;
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
Dungeon& World::mainDungeon()
{
    assert( mpMainDungeon && "Dungeon pointer cannot be null" );
    return *mpMainDungeon.get();
}

/**
 * Returns a shared constant pointer to the main dungeon in the world
 */
const Dungeon& World::mainDungeon() const
{
    assert( mpMainDungeon != NULL );
    return *mpMainDungeon;
}

/**
 * Returns a shared pointer to the level that players initially spawn in when
 * they start playing
 */
Level& World::spawnLevel()
{
    assert( mpSpawnLevel != NULL );
    return *mpSpawnLevel;
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
Actor& World::activePlayer()
{
    assert( mpPlayer && "Active player cannot be null" );
    return *mpPlayer.get();
}

/**
 * Returns a shared pointer to the active player
 *
 */
const Actor& World::activePlayer() const
{
    assert( mpPlayer && "Active player cannot be null" );
    return *mpPlayer.get();
}
