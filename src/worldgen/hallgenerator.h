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
#ifndef SCOTT_DUNGEON_HALLGENERATOR_H
#define SCOTT_DUNGEON_HALLGENERATOR_H

#include <boost/noncopyable.hpp>
#include "game/pathfinder.h"

struct RoomData;
class TileFactory;
class Random;
class TileGrid;

/**
 * Responsible for carving interesting hallways between rooms
 */
class HallGenerator : boost::noncopyable
{
public:
    HallGenerator( Random& random,
                   const TileFactory& factory,
                   TileGrid& tilegrid );
    ~HallGenerator();

    void connect( RoomData *pStartRoom, RoomData *pEndRoom );

private:
    void reset( RoomData *pStartRoom, RoomData *pEndRoom );
    int findMovementCost( const Point& from,
                          const Point& to,
                          const Point& prev ) const;

private:
    Random& mRandom;
    const TileFactory& mTileFactory;
    TileGrid& mTileGrid;
    PathFinder mPathFinder;

    RoomData *mpStartRoom;
    RoomData *mpDestRoom;
};

#endif
