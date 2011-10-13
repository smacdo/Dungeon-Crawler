#ifndef SCOTT_DUNGEON_LEVEL_H
#define SCOTT_DUNGEON_LEVEL_H
#include "tile.h"
#include "tilegrid.h"

#include <vector>
#include <cassert>
#include <string>

class Level
{
public:
    Level( const TileGrid& tileGrid );
    ~Level();
    
    Tile& tileAt( const Point& p );
    const Tile& tileAt( const Point& p ) const;

    std::string dump() const;

    int width() const;
    int height() const;

private:
    TileGrid mTileGrid;
};

#endif
