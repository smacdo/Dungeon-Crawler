#include "connectiongenerator.h"

/**
 * Default constructor. Initializes the connection generator with sane defaults
 */
ConnectionGenerator::ConnectionGenerator( Random& mRandom )
    : mMinConnections( 2 ),
      mMaxConnections( 6 ),
      mRoomSizeMultiple( 5 )
{
}

/**
 * Default destructor
 */
ConnectionGenerator::~ConnectionGenerator()
{
    // nothing
}

/**
 * Connect the rooms together in a logical fashion
 */
void ConnectionGenerator::connect( const std::vector<RoomData*>& rooms )
{
    // connect each room to its next room to ensure they are connected
    for ( size_t index = 0; index < rooms.size(); ++index )
    {
        size_t nextIndex ( index + 1 ) % rooms.size();

        // Connect this room to the next room
        rooms[index].connectedRooms.push_back( rooms[nextIndex] );

        // Also register the connection in the other room
    }
}

/**
 * Processes one room, connecting it to one or more other rooms
 */
void ConnectionGenerator::connectRoom( RoomData& room )
{

}
