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
#ifndef DUNGEON_ENGINE_TILE_FLAGS
#define DUNGEON_ENGINE_TILE_FLAGS

enum ETileFlags
{
    ETILE_GRANITE,      // set if this tile is granite (unmodifable)
    ETILE_PLACED,       // set if the dungeon generator has modified this tile
    ETILE_IMPASSABLE,   // nothing can enter or be placed on this tile
    ETILE_WALK,         // tile can be walked on
    ETILE_FLY,          // tile can be flown across
    ETILE_SWIM,         // tile can be swum across
    ETILE_TUNNEL,       // tile can be tunneled through
    ETILE_WALL,         // considered wall tile for dungeon generation
    ETILE_FLOOR,        // considered floor tile for dungeon generation
    ETILE_DOORWAY,      // considered doorway for dungeon generation
    ETILE_IS_ROOM,      // tile belongs to a "room"
    ETILE_IS_HALL,      // tile belongs to a "hall"
    ETILE_BLOCK_LOS,    // blocks line of sight for an actor
    ETILE_FLAGS_COUNT
};

#endif
