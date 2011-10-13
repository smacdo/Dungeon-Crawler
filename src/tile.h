#ifndef SCOTT_DUNGEON_TILE_H
#define SCOTT_DUNGEON_TILE_H

#include "tiletype.h"
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
    }

    bool isImpassable() const
    {
        return type == TILE_IMPASSABLE;
    }

    // The type of tile
    ETileType type;
};


#endif
