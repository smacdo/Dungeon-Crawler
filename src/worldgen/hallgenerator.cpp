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
      mPathFinder( tileGrid, boost::bind(&HallGenerator::findMovementCost, this, _1, _2, _3)),
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

    // Generate the wall and floor tile templates, and make sure to flag these
    // as being part of a hallway
    Tile floorTile = mTileFactory.createFloor();
    Tile wallTile  = mTileFactory.createWall();

    floorTile.flags().set( ETILE_PLACED );
    floorTile.flags().set( ETILE_IS_HALL );

    wallTile.flags().set( ETILE_PLACED );
    wallTile.flags().set( ETILE_IS_HALL );

    // Carve the path out
    for ( auto itr = path.begin(); itr != path.end(); ++itr )
    {
        mTileGrid.set( *itr, floorTile );
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
 * The hall generator's cost estimation function. This method is fed into the
 * A* pathfinder, which uses it for estimating the cost of moving between
 * adjacent tiles.
 *
 * By varying the cost of specific moves (say, making diagonals very expensive),
 * we can "encourage" the generation of nicer hallways.
 *
 * \param  from  The current tile we are estimating cost for
 * \param  to    Destination tile we are trying to reach
 * \param  prev  The prior point that lead to 'from'
 *
 * \return The cost of moving from current to destination
 */
int HallGenerator::findMovementCost( const Point& from,
                                     const Point& to,
                                     const Point& prev ) const
{
    const int ILLEGAL_MOVE       = -1;
    const int MOVE_BASE_COST     = 10;

    // What tiles are these positions?
    Tile& fromTile = mTileGrid.get( from );
    Tile& toTile   = mTileGrid.get( to );

    // Calcualte the forward velocity of the movement
    //  (used to figure out the next tile straight)
    int xDistance = std::abs( to.x() - from.x() );
    int yDistance = std::abs( to.y() - from.y() );

    Tile& nextTile = mTileGrid.get( Point( xDistance, yDistance ) );

    // Factor in the basic cost of movement
    int movementCost = MOVE_BASE_COST;
    int turnPenalty  = 0;

    // Disallow diagonal moves entirely
    if ( (from.x() != to.x()) && (from.y() != to.y()) )
    {
        return ILLEGAL_MOVE;
    }

    // Avoid the edge of the dungeon, or other impossible to tunnel through
    // elements
    if ( toTile.isGranite() )
    {
        return ILLEGAL_MOVE;
    }

    // Factor in the cost of turning, unless this is the first path node from
    // the origin
    if ( (prev.x() != to.x()) && (prev.y() != to.y()) &&
         (prev.x() != -1 && prev.y() != -1) )
    {
        turnPenalty += 12;
    }

    // Check if we are in a room? (TODO: Make this more nuanced then a simple
    // are you a floor)
    if ( fromTile.isInRoom() )
    {
        // Make sure we are not entering in a corner
        if ( toTile.isWall() && nextTile.isWall() )
        {
            return ILLEGAL_MOVE;
        }

        // Make sure we do not turn while we are in a room's wall
        else if ( fromTile.isWall() && toTile.isWall() && turnPenalty > 0 )
        {
            return ILLEGAL_MOVE;
        }

        // No turning cost when we are in a room
        turnPenalty = 0;
    }

    // Is this a hallway? We like hallways
    if ( toTile.isInHall() )
    {
        movementCost /= 3;
        turnPenalty   = 0;      // merge the intersection
    }
  

    return movementCost + turnPenalty;
}

