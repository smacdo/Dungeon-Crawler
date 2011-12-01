#ifndef SCOTT_DUNGEON_TILE_H
#define SCOTT_DUNGEON_TILE_H

#include "tiletype.h"
#include <iosfwd>

struct Tile
{
    // Creates an tile of type TILE_EMPTY
    Tile();

    // Create a tile
    Tile( ETileType type_ );

    // Copy constructor
    Tile( const Tile& other );

    // Assignment operator
    Tile& operator = ( const Tile& rhs );

    // Comparison operator
    bool operator == ( const Tile& rhs ) const;

    // Check if tile is impassable
    bool isImpassable() const;

    // Check if tile is wall
    bool isWall() const;

    // Check if tile is floor
    bool isFloor() const;

    // Check if tile has been placed, or is it empty
    bool wasPlaced() const;

    friend std::ostream& operator << ( std::ostream& ss, const Tile& t );

    // The type of tile
    ETileType type;
};

#endif
