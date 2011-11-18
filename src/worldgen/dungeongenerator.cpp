#include "worldgen/dungeongenerator.h"
#include "worldgen/levelgenerator.h"
#include "worldgen/roomgenerator.h"
#include "common/random.h"
#include "common/utils.h"

#include "dungeon.h"

#include <cstddef>
#include <memory>

/**
 * Constructor
 *
 * \param  width  The maximum width of a dungeon level
 * \param  height The maximum height of a dungeon level
 */
DungeonGenerator::DungeonGenerator( size_t width, 
                                    size_t height,
                                    unsigned int randomSeed )
    : mLevelGenerator( NULL ),
      mRandom( randomSeed ),
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
    Delete( mLevelGenerator );
}

/**
 * Generates, populates and returns a newly constructed dungeon to play
 * in!
 *
 * \param  seed  Seed to use when constructing dungeon
 * \return Pointer to a newly constructed dungeon
 */
Dungeon* DungeonGenerator::generate()
{
    std::vector< std::shared_ptr<Level> > levels;
    const size_t numberOfLevels = 1;
    const size_t maxWidth       = mLevelWidth;
    const size_t maxHeight      = mLevelHeight;

    // Generate the requested number of levels
    for ( size_t i = 0; i < numberOfLevels; ++i )
    {
        std::shared_ptr<Level> level( generateLevel() );
        levels.push_back( level );
    }

    return new Dungeon( "Unknown Dungeon", maxWidth, maxHeight, levels );
}

/**
 * Create an individual level for the dungeon
 */
Level* DungeonGenerator::generateLevel()
{
    return mLevelGenerator->generate();
}
