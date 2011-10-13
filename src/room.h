#ifndef SCOTT_DUNGEON_ROOM_H
#define SCOTT_DUNGEON_ROOM_H

#include "core/point.h"
#include "core/rect.h"
#include "tiletype.h"

#include <cstddef>
#include <cassert>

class Rect;
class TileGrid;

class Room
{
public:
    Room( Rect& area, const TileGrid& tileGrid );
    ~Room();
};


#endif
