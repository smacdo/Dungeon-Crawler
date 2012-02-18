/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
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

