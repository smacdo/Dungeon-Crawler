#include "worldgen/dungeongenerator.h"
#include "worldgen/levelgenerator.h"
#include "worldgen/roomgenerator.h"
#include "common/random.h"

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
      mRandom( Random() ),
      mLevelWidth( width ),
      mLevelHeight( height )
{
    mLevelGenerator = new LevelGenerator( mRandom,
                                          mLevelWidth,
                                          mLevelHeight );
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