#include "common/rect.h"
#include <ostream>
#include <gtest.h>
#include <stdint.h>

TEST(Common,Rect_Constructor_Default)
{
    Rect r;

    EXPECT_EQ( 0, r.top() );
    EXPECT_EQ( 0, r.bottom() );
    EXPECT_EQ( 0, r.right() );
    EXPECT_EQ( 0, r.left() );
}

TEST(Common,Rect_Constructor_PositionWidthHeight)
{
    Rect r( Point( 2, 4 ), 5, 6 );

    EXPECT_EQ( 4,  r.top()    );
    EXPECT_EQ( 2,  r.left()   );
    EXPECT_EQ( 10, r.bottom() );
    EXPECT_EQ( 7,  r.right()  );
}

TEST(Common,Rect_Constructor_XYWidthHeight)
{
    Rect r( 2, 4, 5, 6 );

    EXPECT_EQ( 4,  r.top()    );
    EXPECT_EQ( 2,  r.left()   );
    EXPECT_EQ( 10, r.bottom() );
    EXPECT_EQ( 7,  r.right()  );
}

TEST(Common,Rect_Constructor_UpperLeftBottomRight)
{
    Rect r( Point( 3, 2 ), Point( 8, 5 ) );

    EXPECT_EQ( 2, r.top()    );
    EXPECT_EQ( 3, r.left()   );
    EXPECT_EQ( 5, r.bottom() );
    EXPECT_EQ( 8, r.right()  );
}

TEST(Common,Rect_Constructor_Copy)
{
    Rect t( 2, 4, 5, 6 );
    Rect r( t );

    EXPECT_EQ( 4,  r.top()    );
    EXPECT_EQ( 2,  r.left()   );
    EXPECT_EQ( 10, r.bottom() );
    EXPECT_EQ( 7,  r.right()  );
}

TEST(Common,Rect_Operator_Equality)
{
    Rect a( 1, 2, 3, 4 );
    Rect b( 1, 2, 3, 4 );
    Rect c( 0, 2, 3, 4 );
    Rect e( 1, 0, 3, 4 );
    Rect d( 1, 2, 1, 4 );
    Rect f( 9, 8, 7, 7 );

    EXPECT_EQ( a, a );
    EXPECT_EQ( b, a );
    EXPECT_FALSE( a == c );
    EXPECT_FALSE( a == e );
    EXPECT_FALSE( a == d );
    EXPECT_FALSE( a == f );
}

TEST(Common,Rect_Operator_Assignment)
{
    Rect t( 2, 4, 5, 6 );
    Rect a( 0, 0, 1, 1 );

    a = t;

    EXPECT_EQ( Rect( 2, 4, 5, 6 ), a );
}

TEST(Common,Rect_IsNull)
{
    Rect null;
    Rect notNull( 0, 0, 1, 1 );

    EXPECT_TRUE( null.isNull() );
    EXPECT_FALSE( notNull.isNull() );
}

TEST(Common,Rect_Top )
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 4, a.top() );
}

TEST(Common,Rect_Bottom)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 10, a.bottom() );
}

TEST(Common,Rect_Left)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 2, a.left() );
}

TEST(Common,Rect_Right)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 7, a.right() );
}

TEST(Common,Rect_XIsLeft)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 2, a.x() );
}

TEST(Common,Rect_YIsTop)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 4, a.y() );
}

TEST(Common,Rect_Width)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 5, a.width() );
}

TEST(Common,Rect_Height)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( 6, a.height() );
}

TEST(Common,Rect_TopLeft)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( Point( 2, 4 ), a.topLeft() );
}

TEST(Common,Rect_BottomRight)
{
    Rect a( 2, 4, 5, 6 );
    EXPECT_EQ( Point( 7, 10 ), a.bottomRight() );
}

TEST(Common,Rect_ApproximateCenter)
{
    Rect a( 2, 4, 3, 4 );       // { 2, 4, 5, 8 }
    EXPECT_EQ( Point( 3, 6 ), a.approximateCenter() );
}

TEST(Common,Rect_Translate)
{
    Rect a( 2, 4, 3, 4 );
    Rect b( 5, 6, 3, 4 );

    EXPECT_EQ( b, a.translate( Point(3, 2) ) );
}

TEST(Common,Rect_MoveTo)
{
    Rect a( 2, 4, 3, 4 );
    Rect b( 5, 6, 3, 4 );

}

TEST(Common,Rect_Area)
{
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
