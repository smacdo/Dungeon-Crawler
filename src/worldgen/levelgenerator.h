#ifndef SCOTT_DUNGEON_LEVEL_GENERATOR
#define SCOTT_DUNGEON_LEVEL_GENERATOR

#include <cstddef>
#include <boost/noncopyable.hpp>

#include "tilegrid.h"

class Random;
class Level;
class Point;
struct RoomData;

class LevelGenerator : boost::noncopyable
{
public:
    LevelGenerator( Random& random,
                    int levelWidth,
                    int levelHeight );
    ~LevelGenerator();

    Level * generate();

protected:
    ERoomSize generateRandomRoomSize( ERoomSize maxRoomSize ) const;
    Point findRandomPointFor( const RoomData& roomData ) const;
    bool canPlaceRoomAt( const RoomData& roomData, const Point& pos ) const;

private:
    Random& mRandom;
    int mLevelWidth;
    int mLevelHeight;
    TileGrid mTileGrid;
};

#endif
