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
#ifndef SCOTT_DUNGEON_WORLD_H
#define SCOTT_DUNGEON_WORLD_H

#include <memory>
#include <boost/utility.hpp>

#include "common/point.h"

class Dungeon;
class Level;
class Actor;

/**
 * World is the class that is responsible for maintaing the state of
 * the entire game world, as well as high level simulation functionality.
 *
 * We might want to split this up in the future into a class for storing
 * the state of the game world, and a class for simulating it.
 */
class World : boost::noncopyable
{
public:
    World( Dungeon* mainDungeon );
    ~World();

    void addPlayer( std::shared_ptr<Actor> pActor );

    void simulate( size_t sliceCount );

    std::shared_ptr<Dungeon> mainDungeon();
    std::shared_ptr<const Dungeon> mainDungeon() const;
    std::shared_ptr<Level> spawnLevel();
    std::shared_ptr<Actor> activePlayer();
    std::shared_ptr<const Actor> activePlayer() const;

    Point spawnPoint() const;


protected:

private:
    // The player who is playing this game
    std::shared_ptr<Actor> mpPlayer;

    // Right now we're only keeping track of one dungeon
    std::shared_ptr<Dungeon> mpMainDungeon;

    // The level that players spawn into
    std::shared_ptr<Level> mpSpawnLevel;

    // Players spawn at this position (in the main dungeon)
    Point mSpawnPoint;

};

#endif

