#ifndef SCOTT_DUNGEON_LEVEL_GENERATOR
#define SCOTT_DUNGEON_LEVEL_GENERATOR

#include <cstddef>
#include <boost/noncopyable.hpp>
#include "game/tilegrid.h"

class Random;
class Level;
class Point;
class TileFactory;
struct RoomData;

/**
 * Level generator is a class that accepts configurable settings, and then
 * generates one or more random grid levels that consist of a tileset, mobs
 * and items
 */
class LevelGenerator : boost::noncopyable
{
public:
    // Constructor
    LevelGenerator( Random& random,
                    const TileFactory& tileFactory,
                    int levelWidth,
                    int levelHeight );

    // Destructor
    ~LevelGenerator();

    // Request a new level to be generated
    Level * generate();

protected:
    // Generate s a
    ERoomSize generateRandomRoomSize( ERoomSize maxRoomSize ) const;

    // Finds a random point to place a room at
    Point findRandomPointFor( const RoomData& roomData ) const;

    // Checks if we can place a room here
    bool canPlaceRoomAt( const TileGrid& tilegrid,
                         const RoomData& roomData,
                         const Point& pos ) const;

private:
    Random& mRandom;
    const TileFactory& mTileFactory;
    int mLevelWidth;
    int mLevelHeight;
};

#endif
