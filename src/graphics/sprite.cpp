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
#include "graphics/sprite.h"
#include "common/platform.h"
#include <SDL2/SDL.h>

/**
 * Sprite constructor for a sprite that is in a standalone
 * image
 */
Sprite::Sprite( SDL_Texture *pTexture )
    : mpTexture( pTexture ),
      mX( 0 ),
      mY( 0 ),
      mW( 0 ),
      mH( 0 )
{
    assert( pTexture != NULL );

    // Copy the width and height from the sdl surface
    unsigned int format;
    int temp;
    
    if ( SDL_QueryTexture( pTexture, &format, &temp, &mW, &mH ) != 0 )
    {
        App::raiseFatalError( "Failed to query texture for height/width",
                              SDL_GetError() );
    }

    // Make sure they are valid
    assert( mW > 0 );
    assert( mH > 0 );
}

/**
 * Sprite constructor for a sprite that is located within a
 * spritesheet image. X and Y denote the offset, and width/height
 * denote how big the sprite is.
 */
Sprite::Sprite( SDL_Texture *pTexture,
                int xOffset,
                int yOffset,
                int width,
                int height )
    : mpTexture( pTexture ),
      mX( xOffset ),
      mY( yOffset ),
      mW( width ),
      mH( height )
{
    assert( mpTexture != NULL );
    assert( mX >= 0 );
    assert( mY >= 0 );
    assert( mW > 0 );
    assert( mH > 0 );
}

/**
 * Copy constructor. Clones another sprite, but does not deep copy the SDL
 * source texture
 */
Sprite::Sprite( const Sprite& s )
    : mpTexture( s.mpTexture ),
      mX( s.mX ),
      mY( s.mY ),
      mW( s.mW ),
      mH( s.mH )
{
    assert( mpTexture != NULL );
    assert( mX >= 0 );
    assert( mY >= 0 );
    assert( mW > 0 );
    assert( mH > 0 );
}

/**
 * Sprite destructor
 */
Sprite::~Sprite()
{
    // SDL_Surfaces are not released, since other sprites could potentially the
    // texture. This is especially the case with tilesheet sprites
    mpTexture = NULL;       // prevent stupid mistakes
}

/**
 * Assignment operator. Clones the sprite, but does not deep copy the source
 * texture
 */
Sprite& Sprite::operator = ( const Sprite& rhs )
{
    mpTexture = rhs.mpTexture;
    mX        = rhs.mX;
    mY        = rhs.mY;
    mW        = rhs.mW;
    mH        = rhs.mH;
}

/**
 * Equality operator. Checks if this sprite is an identical copy of another
 * sprite
 */
bool Sprite::operator == ( const Sprite& rhs ) const
{
    return ( mpTexture == rhs.mpTexture && mX == rhs.mX && mY == rhs.mY
                                        && mW == rhs.mW && mH == rhs.mH );
}

/**
 * Inequality operator. Checks if this sprite is not an indentical copy of another
 * sprite
 */
bool Sprite::operator != ( const Sprite& rhs ) const
{
    return ( mpTexture != rhs.mpTexture || mX != rhs.mX || mY != rhs.mY
                                        || mW != rhs.mW || mH != rhs.mH );
}

/**
 * Return a pointer to the sprite's underlying texture object
 */
const SDL_Texture* Sprite::texture() const
{
    return mpTexture;
}

/**
 * The texture x offset for the sprite
 */
int Sprite::x() const
{
    return mX;
}

/**
 * The texture y offset for the sprite
 */
int Sprite::y() const
{
    return mY;
}

/**
 * The sprite's width
 */
int Sprite::width() const
{
    return mW;
}

/**
 * The sprite's height
 */
int Sprite::height() const
{
    return mH;
}
