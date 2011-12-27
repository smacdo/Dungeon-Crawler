#include "tilegrid.h"
#include <gtest.h>

class TileGridTest : public ::testing::Test
{
public:
    TileGridTest()
        : emptyGrid( 4, 6 ),
          sampleGrid( 4, 6 )
    {
    }

protected:
    TileGrid emptyGrid;
    TileGrid sampleGrid;

    virtual void SetUp()
    {
        TileGrid &t = sampleGrid;
        Tile      w = Tile( TILE_WALL );
        Tile      f = Tile( TILE_FLOOR );

        // Set up the sample grid like so
        //   W W W W 
        //   W F F W
        //   W F F W
        //   W F F W
        //   W W W W
        t.set( 0, 0, w ); t.set( 1, 0, w ); t.set( 2, 0, w ); t.set( 3, 0, w );
        t.set( 0, 1, w ); t.set( 1, 1, f ); t.set( 2, 1, f ); t.set( 3, 1, w );
        t.set( 0, 2, w ); t.set( 1, 2, f ); t.set( 2, 2, f ); t.set( 3, 2, w );
        t.set( 0, 3, w ); t.set( 1, 3, f ); t.set( 2, 3, f ); t.set( 3, 3, w );
        t.set( 0, 4, w ); t.set( 1, 4, w ); t.set( 2, 4, w ); t.set( 3, 4, w );
    }
};

TEST_F(TileGridTest,Constructor_Makes_Area_Empty)
{
    const TileGrid grid( 5, 8 );
    EXPECT_TRUE( grid.isAreaEmpty( Rect(0, 0, 5, 8) ) );
}

TEST_F(TileGridTest,CarveRoom)
{
    TileGrid grid( 4, 6 );

    // It shouldn't be equal yet
    EXPECT_NE( sampleGrid, grid );

    // Carve the room, see if it is what we expected
    grid.carveRoom( Rect( 1, 1, 2, 3 ),
                    1,
                    Tile( TILE_WALL ),
                    Tile( TILE_FLOOR ) );
    EXPECT_EQ( sampleGrid, grid );
}