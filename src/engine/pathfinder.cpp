#include "engine/pathfinder.h"
#include "common/point.h"
#include "common/fixedgrid.h"
#include "common/logger.h"
#include "tilegrid.h"

#include <iostream>
#include <string>
#include <vector>
#include <cassert>
#include <algorithm>
#include <queue>

/**
 * Path node constructor. Path node is an internal structure used by
 * the path finder class to keep track of potential paths while searching
 * for the optimal one
 */
PathNode::PathNode()
    : prevPos( -1, -1 ),
      movementCost( -1 ).
      estimatedCost( -1 ),
      mOpenState( false ),
      mCloseState( false )
{
}

/**
 * Used to compare one path node's cost with another path node's cost
 * for use when ordering path nodes in a list.
 *
 * \param  rhs  Node to check against
 * \return      True if this node has a lesser total cost than rhs
 */
bool PathNode::operator < ( const PathNode& rhs ) const
{
    return (movementCost     + estimatedCost) < 
           (rhs.movementCost + rhs.estimatedCost);
}

/**
 * Pathfinder constructor.
 */
PathFinder::PathFinder( const Tilemap& map )
    : mMap( map ),
      mAllowDiagonals( false ),
      mBaseMoveCost( 0 ),
      mMoveStraightCost( 10 ),
      mMoveDiagonalCost( 14 ),
      mPathGrid( map.width(), map.height() ),
      mOpenStore( 256 ),
      mOpenNodes( mOpenStore.begin(), mOpenStore.end() ),
      mStartPoint(),
      mDestPoint(),
      mDidPathToEnd( false ),
      mFailedToPath( false ),
{
}

/**
 * Pathfinder destructor
 */
PathFinder::~PathFinder()
{
}

/**
 * Using A*, this method attempts to find the shortest path between
 * the original starting location and the requested destination location.
 * Find path will make a good faith attempt to discover a path from the
 * start to the destination, but if it fails to do so the returned list of
 * points will be empty
 *
 * \param  start  The starting point
 * \param  dest   The ending point that we should path to
 * \return        A vector containing the points to take to get from the
 *                starting point to the end point.
 */
std::vector<Point> PathFinder::findPath( const Point& start, const Point& dest )
{
    // Initialize the pathfinder by first resetting our internal state,
    // and then adding the starting position to our list of open nodes
    resetPathFinderState();
    markAsOpen( start );

    mStartPoint = start;
    mDestPoint  = dest;

    std::cerr << "===================================" << std::endl
              << "Starting findpath... "               << std::endl
              << "  start: "  << mStartPoint           << std::endl
              << "  dest : "  << mDestPoint            << std::endl
              << " " << std::endl;

    // Now start the iterative path finding operation by continually
    // trying to find a path to the destination
    const size_t MAX_STEPS = 25;
    bool keepSearching     = true;
    size_t step            = 0;

    while ( step < MAX_STEPS && (!mDidPathToEnd) && (!mFailedToPath) )
    {
        findPathStep();
        ++step;
    }

    // Check if we have found a valid path from the starting point to the
    // ending point. If so, follow the path backward and add each node to
    // the path that we will return to the caller
    std::vector<Point> path;

    if ( mDidPathToEnd )
    {
        Point currentPos = dest;

        // Walk backward from the destination back to the starting position
        while ( p != start )
        {
            path.push_back( currentPos );
            currentPos = mTileGrid.get(currentPos).prevPos;
        }

        // Reverse the list, so the caller sees it as a list of points from
        // the start to the end
        std::reverse( path.begin(), path.end() );
    }

    return path;
}

/**
 * Performs a single step of the A* algorithm, and updates the internal
 * state of the path finder each time it is called
 */
