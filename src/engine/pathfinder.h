#ifndef SCOTT_DUNGEON_PATHFINDER_H
#define SCOTT_DUNGEON_PATHFINDER_H

#include <vector>
#include <queue>

#include "common/point.h"

// Forward declarations
class Tilemap;

/**
 * Path node is an internal helper structure that is used by the path
 * finder class. It is used to keep track of all possible paths and their
 * weights
 */
struct PathNode
{
public:
    PathNode();
    bool operator < ( const PathNode& rhs ) const;

    Point prevPos;
    int movementCost;
    int estimatedCost;
    bool openState;
    bool closeState;
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
    PathFinder( const Tilemap& map );

    // Destructor
    ~PathFinder();

    // Attempts to path from the starting point to the destination
    std::vector<Point> findPath( const Point& start, const Point& dest );

private:
    // Performs one step in the pathfinding algorithm
    void findPathStep();

    // Calculates the estimated cost for pathing from the source point to
    // the destination point
    int findEstimatedCost( const Point& from, const Point& to ) const;

    // Calculates the (exact) cost of moving from the starting node to this
    // node, using the path followed from the start
    int findMovementCost( const Point& currentPt, const Point& prevPoint ) const;

    // Generates a list of potentially valid neighbors
    std::vector<Point> generateNeighbors( const Point& currentPoint ) const;

    // Reset the path finder's internal state
    void resetPathFinderState();

    // Checks if allows pathing
    bool isPathable( const Point& ) const;

    // Marks a node as closed
    void markAsClosed( const Point& p ) const;

    // Marks a point as open
    void markAsOpen( const Point& p ) const;

    // Checks if a point is open
    bool isOpen( const Point& p ) const;

    // Checks if a point is closed
    bool isClosed( const Point& p ) const;

private:
    void debugFindPathStep( const Point& p ) const;

private:
    const Tilemap& mMap;
    const bool mAllowDiagonals;
    const int mBaseMoveCost;
    const int mMoveStraightCost;
    const int mMoveDiagonalCost;

    FixedGrid<PathNode> mPathGrid;
    std::vector<Point> mOpenStore;
    std::priority_queue<Point> mOpenNodes;
    Point mStartPoint;
    Point mDestPoint;
    bool mDidPathToEnd;
    bool mFailedToPath;
};

#endif
