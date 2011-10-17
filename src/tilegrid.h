/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#ifndef SCOTT_DUNGEON_TILEGRID_H
#define SCOTT_DUNGEON_TILEGRID_H

#include "common/fixedgrid.h"
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

    void carveRoom( const Rect& area,
                    const Tile& wallTemplate,
                    const Tile& floorTemplate );

    void carveOverlappingRoom( const Rect& area,
                               const Tile& wallTemplate,
                               const Tile& floorTemplate );

    /**
     * Adds a non-overlapping room into the tile grid. If the insertion would
     * cause already placed tiles to be overwritten then this method will raise
     * an error.
     */
    void addRoom( const Rect& area, const TileGrid& roomGrid );

    // Adds an overlapping room in the tile grid
    void addOverlappingRoom( const Point& upperLeft, const TileGrid& roomGrid );

private:
};

#endif