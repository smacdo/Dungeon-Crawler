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
#include "game/tile.h"
#include "game/tiletype.h"
#include "game/tileflags.h"
#include "common/platform.h"
#include <cstddef>
#include <ostream>

// Default tile data (relies on TileFactory creating a void tile type)
const TileType * Tile::DEFAULT_TILE_TYPE = new TileType;

/**
 * Default tile constructor. Creates a null tile that is treated as a VOID
 * tile type
 */
Tile::Tile()
    : mpType( DEFAULT_TILE_TYPE ),
      mFlags()
{
}

/**
 * Tile constructor. Takes a pointer to the tiledata that this tile is
 * being constructed from.
 *
 * \param  pTileData  Pointer to data for the tile that we are to become
 */
Tile::Tile( TileType* pTileType )
    : mpType( pTileType ),
      mFlags()
{
    assert( pTileType != NULL );
}

/**
 * Tile constructor
 */
Tile::Tile( TileType *pTileType, TileFlagSet flags )
    : mpType( pTileType ),
      mFlags( flags )
{
    assert( pTileType != NULL );
}

/**
 * Tile copy constructor
 */
Tile::Tile( const Tile& source )
    : mpType( source.mpType ),
      mFlags( source.mFlags )
{
    assert( mpType != NULL );
}

/**
 * Tile assignment operator
 *
 * \param  rhs  The tile to copy data from
 * \return      Reference to this instance
 */
Tile& Tile::operator = ( const Tile& rhs )
{
    mpType = rhs.mpType;
    mFlags = rhs.mFlags;
    return *this;
}

/**
 * Tile equality operator. This is a strict equality operator, so both
 * the tile data pointer must match, as well as tile specific data such as
 * the light level.
 *
 * \param  rhs  The tile that we are comparing to
 * \return      True if the two tiles are equal, false otherwise
 */
bool Tile::operator == ( const Tile& rhs ) const
{
    return mpType == rhs.mpType && mFlags == rhs.mFlags;
}

/**
 * Checks if the tile is marked as granite. A granite tile cannot be altered
 * and almost always marks boundaries or borders
 */
bool Tile::isGranite() const
{
    assert( mpType != NULL );
    return mpType->flags().test( ETILE_GRANITE );
}

/**
 * Checks if the tile is totally impassable. An impassable tile means that
 * no actor or object can be placed or enter.
 *
 * \return  True if the tile is impassable, false otherwise
 */
bool Tile::isImpassable() const
{
    assert( mpType != NULL );
    return mpType->flags().test( ETILE_IMPASSABLE );
}

/**
 * Checks if the tile is a wall.
 *
 * \return  True if the tile is a wall, false otherwise
 */
bool Tile::isWall() const
{
    assert( mpType != NULL );
    return mpType->flags().test( ETILE_WALL );
}

/**
 * Checks if the tile is considered a floor
 *
 * \return  True if the tile is a floor, false otherwise
 */
bool Tile::isFloor() const
{
    assert( mpType != NULL );
    return mpType->flags().test( ETILE_FLOOR );
}

/**
 * Checks if this tile was placed by the dungeon generator. If the tile was
 * not placed, then it is considered "void" and should not be used by the
 * game.
 *
 * \return  True if the tile was placed by the dungeon generator, false
 *          otherwise
 */
bool Tile::isPlaced() const
{
    return mFlags.test( ETILE_PLACED );
}

/**
 * Returns the id of this tile
 */
unsigned int Tile::tileid() const
{
    assert( mpType != NULL );
    return mpType->id();
}

/**
 * Prints the tile to standard out. Useful for debugging
 *
 * \param  ss  The stream to print out to
 * \param  t   The tile to print
 * \return     Reference to the passed stream
 */
std::ostream& operator << ( std::ostream& ss, const Tile& t )
{
    if ( t.isFloor() )
    {
        ss << '.';
    }
    else if ( t.isWall() )
    {
        ss << "#";
    }
    else if ( t.isImpassable() )
    {
        ss << "x";
    }
    else
    {
        ss << "!";
    }

    return ss;
}

