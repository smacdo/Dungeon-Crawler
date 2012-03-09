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
#ifndef SCOTT_DUNGEON_LEVEL_H
#define SCOTT_DUNGEON_LEVEL_H

#include "tilegrid.h"

#include <string>
#include <boost/utility.hpp>

class Tile;

/**
 * This class represents a playable game level in dungeon crawler. It
 * contains the level's terrain tiles, actors, items and anything else
 * relating to its current state
 */
class Level : boost::noncopyable
{
public:
    // Level constructor
    Level( const std::string& levelName,
           const TileGrid& tileGrid,
           const Point& stairsUp );

    // Level destructor
    ~Level();

    // Return a reference to a requested tile
    Tile& tileAt( const Point& p );

    // Return a const reference to a requested tile
    const Tile& tileAt( const Point& p ) const;

    // Dump information about this level
    std::string dump() const;

    // Return the name of this level
    std::string name() const;

    // Return the width of this level
    size_t width() const;

    // Return the height of this level
    size_t height() const;

    // Temp accessor
    Point stairsUp() const;

private:
    std::string mName;
    TileGrid mTileGrid;
    Point mStairsUp;
};

#endif
