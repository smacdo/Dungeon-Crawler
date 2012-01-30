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
#ifndef SCOTT_DUNGEON_GRAPHICS_SPRITEMANAGER_H
#define SCOTT_DUNGEON_GRAPHICS_SPRITEMANAGER_H

#include <map>
#include <vector>
#include <string>
#include <boost/utility.hpp>

class Sprite;
struct SDL_Renderer;
struct SDL_Texture;

class SpriteManager : boost::noncopyable
{
public:
    SpriteManager();
    ~SpriteManager();

    void setRenderer( SDL_Renderer * pRenderer );

    // Creates a new sprite object and stores it in the sprite manager
    void addSpriteTemplate( const std::string& spriteName,
                            const std::string& filename );

    // Creates a new sprite object and store it in the sprite manager
    void addSpriteTemplate( const std::string& spriteName,
                            const std::string& filename,
                            int xOffset,
                            int yOffset,
                            int width,
                            int height );

    // Locates a loaded sprite and returns a copy of it
    Sprite* createSprite( const std::string& spriteName ) const;

    // Returns the number of sprites loaded
    size_t spriteCount() const;

    // Returns the number of images loaded
    size_t imageCount() const;

protected:
    // Loads an image file and returns a SDL surface
    SDL_Texture* loadImage( const std::string& filepath );

    // Destroys all loaded sprites and surfaces
    void unload();

private:
    // current sdl renderer
    SDL_Renderer * mpRenderer;

    // Image content directory
    std::string mImageRoot;

    // Association of image names to loaded texture files
    std::map<std::string, SDL_Texture*> mLoadedTextures;

    // List of loaded sprite definitions
    std::map<std::string, Sprite*> mSpriteCache;
};

#endif
