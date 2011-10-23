#ifndef SCOTT_DUNGEON_ROOM_GENERATOR_H
#define SCOTT_DUNGEON_ROOM_GENERATOR_H

#include "tilegrid.h"
#include "dungeoncrawler.h"

class Rect;
class Random;
struct RoomData;


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
    RoomGenerator( Random& mRandom );
    ~RoomGenerator();

    /**
     * Generates a room to place in the requested area. This method
     * can potentially refuse to place a level, in which case the returned
     * pointer will be null
     */
    RoomData* generate( ERoomSize roomSize );

private:
    Rect generateRoomRect( int minSize, int maxSize ) const;
    Rect generateOverlapRect( int minSize,
                              int maxSize,
                              const Rect& mainRoom ) const;
    Rect findBounds( const Rect& a, const Rect& b ) const;

private:
    // Reference to the random instance used by the dungeon generator
    Random& mRandom;
};

#endif
