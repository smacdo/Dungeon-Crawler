#include "worldgen/levelgenerator.h"
#include "worldgen/roomgenerator.h"
#include "common/utils.h"

#include "level.h"
#include "room.h"

#include <cassert>

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

    // Try to fit those rooms into the level

    // Hook everything up

    // Return the generated level
    return new Level( mTileGrid );
}