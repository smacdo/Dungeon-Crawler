#include "worldgen/levelgenerator.h"
#include "worldgen/roomgenerator.h"

#include "level.h"
#include "room.h"
#include "utils.h"

#include <cassert>

LevelGenerator::LevelGenerator( RoomGenerator *pRoomGen,
                                size_t width,
                                size_t height )
    : mRoomGenerator( pRoomGen ),
      mLevel( NULL ),
      mLevelWidth( width ),
      mLevelHeight( height )
{
    assert( width > 2 );
    assert( height > 2 );
}

LevelGenerator::~LevelGenerator(void)
{
}

Level* LevelGenerator::generate()
{
    mLevel = new Level( mLevelWidth, mLevelHeight );

    // Pre-generate the level borders
    emplaceLevelBorders( deref(mLevel) );

    // Generate the requested number of rooms

    // Try to fit those rooms into the level

    // Hook everything up

    // Return the generated level
    return mLevel;
}

void LevelGenerator::emplaceLevelBorders( Level& level ) const
{
    // Place impassable tiles along the borders of the level
    size_t maxX = level.width()  - 1;
    size_t maxY = level.height() - 1;

    for ( size_t x = 0; x < level.width(); ++x )
    {
        level.tileAt( Point(x, 0) )->makeImpassable();
        level.tileAt( Point(x, maxY))->makeImpassable();
    }

    for ( size_t y = 0; y < level.height(); ++y )
    {
        level.tileAt( Point(0, y) )->makeImpassable();
        level.tileAt( Point(maxX,y) )->makeImpassable();
    }
}

