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
