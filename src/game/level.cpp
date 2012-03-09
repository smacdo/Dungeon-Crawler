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
