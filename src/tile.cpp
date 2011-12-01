#include "tile.h"
#include "tiletype.h"
#include "common/platform.h"
#include <cstddef>
#include <ostream>

Tile::Tile()
    : type( TILE_EMPTY )
{
}

Tile::Tile( ETileType type_ )
    : type( type_ )
{
    assert( type_ < ETileType_Count );
}

Tile::Tile( const Tile& other )
    : type( other.type )
{
    assert( type < ETileType_Count );
}

Tile& Tile::operator = ( const Tile& rhs )
{
    assert( rhs.type < ETileType_Count );
    type = rhs.type;
    return *this;
}

bool Tile::operator == ( const Tile& rhs ) const
{
    return type == rhs.type;
}

bool Tile::isImpassable() const
{
    return type == TILE_IMPASSABLE;
}

bool Tile::isWall() const
{
    return type == TILE_WALL;
}

bool Tile::isFloor() const
{
    return type == TILE_FLOOR;
}

bool Tile::wasPlaced() const
{
    return ( type != TILE_EMPTY );
}

std::ostream& operator << ( std::ostream& ss, const Tile& t )
{
    switch ( t.type )
    {
        case TILE_IMPASSABLE:
            ss << '*';
            break;
        case TILE_WALL:
            ss << '#';
            break;
        case TILE_FLOOR:
            ss << '.';
            break;
        case TILE_EMPTY:
            ss << 'X';
            break;
        default:
            ss << '!';
            break;
    }

    return ss;
}

