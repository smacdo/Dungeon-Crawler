/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#ifndef SCOTT_DUNGEON_TILEGRID_H
#define SCOTT_DUNGEON_TILEGRID_H

#include "common/fixedgrid.h"
#include "tile.h"

#include <iosfwd>

class Rect;

/**
 * Represents a grid of tiles
 */
class TileGrid : public FixedGrid<Tile>
{
public:
    // Creates a sized tile grid containing void tiles
    TileGrid( size_t width,
              size_t height );

    // Creates a rectangular tile grid with each spot containing a copy
    // of the default tile template
    TileGrid( size_t width,
              size_t height,
              const Tile& defaultTile );

    // Copy constructor
    TileGrid( const TileGrid& grid );
    ~TileGrid();

    // Checks if the area occupied by the rectangle is empty
    bool isAreaEmpty( const Rect& area ) const;

    // Carves a room into the tile grid
    void carveRoom( const Rect& floorArea,
                    int wallWidth,
                    const Tile& wallTemplate,
                    const Tile& floorTemplate );

    // Carves a room that overlaps another room into the tile grid
    void carveOverlappingRoom( const Rect& floorArea,
                               int wallWidth,
                               const Tile& wallTemplate,
                               const Tile& floorTemplate );
    
    // stream output
    friend std::ostream& operator << ( std::ostream& ss,
                                       const TileGrid& grid );
private:
};

#endif
