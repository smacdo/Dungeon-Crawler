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
#include "worldgen/roomgenerator.h"
#include "common/rect.h"
#include "common/logging.h"
#include "common/random.h"
#include "tilegrid.h"
#include "level.h"
#include "tile.h"
#include "common/utils.h"
#include "tiletype.h"
#include "dungeoncrawler.h"

/////////////////////////////////////////////////////////////////////////////
// Room generation constants
/////////////////////////////////////////////////////////////////////////////
// The floor size of the room. This does not include walls!!!
const int ROOM_SIZES[ERoomSize_COUNT][2] =
{
    { 6,  3 },   /* EROOM_TINY */
    { 10, 5 },   /* EROOM_SMALL */
    { 15, 8 },   /* EROOM_MEDIUM */
    { 20, 13 },   /* EROOM_LARGE */
    { 30, 18 },  /* EROOM_HUGE */
    { 50, 25 }   /* ERROM_GIGANTIC */
};

RoomGenerator::RoomGenerator()
{
}

RoomGenerator::~RoomGenerator()
{

}

/**
 * Requests that the room generator generates a new random room in the
 * specified area. This method will return a pointer if the creation
 * succeeded, and NULL if the generator decided not to create a room here.
 *
 * \param  roomSize  The size of the room to construct
 * \return A TileGrid containing the room
 */
TileGrid RoomGenerator::generate( ERoomSize roomSize, Random& random )
{
    assert( roomSize < ERoomSize_COUNT );

    // Randomly determine the size of the new room
    int minSize  = ROOM_SIZES[ roomSize ][1];
    int maxSize  = ROOM_SIZES[ roomSize ][0];

    // Now first generate random dimensions for the first room
    int rA_width  = random.randInt( minSize, maxSize );
    int rA_height = random.randInt( minSize, maxSize );
    int rA_x      = 0;
    int rA_y      = 0;

    if ( rA_width < maxSize )
    {
        rA_x = random.randInt( 0, ( maxSize - rA_width ) );
    }

    if ( rA_height < maxSize )
    {
        rA_y = random.randInt( 0, ( maxSize - rA_height ) );
    }

    LOG_DEBUG("WorldGen") << "Generating room A, "
        << "minSize=" << minSize  << ", maxSize=" << maxSize   << ", "
        << "width="   << rA_width << ", height="  << rA_height << ", "
        << "x="       << rA_x     << ", y="       << rA_y;

    // Carve the initial room in tile grid
    TileGrid room( maxSize + 2, maxSize + 2 );

    room.carveRoom( Rect( rA_x, rA_y, rA_width + 2, rA_height  + 2 ),
                    Tile( TILE_WALL ),
                    Tile( TILE_FLOOR ) );

    // Attempt a second rectangle carving to make the room more interesting.
    // It should be at least as large as the "buffer" area (which is the amount
    // of empty space between the carved room and the top left corner), and
    // randomly placed somewhere inside the room area we are given.
    if ( true )
    {
        // Calculate the width of a second rectangle
        int rB_width  = random.randInt( rA_x, maxSize );
        int rB_height = random.randInt( rA_y, maxSize );

        // Now generate a random upper left position to place this extra
        // rect at
        int rB_x      = 0;
        int rB_y      = 0;

        if ( rB_width < maxSize )
        {
            rB_x = random.randInt( 0, ( maxSize - rB_width ) );
        }

        if ( rB_height < maxSize )
        {
            rB_y = random.randInt( 0, ( maxSize - rB_height ) );
        }
   
        // Now card an overlap into the room, taking care not to add
        // walls
        room.carveOverlappingRoom( Rect( rB_x, rB_y, rB_width + 2, rB_height+2),
                                   Tile( TILE_WALL ),
                                   Tile( TILE_FLOOR ) );
    }

    return room;
}