void PathFinder::findPathStep()
{
    assert( mDidPathToEnd == false && mFailedToPath == false );

    // Pull the next cheapest open position from our list of still open
    // tiles. Make sure to mark it as closed to prevent us from re-visiting
    // this tile again
    //
    // If there are no entries left in the open nodes list, then we have
    // exhausted all posibilities and must return an error
    if ( openNodes.empty() )
    {
        mFailedToPath = true;
        return;
    }

    Point currentPos = openNodes.top();
    openNodes.pop();

    markAsClosed( currentPos );

    debugFindPathStep( currentPos );

    // Keep track of the recorded movement cost to go from the start to this
    // position. We will need it when adding new open neighbor tiles
    int currentMovementCost = mPathGrid.get( currentPos ).movementCost;

    // Did we just pop off the destination tile? If so, we managed to path
    // to the destination succesfully!
    if ( currentPos == dest )
    {
        mDidPathToEnd = true;
        return;
    }

    // Who will be our neighbors? Will they be our friends?
    std::vector<Point> neighbors =
        generateNeighbors( currentPos, neighbors );

    // Consider each generated neighbor. If the tile is both a valid
    // walkable tile, is in bounds and has not been visited yet then
    // calculate its cost and add it to the list of open tiles
    for ( size_t i = 0; i < neighbors.size(); ++i )
    {
        // Make sure that this neighbor isn't already marked as closed. If
        // it is, then don't even bother considering it
        if ( isClosed( neighbors[i] ) )
        {
            continue;
        }

        // Before counting cost, ensure that this tile we are considering
        // supports pathing. If isPathable returns false, we consider this
        // unreachable and therefore should skip it
        if ( isPathable( neighbors[i] ) == false )
        {
            continue;
        }

        // Calculate the costs of moving to this node, and the
        // estimated cost of getting to the destination
        int estimatedCost = findEstimatedCost( neighbors[i], mDestPoint );
        int movementCost  = findMovementCost( neighbors[i], currentPos ) +
                            currentMovementCost;

        // Ensure that this position supports a path. If the returned
        // movement cost is less than zero skip this tile
        if ( movementCost < 0  )
        {
            continue;
        }
        
        // Debugging help
        std::cerr << "  ==> Considering " << neighbors[i]
                  << ", e: "              << estimatedCost
                  << ", m: "              << movementCost
                  << ", t: "              << estimatedCost + movementCost
                  << std::endl;


        // Now is this node already listed on the open list? Search the
        // list of open nodes, and see if we find a match
        PathNode& existingNode = mPathGrid.get( neighbors[i] );

        if ( isOpen( neighbors[i] ) ) 
        {
            // The node is already in the list of open tiles. Do we have
            // a better movement score than the already stored node?
            if ( movementCost < existingNode.movementCost )
            {
                existingNode.prevPos       = currentPos;
                existingNode.movementCost  = movementCost;
                existingNode.estimatedCost = estimatedCost;

                std::cerr << "   * Replacing existing open node"
                          << ",old e: " << existingNode.estimatedCost
                          << ",old f: " << existingNode.movementCost
                          << std::endl;
            }
            else
            {
                std::cerr << "   * Existing open node is already better"
                          << std::endl;
            }
        }
        else
        {
            std::cerr << "   * Adding to open node list" << std::endl;

            // Update the path node element for this position
            existingNode.prevPos       = currentPos;
            existingNode.movementCost  = movementCost;
            existingNode.estimatedCost = estimatedCost;

            // Make sure this tile is added to the list of open points
            markAsOpen( neighbors[i] );
        }
    }
}

/**
 * Calculates the estimated cost for moving from the given current tile
 * location to the destination tile location. This is a conservative
 * cost estimator that attempts to never over estimate the actual cost,
 * and is needed for accurate a* path finding
 *
 * \param  from  The current tile we are estimating cost for
 * \param  to    Destination tile we are trying to reach
 * \return The estimated cost of moving from current to destination
 */
int PathFinder::findEstimatedCost( const Point& from,
                                   const Point& to ) const
{
    int xDistance = std::abs( from.x() - to.x() );
    int yDistance = std::abs( from.y() - to.y() );
    int estimated = 0;

    if ( xDistance > yDistance )
    {
        estimated = mMoveDiagonalCost * yDistance +
                    mMoveStraightCost * ( xDistance - yDistance );
    }
    else
    {
        estimated = mMoveDiagonalCost * xDistance +
                    mMoveStraightCost * ( yDistance - xDistance );
    }

    return estimated;
}

/**
 * Calculates the cost incurred by moving from the previous point to the
 * given current point. This method assumes that the two points are adjacent
 * to each other.
 *
 * \param  currentPoint  The point to calculate the cost for moving to
 * \param  prevPoint     The point that we are moving from
 * \return               Cost of moving from prevPoint to currentPoint
*/
int PathFinder::findMovementCost( const Point& currentPoint,
                                  const Point& prevPoint ) const
{
    // Factor in the basic cost of movement
    int movementCost = mBaseMoveCost;

    // Is the move from prevPoint to currentPoint a straight move, or
    // is it cutting across diagonally?
    if ( currentPt.x() == prevPt.x() || currentPt.y() == prevPt.y() )
    {
        // straight move
        movementCost += mMoveStraightCost;
    }
    else
    {
        // diagonal move
        movementCost += mMoveDiagonalCost;
    }

    return movementCost;
}

