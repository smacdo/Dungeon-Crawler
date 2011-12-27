#include "game/tile.h"
#include "game/tiledata.h"
#include "game/tileflags.h"
#include "common/platform.h"
#include <cstddef>
#include <ostream>

/**
 * Default tile constructor. Creates a null tile that is treated as a VOID
 * tile type
 */
Tile::Tile()
    : mpData( NULL )
{
}

/**
 * Tile constructor. Takes a pointer to the tiledata that this tile is
 * being constructed from.
 *
 * \param  pTileData  Pointer to data for the tile that we are to become
 */
Tile::Tile( TileData* pTileData )
    : mpData( pTileData )
{
    assert( pTileData != NULL );
}

/**
 * Tile copy constructor
 */
Tile::Tile( const Tile& source )
    : mpData( source.mpData )
{
}

/**
 * Tile assignment operator
 *
 * \param  rhs  The tile to copy data from
 * \return      Reference to this instance
 */
Tile& Tile::operator = ( const Tile& rhs )
{
    mpData = rhs.mpData;
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
    return mpData == rhs.mpData && mLightLevel == rhs.mLightLevel;
}

/**
 * Checks if the tile is totally impassable. An impassable tile means that
 * no actor or object can be placed or enter.
 *
 * \return  True if the tile is impassable, false otherwise
 */
bool Tile::isImpassable() const
{
    bool impassable = true;

    if ( mpData != NULL )
    {
        impassable = mpData->flags.test( ETILE_IMPASSABLE );
    }

    return impassable;
}

/**
 * Checks if the tile is a wall.
 *
 * \return  True if the tile is a wall, false otherwise
 */
bool Tile::isWall() const
{
    bool wall = false;

    if ( mpData != NULL )
    {
        wall = mpData->flags.test( ETILE_WALL );
    }

    return wall;
}

/**
 * Checks if the tile is considered a floor
 *
 * \return  True if the tile is a floor, false otherwise
 */
bool Tile::isFloor() const
{
    bool floor = false;

    if ( mpData != NULL )
    {
        floor = mpData->flags.test( ETILE_FLOOR );
    }

    return floor;
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
    bool placed = false;

    if ( mpData != NULL )
    {
        placed = mpData->flags.test( ETILE_PLACED );
    }

    return placed;
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

