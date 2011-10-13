#ifndef SCOTT_DUNGEON_FIXEDGRID_H
#define SCOTT_DUNGEON_FIXEDGRID_H

#include "core/point.h"
#include <boost/checked_delete.hpp>

template<typename T>
class FixedGrid
{
public:
    FixedGrid( size_t width, size_t height, const T& base )
        : mWidth( width ), mHeight( height ),
          mTiles( new T[ mWidth * mHeight ] )
    {
    }

    FixedGrid( FixedGrid& grid )
        : mWidth( grid.width ),
          mHeight( grid.height ),
          mTiles( new T[ mWidth * mHeight ] )
    {
        assert( grid.mTiles != NULL );

        std::copy( &grid.mTiles[0],
                   &grid.mTiles[mWidth*mHeight],
                   &mTiles[0] );

    }

    virtual ~FixedGrid()
    {
        boost::checked_array_delete( mTiles );
    }

    FixedGrid<T>& operator = ( const FixedGrid<T>& rhs )
    {
        assert( this != &rhs );
        assert( rhs.mTiles != NULL );

        // Destroy our current tile array, since we're getting rid of it
        // in the assignment
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

    void clear( const T& base )
    {
        assert( mTiles != NULL );
        std::fill( &mTiles[0], &mTiles[mWidth*mHeight], base );
    }

    T& get( const Point& point )
    {
        return get( point.x(), point.y() );
    }

    T& get( size_t x, size_t y )
    {
        assert( x < mWidth && y < mHeight );
        return mTiles[ offset( x, y ) ];
    }

    const T& get( const Point& point ) const
    {
        return get( point.x(), point.y() );
    }

    const T& get( size_t x, size_t y ) const
    {
        assert( x < mWidth && y < mHeight );
        return mTiles[ offset( x, y ) ];
    }

    void set( const Point& point, const T& value )
    {
        set( point.x(), point.y(), value );
    }

    void set( size_t x, size_t y, const T& value )
    {
        assert( x < mWidth && y < mHeight );
        return mTiles[ offset( x, y ) ] = value;
    }

    size_t width() const
    {
        return mWidth;
    }

    size_t height() const
    {
        return mHeight;
    }

    size_t size() const
    {
        return mWidth * mHeight;
    }

private:
    size_t offset( size_t x, size_t y ) const
    {
        return y * mWidth + x;
    }

    size_t mWidth;
    size_t mHeight;
    T * mTiles;
};

#endif
