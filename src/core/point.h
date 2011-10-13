#ifndef SCOTT_COMMON_MATH_POINT_H
#define SCOTT_COMMON_MATH_POINT_H

/**
 * Represents a integer point in 2d cartesian space
 */
class Point
{
public:
    Point()
        : mX( 0 ),
          mY( 0 )
    {
    }

    Point( int x, int y )
        : mX( x ),
          mY( y )
    {
    }

    Point( const Point& p )
        : mX( p.mX ),
          mY( p.mY )
    {
    }

    bool isPositive()
    {
        return ( mX >= 0 && mY >= 0 );
    }

    bool isZero()
    {
        return mX == 0 && mY == 0;
    }

    int x() const
    {
        return mX;
    }

    int y() const
    {
        return mY;
    }

private:
    int mX;
    int mY;
};

#endif
