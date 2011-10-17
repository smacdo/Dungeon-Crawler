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
#include "tilegrid.h"
#include "tile.h"
#include "common/rect.h"
#include "common/platform.h"


TileGrid::TileGrid( size_t width, size_t height )
    : FixedGrid( width, height, Tile( TILE_EMPTY ) )
{
}

TileGrid::TileGrid( const TileGrid& tileGrid )
    : FixedGrid( tileGrid )
{
}

TileGrid::~TileGrid(void)
{
}

/**
 * Checks if all the tiles in the requested area are empty
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
            isEmpty = (mTiles[ offset(x, y) ].wasPlaced() == false);
        }
    }

    return isEmpty;
}

void TileGrid::carveRoom( const Rect& area,
                          const Tile& wallTemplate,
                          const Tile& floorTemplate )
{
    Rect gridBounds( 0, 0, mWidth, mHeight );

    // Make sure the carving is within the room's boundaries
    assert( area.isNull() == false );
    assert( gridBounds.contains( area ) );

    // Now carve out the wall and floor tiles
    for ( int iy = 0; iy < area.height(); ++iy )
    {
        for ( int ix = 0; ix < area.width(); ++ix )
        {
            // We cannot in a restricted tile

            // Is this the border or the inner portion?
            if ( iy == 0 || iy == (area.height()-1) ||
                 ix == 0 || ix == (area.width()-1) )
            {
                set( ix + area.x(), iy + area.y(), wallTemplate );
            }
            else
            {
                set( ix + area.x(), iy + area.y(), floorTemplate );
            }
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
void TileGrid::carveOverlappingRoom( const Rect& sourceBounds,
                                     const Tile& wallTemplate,
                                     const Tile& floorTemplate )
{
    Rect destBounds( 0, 0, mWidth, mHeight );

    // Verify that the source grid will fit in us
    assert( destBounds.contains( sourceBounds ) );

    // Now copy the tiles over
    for ( int sy = 0; sy < sourceBounds.height(); ++sy )
    {
        for ( int sx = 0; sx < sourceBounds.width(); ++sx )
        {
            // Calculate array index offsets
            size_t si = ( sy * sourceBounds.width() + sx ); 
            size_t di = this->offset( sx + sourceBounds.x(),
                                      sy + sourceBounds.y() );

            // Should we be placing a wall here, or floor?
            bool placeWall = ( sy == 0 || sy == ( sourceBounds.height()-1 ) ||
                               sx == 0 || sx == ( sourceBounds.width() -1 ) );

            // Attempt to place the correct tile down (either wall or floor
            // depending on position), but make sure that we are not putting
            // a wall where there is already a floor tile in place
            if ( placeWall )
            {
                if ( mTiles[di].isFloor() )
                {
                    continue;       // refuse
                }
                
                mTiles[di] = wallTemplate;
            }
            else
            {
                mTiles[di] = floorTemplate;
            }
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
void TileGrid::addOverlappingRoom( const Point& upperLeft,
                                   const TileGrid& roomGrid )
{
    Rect destBounds( 0, 0, mWidth, mHeight );
    Rect sourceBounds( upperLeft, roomGrid.mWidth, roomGrid.mHeight );

    // Verify that the source grid will fit in us
    assert( destBounds.contains( sourceBounds ) );

    // Now copy the tiles over
    for ( int sy = 0; sy < roomGrid.mHeight; ++sy )
    {
        for ( int sx = 0; sx < roomGrid.mWidth; ++sx )
        {
            size_t si = roomGrid.offset( sx, sy );
            size_t di = this->offset( sx + upperLeft.x(),
                                      sy + upperLeft.y() );

            // Do not copy if the source tile is a wall, and the destination
            // already has a tile
            Tile& sourceTile = roomGrid.mTiles[si];

            if ( sourceTile.isWall() && mTiles[di].wasPlaced() )
            {
                continue;
            }
            else
            {
                mTiles[di] = roomGrid.mTiles[si];
            }
        }
    }
}