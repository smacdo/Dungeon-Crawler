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

class ActorData;
class Point;
class Level;

/**
 * Represents an interactive and controllable actor. This forms the controller
 * portion of an actor instance
 */
class Actor : boost::noncopyable
{
public:
    Actor( ActorData * pActorData );
    ~Actor();

    // The actor's position
    Point position() const;

    bool isValidPosition( const Point& point );

    // Set a new position for the actor
    bool setPosition( const Point& point );

    // Tells the actor to update itself
    void update();

    // The level that this actor is currently in
    Level& activeLevel();

    // Constant pointer to this actor's level
    const Level& activeLevel() const;

private:
    ActorData * mpData;
};

#endif
