/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#include "common/point.h"
#include "common/types.h"
#include <ostream>

/**
 * Default constructor. Sets the point to (0,0)
 */
Point::Point()
    : mX( 0 ),
      mY( 0 )
{
}

/**
 * Initialize point to be (x,y).
 *
 * \param  x  The x coordinate
 * \param  y  The y coordinate
 */
Point::Point( int x, int y )
    : mX( x ),
      mY( y )
{
}

/**
 * Copy constructor
 *
 * \param  p  The point to copy values from
 */
Point::Point( const Point& p )
    : mX( p.mX ),
      mY( p.mY )
{
}

/**
 * Assignment operator
 *
 * \param  rhs  The point to copy values from
 */
Point& Point::operator = ( const Point& rhs  )
{
    mX = rhs.mX;
    mY = rhs.mY;

    return *this;
}

/**
 * Equality operator
 */
bool Point::operator == ( const Point& rhs ) const
{
    return mX == rhs.mX && mY == rhs.mY;
}

/**
 * Inequality operator
  */
bool Point::operator != ( const Point& rhs ) const
{
    return ( mX != rhs.mX || mY != rhs.mY );
}

bool Point::operator < ( const Point& rhs ) const
{
    return ( mY < rhs.mY || ( mY == rhs.mY && mX < rhs.mX ) );
}

bool Point::operator > ( const Point& rhs ) const
{
    return ( mY > rhs.mY || ( mY == rhs.mY && mX > rhs.mX ) );
}

/**
 * Addition operator
 */
Point Point::operator + ( const Point& rhs ) const
{
    return Point( mX + rhs.mX, mY + rhs.mY );
}

/**
 * Subtraction operator
 */
Point Point::operator - ( const Point& rhs ) const
{
    return Point( mX - rhs.mX, mY - rhs.mY );
}

/**
 * Negation operator
 */
Point Point::operator - () const
{
    return Point( -mX, -mY );
}

/**
 * Self addition operator
 */
Point& Point::operator += ( const Point& rhs )
{
    mX += rhs.mX;
    mY += rhs.mY;
    return *this;
}

/**
 * Self subtraction operator
 */
Point& Point::operator -= ( const Point& rhs )
{
    mX -= rhs.mX;
    mY -= rhs.mY;
    return *this;
}

/**
 * Return a copy of the point that has been moved by the specified
 * distance
 */
Point Point::translate( int dx, int dy ) const
{
    return Point( mX + dx, mY + dy );
}

/**
 * Checks if the point is at the origin (0,0)
 *
 * \return True if the point is located at (0,0)
 */
bool Point::isZero() const
{
    return mX == 0 && mY == 0;
}

/**
 * Return the x component of the point
 */
int Point::x() const
{
    return mX;
}

/**
 * Return the y component of the point
 */
int Point::y() const
{
    return mY;
}

/**
 * Set the x and y component of the point
 *
 * \param  x  Value to set the x component to
 * \param  y  Value to set the y component to
 */
void Point::set( int x, int y )
{
    mX = x;
    mY = y;
}

std::ostream& operator << ( std::ostream& stream, const Point& point )
{
    stream << "<x: " << point.x() << ", y: " << point.y() << ">";
    return stream;
}
