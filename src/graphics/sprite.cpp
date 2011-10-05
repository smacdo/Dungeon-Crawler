#include "graphics/sprite.h"
#include <SDL.h>

#include <cassert>

/**
 * Sprite constructor for a sprite that is in a standalone
 * image
 */
Sprite::Sprite( SDL_Surface *pSurface )
    : mpSurface( pSurface ),
      mX( 0 ),
      mY( 0 ),
      mW( 0 ),
      mH( 0 )
{
    assert( mpSurface != NULL );

    // Copy the width and height from the sdl surface
    mW = pSurface->w;
    mH = pSurface->h;

    // Make sure they are valid
    assert( mW > 0 );
    assert( mH > 0 );
}

/**
 * Sprite constructor for a sprite that is located within a
 * spritesheet image. X and Y denote the offset, and width/height
 * denote how big the sprite is.
 */
Sprite::Sprite( SDL_Surface *pSurface,
                int xOffset,
                int yOffset,
                int width,
                int height )
    : mpSurface( pSurface ),
      mX( xOffset ),
      mY( yOffset ),
      mW( width ),
      mH( height )
{
    assert( pSurface != NULL );
    assert( mX >= 0 );
    assert( mY >= 0 );
    assert( mW > 0 );
    assert( mH > 0 );
}

Sprite::~Sprite()
{
    // SDL_Surfaces are not released, since other sprites could
    // be using them
}
const SDL_Surface* Sprite::surface() const
{
    return mpSurface;
}

int Sprite::x() const
{
    return mX;
}

int Sprite::y() const
{
    return mY;
}

int Sprite::width() const
{
    return mW;
}

int Sprite::height() const
{
    return mH;
}