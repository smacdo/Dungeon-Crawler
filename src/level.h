#ifndef SCOTT_DUNGEON_LEVEL_H
#define SCOTT_DUNGEON_LEVEL_H
#include "tile.h"

#include <vector>
#include <cassert>

class Level;
class Room;
struct Tile;

class Level
{
public:
    Level( size_t width, size_t height );
    ~Level();

    // Adds a room
    Room* addRectangleRoom( size_t x, size_t y,
                            size_t width, size_t height,
                            ETileType wall,
                            ETileType floor );

    // Checks if any of the tiles in the given rect have been allocated
    // to a room or corridor
    bool hasAllocatedTiles( size_t topX, size_t topY,
                            size_t width, size_t height ) const;

    // Returns the tile at the requested position
    Tile getTileAt( size_t r, size_t c ) const;
    Tile* tileAt( size_t r, size_t c );

    void print() const;

    size_t width() const { return mWidth; }
    size_t height() const{  return mHeight; }

protected:
    void init();
    size_t offset( size_t x, size_t y ) const;

    // Tries to carve out a chunk of the map with the given
    // dimensions, and assigns the tiles to their proper wall/floor
    // type.
    void carveRoom( size_t x, size_t y,
                    size_t width,
                    size_t height,
                    ETileType wall,
                    ETileType floor,
                    Room* pRoom );

private:
    size_t mWidth, mHeight;
    Tile * mTiles;
    std::vector<Room*> mRooms;
};

#endif
