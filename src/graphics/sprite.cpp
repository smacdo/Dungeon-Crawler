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
#include "graphics/spritedata.h"
#include "common/platform.h"

/**
 * Sprite constructor, places sprite at position 0,0
 */
Sprite::Sprite( SpriteData * pSpriteData )
    : mpData( pSpriteData ),
      mPosition( 0, 0 )
{
    assert( pSpriteData != NULL );
}

/**
 * Sprite constructor
 */
Sprite::Sprite( SpriteData * pSpriteData, const Point& position )
    : mpData( pSpriteData ),
      mPosition( position )
{
    assert( pSpriteData != NULL );
}

/**
 * Copy constructor (shallow copies the sprite data)
 */
Sprite::Sprite( const Sprite& s )
    : mpData( s.mpData ),
      mPosition( s.mPosition )
{
}

/**
 * Sprite destructor
 */
Sprite::~Sprite()
{
    // Sprite data is not released, since other sprites may be potentially
    // sharing it
    mpData = NULL;
}

/**
 * Assignment operator. Shallow copy
 */
Sprite& Sprite::operator = ( const Sprite& rhs )
{
    mpData    = rhs.mpData;
    mPosition = rhs.mPosition;

    return *this;
}

/**
 * Return a pointer to the sprite's underlying sprite data
 */
const SpriteData* Sprite::spriteData() const
{
    return mpData;
}

/**
 * Position of the sprite
 */
Point Sprite::position() const
{
    return mPosition;
}
