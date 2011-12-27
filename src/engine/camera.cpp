#include "camera.h"

#include <cassert>


Camera::Camera( int upperX, int upperY,
                int visibleWidth, int visibleHeight,
                int levelWidth,   int levelHeight )
    : mUpperX( upperX ), mUpperY( upperY ),
      mVisibleWidth( visibleWidth ), mVisibleHeight( visibleHeight ),
      mLevelWidth( levelWidth ), mLevelHeight( levelHeight ),
      mDX( 0.0f ), mDY( 0.0f )
{
    assert( mUpperX >= 0 && mUpperY >= 0 );
    assert( mVisibleWidth > 0 && mVisibleHeight > 0 );
    assert( mLevelWidth > 0 && mLevelHeight > 0 );
   
    assert( mUpperX + mVisibleWidth < mLevelWidth );
    assert( mUpperY + mVisibleHeight < mLevelHeight );
}


Camera::~Camera()
{
}

void Camera::move( int x, int y )
{
    // Update and move the camera by the requested amount, making sure
    // to within the boundaries of the level
    int newX = mUpperX + x;
    int newY = mUpperY + y;

    if ( newX >= 0 && ((newX + mVisibleWidth) < mLevelWidth) )
    {
        mUpperX = newX;
    }

    if ( newY >= 0 && ((newY + mVisibleHeight) < mLevelHeight) )
    {
        mUpperY = newY;
    }

    // Sanity checks
    assert( mUpperX >= 0 );
    assert( mUpperY >= 0 );
    assert( mUpperX + mVisibleWidth <= mLevelWidth );
    assert( mUpperY + mVisibleHeight <= mLevelHeight );
}