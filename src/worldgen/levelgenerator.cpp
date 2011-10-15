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

Level* LevelGenerator::generate()
{
    // Turn the level border tiles into impassable bedrock tiles to prevent
    // the player (or anyone really) from escaping into the void
    mTileGrid.carveTiles( Rect( 0, 0, mLevelWidth, mLevelHeight ),
                          Tile( TILE_IMPASSABLE ),
                          Tile( TILE_EMPTY ) );

    // Generate the requested number of rooms
    TileGrid roomTileGrid = mRoomGenerator->generate( ROOM_SIZE_LARGE );

    // Try to fit those rooms into the level
    mTileGrid.insert( Point(1,1), roomTileGrid );

    // Hook everything up

    // Return the generated level
    return new Level( mTileGrid );
}