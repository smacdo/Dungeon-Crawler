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

RoomGenerator::RoomGenerator( Random& random )
    : mRandom( random )
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
//RoomData* RoomGenerator::generate( ERoomSize roomSize, Random& random )
TileGrid RoomGenerator::generate( ERoomSize roomSize, Random& random )
{
    assert( roomSize < ERoomSize_COUNT );

    // Randomly determine the size of the new room
    int minSize  = ROOM_SIZES[ roomSize ][1];
    int maxSize  = ROOM_SIZES[ roomSize ][0];

    // Generate a room, and an overlapping room to layer on top (this is an
    // easy way to make the rooms appear more complex)
    Rect mainRoomRect = generateRoomRect( minSize, maxSize );
    Rect overlapRect  = generateOverlapRect( minSize, maxSize, mainRoomRect );

    // Calculate a rectangle that is a tight bounds for the two generated
    // room rects

   
    //
    // Create a new room data struct to hold all of our room data's informaton.
    // Then carve the room out
    //
    TileGrid room( maxSize + 2, maxSize + 2 );

    room.carveRoom( mainRoomRect,
                    1,
                    Tile( TILE_WALL ),
                    Tile( TILE_FLOOR ) );

    room.carveOverlappingRoom( overlapRect,
                               1,
                               Tile( TILE_WALL ),
                               Tile( TILE_FLOOR ) );

    // All done, return information to the level generator about our newly
    // created room
    return room;
//    return new RoomData( )
}

Rect RoomGenerator::generateRoomRect( int minSize, int maxSize ) const
{
    // Randomly generate room dimensions, using the minSize/maxSize metrics
    // given to determine the minimum and maximum size of our room
    int width  = mRandom.randInt( minSize, maxSize );
    int height = mRandom.randInt( minSize, maxSize );

    return Rect( 1, 1, width, height );
}

Rect RoomGenerator::generateOverlapRect( int minSize,
                                         int maxSize,
                                         const Rect& mainRoom ) const
{
    // Attempt a second rectangle carving to make the room more interesting.
    // It should be at least as large as the "buffer" area (which is the amount
    // of empty space between the carved room and the top left corner), and
    // randomly placed somewhere inside the room area we are given.

    // Calculate the width of a second rectangle
    int width  = mRandom.randInt( 1, maxSize );
    int height = mRandom.randInt( 1, maxSize );
    int x      = 1;
    int y      = 1;

    if ( width < maxSize-1 )
    {
        x = mRandom.randInt( 1, maxSize - width );
    }
     
    if ( height < maxSize-1 )
    {
        y = mRandom.randInt( 1, maxSize - height );
    }

    // We need to make sure that the overlapping rect actually, you know,
    // overlaps the main room
    int left   = x;
    int top    = y;
    int right  = width + left;
    int bottom = height + top;

    left   = std::min( left,   mainRoom.right() );
    right  = std::max( right,  mainRoom.left() );
    top    = std::min( top,    mainRoom.bottom() );
    bottom = std::max( bottom, mainRoom.top() );

    // Return dat rect
    return Rect( left, top, right - left, bottom - top );
}

/**
 * "Coalesces" two overlapping rectangles together. It returns the tightest
 * fitting rectangle for the two provided rectangles.
 */
Rect RoomGenerator::coalesceRects( const Rect& a, const Rect& b ) const
{
    assert( a.intersects(b) );
    int minX = 0, minY = 0, maxX = 0, maxY = 0;

    // find tight boundaries
    minX = std::min( a.left(), b.left() );
    minY = std::min( a.top(), b.top() );

    maxX = std::max( a.right(), b.right() );
    maxY = std::max( a.bottom(), b.bottom() );

    return Rect( Point( minX, minY ), Point( maxX, maxY ) );
}