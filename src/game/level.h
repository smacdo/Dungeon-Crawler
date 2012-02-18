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
