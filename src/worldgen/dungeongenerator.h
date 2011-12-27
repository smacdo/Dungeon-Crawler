#ifndef SCOTT_DUNGEON_DUNGEONGENERATOR_H
#define SCOTT_DUNGEON_DUNGEONGENERATOR_H

#include <cstddef>
#include <boost/noncopyable.hpp>

#include "common/random.h"

class Dungeon;
class TileFactory;
class Level;
class LevelGenerator;

/**
 * This class takes configurable parameters to the dungeon generation proess,
 * and will then return generated dungeons on demand.
 */
class DungeonGenerator : boost::noncopyable
{
public:
    DungeonGenerator( const TileFactory& tileFactory,
                      size_t width,
                      size_t height,
                      unsigned int randomSeed );
    ~DungeonGenerator();

    // Creates a new dungeon object
    Dungeon* generate();

protected:
    // Right now this is publically exposed since we don't have a dungeon
    // model class
    Level* generateLevel();

private:
    const TileFactory& mTileFactory;
    LevelGenerator * mLevelGenerator;
    Random mRandom;
    size_t mLevelWidth;
    size_t mLevelHeight;
};

#endif
