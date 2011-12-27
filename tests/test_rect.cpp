#include "common/rect.h"
#include <ostream>
#include <gtest.h>
#include <stdint.h>

TEST(Common,Rect_Constructor_Default)
{
    // Default constructor should always create a invalid rect that is
    // positioned on the origin (0,0) with zero width and height
    Rect r;

    EXPECT_EQ( 0, r.top() );
    EXPECT_EQ( 0, r.bottom() );
    EXPECT_EQ( 0, r.right() );
    EXPECT_EQ( 0, r.left() );
}

TEST(Common,Rect_Constructor_PositionWidthHeight)
{
    // Constructor that takes UL position and width/height
    Rect r( Point( 2, 4 ), 5, 6 );

    EXPECT_EQ( 4,  r.top()    );
    EXPECT_EQ( 2,  r.left()   );
    EXPECT_EQ( 10, r.bottom() );
    EXPECT_EQ( 7,  r.right()  );
}

TEST(Common,Rect_Constructor_XYWidthHeight)
{
    // Same as above, but takes x/y rather than a point object
    Rect r( 2, 4, 5, 6 );

    EXPECT_EQ( 4,  r.top()    );
    EXPECT_EQ( 2,  r.left()   );
    EXPECT_EQ( 10, r.bottom() );
    EXPECT_EQ( 7,  r.right()  );
}

TEST(Common,Rect_Constructor_UpperLeftBottomRight)
{
    // Constructor that takes a upper left and bottom right constructor
    Rect r( Point( 3, 2 ), Point( 8, 5 ) );

    EXPECT_EQ( 2, r.top()    );
    EXPECT_EQ( 3, r.left()   );
    EXPECT_EQ( 5, r.bottom() );
    EXPECT_EQ( 8, r.right()  );
}

TEST(Common,Rect_Constructor_Copy)
{
    // COpy constructor
    Rect t( 2, 4, 5, 6 );
    Rect r( t );

    EXPECT_EQ( 4,  r.top()    );
    EXPECT_EQ( 2,  r.left()   );
    EXPECT_EQ( 10, r.bottom() );
    EXPECT_EQ( 7,  r.right()  );
}

TEST(Common,Rect_Operator_Equality)
{
    Rect a( 1, 2, 3, 4 );   // control rect. {ul=(1,2), lr=(4,6) w=3, h=4}
    Rect b( 1, 2, 3, 4 );   // another rect with identical stats
    Rect c( 0, 2, 3, 4 );   // rect with different position but same dims
    Rect e( 1, 0, 3, 4 );   // rect with same x, same dims but diff y
    Rect d( 1, 2, 1, 4 );   // rect with same position, but diff width
    Rect f( 9, 8, 7, 7 );   // rect with diff position, diff dims

    EXPECT_EQ( a, a );      // should always equal self
    EXPECT_EQ( b, a );      // two rects with same position + dims are equal
    EXPECT_FALSE( a == c ); // rects w/ different positions are not equal
    EXPECT_FALSE( a == e ); // same as above
    EXPECT_FALSE( a == d ); // rects w/ different dims are not equal
    EXPECT_FALSE( a == f ); // same as above
}

TEST(Common,Rect_Operator_Inequality)
{
    Rect a( 1, 2, 3, 4 );   // control rect. {ul=(1,2), lr=(4,6) w=3, h=4}
    Rect b( 1, 2, 3, 4 );   // another rect with identical stats
    Rect c( 0, 2, 3, 4 );   // rect with different position but same dims
    Rect e( 1, 0, 3, 4 );   // rect with same x, same dims but diff y
    Rect d( 1, 2, 1, 4 );   // rect with same position, but diff width
    Rect f( 9, 8, 7, 7 );   // rect with diff position, diff dims

    EXPECT_FALSE( a != a ); // should always equal self, therefore this is false
    EXPECT_FALSE( b != a ); // identical pos + dims, therefore this is false
    EXPECT_NE( c, a );      // rects w/ different positions are not equal
    EXPECT_NE( e, a );      // same as above
    EXPECT_NE( d, a );      // rects w/ different dims are not equal
    EXPECT_NE( f, a );      // same as above
}

