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
#include "game/level.h"
#include "game/tile.h"
#include "game/tiletype.h"
#include "common/utils.h"

#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include <cassert>

/**
 * Level constructor
 *
 * \param  levelName  Name of the levle
 * \param  grid       The tile grid that represents this level
 * \param  stairsUp   Location of the stairs up
 */
Level::Level( const std::string& levelName,
              const TileGrid& grid,
              const Point& stairsUp )
    : mName( levelName ),
      mTileGrid( grid ),
      mStairsUp( stairsUp )
{
    assert( mTileGrid.get( mStairsUp ).tileid() == ETILETYPE_STAIRS_UP );
    // empty
}

/**
 * Destructor
 */
Level::~Level()
{
    // empty
}

/**
 * Return a reference to the tile stored at the requested position
 *
 * \param  p  The position to look up
 * \return    Reference to the tile
 */
Tile& Level::tileAt( const Point& p )
{
    return mTileGrid.get( p );
}

/**
 * Return a constant reference to the tile stored at the requested
 * position
 *
 * \param  p  The position to look up
 * \return    Constant reference to the tile
 */
const Tile& Level::tileAt( const Point& p ) const
{
    return mTileGrid.get( p );
}

/**
 * Returns the contents of the level in string form
 *
 * \return String representing the level
 */
std::string Level::dump() const
{
    std::stringstream ss;

    for ( int y = 0; y < mTileGrid.height(); ++y )
    {
        for ( int x = 0; x < mTileGrid.width(); ++x )
        {
            ss << mTileGrid.get( x, y );
        }

        ss << '\n';
    }

    return ss.str();
}

/**
 * Returns the name of the level
 */
std::string Level::name() const
{
    return mName;
}

/**
 * Returns the width of the level
 */
size_t Level::width() const
{
    return mTileGrid.width();
}

/**
 * Returns the height of the level
 */
size_t Level::height() const
{
    return mTileGrid.height();
}

/**
 * Return the location of the stairs up. This should be changed
 */
Point Level::stairsUp() const
{
    return mStairsUp;
}
