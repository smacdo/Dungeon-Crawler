/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#ifndef SCOTT_COMMON_MATH_RECT_H
#define SCOTT_COMMON_MATH_RECT_H

#include <iosfwd>
#include "common/point.h"

// boost serialization forward declaration
namespace boost { namespace serialization { class access; } }

/**
 * A class that represents a 2d rectangle object. Rect assumes that the
 * the coordinate system's (0,0) is centered at the top left.
 *
 */
class Rect
{
public:
    // Default constructor
    Rect();

    // Upper left + width and height constructor
    Rect( const Point& top, int width, int height );

    // Upper left + width and height constructor
    Rect( int x, int y, int width, int height );
    
    // Upper left and bottom right point constructor
    Rect( const Point& upperLeft, const Point& bottomRight );

    // Copy constructor
    Rect( const Rect& r );

    // Equality operator
    bool operator == ( const Rect& rhs ) const;

    // Inequality operator
    bool operator != ( const Rect& rhs ) const;

    // Assignment operator
    Rect& operator = ( const Rect& rhs );

    // Checks if rectangle has invalid width/height
    bool isNull() const;

    // Return x position of rectangle (upper left x)
    int x() const;

    // Return y position of rectangle (upper left y)
    int y() const;

    // Return top y of rectangle
    int top() const;

    // Return left x of rectangle
    int left() const;

    // Return right x of rectangle 
    int right() const;

    // Return bottom y of rectangle
    int bottom() const;

    // Width of rectangle
    int width() const;

    // Height of rectangle
    int height() const;

    // Top left of rectangle
    Point topLeft() const;

    // Top right of rectangle
    Point topRight() const;

    // Bottom left of rectangle
    Point bottomLeft() const;

    // Bottom right corner of rectangle
    Point bottomRight() const;

    // Approximate center point (rounded down) of rectangle
    Point approximateCenter() const;

    // Returns a new rectangle translated from this rect
    Rect translate( const Point& distance ) const;

    // Moves this vector to a new position
    void moveTo( const Point& position );

    // Returns the area of this rectangle
    size_t area() const;

    // Checks if a rectangle touches this rectangle
    bool touches( const Rect& rect ) const;
    bool intersects( const Rect& rect ) const;
    bool contains( const Rect& rect ) const;
    bool contains( const Point& p ) const;

    // Output stream operator
    friend std::ostream& operator << ( std::ostream& stream, const Rect& r );

    /////////////////////////
    // Boost serialization //
    /////////////////////////
    friend class boost::serialization::access;

    template<class Archive>
    void serialize( Archive& ar, const unsigned int version )
    {
        ar & mTop;
        ar & mLeft;
        ar & mBottom;
        ar & mRight;
    }

private:
    int mTop;
    int mLeft;
    int mBottom;
    int mRight;
};

#endif
