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
#ifndef SCOTT_DUNGEON_ACTOR_H
#define SCOTT_DUNGEON_ACTOR_H

#include <boost/utility.hpp>
#include <memory>

#include "common/point.h"

class Level;
class Point;

/**
 * Repreesnts a creature capable of moving and interacting with the game
 * world
 */
class Actor : boost::noncopyable
{
public:
    Actor( std::shared_ptr<Level> pSpawnLevel, const Point& spawnAt );
    ~Actor();

    // The actor's position
    Point position() const;

    // Set a new position for the actor
    void setPosition( const Point& point );

    // Tells the actor to update itself
    void update();

    // The level that this actor is currently in
    std::shared_ptr<Level> activeLevel();

    // Constant pointer to this actor's level
    std::shared_ptr<const Level> activeLevel() const;

private:
    std::shared_ptr<Level> mpActiveLevel;
    Point mPosition;
};

#endif
