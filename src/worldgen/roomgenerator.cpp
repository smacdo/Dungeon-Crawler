#include "worldgen/roomgenerator.h"
#include "core/rect.h"
#include "tilegrid.h"
#include "level.h"
#include "tile.h"
#include "common/utils.h"
#include "tiletype.h"

#include <cassert>

const int MinRoomSize = 5;

RoomGenerator::RoomGenerator()
{
}

RoomGenerator::~RoomGenerator()
{

}

/**
 * Requests that the room generator generates a new random room in the
 * specified area. This method will return a pointer if the creation
 * succeeded, and NULL if the generator decided not to create a room here.
 *
 * \param area The area to construct the room in. The room could be smaller
 * \return Pointer to the newly constructed room, or NULL if refused
 */
TileGrid RoomGenerator::generate( const Rect& area )
{
    TileGrid room( area.width(), area.height() );

    assert( area.width() > MinRoomSize );
    assert( area.height() > MinRoomSize );

    // Randomly determine the sizes of the new room
    int width  = Utils::random( MinRoomSize, area.width() - 1 );
    int height = Utils::random( MinRoomSize, area.height() - 1 );

    // Figure out how much "buffer" room we have from the end of the room to the walls
    int bufferWidth  = area.width() - width;
    int bufferHeight = area.height() - height;

    assert( bufferWidth > 0 );
    assert( bufferHeight > 0 );

    // Now randomly determine where in the quadrant we should place this room
    int x = Utils::random( 0, bufferWidth+1 );
    int y = Utils::random( 0, bufferHeight+1 );

    // Carve out the room, and return the tile grid
    room.carveTiles( Rect( x, y, width, height),
                     Tile( TILE_WALL ),
                     Tile( TILE_FLOOR ) );
    return room;
}