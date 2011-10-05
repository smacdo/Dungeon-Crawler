#include "level.h"
#include "tile.h"
#include "tiletype.h"
#include "utils.h"
#include "room.h"

#include <iostream>
#include <vector>

#include <cassert>

Level::Level( size_t width, size_t height )
    : mWidth( width ), mHeight( height )
{
    init();
}

Level::~Level()
{
    delete[] mTiles;
}

Room* Level::addRectangleRoom( size_t x, size_t y,
                               size_t width, size_t height,
                               ETileType wall,
                               ETileType floor )
{
    // make sure the room fits in the borders
    assert( x + width  <= mWidth  && mWidth  > 0 );
    assert( y + height <= mHeight && mHeight > 0 );

    // make sure the area is clean
    assert( hasAllocatedTiles( x, y, width, height ) == false );

    // Allocate a room to hold this new rectangle room
    Room * pRoom = new Room( x, y, width, height, this );
    mRooms.push_back( pRoom );

    // Now carve the room out
    pRoom->carveRect( x, y, width, height, wall, floor );

    return pRoom;
}
    
Tile Level::getTileAt( size_t row, size_t col ) const
{
    assert( col < mWidth  );
    assert( row < mHeight );
    
    return mTiles[ offset(row,col) ];
}

Tile* Level::tileAt( size_t row, size_t col )
{
    assert( col < mWidth );
    assert( row < mHeight );

    return &mTiles[ offset(row,col) ];
}

bool Level::hasAllocatedTiles( size_t x, size_t y,
                               size_t width, size_t height ) const
{
    bool hasOwner = false;

    for ( size_t row = 0; row < height && (!hasOwner); ++row )
    {
        for ( size_t col = 0; col < width && (!hasOwner); ++col )
        {
            size_t i = offset( row + y, col + x );

            // Make sure the tile is eithe rblocked or unallocated for it
            // to have no owner
            hasOwner = ( mTiles[i].type != TILE_BLOCKED &&
                         mTiles[i].type != TILE_UNALLOCATED );

            // just for sanity, it can't have a room
            assert( hasOwner || mTiles[i].room == NULL );
        }
    }

    return hasOwner;
}

void Level::print() const
{
    for ( size_t row = 0; row < mHeight; ++row )
    {
        for ( size_t col = 0; col < mWidth; ++col )
        {
            std::cout << mTiles[offset(row,col)].display();
        }

        std::cout << std::endl;
    }
}

void Level::init()
{
    mTiles = new Tile[ mHeight * mWidth ];
}

size_t Level::offset( size_t row, size_t col ) const
{
    assert( row < mHeight );
    assert( col < mWidth );
    return row * mWidth + col;
}

