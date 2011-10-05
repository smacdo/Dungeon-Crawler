#ifndef SCOTT_DUNGEON_ROOM_H
#define SCOTT_DUNGEON_ROOM_H

#include "point.h"
#include "rect.h"
#include "tiletype.h"

#include <cstddef>
#include <cassert>

class Level;

class Room
{
public:
    Room( size_t topX,
          size_t topY,
          size_t width,
          size_t height,
          Level* level );
    ~Room();

    // Adds a door on one of the wall tiles
    void addRandomDoor();

    // Counts the number of a given tile type that exists in the room
    size_t numTilesOf( ETileType type ) const;

    void carveRect( size_t x, size_t y,
                    size_t width, size_t height,
                    ETileType wall,
                    ETileType floor );

    void print() const;
    size_t topX() const { return mTopX; }
    size_t topY() const { return mTopY; }
    size_t width() const { return mWidth; }
    size_t height() const { return mHeight; }

protected:
    size_t offset( size_t r, size_t c ) const;

private:
    size_t mTopX, mTopY;
    size_t mWidth, mHeight;
    Level * mLevel;
};


#endif
