#include "room.h"
#include "level.h"
#include "tile.h"
#include "utils.h"

#include <iostream>
#include <vector>
#include <string>

#include <cassert>
#include <time.h>
#include <stdlib.h>

Room::Room( size_t topX, size_t topY,
            size_t width, size_t height,
            Level *level )
    : mTopX( topX ),
      mTopY( topY ),
      mWidth( width ),
      mHeight( height ),
      mLevel( level )
{
    assert( level != NULL );
    assert( topX + width <= level->width() );
    assert( topY + height <= level->height() );
    assert( width > 0 );
    assert( height > 0 );
}

Room::~Room()
{
}

/**
 * Count the number of the given tile type that exist in this room
 */
size_t Room::numTilesOf( ETileType type ) const
{
    size_t numFound = 0;

    for ( size_t r = 0; r < mHeight; ++r )
    {
        for ( size_t c = 0; c < mWidth; ++c )
        {
            Tile tile = mLevel->getTileAt( r + mTopY, c + mTopX );

            if ( tile.type == type )
            {
                numFound++;
            }
        }
    }

    return numFound;
}

void Room::carveRect( size_t x, size_t y,
                      size_t width, size_t height,
                      ETileType wall,
                      ETileType floor )
{
    // Make sure the carving is within the room's boundaries
    assert( x >= mTopX );
    assert( y >= mTopY );
    assert( x + width  <= mTopX + mWidth );
    assert( y + height <= mTopY + mHeight );
    assert( width > 0 );
    assert( height > 0 );

    // Now carve out the wall and floor tiles
    for ( size_t r = 0; r < height; ++r )
    {
        for ( size_t c = 0; c < width; ++c )
        {
            size_t row = r + y;
            size_t col = c + x;
            Tile* tile = mLevel->tileAt( row, col );

            // Make sure this tile has no room owner, or it is unallocated
            assert( tile->room == NULL || tile->room == this );

            // Is this the border or the inner portion?
            if ( r == 0 || r == (height-1) ||
                 c == 0 || c == (width-1) )
            {
                tile->type = wall;
            }
            else
            {
                tile->type = floor;
            }

            // Properly assign the other fields
            tile->room = this;
        }
    }
}
size_t Room::offset( size_t row, size_t col ) const
{
    assert( row < mHeight );
    assert( col < mWidth );
    return row * mWidth + col;
}
