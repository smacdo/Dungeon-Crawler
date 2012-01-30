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
#ifndef SCOTT_DUNGEON_TILEDATA_H
#define SCOTT_DUNGEON_TILEDATA_H

#include <bitset>
#include <string>

#include "game/tileflags.h"

/**
 * Tile tile data structure serves two purposes for the game. The first
 * (and most obvious) is that it contains all the intial settings used to
 * instantiate a tile of a given type.
 *
 * The other use is to minimize memory use by storing constant, read only
 * tile data for each tile type. An actual tile instance will store a pointer
 * to the TileData instance that it was created from, along with values that
 * can be changed. Everything else (such as the name of the tile) will be
 * stored in the tile data structure.
 *
 * Generally, the only data that can be changed on a per tile basis are the
 * flags
 */
struct TileData
{
    TileData()
        : id( 0 ),
          name( "void" ),
          title( "The Void" ),
          flags( ETILE_IMPASSABLE )
    {
    }

    TileData( unsigned int id,
              const std::string& name,
              std::bitset<ETileFlags::ETILE_FLAGS_COUNT> flags )
        : id(id),
          name( name ),
          title( name ),
          flags( flags )
    {
    }

    // This is the numeric identifier for this tile
    unsigned int id;

    // This is the string name given to the tile, and can be used to look
    // it up
    std::string name;

    // Displayed named
    std::string title;

    // Flags that determine behavior of the tile
    std::bitset<ETileFlags::ETILE_FLAGS_COUNT> flags;
};

#endif
