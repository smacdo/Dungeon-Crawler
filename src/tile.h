#ifndef SCOTT_DUNGEON_TILE_H
#define SCOTT_DUNGEON_TILE_H

#include "tiletype.h"
#include <cassert>
#include <cstddef>

class Room;

struct Tile
{
    Tile()
        : type( TILE_UNALLOCATED ),
          room( NULL )
    {
    }

    char display() const
    {
        switch ( type )
        {
            case TILE_IMPASSABLE:
                return '*';
            case TILE_UNALLOCATED:
                return ' ';
            case TILE_VOID:
                return '0';
            case TILE_WALL:
                return '#';
            case TILE_FLOOR:
                return '.';
            case TILE_DOOR:
                return '~';
            default:
                return '?';
        }
    }

    void makeImpassable()
    {
        type = TILE_IMPASSABLE;
    }

    // The type of tile
    ETileType type;

    // Room that the tile belongs to
    Room *    room;

    // The amount of light that this tile is receving
    // ( 0 = pitch black, 5 = shadowy, 8 = gloomy light
    //   12 = daylight, 15 = intensely bright )
    int light;
};


#endif
