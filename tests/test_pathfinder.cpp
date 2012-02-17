#include "game/pathfinder.h"
#include "game/tilegrid.h"
#include "game/tiletype.h"
#include "common/point.h"
#include <gtest.h>
#include <vector>

/////////////////////////////////////////////////////////////////////////
// Path finder test fixture
/////////////////////////////////////////////////////////////////////////
class PathFinderTest : public ::testing::Test
{
public:
    PathFinderTest();
    virtual ~PathFinderTest();

protected:
    virtual void SetUp();
    TileGrid createMap( size_t width, size_t height, const int* tiles );
    int noMoveFunction( const Point& from, const Point& to );

private:
    TileType * mpFloorData;
    TileType * mpWallData;
};

/////////////////////////////////////////////////////////////////////////
// Fixed grid unit tests
/////////////////////////////////////////////////////////////////////////

//
// Tests if the path finder can path  if the goal == start
//
TEST_F(PathFinderTest,PathFinder_OneTile_WithGoal)
{
    // Create the map
    const size_t WIDTH  = 1;
    const size_t HEIGHT = 1;
    const int TILES[WIDTH*HEIGHT] = { 0 };
    TileGrid map = createMap( 1, 1, (const int*) TILES );

    // Create pathfinder
    PathFinder pathfinder( map );

    // Find path
    std::vector<Point> path =
        pathfinder.findPath( Point(0,0), Point(0,0) );

    // Verify path
    ASSERT_EQ( 1u, path.size() );
    EXPECT_EQ( Point( 0, 0 ), path[0] );
}

//
// Tests if the path finder reports an error if the goal is out of bounds
//
TEST_F(PathFinderTest,PathFinder_OneTile_GoalOutOfBounds)
{
    // Create the map
    const size_t WIDTH  = 1;
    const size_t HEIGHT = 1;
    const int TILES[WIDTH*HEIGHT] = { 0 };
    TileGrid map = createMap( 1, 1, (const int*) TILES );

    // Create pathfinder
    PathFinder pathfinder( map );

    // Find path
    std::vector<Point> path =
        pathfinder.findPath( Point(0,0), Point(1,0) );

    // Verify path
    ASSERT_EQ( 0u, path.size() );
}

TEST_F(PathFinderTest,PathFinder_SimpleOneStep)
{
    // Create the map
    const size_t WIDTH  = 4;
    const size_t HEIGHT = 3;
    const int TILES[WIDTH*HEIGHT] = { 1, 1, 1, 1,
                                      1, 0, 0, 1,
                                      1, 1, 1, 1 };
    TileGrid map = createMap( 3, 3, (const int*) TILES );

    // Create pathfinder
    PathFinder pathfinder( map );

    // Find path
    std::vector<Point> path =
        pathfinder.findPath( Point(1,1), Point(2,1) );

    // Verify path
    ASSERT_EQ( 2u, path.size() );
    EXPECT_EQ( Point( 1, 1 ), path[0] );
    EXPECT_EQ( Point( 2, 1 ), path[1] );
}

//
// Tests if the path finder can trace a path to a different x and y
// goal. Also ensures that the pathfinder is always generating the same
// path
//
TEST_F(PathFinderTest,PathFinder_NormalPathIsJagged)
{
    // Create the map
    const size_t WIDTH  = 5;
    const size_t HEIGHT = 5;
    const int TILES[WIDTH*HEIGHT] = { 0, 0, 0, 0, 0,
                                      0, 0, 0, 0, 0, 
                                      0, 0, 0, 0, 0,
                                      0, 0, 0, 0, 0,
                                      0, 0, 0, 0, 0 };
    TileGrid map = createMap( WIDTH, HEIGHT, (const int*) TILES );

    // Create pathfinder
    PathFinder pathfinder( map );

    // Find path
    std::vector<Point> path =
        pathfinder.findPath( Point(1,1), Point(4,3) );

    // Verify path
    ASSERT_EQ( 6u, path.size() );
    EXPECT_EQ( Point( 1, 1 ), path[0] );
    EXPECT_EQ( Point( 2, 1 ), path[1] );
    EXPECT_EQ( Point( 2, 2 ), path[2] );
    EXPECT_EQ( Point( 3, 2 ), path[3] );
    EXPECT_EQ( Point( 3, 3 ), path[4] );
    EXPECT_EQ( Point( 4, 3 ), path[5] );
}

//
// Tests if the path finder can find its way around impassable walls
//
TEST_F(PathFinderTest,PathFinder_PathWithWallBlocking)
{
    // Create the map
    const size_t WIDTH  = 5;
    const size_t HEIGHT = 5;
    const int TILES[WIDTH*HEIGHT] = { 0, 0, 0, 0, 0,
                                      0, 0, 0, 1, 1, 
                                      0, 1, 1, 0, 0,
                                      0, 0, 1, 0, 0,
                                      0, 0, 0, 0, 0 };
    TileGrid map = createMap( WIDTH, HEIGHT, (const int*) TILES );

    // Create pathfinder
    PathFinder pathfinder( map );

    // Find path
    std::vector<Point> path =
        pathfinder.findPath( Point(1,1), Point(3,2) );

    // Verify path
    ASSERT_EQ( 10u, path.size() );
    EXPECT_EQ( Point( 1, 1 ), path[0] );
    EXPECT_EQ( Point( 0, 1 ), path[1] );
    EXPECT_EQ( Point( 0, 2 ), path[2] );
    EXPECT_EQ( Point( 0, 3 ), path[3] );
    EXPECT_EQ( Point( 1, 3 ), path[4] );
    EXPECT_EQ( Point( 1, 4 ), path[5] );
    EXPECT_EQ( Point( 2, 4 ), path[6] );
    EXPECT_EQ( Point( 3, 4 ), path[7] );
    EXPECT_EQ( Point( 3, 3 ), path[8] );
    EXPECT_EQ( Point( 3, 2 ), path[9] );
}
///////////////////////////////////////////////////////////////////////////
// Internal helper methods
///////////////////////////////////////////////////////////////////////////
PathFinderTest::PathFinderTest()
    : mpFloorData( new TileType( 1, "floor", ETILE_FLOOR ) ),
      mpWallData( new TileType( 2, "wall", ETILE_WALL ) )
{
}

PathFinderTest::~PathFinderTest()
{
    delete mpFloorData;
    delete mpWallData;
}

void PathFinderTest::SetUp()
{
}

TileGrid PathFinderTest::createMap( size_t width,
                                    size_t height,
                                    const int* tiles )
{
    TileGrid map( width, height );

    for ( size_t y = 0; y < height; ++y )
    {
        for ( size_t x = 0; x < width; ++x )
        {
            size_t offset = y * width + x;
            int    value  = tiles[offset];

            EXPECT_TRUE( value == 0 || value == 1 );

            switch ( value )
            {
                case 0:     // floor
                    map.set( x, y, Tile( mpFloorData ) );
                    break;

                case 1:     // wall
                    map.set( x, y, Tile( mpWallData ) );
                    break;

            }
        }
    }

    return map;
}

int PathFinderTest::noMoveFunction( const Point& from, const Point& to )
{
    return -1;
}
