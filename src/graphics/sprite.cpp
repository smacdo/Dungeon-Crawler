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
    assert( pTexture != NULL );
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

const SDL_Texture* Sprite::texture() const
{
    return mpTexture;
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
