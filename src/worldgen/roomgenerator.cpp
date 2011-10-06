#include "worldgen/roomgenerator.h"
#include "core/rect.h"
#include "room.h"
#include "level.h"
#include "tile.h"
#include "utils.h"
#include "tiletype.h"

#include <cassert>

const int MinRoomSize = 5;

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
    assert( area.width() > MinRoomSize );
    assert( area.height() > MinRoomSize );
    assert( mLevel != NULL );

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

    Room *pRoom = mLevel->addRectangleRoom( area.x() + x, area.y() + y,
                                            width,
                                            height,
                                            TILE_WALL,
                                            TILE_FLOOR );

    // Add some doors to the room
    int numDoors = Utils::random( 1, 5 );

    for ( int i = 0; i < numDoors; ++i )
    {
        addRandomDoor( deref(pRoom) );
    }

    return pRoom;
}

/**
 * Adds a random door to the room
 */
void RoomGenerator::addRandomDoor( Room& room ) const
{
    size_t numWalls = room.numTilesOf( TILE_WALL );
    assert( numWalls > 0 );
    
    // Now randomly determine which of these wall tiles will turn into
    // a door.
    size_t theWall = Utils::random( 0, numWalls );

    // Transform it
    size_t wallsFound = 0;

    for ( size_t r = 0; r < room.height(); ++r )
    {
        for ( size_t c = 0; c < room.width(); ++c )
        {
            Tile * tile = mLevel->tileAt( r + room.topY(), c + room.topX() );
            
            if ( tile->type == TILE_WALL )
            {
                if ( wallsFound++ == theWall )
                {
                    tile->type = TILE_DOOR;
                }
            }
        }
    }
}