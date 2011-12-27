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
#ifndef SCOTT_DUNGEON_FIXEDGRID_H
#define SCOTT_DUNGEON_FIXEDGRID_H

#include "common/point.h"
#include "common/rect.h"
#include "common/platform.h"
#include <boost/checked_delete.hpp>
#include <algorithm>
#include <ostream>

/**
 * FixedGrid is a useful utility class that represents a 2d rectangular
 * "grid" of values. This class hides the work that goes into using a 2d
 * c array, or the work of mapping 2d points into a one dimensional array.
 *
 * This class has built to be as efficient as possible, but since it is
 * dealing with dynamically sized grids, it has to allocate memory each time
 * it creates or resizes the fixed grid. FixedGrid does attempt to alleviate
 * this problem with support for move constructors.
 *
 * The fixed grid uses a standard upper-left hand corner origin, with x
 * increasing in value to the right and y increasing in value as you move down.
 * All positions are zero indexed.
 */
template<typename T>
class FixedGrid
{
public:
    /**
     * Fixed grid constructor. Creates a new fixed grid with a width of
     * 'width' and a height of 'height'. Additionally, this constructor will
     * initialize all cells in the fixed grid to the provided default value.
     *
     * \param  width   The width of the fixed grid. This cannot be negative
     * \param  height  The height of the fixed grid. This cannot be negative
     * \param  value   The default value for values in the fixed grid
     */
    FixedGrid( int width, int height, const T& value )
        : mWidth( width ),
          mHeight( height ),
          mTiles( new T[ mWidth * mHeight ] )
    {
        assert( width > 0 && height > 0 );
        clear( value );
    }

    /**
     * Fixed grid copy constructor. Allocates a new fixed grid array, and
     * copies the dimensions and values from the provided fixed grid.
     *
     * (NOTE: The inner array itself is deep copied, however the values
     *        stored in the array are only shallow copied)
     *
     * \param  grid  The fixed grid to copy from
     */
    FixedGrid( const FixedGrid& grid )
        : mWidth( grid.mWidth ),
          mHeight( grid.mHeight ),
          mTiles( new T[ mWidth * mHeight ] )
    {
        std::copy( &grid.mTiles[0],
                   &grid.mTiles[mWidth*mHeight],
                   &mTiles[0] );
    }

    /**
     * Fixed grid move constructor. Allows a fixed grid to be returned
     * and constructed in place
     *
     * \param  other  The grid to take values from
     */
    FixedGrid( FixedGrid&& other )
        : mWidth( other.mWidth ),
          mHeight( other.mHeight ),
          mTiles( other.mTiles )
    {
        // reset other.mTiles to NULL so it the destructor of the 'other'
        // fixed grid will not delete our new tile array
        other.mTiles = NULL;
    }

    /**
     * Fixed grid destructor. Properly unallocates the inner array storage
     */
    virtual ~FixedGrid()
    {
        boost::checked_array_delete( mTiles );
    }

    /**
     * Assignment operator. Destroys the current fixed grid, allocates a
     * new one and then copies values over from the rhs fixed grid.
     *
     * \param  rhs  The fixed grid to copy from
     * \return      Reference to this fixed grid
     */
    FixedGrid<T>& operator = ( const FixedGrid<T>& rhs )
    {
        if ( this != &rhs )
        {
            assert( rhs.mTiles != NULL );
            assert( rhs.mWidth > 0 && rhs.mHeight > 0 );

            // Destroy our current tile array, since we will be losing the
            // pointer in the assignment
            boost::checked_array_delete( mTiles );

            // Initialize our fixed grid
            mWidth  = rhs.mWidth;
            mHeight = rhs.mHeight;
            mTiles  = new T[ mWidth * mHeight ];

            // Now copy all of the elements over
            std::copy( &rhs.mTiles[0],
                       &rhs.mTiles[mWidth * mHeight],
                       &mTiles[0] );
        }

        return *this;
    }

