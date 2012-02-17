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
#ifndef SCOTT_DUNGEON_ROOM_GENERATOR_H
#define SCOTT_DUNGEON_ROOM_GENERATOR_H

#include "game/tilegrid.h"

class Rect;
class Random;
class TileFactory;
struct RoomData;


/*
 * Makes and builds rooms. Can be subclassed to generate thematic levels
 * with similar (or different!) room types and whatnot.
 *
 * For example, you could subclass the room generator to make crypt levels,
 * a castle room generator, etc.
 */
class RoomGenerator
{
public:
    RoomGenerator( const TileFactory& tileFactory, Random& mRandom );
    ~RoomGenerator();

    /**
     * Generates a randomly created room, and returns a pointer to the
     * room data
     */
    RoomData* generate( ERoomSize roomSize );

private:
    Rect generateRoomRect( int minSize, int maxSize ) const;
    Rect generateOverlapRect( int minSize,
                              int maxSize,
                              const Rect& mainRoom ) const;
    Rect findBounds( const Rect& a, const Rect& b ) const;

private:
    const TileFactory &mTileFactory;

    // Reference to the random instance used by the dungeon generator
    Random& mRandom;
};

#endif
