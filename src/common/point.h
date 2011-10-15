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
#ifndef SCOTT_COMMON_MATH_POINT_H
#define SCOTT_COMMON_MATH_POINT_H

/**
 * Represents a 2d cartesian point using integer values. Values are
 * constrained to the range [0, INT_MAX]
 */
class Point
{
public:
    /**
     * Default constructor. Sets the point to (0,0)
     */
    Point()
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
    Point( int x, int y )
        : mX( x ),
          mY( y )
    {
    }

    /**
     * Copy constructor
     *
     * \param  p  The point to copy values from
     */
    Point( const Point& p )
        : mX( p.mX ),
          mY( p.mY )
    {
    }

    /**
     * Assignment operator
     *
     * \param  rhs  The point to copy values from
     */
    Point& operator = ( const Point& rhs  )
    {
        mX = rhs.mX;
        mY = rhs.mY;

        return *this;
    }

    /**
     * Equality operator
     */
    bool operator == ( const Point& rhs ) const
    {
        return mX == rhs.mX && mY == rhs.mY;
    }

    /**
     * Inequality operator
     */
    bool operator != ( const Point& rhs ) const
    {
        return ( mX != rhs.mX || mY != rhs.mY );
    }

    /**
     * Checks if the point is at the origin (0,0)
     *
     * \return True if the point is located at (0,0)
     */
    bool isZero()
    {
        return mX == 0 && mY == 0;
    }

    /**
     * Return the x component of the point
     */
    int x() const
    {
        return mX;
    }

    /**
     * Return the y component of the point
     */
    int y() const
    {
        return mY;
    }

private:
    int mX;
    int mY;
};

#endif
