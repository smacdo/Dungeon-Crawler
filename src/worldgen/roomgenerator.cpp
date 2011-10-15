#include "worldgen/roomgenerator.h"
#include "core/rect.h"
#include "tilegrid.h"
#include "level.h"
#include "tile.h"
#include "common/utils.h"
#include "tiletype.h"
#include "dungeoncrawler.h"

#include <cassert>

/////////////////////////////////////////////////////////////////////////////
// Room generation constants
/////////////////////////////////////////////////////////////////////////////
// The floor size of the room. This does not include walls!!!
const int ROOM_SIZES[ERoomSize_COUNT][2] =
{
    { 3,  2 },   /* EROOM_TINY */
    { 5,  2 },   /* EROOM_SMALL */
    { 9,  4 },   /* EROOM_MEDIUM */
    { 15, 8 },   /* EROOM_LARGE */
    { 25, 12 },  /* EROOM_HUGE */
    { 50, 25 }   /* ERROM_GIGANTIC */
};

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
 * \param  roomSize  The size of the room to construct
 * \return A TileGrid containing the room
 */
TileGrid RoomGenerator::generate( ERoomSize roomSize )
{
    assert( roomSize < ERoomSize_COUNT );

    // Randomly determine the size of the new room
    int minSize = ROOM_SIZES[ roomSize ][1];
    int maxSize = ROOM_SIZES[ roomSize ][0];

    int width   = Utils::random( minSize, maxSize+1 );
    int height  = Utils::random( minSize, maxSize+1 );

    // Carve out the room, and return the tile grid
    //  We need to add two to both dimensions to account for the walls
    TileGrid room( width + 2, height + 2 );

    room.carveTiles( Rect( 0, 0, width + 2, height  + 2 ),
                     Tile( TILE_WALL ),
                     Tile( TILE_FLOOR ) );
    return room;
}
