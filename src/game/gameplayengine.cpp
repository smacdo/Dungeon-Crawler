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
#include "game/gameplayengine.h"
#include "game/tilefactory.h"
#include "game/actor.h"
#include "game/world.h"
#include "game/level.h"
#include "game/playerinputcontroller.h"
#include "game/actorcontroller.h"

#include "worldgen/worldgenerator.h"

#include "common/utils.h"

namespace
{
    const unsigned int MAX_LEVEL_WIDTH  = 76;
    const unsigned int MAX_LEVEL_HEIGHT = 50;
    const unsigned int DEFAULT_SEED     = 42;
}

/**
 * Game play engine constructor
 */
GamePlayEngine::GamePlayEngine( PlayerInputController& inputController )
    : mpWorld( NULL ),
      mpTileFactory( new TileFactory ),
      mInputController( inputController ),
      mpPlayerActor()
{

}

/**
 * Game play engine destructor
 */
GamePlayEngine::~GamePlayEngine()
{
    Delete( mpWorld );
    Delete( mpTileFactory );
}

/**
 * Runs the world simulation
 */
void GamePlayEngine::simulate()
{
    assert( mpWorld       && "Cannot simulate game without a world" );
    assert( mpPlayerActor && "Must have an active player to simulate game" );
    

    // Update all actor controllers (both the player's input and the game's
    // AI controllers)
    

    // Now simulate the game world
    //mpWorld->simulate( 1 );
}

/**
 * Creates a new world and spawn the player's character into this beautiful
 * and brave new world
 */
void GamePlayEngine::createNewWorld()
{
    WorldGenerator worldGen( MAX_LEVEL_WIDTH,
                             MAX_LEVEL_HEIGHT,
                             DEFAULT_SEED );

    // Generate a new game world to play in
    mpWorld = worldGen.generate( deref( mpTileFactory) );
    assert( mpWorld != NULL );

    // Spawn the player's character and wire him (or her) in
    mpPlayerActor.reset( new Actor( mpWorld->spawnLevel(),
                                    mpWorld->spawnPoint() ) );

    mpWorld->addPlayer( mpPlayerActor );

    // Update the player controller
    mInputController.attachTo( mpPlayerActor );
}

/**
 * Returns a reference to the active game world
 */
const World& GamePlayEngine::activeWorld() const
{
    return deref( mpWorld );
}
