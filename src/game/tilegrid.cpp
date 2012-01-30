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
#include "tilegrid.h"
#include "tile.h"
#include "common/rect.h"
#include "common/platform.h"

#include <ostream>

/**
 * Constructor. Creates a blank tile grid with the requested dimensions and
 * popualted with void tiles
 *
 * \param   width   Width of the tile grid
 * \height  height  Height of the tile grid
 */
TileGrid::TileGrid( size_t width, size_t height )
    : FixedGrid<Tile>( width, height, Tile() )
{
}

/**
 * Constructor. Creates a tile grid with the requested dimensions and populated
 * with copies of the given tile template
 *
 * \param  width   Width of the tile grid
 * \param  height  Height of the tile grid
 */
TileGrid::TileGrid( size_t width, size_t height, const Tile& defaultTile )
    : FixedGrid<Tile>( width, height, defaultTile )
{
}

/**
 * Tile grid copy constructor
 *
 * \param  source  The tile grid to copy values from
 */
TileGrid::TileGrid( const TileGrid& source )
    : FixedGrid<Tile>( source )
{
    // Empty
}

/**
 * Tile grid destructor
 */
TileGrid::~TileGrid()
{
    // Empty
}

/**
 * Checks if all the tiles inside of the provided rectangle are considered
 * "empty", which means that the dungeon generator has not placed a non-void
 * tile.
 *
 * \param  area  The area inside the tile grid to check
 * \return       True if all tiles inside of the rectangle are empty
 */
bool TileGrid::isAreaEmpty( const Rect& area ) const
{
    Rect gridBounds( 0, 0, mWidth, mHeight );
    bool isEmpty = true;

    // Make sure the search area fits inside of the tile grid
    assert( gridBounds.contains( area ) );

    // Search each tile to see if it fits
    for ( int iy = 0; iy < area.height() && isEmpty; ++iy )
    {
        for ( int ix = 0; ix < area.width() && isEmpty; ++ix )
        {
            // Find actual coordinates
            int x = ix + area.x();
            int y = iy + area.y();

            // Is there anything here?
            isEmpty = (mTiles[ offset(x,y) ].isPlaced() == false);
        }
    }

    return isEmpty;
}

/**
 * Carves a rectangular room into the tilegrid, consisting of a solid area
 * of floor surrounded on all four sides with wall.
 *
 * \param  area           Area to transform into floor
 * \param  wallWidth      Width of the walls to surround the floor with
 * \param  wallTemplate   Each wall should be a copy of this tile data
 * \param  floorTemplate  Each floor should be a copy of this tile data
 */
void TileGrid::carveRoom( const Rect& floorArea,
                          int wallWidth,
                          const Tile& wallTemplate,
                          const Tile& floorTemplate )
{
    Rect gridBounds( 0, 0, mWidth, mHeight );
    Rect carveBounds( floorArea.x() - wallWidth, floorArea.y() - wallWidth,
                      floorArea.width() + 2 * wallWidth,
                      floorArea.height() + 2 * wallWidth );

    // Make sure the carving is within the room's boundaries
    assert( floorArea.isNull() == false );
    assert( wallWidth == 1 );       // need to update method to support > 1
    assert( gridBounds.contains( carveBounds ) );

    // First step is to carve the walls out
    for ( int x = carveBounds.x(); x < carveBounds.right(); ++x )
    {
        set( x, carveBounds.top(), wallTemplate );
        set( x, carveBounds.bottom()-1, wallTemplate );
    }

    for ( int y = carveBounds.y(); y < carveBounds.bottom(); ++y )
    {
        set( carveBounds.left(), y, wallTemplate );
        set( carveBounds.right()-1, y, wallTemplate );
    }

    // Second step is to carve the floor tiles out
    for ( int y = floorArea.y(); y < floorArea.bottom(); ++y )
    {
        for ( int x = floorArea.x(); x < floorArea.right(); ++x )
        {
            set( x, y, floorTemplate );
        }
    }
}

/**
 * Adds an overlapping room into the tile grid. What this does is insert the
 * room's tiles into our tile grid. However, if the insertion would insert a
 * tile where a floor tile already exists, then the previous floor tile will 
 * be kept. That way we don't build walls into pre-existing rooms
 * 
 * TODO: Maybe we want to do this as well? Add it as a flag and perhaps
 *       merge behaviors?
 */
void TileGrid::carveOverlappingRoom( const Rect& floorArea,
                                     int wallWidth,
                                     const Tile& wallTemplate,
                                     const Tile& floorTemplate )
{
    Rect destBounds( 0, 0, mWidth, mHeight );
    Rect carveBounds( floorArea.x() - 1, floorArea.y() - 1,
                      floorArea.width() + 2, floorArea.height() + 2 );

    // Verify that the source grid will fit in us
    assert( destBounds.contains( carveBounds ) );

    // First step is to carve the walls out and take care not to place a wall
    // where there is already a floor in place
    for ( int x = carveBounds.x(); x < carveBounds.right(); ++x )
    {
        Tile& t = get( x, carveBounds.top() );
        Tile& b = get( x, carveBounds.bottom() - 1 );

        if ( t.isFloor() == false )
        {
            set( x, carveBounds.top(), wallTemplate );
        }

        if ( b.isFloor() == false )
        {
            set( x, carveBounds.bottom()-1, wallTemplate );
        }
    }

    for ( int y = carveBounds.y(); y < carveBounds.bottom(); ++y )
    {
        Tile& l = get( carveBounds.left(), y );
        Tile& r = get( carveBounds.right()-1, y );

        if (! l.isFloor() )
        {
            set( carveBounds.left(), y, wallTemplate );
        }

        if (! r.isFloor() )
        {
            set( carveBounds.right()-1, y, wallTemplate );
        }
    }

    // Second step is to carve the floor tiles out
    for ( int y = floorArea.y(); y < floorArea.bottom(); ++y )
    {
        for ( int x = floorArea.x(); x < floorArea.right(); ++x )
        {
            set( x, y, floorTemplate );
        }
    }
}

/**
 * Stream output operator
 */
std::ostream& operator << ( std::ostream& ss, const TileGrid& grid )
{
    ss << "\n";

    for ( int y = 0; y < grid.height(); ++y )
    {
        for ( int x = 0; x < grid.width(); ++x )
        {
            ss << grid.get( x, y );
        }

        ss << "\n";
    }

    return ss;
}
