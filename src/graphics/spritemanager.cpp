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
#include "graphics/spritemanager.h"
#include "graphics/sprite.h"
#include "common/utils.h"
#include "common/platform.h"
#include "common/logging.h"

#include <string>
#include <map>
#include <vector>
#include <SDL2/SDL.h>
#include <SDL2/SDL_image.h>
#include <boost/checked_delete.hpp>

#include <iostream>
#include <cassert>

typedef std::map<std::string, Sprite*>::iterator SpriteCacheItr;
typedef std::map<std::string, Sprite*>::const_iterator SpriteCacheConstItr;
typedef std::pair<std::string, Sprite*> SpriteCacheEntry;

typedef std::map<std::string, SDL_Texture*>::iterator TextureListItr;
typedef std::map<std::string, SDL_Texture*>::const_iterator TextureListConstItr;
typedef std::pair<std::string, SDL_Texture*> TextureListEntry;

/**
 * Constructor
 */
SpriteManager::SpriteManager()
    : mpRenderer( NULL ),
      mImageRoot("data/")
{
}

/**
 * Unload the sprite manager
 */
SpriteManager::~SpriteManager()
{
    unload();
}

void SpriteManager::setRenderer( SDL_Renderer * pRenderer )
{
    assert( pRenderer != NULL );
    mpRenderer = pRenderer;
}

/**
 * Instructs the sprite manager to create a new sprite using the given
 * image as a source and caching it for further use.
 * 
 * \param  spriteName  Name to give to the newly created sprite
 * \param  filepath    Path to the image that this sprite will use
 */
void SpriteManager::addSpriteTemplate( const std::string& spriteName,
                                       const std::string& filepath )
{

    // Has this sprite already been loaded once before? If so, don't
    // load anything
    SpriteCacheItr itr = mSpriteCache.find( spriteName );
 
    if ( itr == mSpriteCache.end() )
    {
        // This sprite needs to be loaded and then cached
        SDL_Texture *pTexture = loadImage( filepath );
        Sprite      *pSprite  = new Sprite( pTexture );

        mSpriteCache.insert( SpriteCacheEntry( spriteName, pSprite ) );
    }
    else
    {
        LOG_WARN("Graphics") << "Sprite '" << spriteName << "' loaded twice";
    }
}

/**
 * Instructs the sprite manager to create a new sprite using the given
 * image as a source and caching it for further use.
 *
 * \param  spriteName Name to give to the sprite
 * \param  imagepath  Path to the sprite image
 * \param  xOffset    Sprite's upper left x coordinate in the sprite sheet
 * \param  yOffset    Sprite's upper left y coordinate in the sprite sheet
 * \param  width      Width of the sprite
 * \parma  height     Height of the sprite
 * \return            Pointer to the generated sprite
 */
void SpriteManager::addSpriteTemplate( const std::string& spriteName,
                                       const std::string& imagepath,
                                       int xOffset,
                                       int yOffset,
                                       int width,
                                       int height )
{
    // Has this sprite already been loaded once before? If so, don't
    // load anything
    SpriteCacheItr itr = mSpriteCache.find( spriteName );
 
    if ( itr == mSpriteCache.end() )
    {
        // This sprite needs to be loaded and then cached
        SDL_Texture *pTexture = loadImage( imagepath );
        Sprite      *pSprite  = new Sprite( pTexture,
                                            xOffset, yOffset,
                                            width, height );

        mSpriteCache.insert( SpriteCacheEntry( spriteName, pSprite ) );
    }
    else
    {
        LOG_WARN("Graphics") << "Sprite '" << spriteName << "' loaded twice";
    }
}

/**
 * Generates a copy of a load sprite template and returns a pointer to
 * the sprite.
 *
 * \param  spriteName  Name of the sprite to instantiate
 * \return             Pointer to a sprite object
 */
Sprite* SpriteManager::createSprite( const std::string& spriteName ) const
{
    // Look the sprite up. Hopefully it has been loaded, otherwise
    // we're going to be in trouble...
    SpriteCacheConstItr itr = mSpriteCache.find( spriteName );
 
    if ( itr == mSpriteCache.end() )
    {
        App::raiseFatalError( "The requested sprite name does not exist",
                              spriteName );
    }

    return itr->second;
}

/**
 * Internal helper method that will load an image from disk, convert it
 * into an optimized format and then return its SDL_Surface pointer.
 *
 * Calling this method will cause the SDL_Surface* to be placed in the
 * list of loaded surfaces which will be unloaded at destruction time
 *
 * \param  filename  Path to the image
 * \return Pointer to the loaded image's SDL surface
 */
SDL_Texture* SpriteManager::loadImage( const std::string& filename )
{
    assert( mpRenderer != NULL );
    SDL_Surface *rawSurface = NULL;

    // Tack the content prefix on
    std::string imagepath = mImageRoot + filename;

    // Do not load the image if it has already been loaded
    TextureListItr itr = mLoadedTextures.find( imagepath );

    if ( itr != mLoadedTextures.end() )
    {
        return itr->second;
    }

    // Since we can't instruct SDL_image to directly load a picture into a
    // texture, we'll have to first assign it to a SDL_Surface* object
    rawSurface = IMG_Load( imagepath.c_str() );

    if ( rawSurface == NULL )
    {
        std::string error =
            std::string("IMAGE: ") +
            imagepath              + "\n" +
            "SDL: "                + SDL_GetError();

        App::raiseFatalError( "Could not load the requested image from disk",
                               error );
    }

    // Now that we have the surface loaded in memory, convert it into an
    // opimtized hardware texture
    SDL_Texture * pTexture =
        SDL_CreateTextureFromSurface( mpRenderer, rawSurface );

    assert( pTexture != NULL );

    // Release the software sdl surface, and cache the optimized hardware
    // texture before returning the newly created texture
    mLoadedTextures.insert( TextureListEntry( imagepath, pTexture ) );
    SDL_FreeSurface( rawSurface );

    return pTexture;
}

/**
 * Unloads all loaded sprites and images
 */
void SpriteManager::unload()
{
    size_t freedTextures = 0, freedSprites = 0;

    // Destroy all sprites
    DeleteMapPointers( mSpriteCache );

    // Kill all loaded textures
    TextureListItr itr;

    for ( itr = mLoadedTextures.begin(); itr != mLoadedTextures.end(); ++itr )
    {
        SDL_DestroyTexture( itr->second );
        freedTextures++;
    }

    // Let 'em know how many things were unloaded
    LOG_INFO("Graphics") << "Unloaded " << freedSprites  << " sprites";
    LOG_INFO("Graphics") << "Unloaded " << freedTextures << " textures";
}

/**
 * Returns the number of sprites that have been loaded into the sprite
 * manager
 *
 * \return  Number of loaded spites
 */
size_t SpriteManager::spriteCount() const
{
    return mSpriteCache.size();
}

/**
 * Returns the number of images (surfaces) that have been loaded into the
 * sprite manager
 *
 * \return  Number of loaded images
 */
size_t SpriteManager::imageCount() const
{
    return mLoadedTextures.size();
}
