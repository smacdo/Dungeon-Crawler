/*
 * Copyright (C) 2012 Scott MacDonald. All rights reserved.
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
#ifndef SCOTT_DUNGEON_WORLD_GENERATOR_H
#define SCOTT_DUNGEON_WORLD_GENERATOR_H

#include <boost/utility.hpp>

class World;
class TileFactory;

/**
 * Responsible for creating new worlds
 */
class WorldGenerator
{
public:
    WorldGenerator( size_t maxWidth, size_t maxHeight, unsigned int seed );
    ~WorldGenerator();

    World * generate( TileFactory& tileFactory ) const;

private:
    const size_t LEVEL_WIDTH;
    const size_t LEVEL_HEIGHT;
    const size_t RANDOM_SEED;
};

#endif
