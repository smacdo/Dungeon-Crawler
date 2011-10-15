#ifndef SCOTT_COMMON_MATH_RECT_H
#define SCOTT_COMMON_MATH_RECT_H

#include <cstddef>
#include <ostream>
#include "common/point.h"
#include "common/platform.h"

/**
 * A class that represents a 2d rectangle object. Rect assumes that the
 * the coordinate system's (0,0) is centered at the top left.
 *
 */
class Rect
{
public:
    /**
     * Default constructor. Creates a null rectangle with its top left
     * corner at the origin, and sets the width and height to be zero.
     *
     * This rectangle is not valid for use! You must initialize it to
     * have a non-zero width and height
     */
    Rect()
        : mTop( 0 ), mLeft( 0 ), mBottom( 0 ), mRight( 0 )
    {
    }

    /**
     * Creates a rectangle with the top left corner positionined at
     * the given point, and of the requested width and height.
     *
     * \param  top     Top left corner of the rectangle
     * \param  width   Width of the rectangle, extending right from 'top'
     * \param  height  Height of the rectangle, extending down from 'top'
     */
    Rect( const Point& top, int width, int height )
        : mTop( top.y() ),
          mLeft( top.x() ),
          mBottom( mTop + height ),
          mRight( mLeft + width )
    {
        assert( width > 0 );
        assert( height > 0 );
    }

    /**
     * Creates a rectangle with the top left corner positioned at (x,y)
     * and of the requested width and height
     *
     * \param  x      Left position of the rectangle
     * \param  y      Right position of the rectangle
     * \param  width  The width of the rectangle (must be greater than zero)
     * \param  height Height of the rectangle (must be greater than zero)
     */
    Rect( int x, int y, int width, int height )
        : mTop( y ),
          mLeft( x ),
          mBottom( y + height ),
          mRight( x + width )
    {
        assert( width > 0 );
        assert( height > 0 );
    }

    /**
     * Rectangle copy constructor
     */
    Rect( const Rect& r )
        : mTop( r.mTop ),
          mLeft( r.mLeft ),
          mBottom( r.mBottom ),
          mRight( r.mRight )
    {
    }

    /**
     * Equality operator. Check if the other rect is equal to us.
     *
     * \param   rhs  The rectangle to for equality to
     * \return  True if the rectangles are equal in value
     */
    bool operator == ( const Rect& rhs ) const
    {
        return ( mTop    == rhs.mTop    && mLeft  == rhs.mLeft &&
                 mBottom == rhs.mBottom && mRight == rhs.mRight );
    }

    /**
     * Assignment operator
     */
    Rect& operator = ( const Rect& rhs )
    {
        mTop    = rhs.mTop;
        mLeft   = rhs.mLeft;
        mBottom = rhs.mBottom;
        mRight  = rhs.mRight;

        return *this;
    }

    /**
     * Checks if the rectangle is null, which is when the rectangle has
     * an invalid width or height property. This can only happen if the
     * rectangle was default initialized
     */
    bool isNull() const
    {
        return ( mTop   == 0 && mLeft   == 0 &&
                 mRight == 0 && mBottom == 0 );
    }

    /**
     * Returns the x coordinate of the rectangle, which is the leftmost
     * point in the rectangle
     *
     * \return  The leftmost x value in the rectangle
     */
    int x() const
    {
        return mLeft;
    }

    /**
     * Returns the y coordinate of the rectangle, which is the topmost
     * point in the rectangle
     *
     * \return  The topmost y value in the rectangle
     */
    int y() const
    {
        return mTop;
    }

    /**
     * Returns the top of the rectangle
     *
     * \return  The topmost y value in the rectangle
     */
    int top() const
    {
        return mTop;
    }

    /**
     * Returns the left of the rectangle
     *
     * \return  The leftmost x value in the rectangle
     */
    int left() const
    {
        return mLeft;
    }

    /**
     * Returns the right of the rectangle
     *
     * \return  The rightmost x value in the rectangle
     */
    int right() const
    {
        return mRight;
    }

    /**
     * Returns the bottom of the rectangle
     *
     * \return  The bottom most y value in the rectangle
     */
    int bottom() const
    {
        return mBottom;
    }

    /**
     * Returns the width of the rectangle
     *
     * \return Rectangle width
     */
    int width() const
    {
        return mRight - mLeft;
    }

    /**
     * Returns the height of the rectangle
     *
     * \return Rectangle height
     */
    int height() const
    {
        return mBottom - mTop;
    }

    /**
     * Moves the top left corner of the rect by the requested amount
     *
     * \param  x  Amount to move the rectangle left by
     * \param  y  Amount to move the rectangle down by
     */
    void translate( int x, int y )
    {
        mTop    += y;
        mLeft   += x;
        mBottom += y;
        mRight  += x;
    }

    /**
     * Moves the top left corner of the rect to the requested position
     *
     * \param  x  New top left x value
     * \param  y  New top left y value
     */
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

    /**
     * Calculate and returns the area of the rectangle
     *
     * \return Area of the rectangle
     */
    size_t area() const
    {
        assert(! isNull() );
        return (mRight - mLeft) * (mBottom - mTop);
    }

    /**
     * Checks if the given rectangle touches this rectangle
     */
    bool touches( const Rect& rect ) const
    {
        // not fully valid, what if borders touch? or totally inside of
        return ( intersects( rect ) || contains( rect ) );
    }

    bool intersects( const Rect& rect ) const
    {
        assert(! isNull() );
        return ( mLeft < rect.mRight  && mRight  > rect.mLeft &&
                 mTop  < rect.mBottom && mBottom < rect.mTop );
    }

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
