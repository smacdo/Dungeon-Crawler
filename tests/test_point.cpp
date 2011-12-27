#include "common/point.h"
#include <ostream>
#include <gtest.h>

TEST(Common,Point_Constructor_Default)
{
    const Point a;

    EXPECT_EQ( 0, a.x() );
    EXPECT_EQ( 0, a.y() );
}

TEST(Common,Point_Constructor_XY)
{
    const Point a( 2, 5 );

    EXPECT_EQ( 2, a.x() );
    EXPECT_EQ( 5, a.y() );
}

TEST(Common,Point_Constructor_Copy)
{
    const Point a( 2, 5 );
    const Point b( a );

    EXPECT_EQ( 2, b.x() );
    EXPECT_EQ( 5, b.y() );
}

TEST(Common,Point_Operator_Equality)
{
    const Point a( 1, 4 );
    const Point b( 0, 4 );
    const Point c( 1, 0 );
    const Point d( 4, 1 );
    const Point e( 1, 4 );

    EXPECT_EQ( a, a );
    EXPECT_EQ( e, a );

    EXPECT_FALSE( a == b );
    EXPECT_FALSE( a == c );
    EXPECT_FALSE( a == d );
    EXPECT_TRUE( a == e );
}

TEST(Common,Point_Operator_Inequality)
{
    const Point a( 1, 4 );
    const Point b( 0, 4 );
    const Point c( 1, 0 );
    const Point d( 4, 1 );
    const Point e( 1, 4 );

    EXPECT_TRUE( a != b );
    EXPECT_TRUE( a != c );
    EXPECT_TRUE( a != d );

    EXPECT_FALSE( a != a );
    EXPECT_FALSE( a != e );
}

TEST(Common,Point_Operator_Assignment)
{
    Point a( 2, 4  );
    const Point b( 5, 7  );

    a = b;

    EXPECT_EQ( Point( 5, 7 ), a );
}

TEST(Common,Point_Operator_LessThan)
{
    Point a( 5, 7 );

    Point b( 6, 9 );      // a < b, both components are obviously larger
    Point c( 6, 7 );      // a < b, y is the same but x is larger
    Point d( 4, 8 );      // a < b, y is larger but x is smaller

    Point e( 5, 6 );      // a > e, y is smaller and x is the same
    Point f( 1, 1 );      // a > f, both y and x are smaller

    EXPECT_LT( a, b );
    EXPECT_LT( a, c );
    EXPECT_LT( a, d );

    EXPECT_FALSE( a < a );
    EXPECT_FALSE( a < e );
    EXPECT_FALSE( a < f );

    EXPECT_LT( e, a );
    EXPECT_LT( f, a );
}

TEST(Common,Point_Operator_GreaterThan)
{
    Point a( 5, 7 );

    Point b( 3, 2 );    // a > b because b.x and b.y are smaller
    Point c( 6, 6 );    // a > b, c.x is larger than a.x but c.y smaller
    Point d( 4, 7 );    // a > c, d.x is smaller even if a.y == d.y

    Point e( 6, 7 );    // e > a because e.x is larger even if a.y == e.y
    Point f( 2, 8 );    // f > a because f.y is larger even if f.x is smaller

    EXPECT_FALSE( a > a );      // not possible logically

    EXPECT_GT( a, b );
    EXPECT_GT( a, c );
    EXPECT_GT( a, d );

    EXPECT_FALSE( a > e );
    EXPECT_FALSE( a > f );

    EXPECT_GT( e, a );
    EXPECT_GT( f, a );
}

TEST(Common,Point_Operator_Addition)
{
    const Point a( 2, 5 );
    const Point b( 7, 1 );
    const Point r( 9, 6 );

    EXPECT_EQ( r, a + b );
}

TEST(Common,Point_Operator_SelfAddition)
{
    const Point a( 2, 5 );
    const Point b( 7, 1 );
    const Point r( 9, 6 );

    Point t = a;
    t += b;

    EXPECT_EQ( r, t );
}

TEST(Common,Point_Operator_Subtraction)
{
    const Point a(  2, 5 );
    const Point b(  7, 1 );
    const Point r( -5, 4 );

    EXPECT_EQ( r, a - b );
}

TEST(Common,Point_Operator_SelfSubtraction)
{
    const Point a(  2, 5 );
    const Point b(  7, 1 );
    const Point r( -5, 4 );

    Point t = a;
    t -= b;

    EXPECT_EQ( r, t );
}

TEST(Common,Point_Operator_Negation)
{
    Point a( 2, -3 );
    const Point r( -2, 3 );

    EXPECT_EQ( r, -a );
}

TEST(Common,Point_Translate)
{
    const Point a( 2, 4 );
    const Point r( 7, 10 );

    Point t = a.translate( 5, 6 );

    EXPECT_EQ( r, t );
    EXPECT_EQ( Point( 2, 4 ), a );
}

TEST(Common,Point_IsZero)
{
    const Point a;
    const Point b( 0, 0 );
    const Point c( 1, 0 );
    const Point d( 0, 1 );
    const Point e( 1, 1 );

    EXPECT_TRUE( a.isZero() );
    EXPECT_TRUE( b.isZero() );
    EXPECT_FALSE( c.isZero() );
    EXPECT_FALSE( d.isZero() );
    EXPECT_FALSE( e.isZero() );
}

TEST(Common,Point_Set)
{
    Point a( 2, 5 );
    a.set( 3, 2 );

    EXPECT_EQ( Point( 3, 2 ), a );
}

TEST(Common,Point_Cout)
{
    const Point r( 2, 5 );
    std::ostringstream ss;

    ss << r;

    EXPECT_EQ( "<x: 2, y: 5>", ss.str() );
}
