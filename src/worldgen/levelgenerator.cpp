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
#include "worldgen/levelgenerator.h"
#include "worldgen/roomgenerator.h"
#include "worldgen/roomdata.h"
#include "common/utils.h"
#include "common/random.h"

#include "level.h"

#include "room.h"

/**
 * Level generator constructor. Creates a new level generator that is ready
 * to construct as many random levels as you wish.
 *
 * \param  random  Reference to the dungeon generator's random
 * \param  width   Width of the level
 * \param  height  Height of the level
 */
LevelGenerator::LevelGenerator( Random& random,
                                int width,
                                int height )
    : mRandom( random ),
      mLevelWidth( width ),
      mLevelHeight( height ),
      mTileGrid( width, height )
{
    assert( width > 5 );
    assert( height > 5 );
}

/**
 * Destructor
 */
LevelGenerator::~LevelGenerator()
{
}

/**
 * Generates and returns a random level
 */
Level* LevelGenerator::generate()
{
    RoomGenerator roomGenerator( mRandom );
    std::vector<RoomData*> levelRooms;

    // Turn the level border tiles into impassable bedrock tiles to prevent
    // the player (or anyone really) from escaping into the void
    mTileGrid.carveRoom( Rect( 1, 1, mLevelWidth-2, mLevelHeight-2 ),
                         1,
                         Tile( TILE_IMPASSABLE ),
                         Tile( TILE_EMPTY ) );

    // Generate the requested number of rooms
    for ( int i = 0; i < 150; ++i )
    {
        // Generate a random room
        ERoomSize roomSize  = generateRandomRoomSize( ROOM_SIZE_LARGE );
        RoomData *pRoomData = roomGenerator.generate( roomSize );

        // Generate a random position to place it in
        Point placeAt = findRandomPointFor( deref(pRoomData) );

        // Try to place the room's tile grid into our level's tile grid. If it
        // doesn't succeed, make sure to delete the room data before trying
        // another room
        if ( canPlaceRoomAt( deref(pRoomData), placeAt ) )
        {
            mTileGrid.insert( placeAt, pRoomData->tiles );
            levelRooms.push_back( pRoomData );
        }
        else
        {
            Delete( pRoomData );
        }
    }

    // Show debug stats about rooms generated and whatnot

    // Connect rooms together

    // Return the generated level
    return new Level( mTileGrid );
}

/**
 * Creates a random room size, depending on the maximum room size passed
 */
ERoomSize LevelGenerator::generateRandomRoomSize( ERoomSize maxRoomSize ) const
{
    // This needs to be improved
    int whichOne = mRandom.randInt( 0, 100 );

    if ( whichOne < 10 )
    {
        return ROOM_SIZE_TINY;
    }
    else if ( whichOne < 30 )
    {
        return ROOM_SIZE_SMALL;
    }
    else if ( whichOne < 90 )
    {
        return ROOM_SIZE_MEDIUM;
    }
    else
    {
        return ROOM_SIZE_LARGE;
    }
}

/**
 * Finds a random position to attempt to place a room at
 *
 * \param  roomData  The room you are trying to place
 * \return A randomly determined position
 */
Point LevelGenerator::findRandomPointFor( const RoomData& roomData ) const
{
    int maxX = mLevelWidth  - roomData.totalArea.width() - 2;
    int maxY = mLevelHeight - roomData.totalArea.height() - 2;

    return Point( mRandom.randInt( 1, maxX ),
                  mRandom.randInt( 1, maxY ) );
}

/**
 * Checks if the given room can be placed at the requested location
 *
 * \param  roomData  Data for the room you are trying to place
 * \param  pos       Position you are trying to place the room at
 * \return True if the room can be positioned there, false otherwise
 */
bool LevelGenerator::canPlaceRoomAt( const RoomData& roomData,
                                     const Point& pos ) const
{
    return mTileGrid.isAreaEmpty( roomData.totalArea.translate( pos ) );
}
