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
#ifndef SCOTT_DUNGEON_DUNGEONGENERATOR_H
#define SCOTT_DUNGEON_DUNGEONGENERATOR_H

#include <cstddef>
#include <boost/noncopyable.hpp>

#include "common/random.h"

class Dungeon;
class TileFactory;
class Level;
class LevelGenerator;

/**
 * This class takes configurable parameters to the dungeon generation proess,
 * and will then return generated dungeons on demand.
 */
class DungeonGenerator : boost::noncopyable
{
public:
    DungeonGenerator( const TileFactory& tileFactory,
                      size_t width,
                      size_t height,
                      unsigned int randomSeed );
    ~DungeonGenerator();

    // Creates a new dungeon object
    Dungeon* generate();

protected:
    // Right now this is publically exposed since we don't have a dungeon
    // model class
    Level* generateLevel();

private:
    const TileFactory& mTileFactory;
    LevelGenerator * mLevelGenerator;
    Random mRandom;
    size_t mLevelWidth;
    size_t mLevelHeight;
};

#endif
