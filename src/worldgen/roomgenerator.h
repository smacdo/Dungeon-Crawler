#ifndef SCOTT_DUNGEON_ROOM_GENERATOR_H
#define SCOTT_DUNGEON_ROOM_GENERATOR_H

#include "game/tilegrid.h"

class Rect;
class Random;
class TileFactory;
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
    RoomGenerator( const TileFactory& tileFactory, Random& mRandom );
    ~RoomGenerator();

    /**
     * Generates a randomly created room, and returns a pointer to the
     * room data
     */
    RoomData* generate( ERoomSize roomSize );

private:
    Rect generateRoomRect( int minSize, int maxSize ) const;
    Rect generateOverlapRect( int minSize,
                              int maxSize,
                              const Rect& mainRoom ) const;
    Rect findBounds( const Rect& a, const Rect& b ) const;

private:
    const TileFactory &mTileFactory;

    // Reference to the random instance used by the dungeon generator
    Random& mRandom;
};

#endif
