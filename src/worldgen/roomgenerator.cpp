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
#include "worldgen/roomdata.h"
#include "common/rect.h"
#include "common/logging.h"
#include "common/random.h"
#include "tilegrid.h"
#include "level.h"
#include "tile.h"
#include "common/utils.h"
#include "tiletype.h"
#include "dungeoncrawler.h"

#include "common/platform.h"

/////////////////////////////////////////////////////////////////////////////
// Room generation constants
/////////////////////////////////////////////////////////////////////////////
// The floor size of the room. This does not include walls!!!
const int ROOM_SIZES[ERoomSize_COUNT][2] =
{
    { 3,  5 },   /* EROOM_TINY */
    { 4,  8 },   /* EROOM_SMALL */
    { 6,  12 },   /* EROOM_MEDIUM */
    { 10, 15 },   /* EROOM_LARGE */
    { 13, 22 },  /* EROOM_HUGE */
    { 20, 30 }   /* ERROM_GIGANTIC */
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
RoomData* RoomGenerator::generate( ERoomSize roomSize )
{
    assert( roomSize < ERoomSize_COUNT );
    int minSize  = ROOM_SIZES[ roomSize ][0];
    int maxSize  = ROOM_SIZES[ roomSize ][1];

    // Generate a room, and an overlapping room to layer on top (this is an
    // easy way to make the rooms appear more complex)
    Rect mainRoomRect = generateRoomRect( minSize, maxSize );
    Rect overlapRect  = generateOverlapRect( minSize, maxSize, mainRoomRect );

    // Calculate a rectangle that is a tight bounds for the two generated
    // room rects
    Rect floorRect = findBounds( mainRoomRect, overlapRect );
   
    // Create the structure that will hold data about our room. Need to do this
    // before we start carving
    RoomData * pRoomData = new RoomData( floorRect );

    // Finally we can carve our room out!
    Tile wallTile  = Tile( TILE_WALL );
    Tile floorTile = Tile( TILE_FLOOR );
    
    pRoomData->tiles.carveRoom( mainRoomRect, 1, wallTile, floorTile );
    pRoomData->tiles.carveOverlappingRoom( overlapRect, 1, wallTile, floorTile );

    // All done, return information to the level generator about our newly
    // created room
    return pRoomData;
}

/**
 * Generates a random rectangle describing the floor layout for a room.
 *
 * \param  minSize  Minimum width and height for the room
 * \param  maxSize  Maximum width and height for the room
 * \return Rectangle describing the dimensions of the room
 */
Rect RoomGenerator::generateRoomRect( int minSize, int maxSize ) const
{
    // Randomly generate room dimensions, using the minSize/maxSize metrics
    // given to determine the minimum and maximum size of our room
    int width  = mRandom.randInt( minSize, maxSize );
    int height = mRandom.randInt( minSize, maxSize );

    return Rect( 1, 1, width, height );
}

/**
 * Generates a secondary rectangle that overlaps the primary room rectangle
 *
 * \param  minSize   Minimum width and height for the room
 * \param  maxSize   Maximum width and height for the room
 * \param  mainRoom  Rectangle contaning the dimensions of the primary room
 * \return Rectangle describing the dimensions of the overlapping room
 */
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
 * Returns a bounding rectangle that tightly bounds both rectangles "a" and "b"
 *
 * \param  a  The first rectangle to include in the boundaries
 * \param  b  The second rectangle to include in the boundaries
 * \return The bounding rectangle
 */
Rect RoomGenerator::findBounds( const Rect& a, const Rect& b ) const
{
    assert( a.contains(b) || a.intersects(b) );
    int minX = 0, minY = 0, maxX = 0, maxY = 0;

    // find tight boundaries
    minX = std::min( a.left(), b.left() );
    minY = std::min( a.top(), b.top() );

    maxX = std::max( a.right(), b.right() );
    maxY = std::max( a.bottom(), b.bottom() );

    return Rect( Point( minX, minY ), Point( maxX, maxY ) );
}
