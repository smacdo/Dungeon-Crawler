#include "tilegrid.h"
#include "tile.h"
#include "common/rect.h"

#include <cassert>


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

void TileGrid::carveTiles( const Rect& area,
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