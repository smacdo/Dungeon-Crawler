#include "common/rect.h"

#include <cstddef>
#include <ostream>
#include <cassert>
#include "common/point.h"
 
/**
 * Default constructor. Creates a rectangle positioned at (0,0) with
 * an invalid width and height.
 */
Rect::Rect()
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
Rect::Rect( const Point& top, int width, int height )
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
Rect::Rect( int x, int y, int width, int height )
    : mTop( y ),
      mLeft( x ),
      mBottom( y + height ),
      mRight( x + width )
{
    assert( width > 0 );
    assert( height > 0 );
}

/**
 * Creates a rectangle that encompasses the upperLeft point and the
 * bottomRight point
 *
 * \param  upperLeft    Upper left point of the rectangle
 * \param  bottomRight  Lower right point of the rectangle
 */
Rect::Rect( const Point& upperLeft, const Point& bottomRight )
    : mTop( upperLeft.y() ),
      mLeft( upperLeft.x() ),
      mBottom( bottomRight.y() ),
      mRight( bottomRight.x() )
{
    assert( mTop < mBottom );
    assert( mRight > mLeft );
}

/**
 * Rectangle copy constructor
 */
Rect::Rect( const Rect& r )
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
bool Rect::operator == ( const Rect& rhs ) const
{
    return ( mTop    == rhs.mTop    && mLeft  == rhs.mLeft &&
             mBottom == rhs.mBottom && mRight == rhs.mRight );
}

/**
 * Inequality operator. Check if the other rectangle is not equal to us
 *
 * \param   rhs  The rectangle to for inequality to
 * \return  True if the rectangles are not equal in value
 */
bool Rect::operator != ( const Rect& rhs ) const
{
    return ( mTop    != rhs.mTop    || mLeft  != rhs.mLeft ||
             mBottom != rhs.mBottom || mRight != rhs.mRight );
}

/**
 * Assignment operator
 *
 * \param  rhs  Source rectangle to copy values from
 * \return      Reference to this rectangle
 */
Rect& Rect::operator = ( const Rect& rhs )
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
 *
 * \return  True if the rectangle has zero width or zero height. False
 *          otherwise
 */
bool Rect::isNull() const
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
int Rect::x() const
{
    return mLeft;
}

/**
 * Returns the y coordinate of the rectangle, which is the topmost
 * point in the rectangle
 *
 * \return  The topmost y value in the rectangle
 */
int Rect::y() const
{
    return mTop;
}

/**
 * Returns the top of the rectangle
 *
 * \return  The topmost y value in the rectangle
 */
int Rect::top() const
{
    return mTop;
}

/**
 * Returns the left of the rectangle
 *
 * \return  The leftmost x value in the rectangle
 */
int Rect::left() const
{
    return mLeft;
}

/**
 * Returns the right of the rectangle
 *
 * \return  The rightmost x value in the rectangle
 */
int Rect::right() const
{
    return mRight;
}

/**
 * Returns the bottom of the rectangle
 *
 * \return  The bottom most y value in the rectangle
  */
int Rect::bottom() const
{
    return mBottom;
}

/**
 * Returns the width of the rectangle
 *
 * \return Rectangle width
 */
int Rect::width() const
{
    return mRight - mLeft;
}

/**
 * Returns the height of the rectangle
 *
 * \return Rectangle height
 */
int Rect::height() const
{
    return mBottom - mTop;
}

/**
 * Returns the top left point of the rectangle
 */
Point Rect::topLeft() const
{
    return Point( mLeft, mTop );
}

/**
 * Returns the top right point of the rectangle
 */
Point Rect::topRight() const
{
    return Point( mRight, mTop );
}

/**
 * Returns the bottom right point of the rectangle
 */
Point Rect::bottomRight() const
{
    return Point( mRight, mBottom );
}

/**
 * Returns the bottom left point of the rectangle
 */
Point Rect::bottomLeft() const
{
    return Point( mLeft, mBottom );
}


/**
 * Returns an approximate center point of the rectangle. Values that are
 * not whole numbers will be rounded down to the nearest integer, hence
 * the "approximate" name of the method.
 *
 * \return  Approximate center point of the rectangle
 */
Point Rect::approximateCenter() const
{
    assert(! isNull() );
    int midX = width() / 2;
    int midY = height() / 2;

    return Point( midX + mLeft, midY + mTop );
}

/**
 * Returns a copy of this rectangle translated by the requested distance
 *
 * \param  distance  Distance to move this rectangle by
 * \return           Rectangle that has been translated
 */
Rect Rect::translate( const Point& distance ) const
{
    assert( ! isNull() );

    return Rect( mLeft + distance.x(),
                 mTop  + distance.y(),
                 width(),
                 height()
    );
}

/**
 * Moves the top left corner of the rect to the requested position
 * 
 * \param  pos  The new upper left position of the rectangle
 */
void Rect::moveTo( const Point& pos )
{
    int dX = pos.x() - mLeft;
    int dY = pos.y() - mTop;

    mLeft   += dX;
    mTop    += dY;
    mRight  += dX;
    mBottom += dY;
}

/**
 * Calculate and returns the area of the rectangle
 *
 * \return  Area of the rectangle
 */
size_t Rect::area() const
{
    assert(! isNull() );
    return (mRight - mLeft) * (mBottom - mTop);
}

/**
 * Checks if the given rectangle touches this rectangle
 */
bool Rect::touches( const Rect& rect ) const
{
    // not fully valid, what if borders touch? or totally inside of
    return ( intersects( rect ) || contains( rect ) );
}

bool Rect::intersects( const Rect& rect ) const
{
    return ( mLeft <= rect.mRight  && mRight  >= rect.mLeft &&
             mTop  <= rect.mBottom && mBottom >= rect.mTop );
}

bool Rect::contains( const Rect& rect ) const
{
    return ( rect.mLeft >= mLeft && rect.mRight  <= mRight &&
             rect.mTop  >= mTop  && rect.mBottom <= mBottom );
}

bool Rect::contains( const Point& p ) const
{
    assert(! isNull() );

    return ( p.x() >= mLeft && p.x() <= mRight &&
             p.y() >= mTop  && p.y() <= mBottom );
}

std::ostream& operator << ( std::ostream& os, const Rect& rect )
{
    return os << "<top: " << rect.x() << ", " << rect.y() << "; w: "
              << rect.width() << "; h: " << rect.height() << ">";
}
