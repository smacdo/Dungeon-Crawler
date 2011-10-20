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
#include "common/utils.h"
#include "common/random.h"

#include "level.h"
#include "room.h"

LevelGenerator::LevelGenerator( RoomGenerator *pRoomGen,
                                int width,
                                int height )
    : mRoomGenerator( pRoomGen ),
      mLevelWidth( width ),
      mLevelHeight( height ),
      mTileGrid( width, height )
{
    assert( width > 5 );
    assert( height > 5 );
}

LevelGenerator::~LevelGenerator()
{
    boost::checked_delete( mRoomGenerator );
}

Level* LevelGenerator::generate( Random& random )
{
    // Turn the level border tiles into impassable bedrock tiles to prevent
    // the player (or anyone really) from escaping into the void
    mTileGrid.carveRoom( Rect( 0, 0, mLevelWidth, mLevelHeight ),
                         Tile( TILE_IMPASSABLE ),
                         Tile( TILE_EMPTY ) );

    // Generate the requested number of rooms
    for ( int i = 0; i < 150; ++i )
    {
        // Generate a random room
        ERoomSize roomSize    = generateRandomRoomSize( ROOM_SIZE_HUGE, random );
        TileGrid roomTileGrid = mRoomGenerator->generate( roomSize, random );

        // Generate a random position to place it in
        int roomWidth  = roomTileGrid.width();
        int roomHeight = roomTileGrid.height();

        Point upperLeft( random.randInt( 1, mLevelWidth - roomWidth - 2   ),
                         random.randInt( 1, mLevelHeight - roomHeight - 2 ) );

        // Try to place it
        if ( mTileGrid.isAreaEmpty( Rect( upperLeft, roomWidth, roomHeight ) ) )
        {
            mTileGrid.insert( upperLeft, roomTileGrid );
        }
    }

    // Show debug stats about rooms generated and whatnot


    // Hook everything up

    // Return the generated level
    return new Level( mTileGrid );
}

ERoomSize LevelGenerator::generateRandomRoomSize( ERoomSize maxRoomSize,
                                                  Random& random ) const
{
    // This needs to be improved
    int whichOne = random.randInt( 0, 100 );

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
        return ROOM_SIZE_HUGE;
    }
}