    /**
     * Move assignment operator. Destroys the current fixed grid, allocates a
     * new one and then copies values over from the rhs fixed grid.
     *
     * \param  other  The fixed grid to copy from
     * \return        Reference to this fixed grid
     */
    FixedGrid& operator = ( FixedGrid&& other )
    {
        // Make sure we're not trying to assign to ourself
        if ( this != &other )
        {
            // Make sure we destroy our current tile grid before
            // assuming ownership of the new grid
            boost::checked_array_delete( mTiles );

            // Transfer the values to us
            mWidth  = other.mWidth;
            mHeight = other.mHeight;
            mTiles  = other.mTiles;

            // Now set the other fixed grid's tile pointer to null, so it
            // doesn't destroy the grid when it goes away
            other.mTiles = NULL;
        }

        return *this;
    }

    /**
     * Fixed grid equality operator. Checks if the two fixed grids have
     * identical dimensions and values.
     *
     * \param  rhs  The fixed grid to compare outself against
     * \return      True if the two fixed grids have identical width, height
     *              and values. False otherwise
     */
    bool operator == ( const FixedGrid<T>& rhs ) const
    {
        bool isEqual = false;

        if ( rhs.mWidth == mWidth && rhs.mHeight == mHeight )
        {
            isEqual = std::equal( &mTiles[0],
                                  &mTiles[mWidth * mHeight],
                                  &(rhs.mTiles[0]) );
        }

        return isEqual;
    }

    /**
     * Fixed grid inequality operator. Checks if two fixed grids do not have
     * similiar width, height or values.
     *
     * \param  rhs  The fixed grid to compare outself against
     * \return      True if the two fixed grids do not have identical width,
     *              height or values. False otherwise
     */
    bool operator != ( const FixedGrid<T>& rhs ) const
    {
        bool isEqual = false;

        if ( rhs.mWidth == mWidth && rhs.mHeight == mHeight )
        {
            isEqual = std::equal( &mTiles[0],
                                  &mTiles[mWidth * mHeight],
                                  &(rhs.mTiles[0]) );
        }

        return (!isEqual);
    }

    /**
     * "Inserts" a fixed grid into this grid. What this means is that the
     * source grid will be copied into us at the requested position
     *
     * The source grid must fit within the bounds of the destination grid
     * otherwise this method will raise an assertion failure
     *
     * \param  uperLeft  Position to insert the upper left corner of the
     *                   source fixed grid at
     * \param  source    The fixed grid that should be inserted
     */
    void insert( const Point& upperLeft, FixedGrid& source ) const
    {
        Rect destBounds( 0, 0, mWidth, mHeight );
        Rect sourceBounds( upperLeft, source.mWidth, source.mHeight );

        // Verify that the source grid will fit in us
        assert( destBounds.contains( sourceBounds ) );

        // Now copy the tiles over
        for ( int sy = 0; sy < source.mHeight; ++sy )
        {
            for ( int sx = 0; sx < source.mWidth; ++sx )
            {
                size_t si = source.offset( sx, sy );
                size_t di = this->offset( sx + upperLeft.x(),
                                          sy + upperLeft.y() );

                mTiles[di] = source.mTiles[si];
            }
        }
    }

    /**
     * Stream output operator.
     */
    template<typename U>
    friend std::ostream& operator << ( std::ostream& os,
                                       const FixedGrid<U>& fg );

    /**
     * Clears the fixed grid by calling the default constructor on each
     * tile
     */
    void clear()
    {
        std::fill( &mTiles[0], &mTiles[mWidth*mHeight], T() );

    }

    /**
     * Clears the fixed grid by setting every tile to the requested 'base'
     *
     * \param  base  The value to assign to each tile
     */
    void clear( const T& base )
    {
        std::fill( &mTiles[0], &mTiles[mWidth*mHeight], base );
    }

