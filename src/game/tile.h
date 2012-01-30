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

    // Returns the ID of this tile's type
    unsigned int tileid() const;

    friend std::ostream& operator << ( std::ostream& ss, const Tile& t );

private:
    // Default tile data for a tile that was created without a template
    const static TileData * DEFAULT_TILE_DATA;

    // Pointer back to this tile's immutable tile data
    //   (constant for the moment, we may need to change this when we
    //    implement tile swapping)
    const TileData * mpData;

    // Light level of the tile (eventually switch this to a special light
    // class that can calculate blended lights)
    uint8_t mLightLevel;
};

#endif
