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
#include "client/spritedata.h"
#include "common/platform.h"

#include <QPixmap>
#include <QImage>
#include <QSize>

/**
 * Standalone sprite (one that is not located in a spritesheet) constructor
 */
SpriteData::SpriteData( const QPixmap& pixmap )
    : mPixmap( pixmap ),
      mSize( pixmap.size() )
{
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
SpriteData::SpriteData( const QImage& pixmap,
                        int xOffset,
                        int yOffset,
                        int width,
                        int height )
    : mPixmap(),
      mSize( width, height )
{
    QImage chunk = pixmap.copy( xOffset, yOffset, width, height );
    mPixmap      = QPixmap::fromImage( chunk );
}

/**
 * Copy constructor
 */
SpriteData::SpriteData( const SpriteData& s )
    : mPixmap( s.mPixmap ),
      mSize( s.mSize )
{
}

/**
 * Destructor
 */
SpriteData::~SpriteData()
{
}

/**
 * Assignment operator.
 */
SpriteData& SpriteData::operator = ( const SpriteData& rhs )
{
    mPixmap = rhs.mPixmap;
    mSize  = rhs.mSize;

    return *this;
}

/**
 * Equality operator
bool SpriteData::operator == ( const SpriteData& rhs ) const
{
    return ( mPixmap == rhs.mPixmap && mSize == rhs.mSize );
}

/**
 * Inequality operator

bool SpriteData::operator != ( const SpriteData& rhs ) const
{
    return ( mPixmap != rhs.mPixmap || mSize != rhs.mSize );
}*/

/**
 * Return a pointer to the sprite's underlying texture object
 */
const QPixmap& SpriteData::pixmap() const
{
    return mPixmap;
}


/**
 * The sprite's width
 */
int SpriteData::width() const
{
    return mSize.width();
}

/**
 * The sprite's height
 */
int SpriteData::height() const
{
    return mSize.height();
}
