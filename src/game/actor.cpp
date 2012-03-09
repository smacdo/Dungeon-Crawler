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
#include "game/actor.h"
#include "game/level.h"
#include "common/point.h"
#include "common/utils.h"

#include <memory>

/**
 * Actor constructor
 */
Actor::Actor( std::shared_ptr<Level> pSpawnLevel, const Point& spawnPoint )
    : mpActiveLevel( pSpawnLevel ),
      mPosition( spawnPoint )
{
}

/**
 * Actor destructor
 */
Actor::~Actor()
{
}

/**
 * Tells the actor to update itself
 */
void Actor::update()
{
}

/**
 * Return our position
 */
Point Actor::position() const
{
    return mPosition;
}

/**
 * Set the actor's position. Note that this position he is being placed at must
 * be a valid position for him
 */
void Actor::setPosition( const Point& point )
{
    assert( mpActiveLevel && "Actor must be attached to a level" );
    Tile tile = mpActiveLevel->tileAt( point );

    // Is this a valid position for the actor?
    if ( tile.isImpassable() || tile.isWall() )
    {
        App::raiseFatalError( "Attempted to place actor at invalid location" );
    }
    else
    {
        mPosition = point;
    }
}

/**
 * Return a shared pointer to the level that this actor is residing in
 */
std::shared_ptr<Level> Actor::activeLevel()
{
    return mpActiveLevel;
}

/**
 * Return a shared constant pointer to the level that this actor is residing
 * in
 */
std::shared_ptr<const Level> Actor::activeLevel() const
{
    return mpActiveLevel;
}