TEST(Common,Rect_Operator_Assignment)
{
    Rect t( 2, 4, 5, 6 );   // control rect
    Rect a( 0, 0, 1, 1 );   // a default constructed rect not equal to t

    a = t;

    EXPECT_EQ( Rect( 2, 4, 5, 6 ), a ); // just to make values are correct
    EXPECT_EQ( t, a );                  // the actual equality check
}

TEST(Common,Rect_IsNull)
{
    Rect null;                   // default constructed rect was invalid w/h
    Rect notNull( 0, 0, 1, 1 );  // non-default construct rect can never be null   

    EXPECT_TRUE( null.isNull() );
    EXPECT_FALSE( notNull.isNull() );
}

TEST(Common,Rect_Top )
{
    // Check the top y value of the rectangle
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 4, a.top() );
}

TEST(Common,Rect_Bottom)
{
    // Check the bottom y value of the rectangle
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 10, a.bottom() );
}

TEST(Common,Rect_Left)
{
    // Check the left x value of the rectangle
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 2, a.left() );
}

TEST(Common,Rect_Right)
{
    // Check the right value of the rectangle
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 7, a.right() );
}

TEST(Common,Rect_XIsLeft)
{
    // Ensure that the x value is also the same as the left
    //   x = upper LEFT
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 2, a.x() );
}

TEST(Common,Rect_YIsTop)
{
    // Ensure the y value is also the same as the top
    //   y = UPPER left
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 4, a.y() );
}

TEST(Common,Rect_Width)
{
    // Verify the width is calculated correctly
    Rect a( 2, 4, 5, 6 );               // ctor with width / height valeus
    Rect b( Point(2,4), Point(7,10) );  // ctor with UL, LR values

    EXPECT_EQ( 5, a.width() );
    EXPECT_EQ( 5, b.width() );
}

TEST(Common,Rect_Height)
{
    Rect a( 2, 4, 5, 6 );               // ctor with width / height values
    Rect b( Point(2,4), Point(7,10) );  // ctor with UL, LR values

    EXPECT_EQ( 6, a.height() );
    EXPECT_EQ( 6, b.height() );
}

TEST(Common,Rect_TopLeft)
{
    // Checks top left of rectangle is correct
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( Point( 2, 4 ), a.topLeft() );
}

TEST(Common,Rect_TopRight)
{
    // Checks top right of rectangle is correct
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( Point( 7, 4 ), a.topRight() );
}

TEST(Common,Rect_BottomRight)
{
    // Checks bottom right of rectangle is correct
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( Point( 7, 10 ), a.bottomRight() );
}

TEST(Common,Rect_BottomLeft)
{
    // Checks bottom left of rectangle is correct
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( Point( 2, 10 ), a.bottomLeft() );
}

TEST(Common,Rect_ApproximateCenter)
{
    // Test the the approximate center calculation is correct
    Rect a( Point(3, 1), Point(7, 3) ); // ((7+3)/2, (1+3)/2) => (5,2)
    Rect b( Point(2, 4), Point(5, 8) ); // ((2+5)/2, (4+8)/2) => (3.5,6)
    Rect c( Point(2, 4), Point(6, 7) ); // ((2+6)/2, (4+7)/2) => (4,5.5)

    EXPECT_EQ( Point( 5, 2 ), a.approximateCenter() );
    EXPECT_EQ( Point( 3, 6 ), b.approximateCenter() );
    EXPECT_EQ( Point( 4, 5 ), c.approximateCenter() );
}

TEST(Common,Rect_Translate)
{
    const Rect base( 2, 4, 3, 4 );      // Control rectangle
    const Rect expected( 5, 6, 3, 4 );  // Expected result
    const Point dist( 3, 2 );           // Distance to translate rectangle

    EXPECT_EQ( expected, base.translate( dist ) );
}

TEST(Common,Rect_MoveTo)
{
    const Rect base( 2, 4, 3, 4 );      // Control rectangle
    const Rect expected( 5, 6, 3, 4 );  // Expected result
    const Point position( 5, 6 );       // New rectangle position

    Rect result = base;
    result.moveTo( position );

    EXPECT_EQ( expected, result );
}

