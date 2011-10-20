#ifndef SCOTT_DUNGEON_LEVEL_GENERATOR
#define SCOTT_DUNGEON_LEVEL_GENERATOR

#include <cstddef>
#include <boost/noncopyable.hpp>

#include "tilegrid.h"

class RoomGenerator;
class Level;

class LevelGenerator : boost::noncopyable
{
public:
    LevelGenerator( RoomGenerator *pRoomGen, int width, int height );
    ~LevelGenerator();

    Level * generate();

protected:
    ERoomSize generateRandomRoomSize( ERoomSize maxRoomSize ) const;

private:
    RoomGenerator *mRoomGenerator;
    size_t mLevelWidth;
    size_t mLevelHeight;
    TileGrid mTileGrid;
};

#endif
