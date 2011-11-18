#ifndef SCOTT_COMMON_MATH_RECT_H
#define SCOTT_COMMON_MATH_RECT_H

#include <iosfwd>
#include "common/point.h"

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

    bool operator == ( const Rect& rhs ) const;
    Rect& operator = ( const Rect& rhs );
    bool isNull() const;
    int x() const;
    int y() const;

    int top() const;
    int left() const;
    int right() const;
    int bottom() const;
    int width() const;
    int height() const;
    Point topLeft() const;
    Point bottomRight() const;
    Point approximateCenter() const;
    Rect translate( const Point& distance ) const;
    void moveTo( int x, int y );
    size_t area() const;
    bool touches( const Rect& rect ) const;
    bool intersects( const Rect& rect ) const;
    bool contains( const Rect& rect ) const;
    bool contains( const Point& p ) const;

    friend std::ostream& operator << ( std::ostream& stream, const Rect& r );

    /**
     * Serialization
     */
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
