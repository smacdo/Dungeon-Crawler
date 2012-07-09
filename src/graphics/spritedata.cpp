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
#include "graphics/spritedata.h"
#include "common/platform.h"
#include <SDL2/SDL.h>

/**
 * Standalone sprite (one that is not located in a spritesheet) constructor
 *
 * \param  pTexture  Pointer to texture containing the sprite
 */
SpriteData::SpriteData( SDL_Texture *pTexture )
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
 * SpriteData constructor for a sprite that is located within a
 * spritesheet image. X and Y denote the offset, and width/height
 * denote how big the sprite is.
 *
 * \param  pTexture  Pointer to the texture holding the sprite sheet
 * \param  xOffset   Left origin of the sprite in the sprite sheet
 * \param  yOffset   Top origin of the sprite in the sprite sheet
 * \param  width     Width of the sprite
 * \param  height    Height of the sprite
 */
SpriteData::SpriteData( SDL_Texture *pTexture,
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
 * Copy constructor
 */
SpriteData::SpriteData( const SpriteData& s )
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
 * Destructor
 */
SpriteData::~SpriteData()
{
    // SDL_Surfaces are not released, since other sprites could potentially the
    // texture. This is especially the case with tilesheet sprites
    mpTexture = NULL;       // prevent stupid mistakes
}

/**
 * Assignment operator.
 */
SpriteData& SpriteData::operator = ( const SpriteData& rhs )
{
    mpTexture = rhs.mpTexture;
    mX        = rhs.mX;
    mY        = rhs.mY;
    mW        = rhs.mW;
    mH        = rhs.mH;
}

/**
 * Equality operator
 */
bool SpriteData::operator == ( const SpriteData& rhs ) const
{
    return ( mpTexture == rhs.mpTexture && mX == rhs.mX && mY == rhs.mY
                                        && mW == rhs.mW && mH == rhs.mH );
}

/**
 * Inequality operator
 */
bool SpriteData::operator != ( const SpriteData& rhs ) const
{
    return ( mpTexture != rhs.mpTexture || mX != rhs.mX || mY != rhs.mY
                                        || mW != rhs.mW || mH != rhs.mH );
}

/**
 * Return a pointer to the sprite's underlying texture object
 */
const SDL_Texture* SpriteData::texture() const
{
    return mpTexture;
}

/**
 * The texture x offset for the sprite
 */
int SpriteData::x() const
{
    return mX;
}

/**
 * The texture y offset for the sprite
 */
int SpriteData::y() const
{
    return mY;
}

/**
 * The sprite's width
 */
int SpriteData::width() const
{
    return mW;
}

/**
 * The sprite's height
 */
int SpriteData::height() const
{
    return mH;
}