    /**
     * Returns a reference to the value stored at the requested position
     *
     * \param  point  The position in the fixed grid to retrieve
     * \return        A reference to the value stored at that position
     */
    T& get( const Point& point )
    {
        return get( point.x(), point.y() );
    }

    /**
     * Returns a reference to the value stored at the requested position.
     *
     * \param  x  The x offset to look up
     * \param  y  The y offset to look up
     * \return    A const reference to the value at that position
     */
    T& get( int x, int y )
    {
        assert( x >= 0 && x < mWidth );
        assert( y >= 0 && y < mHeight );
        return mTiles[ offset( x, y ) ];
    }

    /**
     * Returns a const reference to the value stored at the requested
     * position.
     *
     * \param  point  The position in the fixed grid to retrieve
     * \return        A const reference to the value at that position
     */
    const T& get( const Point& point ) const
    {
        return get( point.x(), point.y() );
    }

    /**
     * Returns a const reference to the value stored at the requested
     * position.
     *
     * \param  x  The x offset to look up
     * \param  y  The y offset to look up
     * \return    A const reference to the value at that position
     */
    const T& get( int x, int y ) const
    {
        assert( x >= 0 && x < mWidth  );
        assert( y >= 0 && y < mHeight );
        return mTiles[ offset( x, y ) ];
    }

    /**
     * Sets a value in the fixed grid at the requested position
     *
     * \param  point  The point in the fixed grid to set a value
     * \param  value  The value to set
     */
    void set( const Point& point, const T& value )
    {
        set( point.x(), point.y(), value );
    }

    /**
     * Sets a value in the fixed grid at the requested position
     *
     * \param  x      The x offset
     * \param  y      The y offset
     * \param  value  The value to set
     */
    void set( int x, int y, const T& value )
    {
        assert( x >= 0 && x < mWidth );
        assert( y >= 0 && y < mHeight );
        mTiles[ offset( x, y ) ] = value;
    }

    /**
     * Returns the width of the fixed grid
     *
     * \return  Fixed grid width
     */
    int width() const
    {
        return mWidth;
    }

    /**
     * Returns the height of the fixed grid
     *
     * \return  Fixed grid height
     */
    int height() const
    {
        return mHeight;
    }

    /**
     * Returns the number of elements in the fixed grid
     *
     * \return  Number of elements in the fixed grid
     */
    size_t size() const
    {
        return mWidth * mHeight;
    }

    /////////////////////////
    // Boost serialization //
    /////////////////////////
    friend class boost::serialization::access;
                        
    template<typename Archive>
    void serialize( Archive& ar, const unsigned int version )
    {
        ar & mWidth;
        ar & mHeight;
        ar & mTiles;
    }

protected:
    /**
     * Converts a fixed grid 2d point into a one dimensional array offset.
     * Used to look up and store values
     *
     * \param  x  The x offset
     * \param  y  The y offset
     * \return    Offset of the (x,y) pair in the mTiles array
     */
    size_t offset( int x, int y ) const
    {
        assert( x >= 0 && x < mWidth );
        assert( y >= 0 && y < mHeight );
        return y * mWidth + x;
    }

    int mWidth;
    int mHeight;
    T * mTiles;
};

/**
 * Fixed grid output operator
 */
template<typename T>
std::ostream& operator << ( std::ostream& os, const FixedGrid<T>& fg )
{
    os << "\n";

    for ( int y = 0; y < fg.mHeight; ++y )
    {
        if ( y == 0 )
        {
            os << "{ { ";
        }
        else
        {
            os << "  { ";
        }

        for ( int x = 0; x < fg.mWidth; ++x )
        {
            if ( x > 0 )
            {
                os << ", ";
            }

            os << fg.mTiles[ fg.offset(x,y) ];
        }

        if ( y == (fg.mHeight-1) )
        {
            os << " } }\n";
        }
        else
        {
            os << " }, ";
        }
    }

    return os;
}

#endif