/**
 * Generates a list of all potentially valid points located adjacent
 * to the requested point 'pt', and stores them into the candidates
 * array. Note that the list of neighbors may not out of bounds, or
 * unwalkable so it is up to the caller to test and discard these incorrect
 * locations
 *
 * \param  pt          The point to generate neighbors for
 * \param  candidates  An array to place the neighbor positions into
 */
std::vector<Point> PathFinder::generateNeighbors( const Point& currentPoint ) const
{
    const size_t MAX_STRAIGHT_TILES = 4;
    const size_t MAX_NEIGHBOR_TILES = 8;
    const int NEIGHBOR_OFFSETS[MAX_NEIGHBOR_TILES][2] =
    {
        {  0, -1 },     // north
        {  1,  0 },     // east
        {  0,  1 },     // south
        { -1,  0 },     // west
        {  1, -1 },     // north east
        {  1,  1 },     // south east
        { -1,  1 },     // south west
        { -1, -1 }      // north west
    };

    std::vector<Point> output;

    // Consider each potential neighbor offset and if the point is still
    // within the bounds of the tilemap, add it to the output list
    int mapWidth  = static_cast<int>( mMap.width() );
    int mapHeight = static_cast<int>( mMap.height() );

    for ( size_t i = 0; i < MAX_STRAIGHT_TILES; ++i )
    {
        Point n = currentPoint + Point( NEIGHBOR_OFFSET[i][0],
                                        NEIGHBOR_OFFSET[i][1] );

        if ( n.x() >= 0 && n.x() < mapWidth &&
             n.y() >= 0 && n.y() < mapHeight )
        {
            output.push_back( n );
        }
    }

    return output;
}

/**
 * Reset the internal state of the pathfinder in preparation for a
 * new pathfinding operation
 */
void PathFinder::resetPathFinderState()
{
    mPathGrid.clear();
    mOpenStore.clear();

    mOpenNodes = std::priority_queue( mOpenStore.begin(),
                                      mOpenStore.end() );
 
    mDidPathToEnd = false;
    mFailedToPath = false;
}

/**
 * Checks if the given node can be pathed to or through. If the position
 * does not support pathing (say it is a huge rock), then this function
 * should return false
 *
 * \param  point  The point to consider
 * \return        True if we can path onto this point, false otherwise
 */
bool PathFinder::isPathable( const Point& ) const
{
    return true;    // for the moment, always true
}

/**
 * Marks the requested point as being closed
 *
 * \param  point  Location of the point to mark as closed
 */
void PathFinder::markAsClosed( const Point& point ) const
{
    PathNode& node = mPathGrid.get( point );
    assert( node.openState == false && node.closeState == false );

    node.closeState = true;   
}

/**
 * Marks the requested point as being open
 *
 * \param  point  Location of the point to mark as open
 */
void PathFinder::markAsOpen( const Point& point ) const
{
    PathNode& node = mPathGrid.get( point );
    assert( node.openState  == false && node.closeState == false );

    // Update the path node where this point is located as being open,
    // and then add it to the priority queue of open points
    node.openState  = true;
    mOpenNodes.push( point );
}

/**
 * Checks if the requested point is marked as being open
 *
 * \param  point   The point to check
 * \return         True if the point is open, false otherwise
 */
bool PathFinder::isOpen( const Point& point ) const
{
    return mPathGrid.get( point ).openState;
}

/**
 * Checks if the requested point is marked as being closed
 *
 * \param  point  The point to check
 * \return        True if the point is closed, false otherwise
 */
bool PathFinder::isClosed( const Point& point ) const
{
    return mPathGrid.get( point ).closeState;
}

/**
 * Internal debugging information
 */
void PathFinder::debugFindPathStep( const Point& currentPoint ) const
{
    const PathNode& node = mPathGrid.get( currentPoint );

    std::cerr << "_____________________________ "    << std::endl
              << " Picked  : " << currentPoint       << std::endl
              << "  prevPos: " << node.prevPos       << std::endl
              << "  moveC  : " << node.movementCost  << std::endl
              << "  estimC : " << node.estimatedCost << std::endl
              << " "                                 << std::endl;
}
