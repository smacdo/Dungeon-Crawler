#ifndef SCOTT_COMMON_MATH_RECT_H
#define SCOTT_COMMON_MATH_RECT_H

#include <cstddef>
#include <cassert>
#include <ostream>
#include "point.h"

/**
 * A class that represents a 2d rectangle object. Rect assumes that the
 * the coordinate system's (0,0) is centered at the top left.
 *
 */
class Rect
{
public:
    Rect()
        : mTop( 0 ), mLeft( 0 ), mBottom( 0 ), mRight( 0 )
    {
    }


    Rect( const Point& top, int width, int height )
        : mTop( top.y() ),
          mLeft( top.x() ),
          mBottom( mTop + height ),
          mRight( mLeft + width )
    {
        assert( width > 0 );
        assert( height > 0 );
    }

    Rect( int x, int y, int width, int height )
        : mTop( y ),
          mLeft( x ),
          mBottom( y + height ),
          mRight( x + width )
    {
        assert( width > 0 );
        assert( height > 0 );
    }

    bool isNull() const
    {
        return ( mTop   == 0 && mLeft   == 0 &&
                 mRight == 0 && mBottom == 0 );
    }

    int x() const
    {
        return mLeft;
    }

    int y() const
    {
        return mTop;
    }

    int top() const
    {
        return mTop;
    }

    int left() const
    {
        return mLeft;
    }

    int right() const
    {
        return mRight;
    }

    int bottom() const
    {
        return mBottom;
    }

    int width() const
    {
        return mRight - mLeft;
    }

    int height() const
    {
        return mBottom - mTop;
    }

    void translate( int x, int y )
    {
        mTop    += y;
        mLeft   += x;
        mBottom += y;
        mRight  += x;
    }

    void moveTo( int x, int y )
    {
        moveToX( x );
        moveToY( y );
    }

    void moveToX( int x )
    {
        int w = width();

        mLeft  = x;
        mRight = x + w; 
    }

    void moveToY( int y )
    {
        int h = height();

        mTop    = y;
        mBottom = y + h; 
    }

    size_t area() const
    {
        assert(! isNull() );
        return (mRight - mLeft) * (mBottom - mTop);
    }

    bool touches( const Rect& rect ) const
    {
        return ( /*intersects( rect ) ||*/ contains( rect ) );
    }

    //bool intersects( const Rect& rect ) const
    //{
    //    assert(! isNull() );
    //}

    bool contains( const Rect& rect ) const
    {
        assert(! isNull() );

        return ( rect.mLeft >= mLeft && rect.mRight  <= mRight &&
                 rect.mTop  >= mTop  && rect.mBottom <= mBottom );
    }

    bool contains( const Point& p ) const
    {
        assert(! isNull() );

        return ( p.x() >= mLeft && p.x() <= mRight &&
                 p.y() >= mTop  && p.y() <= mBottom );
    }

    friend std::ostream& operator << ( std::ostream& stream, const Rect& r );

private:
    int mTop;
    int mLeft;
    int mBottom;
    int mRight;
};

#endif
