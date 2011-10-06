#ifndef SCOTT_DUNGEON_BSPLEVELGEN_H
#define SCOTT_DUNGEON_BSPLEVELGEN_H

#include "core/rect.h"
#include "core/point.h"

class Level;
class RoomGenerator;

class BspLevelGenerator // : public LevelGenerator
{
public:
    BspLevelGenerator( RoomGenerator *roomGenerator,
                       int levelWidth,
                       int levelHeight,
                       int minSplitSize,
                       int maxSplitSize );

    ~BspLevelGenerator();

    Level* generate();

private:
    void subgenerate( const Rect& area );
    void splitX( const Rect& area, Rect& roomA, Rect& roomB );
    void splitY( const Rect& area, Rect& roomA, Rect& roomB );

    BspLevelGenerator( const BspLevelGenerator& );
    const BspLevelGenerator& operator = ( const BspLevelGenerator& );

private:
    RoomGenerator * mRoomGenerator;
    Rect mLevelBounds;
    int mMinSplitSize;
    int mMaxSplitSize;

    // Average size that the room should take up in its designated zone
    float mRoomDensity;

    Level* mLevel;
};

#endif
