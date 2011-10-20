#ifndef SCOTT_DUNGEON_ROOM_GENERATOR_H
#define SCOTT_DUNGEON_ROOM_GENERATOR_H

#include "tilegrid.h"
#include "dungeoncrawler.h"

class Rect;
class Random;


/*
 * Makes and builds rooms. Can be subclassed to generate thematic levels
 * with similar (or different!) room types and whatnot.
 *
 * For example, you could subclass the room generator to make crypt levels,
 * a castle room generator, etc.
 */
class RoomGenerator
{
public:
    RoomGenerator();
    ~RoomGenerator();

    /**
     * Generates a room to place in the requested area. This method
     * can potentially refuse to place a level, in which case the returned
     * pointer will be null
     */
    TileGrid generate( ERoomSize roomSize, Random& random );
};

#endif
