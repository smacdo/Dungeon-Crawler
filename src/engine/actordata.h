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
#ifndef SCOTT_DUNGEON_ACTORDATA_H
#define SCOTT_DUNGEON_ACTORDATA_H

#include <boost/utility.hpp>
#include <memory>

#include "common/point.h"

class Level;
class Point;

/**
 * Contains data describing an individual actor and the template from
 * which it was generated
 */
class ActorData : boost::noncopyable    // maybe change this
{
public:
    ActorData( std::shared_ptr<Level> pSpawnLevel, const Point& spawnAt );
    ~ActorData();

    // The actor's position
    Point position() const;

    // Set a new position for the actor
    void setPosition( const Point& point );

    // The level that this actor is currently in
    Level& activeLevel();

    // Constant pointer to this actor's level
    const Level& activeLevel() const;

private:
    /// Level that the actor is in
    std::shared_ptr<Level> mpActiveLevel;

    /// Location of the actor in the level
    Point mPosition;

    /// Name of the actor
    std::string mName;
};

#endif
