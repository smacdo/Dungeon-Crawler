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
