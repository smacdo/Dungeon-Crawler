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
#ifndef SCOTT_DUNGEON_SPRITE_H
#define SCOTT_DUNGEON_SPRITE_H

#include <boost/noncopyable.hpp>

struct SDL_Texture;

/**
 * Defines a sprite that is located in a (potential) spritesheet.
 *
 * TODO: Split this class up into two separate classes. This class should be
 * renamed SpriteData, and a new Sprite class should contain a pointer to it.
 * That way we can instance sprites and store their world coordinates, among
 * other things such as rotation.
 */
class Sprite
{
public:
    // Sprite constructor for a standalone sprite image
    Sprite( SDL_Texture *surface );

    // Sprite constructor a sprite located in a spritesheet
    Sprite( SDL_Texture *surface,
            int xOffset,
            int yOffset,
            int width,
            int height );

    // Copy constructor
    Sprite( const Sprite& s );

    // Destructor
    ~Sprite();

    // Assignment operator
    Sprite& operator = ( const Sprite& rhs );

    // Equality operator
    bool operator == ( const Sprite& rhs ) const;

    // Inequality operator
    bool operator != ( const Sprite& rhs ) const;

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
