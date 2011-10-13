#include "level.h"
#include "tile.h"
#include "tiletype.h"
#include "utils.h"
#include "tilegrid.h"

#include <iostream>
#include <vector>
#include <string>
#include <sstream>
#include <cassert>

Level::Level( const TileGrid& grid )
    : mTileGrid( grid )
{
}

Level::~Level()
{
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

    for ( size_t y = 0; y < mTileGrid.height(); ++y )
    {
        for ( size_t x = 0; x < mTileGrid.width(); ++x )
        {
            const Tile& tile = mTileGrid.get( x, y );

            switch ( tile.type )
            {
                case TILE_IMPASSABLE:
                    ss << 'x';
                case TILE_EMPTY:
                    ss << ' ';
                case TILE_WALL:
                    ss << '#';
                case TILE_FLOOR:
                    ss << '.';
                default:
                    ss << '?';
            }
        }

        ss << '\n';
    }

    return ss.str();
}

int Level::width() const
{
    return mTileGrid.width();
}

int Level::height() const
{
    return mTileGrid.height();
}