#ifndef SCOTT_DUNGEON_TILEGRID_H
#define SCOTT_DUNGEON_TILEGRID_H

#include "core/fixedgrid.h"
#include "tile.h"

class Rect;

/**
 * Represents a grid of tiles
 */
class TileGrid : public FixedGrid<Tile>
{
public:
    TileGrid( size_t width, size_t height );
    TileGrid( const TileGrid& grid );
    ~TileGrid();

    void carveTiles( const Rect& area,
                     const Tile& wallTemplate,
                     const Tile& floorTemplate );

private:
};

#endif