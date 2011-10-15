#ifndef SCOTT_DUNGEON_TILE_H
#define SCOTT_DUNGEON_TILE_H

#include "tiletype.h"
#include "common/platform.h"
#include <cstddef>


struct Tile
{
    Tile()
        : type( TILE_EMPTY )
    {
    }

    Tile( ETileType type_ )
        : type( type_ )
    {
        assert( type_ < ETileType_Count );
    }

    Tile( const Tile& other )
        : type( other.type )
    {
        assert( type < ETileType_Count );
    }

    Tile& operator = ( const Tile& rhs )
    {
        assert( rhs.type < ETileType_Count );
        type = rhs.type;
        return *this;
    }

    bool isImpassable() const
    {
        return type == TILE_IMPASSABLE;
    }

    bool isWall() const
    {
        return type == TILE_WALL;
    }

    bool isFloor() const
    {
        return type == TILE_FLOOR;
    }

    bool wasPlaced() const
    {
        return ( type != TILE_EMPTY );
    }

    // The type of tile
    ETileType type;
};


#endif
