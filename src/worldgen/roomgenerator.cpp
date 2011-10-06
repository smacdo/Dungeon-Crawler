#include "worldgen/roomgenerator.h"
#include "core/rect.h"
#include "room.h"
#include "level.h"
#include "tile.h"
#include "tiletype.h"

#include <cassert>

const int MinRoomSize = 3;

RoomGenerator::RoomGenerator()
{
}

RoomGenerator::~RoomGenerator()
{

}

void RoomGenerator::setLevel( Level* level )
{
    assert( level != NULL );
    mLevel = level;
}

/**
 * Requests that the room generator generates a new random room in the
 * specified area. This method will return a pointer if the creation
 * succeeded, and NULL if the generator decided not to create a room here.
 *
 * \param area The area to construct the room in. The room could be smaller
 * \return Pointer to the newly constructed room, or NULL if refused
 */
Room* RoomGenerator::generate( const Rect& area )
{
    assert( area.width() >= MinRoomSize );
    assert( area.height() >= MinRoomSize );
    assert( mLevel != NULL );

    return mLevel->addRectangleRoom( area.x(), area.y(),
                                     area.width(),
                                     area.height(),
                                     TILE_WALL,
                                     TILE_FLOOR );
}
