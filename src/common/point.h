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

#include <iosfwd>
#include "common/types.h"

/**
 * Represents a 2d cartesian point using integer values.
 */
class Point
{
public:
    // Default constructor. Sets the point to (0,0)
    Point();

    // Constructor that takes an intial x/y point
    Point( int x, int y );

    // Copy constructor
    Point( const Point& p );

    // Assignment operator
    Point& operator = ( const Point& rhs  );

    // Equality operator
    bool operator == ( const Point& rhs ) const;

    // Inequality operator
    bool operator != ( const Point& rhs ) const;

    // Addition operator
    Point operator + ( const Point& rhs ) const;

    // Subtraction operator
    Point operator - ( const Point& rhs ) const;

    // Negation operator
    Point operator - () const;

    // Self addition operator
    Point& operator += ( const Point& rhs );

    // Self subtraction operator
    Point& operator -= ( const Point& rhs );

    // Return a translated point
    Point translate( int dx, int dy ) const;

    // Check if point is zero
    bool isZero() const;

    // Get x component
    int x() const;

    // Get y component
    int y() const;

    // Set the x and y component of the point class
    void set( int x, int y );

    /**
     * Serialization
     */
    friend class boost::serialization::access;

    template<typename Archive>
    void serialize( Archive& ar, const unsigned int version )
    {
        ar & mX;
        ar & mY;
    }

private:
    int mX;
    int mY;
};

std::ostream& operator << ( std::ostream& stream, const Point& point );

#endif
