#include "worldgen/dungeongenerator.h"
#include "worldgen/bsplevelgen.h"
#include "worldgen/roomgenerator.h"

#include <cstddef>
#include <boost/checked_delete.hpp>

/**
 * Constructor
 *
 * \param  width  The maximum width of a dungeon floor
 * \param  height The maximum height of a dungeon floor
 */
DungeonGenerator::DungeonGenerator( size_t width, size_t height )
    : mLevelGenerator( NULL ),
      mLevelWidth( width ),
      mLevelHeight( height ),
      mMinRoomSize( 8 ),
      mMaxRoomSize( 32 )
{
    mLevelGenerator = new BspLevelGenerator( new RoomGenerator,
                                             mLevelWidth,
                                             mLevelHeight,
                                             mMinRoomSize,
                                             mMaxRoomSize );
}

/**
 * Destructor
 */
DungeonGenerator::~DungeonGenerator()
{
    boost::checked_delete( mLevelGenerator );
}

Level* DungeonGenerator::generateLevel()
{
    return mLevelGenerator->generate();
}