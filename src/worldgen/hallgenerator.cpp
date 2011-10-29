#include "worldgen/hallgenerator.h"

HallGenerator::HallGenerator( Random& random )
    : mRandom( random ),
      mpStartRoom( NULL ),
      mpDestRoom( NULL )
{
}

HallGenerator::~HallGenerator()
{
    // empty
}

/**
 * Connects two rooms together by carving a hallway betweeen them
 */
void HallGenerator::connect( RoomData *pStartRoom, RoomData *pEndRoom )
{
    assert( pStartRoom != NULL );
    assert( pEnRoom    != NULL );

    // Reset our generator before beginning
    reset( pStartRoom, pEndRoom );
}

/**
 * Resets the generator
 */
void HallGenerator::reset( RoomData *pStartRoom, RoomData *pEndRoom )
{
    assert( pStartRoom != NULL );
    assert( pEndRoom   != NULL );

    mpStartRoom = pStartRoom;
    mpEndRoom   = pEndRoom;
}
