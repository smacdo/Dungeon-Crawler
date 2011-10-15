#ifndef SCOTT_DUNGEON_ROOM_H
#define SCOTT_DUNGEON_ROOM_H

#include "common/point.h"
#include "common/rect.h"
#include "tiletype.h"

#include <cstddef>

class Rect;
class TileGrid;

class Room
{
public:
    Room( Rect& area, const TileGrid& tileGrid );
    ~Room();
};


#endif
