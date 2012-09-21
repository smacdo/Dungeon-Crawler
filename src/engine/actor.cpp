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
#include "engine/actor.h"
#include "engine/actordata.h"
#include "game/level.h"
#include "common/point.h"
#include "common/utils.h"

#include <memory>

/**
 * Actor constructor
 */
Actor::Actor( ActorData * pActorData )
    : mpData( pActorData )
{
    assert( pActorData != NULL );
}

/**
 * Actor destructor
 */
Actor::~Actor()
{
    Delete( mpData );
}

/**
 * Tells the actor to update itself
 */
void Actor::update()
{
    // empty
}

/**
 * Return our position
 */
Point Actor::position() const
{
    return mpData->position();
}

/**
 * Checks if the given position is a valid tile for the actor to reside in.
 * This makes no checks to see if an actor can actually move to the tile
 * however!
 */
bool Actor::isValidPosition( const Point& point )
{
    Tile tile = mpData->activeLevel().tileAt( point );
    return ( tile.isImpassable() == false && tile.isWall() == false );
}

/**
 * Set the actor's position. Note that this position he is being placed at must
 * be a valid position for him
 */
bool Actor::setPosition( const Point& point )
{
    bool status = false;

    // Is the destination path valid?
    if ( isValidPosition(point) )
    {
        mpData->setPosition( point );
        status = true;
    }
    else
    {
        // failed! log it
    }

    return status;
}

/**
 * Return a shared pointer to the level that this actor is residing in
 */
Level& Actor::activeLevel()
{
    return mpData->activeLevel();
}

/**
 * Return a shared constant pointer to the level that this actor is residing
 * in
 */
const Level& Actor::activeLevel() const
{
    return mpData->activeLevel();
}
