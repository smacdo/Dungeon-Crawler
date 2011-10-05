#include "bsplevelgen.h"
#include "roomgenerator.h"
#include "rect.h"

#include "level.h"
#include "room.h"
#include "tile.h"
#include "tiletype.h"
#include "utils.h"

#include <iostream>
#include <boost/checked_delete.hpp>

static const int LeftMargin  = 0;
static const int RightMargin = 1; 
static const int TopMargin   = 0;
static const int BottomMargin = 1;

BspLevelGenerator::BspLevelGenerator( RoomGenerator *roomGenerator,
                                      int levelWidth,
                                      int levelHeight,
                                      int minRoomSize,
                                      int maxRoomSize )
    : mRoomGenerator( roomGenerator ),
      mLevelBounds( 0, 0, levelWidth, levelHeight ),
      mMinSplitSize( minRoomSize ),
      mMaxSplitSize( maxRoomSize ),
      mRoomDensity( 0.95 ),
      mLevel( NULL )
{
}

BspLevelGenerator::~BspLevelGenerator()
{
    boost::checked_delete( mRoomGenerator );
}

Level* BspLevelGenerator::generate()
{
    // Allocate a new level object, and then start generating it
    mLevel = new Level( mLevelBounds.width(), mLevelBounds.height() );
    mRoomGenerator->setLevel( mLevel );

    subgenerate( mLevelBounds );

    return mLevel;
}

void BspLevelGenerator::subgenerate( const Rect& area )
{
    //
    // Pick a random splitting plane (either x or y), and subdivide our
    // area into two randomly sized parts
    //
    int availableWidth  = area.width() - LeftMargin - RightMargin;
    int availableHeight = area.height() - TopMargin - BottomMargin;
    bool splitOnX       = ( Utils::random( 0, 2 ) == 1 );

    if ( splitOnX == true && (availableWidth > 2 * mMinSplitSize+1) )
    {
        // Generator has enough room and decided to make a vertical split.
        // Split the area in two, and then recursively try again on the
        // smaller parts
        Rect roomA;
        Rect roomB;

        splitX( area, roomA, roomB );

        subgenerate( roomA );
        subgenerate( roomB );

    }
    else if ( splitOnX == false && (availableHeight > 2 * mMinSplitSize+1) )
    {
        // Generate has enough room and decided to make a horizontal split.
        // Split the area in two, and then recursively try again on the
        // smaller parts
        Rect roomA;
        Rect roomB;

        splitY( area, roomA, roomB );

        subgenerate( roomA );
        subgenerate( roomB );
    }
    else
    {
        // Nope! It looks like we've run out of room to split on the
        // requested axis, so we're going to stop here and turn this area
        // of the level into a room of some sort
        assert( area.width() >= mMinSplitSize );
        assert( area.height() >= mMinSplitSize );

        mRoomGenerator->generate( area );
    }
}


void BspLevelGenerator::splitX( const Rect& area, Rect& roomA, Rect& roomB )
{
    int availableWidth = area.width() - LeftMargin - RightMargin;
    assert( availableWidth > ( 2 * mMinSplitSize ) );

    int splitAtX = Utils::random( mMinSplitSize, availableWidth-mMinSplitSize );

    // Now create the split retangles
    roomA = Rect( area.x(), area.y(),
                  splitAtX,
                  area.height() );

    roomB = Rect( area.x() + splitAtX, area.y(),
                  area.width() - splitAtX,
                  area.height() );
}

void BspLevelGenerator::splitY( const Rect& area, Rect& roomA, Rect& roomB )
{
    int availableHeight = area.height() - TopMargin - BottomMargin;
    assert( availableHeight > 2 * mMinSplitSize );

    int splitAtY = Utils::random( mMinSplitSize, availableHeight-mMinSplitSize );

    // Now create the split retangles
    roomA = Rect( area.x(), area.y(),
                  area.width(),
                  splitAtY );

    roomB = Rect( area.x(), area.y() + splitAtY,
                  area.width(),
                  area.height() - splitAtY );
}
