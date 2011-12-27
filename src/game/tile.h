#ifndef SCOTT_DUNGEON_TILE_H
#define SCOTT_DUNGEON_TILE_H

#include <iosfwd>
#include <stdint.h>
struct TileData;

/**
 * Tile represents a terrain tile in the level gridmap
 */
class Tile
{
public:
    // Default constructor
    Tile();

    // Instantiate a new tile
    explicit Tile( TileData *pTileData );

    // Copy constructor
    Tile( const Tile& other );

    // Assignment operator
    Tile& operator = ( const Tile& rhs );

    // Comparison operator
    bool operator == ( const Tile& rhs ) const;

    bool isPlaced() const;

    // Check if tile is impassable
    bool isImpassable() const;

    // Check if this tile is considered a wall
    bool isWall() const;

    // Check if this tile is considered a floor
    bool isFloor() const;

    friend std::ostream& operator << ( std::ostream& ss, const Tile& t );

private:
    // Pointer back to this tile's immutable tile data
    //   (constant for the moment, we may need to change this when we
    //    implement tile swapping)
    const TileData * mpData;

    // Light level of the tile (eventually switch this to a special light
    // class that can calculate blended lights)
    uint8_t mLightLevel;
};

#endif
