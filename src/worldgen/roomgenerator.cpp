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
    { 3,  2 },   /* EROOM_TINY */
    { 5,  2 },   /* EROOM_SMALL */
    { 9,  4 },   /* EROOM_MEDIUM */
    { 15, 8 },   /* EROOM_LARGE */
    { 25, 12 },  /* EROOM_HUGE */
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
TileGrid RoomGenerator::generate( ERoomSize roomSize )
{
    assert( roomSize < ERoomSize_COUNT );

    // Randomly determine the size of the new room
    int minSize  = ROOM_SIZES[ roomSize ][1];
    int maxSize  = ROOM_SIZES[ roomSize ][0];

    // Now first generate random dimensions for the first room
    int rA_width  = Utils::random( minSize, maxSize );
    int rA_height = Utils::random( minSize, maxSize );
    int rA_x      = 0;
    int rA_y      = 0;

    if ( rA_width < maxSize )
    {
        rA_x = Utils::random( 0, ( maxSize - rA_width ) );
    }

    if ( rA_height < maxSize )
    {
        rA_y = Utils::random( 0, ( maxSize - rA_height ) );
    }

    LOG_DEBUG("WorldGen") << "Generating room A, "
        << "minSize=" << minSize  << ", maxSize=" << maxSize   << ", "
        << "width="   << rA_width << ", height="  << rA_height << ", "
        << "x="       << rA_x     << ", y="       << rA_y;

    // Carve out the room, and return the tile grid
    //  We need to add two to both dimensions to account for the walls
    TileGrid room( maxSize + 2, maxSize + 2 );

    room.carveTiles( Rect( rA_x, rA_y, rA_width + 2, rA_height  + 2 ),
                     Tile( TILE_WALL ),
                     Tile( TILE_FLOOR ) );
    return room;
}
