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

struct SDL_Surface;

/**
 * Defines a sprite that is located in a (potential) spritesheet
 */
class Sprite : boost::noncopyable
{
public:
    // Sprite constructor for a standalone sprite image
    Sprite( SDL_Surface *surface );

    // Sprite constructor a sprite located in a spritesheet
    Sprite( SDL_Surface *surface,
            int xOffset,
            int yOffset,
            int width,
            int height );

    ~Sprite();

    const SDL_Surface* surface() const;
    int x() const;
    int y() const;
    int width() const;
    int height() const;

private:
    SDL_Surface *mpSurface;
    int mX, mY, mW, mH;
};

#endif
