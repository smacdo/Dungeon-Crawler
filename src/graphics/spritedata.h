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
#ifndef SCOTT_DUNGEON_SPRITEDATA_H
#define SCOTT_DUNGEON_SPRITEDATA_H

#include <boost/noncopyable.hpp>

struct SDL_Texture;

/**
 * Stores all of the information that is required to draw a sprite from
 * a tilesheet
 */
class SpriteData
{
public:
    // Sprite constructor for a standalone sprite image
    SpriteData( SDL_Texture *surface );

    // Sprite constructor a sprite located in a spritesheet
    SpriteData( SDL_Texture *surface,
                int xOffset,
                int yOffset,
                int width,
                int height );

    // Copy constructor
    SpriteData( const SpriteData& s );

    // Destructor
    ~SpriteData();

    // Assignment operator
    SpriteData& operator = ( const SpriteData& rhs );

    // Equality operator
    bool operator == ( const SpriteData& rhs ) const;

    // Inequality operator
    bool operator != ( const SpriteData& rhs ) const;

    const SDL_Texture* texture() const;
    int x() const;
    int y() const;
    int width() const;
    int height() const;

private:
    SDL_Texture *mpTexture;
    int mX, mY, mW, mH;
};

#endif
