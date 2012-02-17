/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#include "worldgen/hallgenerator.h"
#include "worldgen/roomdata.h"
#include "game/tilefactory.h"

#include <boost/function.hpp>
#include <boost/bind.hpp>

/**
 * Hall generator constructor
 */
HallGenerator::HallGenerator( Random& random, 
                              const TileFactory& factory,
                              TileGrid& tileGrid )
    : mRandom( random ),
      mTileFactory( factory ),
      mTileGrid( tileGrid ),
      mPathFinder( tileGrid, boost::bind(&HallGenerator::findMovementCost, this, _1, _2)),
      mpStartRoom( NULL ),
      mpDestRoom( NULL )
{
}

/**
 * Hall generator destructor
 */
HallGenerator::~HallGenerator()
{
    // empty
}

/**
 * Connects two rooms together by carving a hallway betweeen them
 */
void HallGenerator::connect( RoomData *pStartRoom, RoomData *pEndRoom )
{
    assert( pStartRoom != NULL );
    assert( pEndRoom   != NULL );

    Point start = pStartRoom->floorCenter + pStartRoom->worldOffset;
    Point end   = pEndRoom->floorCenter + pEndRoom->worldOffset;

    // Find a path between the two rooms
    std::vector<Point> path = mPathFinder.findPath( start, end );

    // Carve the path out
    for ( auto itr = path.begin(); itr != path.end(); ++itr )
    {
        mTileGrid.set( *itr, mTileFactory.createFloor() );
    }

    // Reset our generator before beginning
    reset( pStartRoom, pEndRoom );
}

/**
 * Resets the generator
 */
void HallGenerator::reset( RoomData *pStartRoom, RoomData *pEndRoom )
{
    assert( pStartRoom != NULL );
    assert( pEndRoom   != NULL );

    mpStartRoom = pStartRoom;
    mpDestRoom  = pEndRoom;
}

/**
 * Custom cost estimation function that creates unqiue hall ways
 * [NEEDS BETTER DESCRIPTION RIGHT NAO]
 */
/**
 * Calculates the cost incurred by moving from the previous point to the
 * given current point. This method assumes that the two points are adjacent
 * to each other.
 *
 * \param  currentPoint  The point to calculate the cost for moving to
 * \param  prevPoint     The point that we are moving from
 * \return               Cost of moving from prevPoint to currentPoint
*/
int HallGenerator::findMovementCost( const Point& currentPoint,
                                  const Point& prevPoint )
{
    const static size_t MOVE_BASE_COST     = 0;
    const static size_t MOVE_STRAIGHT_COST = 10;

    // Factor in the basic cost of movement
    int movementCost = MOVE_BASE_COST;

    // Is the move from prevPoint to currentPoint a straight move, or
    // is it cutting across diagonally?
    if ( currentPoint.x() != prevPoint.x() &&
         currentPoint.y() != prevPoint.y() )
    {
        // disallow diagonals
        return -1;
    }
    else
    {
        // straight move
        movementCost += MOVE_STRAIGHT_COST; 
    }

    return movementCost ;
}

