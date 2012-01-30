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
#ifndef SCOTT_DUNGEON_PATHFINDER_H
#define SCOTT_DUNGEON_PATHFINDER_H

#include <vector>
#include <queue>
#include <boost/utility.hpp>
#include <boost/function.hpp>

#include "common/fixedgrid.h"
#include "common/point.h"

// Forward declarations
class TileGrid;
struct PathNode;

// Function callback typedefs
typedef boost::function<int (const Point&, const Point&)>
    PathFinderCostFunction;

// Priority queue declaration
typedef
    std::priority_queue<PathNode, std::vector<PathNode>, std::greater<PathNode> >
    PathNodePriorityQueue;

// Node open/closed status
enum EPathTileState
{
    PATHTILE_STATE_START,
    PATHTILE_STATE_OPEN,
    PATHTILE_STATE_CLOSED,
    PATHTILE_STATE_COUNT
};

/**
 * Path tile is an internal helper structure that is used by the path finder
 * to keep track of the state of each tile in the tilegrid.
 */
struct PathTile
{
public:
    PathTile();
    PathTile( PathFinderCostFunction costFunc );

    Point prevPos;
    int movementCost;
    size_t estimatedCost;
    EPathTileState state;
};

/**
 * Path node is another internal helper structure. This struct keeps track
 * of open path nodes inside of the priority queue
 */
struct PathNode
{
    PathNode();
    PathNode( const Point& position_, size_t totalCost );

    bool operator > ( const PathNode& rhs ) const;

    Point position;
    size_t totalCost;
};

/**
 * Pathfinder is a configurable A* path finding class that has been built
 * around the game's tilemap interface. 
 *
 * To customize the behavior of the path finder class (say for monster
 * pathfinding with weights), you will want to derive from this class and
 * re-implement isPathable() and findMovementCost() 
 */
class PathFinder : boost::noncopyable
{
public:
    // Constructor
    PathFinder( const TileGrid& map );

    PathFinder( const TileGrid& map, PathFinderCostFunction costFunc );

    // Destructor
    ~PathFinder();

    // Attempts to path from the starting point to the destination
    std::vector<Point> findPath( const Point& start, const Point& dest );

private:
    // Performs one step in the pathfinding algorithm
    void findPathStep();

    // Calculates the estimated cost for pathing from the source point to
    // the destination point
    size_t findEstimatedCost( const Point& from, const Point& to ) const;

    // Calculates the (exact) cost of moving from the starting node to this
    // node, using the path followed from the start
    int findMovementCost( const Point& currentPt, const Point& prevPoint );

    // Generates a list of potentially valid neighbors
    std::vector<Point> generateNeighbors( const Point& currentPoint ) const;

    // Reset the path finder's internal state
    void resetPathFinderState();

    // Checks if allows pathing
    bool isPathable( const Point& ) const;

    // Marks a node as closed
    void markAsClosed( const Point& p );

    // Marks a point as open
    void markAsOpen( const Point& p );

    // Checks if a point is open
    bool isOpen( const Point& p ) const;

    // Checks if a point is closed
    bool isClosed( const Point& p ) const;

private:
    void debugFindPathStep( const Point& p ) const;

private:
    PathFinderCostFunction mCostFunction;
    const bool mAllowDiagonals;

    FixedGrid<PathTile> mPathGrid;
    std::vector<PathNode> mOpenStore;
    PathNodePriorityQueue mOpenNodes;
    Point mStartPoint;
    Point mDestPoint;
    bool mDidPathToEnd;
    bool mFailedToPath;
};

#endif
