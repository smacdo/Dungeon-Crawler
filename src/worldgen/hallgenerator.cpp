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
#include <boost/array.hpp>

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

    // Neighbor positions
    const boost::array<Point, 8> NEIGHBOR_DIRS
    {
        Point(  0, -1 ),  // up
        Point(  1, -1 ),  // upper right
        Point(  1,  0 ),  // right
        Point(  1,  1 ),  // lower right
        Point(  0,  1 ),  // down
        Point( -1,  1 ),  // lower left
        Point( -1,  0 ),  // left
        Point( -1, -1 ),  // upper left
    };

    // Generate the wall and floor tile templates, and make sure to flag these
    // as being part of a hallway
    Tile floorTile = mTileFactory.createFloor();
    Tile wallTile  = mTileFactory.createWall();
    Tile doorTile  = mTileFactory.createDoorway();

    floorTile.flags().set( ETILE_PLACED );
    floorTile.flags().set( ETILE_IS_HALL );

    wallTile.flags().set( ETILE_PLACED );
    wallTile.flags().set( ETILE_IS_HALL );

    doorTile.flags().set( ETILE_PLACED );
    doorTile.flags().set( ETILE_IS_ROOM );

    // Scan the returned hallway path. We will need to take note of where
    // to generate hallway tiles, door tiles and other things before proceeding
    // with the actual hallway construction
    std::vector<Point> carvePoints;
    std::vector<Point> doorPoints;

    carvePoints.reserve( path.size() );
    doorPoints.reserve( path.size() );
  
    for ( auto itr = path.begin(); itr != path.end(); ++itr )
    {
        Point point = *itr;
        Tile tile   = mTileGrid.get( point );

        // Freak out if we cannot modify this tile (the pathway generator
        // should have prevented this!)
        assert( tile.isSealed() == false );

        // Do not modify the tile if it inside of a room, unless we are piercing
        // a wall (in which deploy a floor)
        if ( tile.isInRoom() )
        {
            if ( tile.isWall() )
            {
                doorPoints.push_back( point );
            }
            else
            {
                continue;
            }
        }

        // Pierce through a wallways
        if ( tile.isInHall() )
        {
            carvePoints.push_back( point );
        }

        // Create the hall point here
        carvePoints.push_back( point );
    }

    // Carve out the hallway tiles
    for ( auto itr = carvePoints.begin(); itr != carvePoints.end(); ++itr )
    {
        mTileGrid.set( *itr, floorTile );
    }

    // Dude it's a door. Mask any walls next to this tile to prevent doors
    // from being adjacent to each other
    for ( auto itr = doorPoints.begin(); itr != doorPoints.end(); ++itr )
    {
        mTileGrid.set( *itr, doorTile );

        for ( auto neighbor  = NEIGHBOR_DIRS.begin();
                   neighbor != NEIGHBOR_DIRS.end();
                 ++neighbor )
        {
            Point p = *itr + *neighbor;

            if ( mTileGrid.get( p ).isWall() )
            {
                mTileGrid.get( *itr + *neighbor ).setIsSealed( true );
            }
        }
    }

    // Once the hallway tiles have been cut out, we can proceed with the walling
    // the sides of the hallway off (unless there is an already existing tile
    // there)
    for ( auto itr = carvePoints.begin(); itr != carvePoints.end(); ++itr )
    {
        for ( auto neighbor  = NEIGHBOR_DIRS.begin();
                   neighbor != NEIGHBOR_DIRS.end();
                 ++neighbor )
        {
            Point p = *neighbor + *itr;

            if (! mTileGrid.get( p ).isPlaced() )
            {
                mTileGrid.set( p, wallTile );
            }
        }
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

    // Avoid tiles that are sealed
    if ( toTile.isSealed() )
    {
        return ILLEGAL_MOVE;
    }

    // Calcualte the forward velocity of the movement
    //  (used to figure out the next tile straight)
    int xDistance   = std::abs( to.x() - from.x() );
    int yDistance   = std::abs( to.y() - from.y() );
    Point nextPoint = Point( xDistance, yDistance ) + to;

    Tile& nextTile = mTileGrid.get( nextPoint );

    // Factor in the cost of turning, unless this is the first path node from
    // the origin
    if ( (prev.x() != to.x()) && (prev.y() != to.y()) &&
         (prev.x() != -1 && prev.y() != -1) )
    {
        turnPenalty += 12;
    }

    // Check if we are in a room? (TODO: Make this more nuanced then a simple
    // are you a floor)
    if ( toTile.isInRoom() )
    {
        // Make sure we are not entering a room in an awkward position
        if ( toTile.isWall() && nextTile.isWall() )
        {
            return ILLEGAL_MOVE;
        }

        // Make sure we do not turn while we are in a room's wall
        if ( fromTile.isWall() && toTile.isWall() && turnPenalty > 0 )
        {
            return ILLEGAL_MOVE;
        }

        // No turning cost when we are in a room
        turnPenalty = 0;
    }

    // Is this a hallway? We like hallways
    if ( toTile.isInHall() && toTile.isFloor() )
    {
        movementCost /= 3;
        turnPenalty   = 0;      // merge the intersection
    }
  

    return movementCost + turnPenalty;
}

