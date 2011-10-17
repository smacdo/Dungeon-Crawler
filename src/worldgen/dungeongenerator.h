#ifndef SCOTT_DUNGEON_DUNGEONGENERATOR_H
#define SCOTT_DUNGEON_DUNGEONGENERATOR_H

#include <cstddef>
#include <boost/noncopyable.hpp>

class Level;
class LevelGenerator;

/**
 * This class takes configurable parameters to the dungeon generation proess,
 * and will then return generated dungeons on demand.
 */
class DungeonGenerator : boost::noncopyable
{
public:
    DungeonGenerator( size_t width, size_t height );
    ~DungeonGenerator();

    // Right now this is publically exposed since we don't have a dungeon
    // model class
    Level* generateLevel();

private:
    LevelGenerator * mLevelGenerator;
    size_t mLevelWidth;
    size_t mLevelHeight;
    size_t mMinRoomSize;
    size_t mMaxRoomSize;
};

#endif