TEST(Common,Rect_Area)
{
    // Test rectangle area calculation
    Rect a( 0, 0, 6, 4 );   // 6*4 => 24
    Rect b( 2, 3, 6, 4 );   // 4*6 => 24
    Rect c( 9, 1, 8, 3 );   // 8*3 => 24

    EXPECT_EQ( static_cast<size_t>(24), a.area() );
    EXPECT_EQ( static_cast<size_t>(24), b.area() );
    EXPECT_EQ( static_cast<size_t>(24), c.area() );
}

TEST(Common,Rect_Touches_AlwaysTouchesSelf)
{
    // This does not mean that the rectangle touches itself, it means
    // that a rectangle will always be touching another rectangle that
    // occupies the exact same space
    Rect a( 2, 3, 2, 3 );
    EXPECT_TRUE( a.touches(a) );
}

TEST(Common,Rect_Touches_TouchOnly)
{
    Rect a( 2, 3, 2, 3 );       // center [2,3 and 4,6]
    Rect b( 2, 6, 2, 4 );       // touching [2,6 and 4,10]
    Rect c( 4, 3, 3, 3 );       // touching [4,3 and 7,6]

    EXPECT_TRUE( a.touches( b ) );
    EXPECT_TRUE( a.touches( c ) );
}

TEST(Common,Rect_Touches_Intersections)
{
    Rect a( 2, 3, 2, 3 );       // center [2,3 and 4,6]
    Rect b( 3, 2, 2, 2 );       // intersects a, c [3,2 and 5,4]

    EXPECT_TRUE( a.touches( b ) );
}

TEST(Common,Rect_Touches_NoTouchNoIntersect)
{
    Rect a(  2, 3, 2, 3 );      // center [2,3 and 4,6]
    Rect b( -2, 3, 3, 2 );      // no touch/intersect [-2,3 and 1,5]

    EXPECT_FALSE( a.touches( b ) );
}

TEST(Common,Rect_Touches_FullyContained)
{
    // A rectangle that is fully within the bounds of a larger rectangle
    // and has no borders that overlap should always test true

    Rect a( 4, 5, 6, 7 );      // outer rectangle
    Rect b( 5, 6, 4, 4 );      // rectangle contained entirely in ia

    EXPECT_TRUE( a.touches( b ) );
}

TEST(Common,Rect_Intersects_AlwaysIntersectsSelf)
{   
    // This does not mean that the rectangle intersects itself, it means
    // that a rectangle will always be intersecting another rectangle that
    // occupies the exact same space
    Rect a( 2, 3, 2, 3 );
    EXPECT_TRUE( a.intersects( a ) );
}

TEST(Common,Rect_Intersects_TouchOnly)
{
    Rect a( 2, 3, 2, 3 );       // center [2,3 and 4,6]
    Rect b( 2, 6, 2, 4 );       // touching [2,6 and 4,10]
    Rect c( 4, 3, 3, 3 );       // touching [4,3 and 7,6]

    EXPECT_TRUE( a.intersects( b ) );
    EXPECT_TRUE( a.intersects( c ) );
}

/*
TEST(Common,Rect_Touches_Intersections)
{
    Rect a( 2, 3, 2, 3 );       // center [2,3 and 4,6]
    Rect b( 3, 2, 2, 2 );       // intersects a, c [3,2 and 5,4]

    EXPECT_TRUE( a.touches( b ) );
}

TEST(Common,Rect_Touches_NoTouchNoIntersect)
{
    Rect a(  2, 3, 2, 3 );      // center [2,3 and 4,6]
    Rect b( -2, 3, 3, 2 );      // no touch/intersect [-2,3 and 1,5]

    EXPECT_FALSE( a.touches( b ) );
}

TEST(Common,Rect_Touches_FullyContained)
{
    Rect a( 4, 5, 6, 7 );      // outer rectangle
    Rect b( 5, 6, 4, 4 );      // rectangle contained entirely in ia

    EXPECT_TRUE( a.touches( b ) );
}*/
TEST(Common,Rect_Cout)
{
    Rect r( 1, 5, 3, 6 );
    std::ostringstream ss;

    ss << r;

    EXPECT_EQ( "<top: 1, 5; w: 3; h: 6>", ss.str() );
}
