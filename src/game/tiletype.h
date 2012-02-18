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
#ifndef SCOTT_DUNGEON_TILE_TYPE_H
#define SCOTT_DUNGEON_TILE_TYPE_H

#include <bitset>
#include <string>

// These are the built in tile types. Eventually we'll want to pull most
// of this out into XML
enum ETileType
{
    ETILETYPE_VOID            = 0,
    ETILETYPE_GRANITE         = 1,
    ETILETYPE_DUNGEON_WALL    = 2,
    ETILETYPE_DUNGEON_FLOOR   = 3,
    ETILETYPE_DUNGEON_DOORWAY = 4,
    ETILETYPE_FILLER_STONE    = 5,
    ETILETYPE_COUNT
};

enum ETileTypeFlags
{
    ETILE_GRANITE,      // set if this tile is granite (unmodifable)
//    ETILE_PLACED,       // set if the dungeon generator has modified this tile
    ETILE_IMPASSABLE,   // nothing can enter or be placed on this tile
    ETILE_WALK,         // tile can be walked on
    ETILE_FLY,          // tile can be flown across
    ETILE_SWIM,         // tile can be swum across
    ETILE_TUNNEL,       // tile can be tunneled through
    ETILE_WALL,         // considered wall tile for dungeon generation
    ETILE_FLOOR,        // considered floor tile for dungeon generation
    ETILE_DOORWAY,      // considered doorway for dungeon generation
//    ETILE_IS_ROOM,      // tile belongs to a "room"
//    ETILE_IS_HALL,      // tile belongs to a "hall"
    ETILE_BLOCK_LOS,    // blocks line of sight for an actor
    ETILETYPE_FLAGS_COUNT
};

typedef std::bitset<ETileTypeFlags::ETILETYPE_FLAGS_COUNT> TileTypeFlagSet;

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
class TileType
{
public:
    // Default constructor
    TileType();

    TileType( unsigned int id,
              const std::string& name,
              TileTypeFlagSet flags );

    // Constructor
    TileType( unsigned int id,
              const std::string& name,
              const std::string& title,
              TileTypeFlagSet flags );

    // Get the tile id
    unsigned int id() const;

    // Get the tile name
    std::string name() const;

    // Reference to the tile's flags
    const TileTypeFlagSet& flags() const;

private:
    //! This is the numeric identifier for this tile
    unsigned int mId;

    //! This is the string name given to the tile, and can be used to look
    //! it up
    std::string mName;

    //! Displayed named
    std::string mTitle;

    //! Flags that determine behavior of the tile
    TileTypeFlagSet mFlags;
};

#endif
