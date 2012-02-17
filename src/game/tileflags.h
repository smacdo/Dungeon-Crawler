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

/**
 * These bit flags can be set per tile instance
 */
enum ETileFlags
{
    ETILE_PLACED,       // set if the dungeon generator has placed this tile
    ETILE_IS_ROOM,      // set if tile is part of a room
    ETILE_IS_HALL,      // set if tile is part of a hallway
    ETILE_FLAGS_COUNT
};

#endif
