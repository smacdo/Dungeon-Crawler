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
#include "client/spritemanager.h"
#include "client/sprite.h"
#include "client/spritedata.h"
#include "common/utils.h"
#include "common/platform.h"
#include "common/logging.h"

#include <string>
#include <map>
#include <vector>

#include <boost/checked_delete.hpp>

#include <iostream>
#include <cassert>

#include <QImage>
#include <QPixmap>

typedef std::map<std::string, QImage>::iterator TextureListItr;
typedef std::map<std::string, QImage>::const_iterator TextureListConstItr;
typedef std::pair<std::string, QImage> TextureListEntry;

typedef std::map<std::string, SpriteData*>::iterator SpriteCacheItr;
typedef std::map<std::string, SpriteData*>::const_iterator SpriteCacheConstItr;
typedef std::pair<std::string, SpriteData*> SpriteCacheEntry;

/**
 * Constructor
 */
SpriteManager::SpriteManager()
    : mImageRoot("data/sprites/"),
      mNumSpritesCreated( 0 )
{
}

/**
 * Unload the sprite manager
 */
SpriteManager::~SpriteManager()
{
    unload();
}

/**
 * Instructs the sprite manager to create a new sprite using the given
 * image as a source and caching it for further use.
 * 
 * \param  spriteName  Name to give to the newly created sprite
 * \param  filepath    Path to the image that this sprite will use
 */
void SpriteManager::addSpriteData( const std::string& spriteName,
                                   const std::string& filepath )
{

    // Has this sprite already been loaded once before? If so, don't
    // load anything
    SpriteCacheItr itr = mSpriteCache.find( spriteName );
 
    if ( itr == mSpriteCache.end() )
    {
        // This sprite needs to be loaded and then cached
        QPixmap pixmap       = QPixmap::fromImage( loadImage( filepath ) );
        SpriteData *pSprite  = new SpriteData( pixmap );

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
void SpriteManager::addSpriteData( const std::string& spriteName,
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
        SpriteData *pSprite = new SpriteData( loadImage( imagepath ),
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

    // Update stats!
    mNumSpritesCreated += 1;

    // Create a copy of the sprite and give it to the caller
    //  TODO: Change this to look up a SpriteData object, then create a new
    //        Sprite instance and pass it a shared pointer to the SpriteData
    return new Sprite( itr->second );
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
QImage SpriteManager::loadImage( const std::string& filename )
{
    // Tack the content prefix on
    std::string imagepath = mImageRoot + filename;

    // Do not load the image if it has already been loaded
    TextureListItr itr = mLoadedTextures.find( imagepath );

    if ( itr != mLoadedTextures.end() )
    {
        return itr->second;
    }

    // Load the image from disk, and then cache it into our texture cache
    QImage image;

    if (! image.load( QString::fromStdString(filename) ) )
    {
        App::raiseFatalError( "Could not load the requested image from disk ",
                              filename );
    }

    mLoadedTextures.insert( TextureListEntry( imagepath, image ) );

    return image;
}

/**
 * Unloads all loaded sprites and images
 */
void SpriteManager::unload()
{
    size_t freedTextures = 0, freedSprites = 0;

    // Destroy all sprites
    freedSprites = DeleteMapPointers( mSpriteCache );
    mSpriteCache.clear();

    // Kill all loaded textures
    mLoadedTextures.clear();

    // Let 'em know how many things were unloaded
    LOG_INFO("Graphics") << "A total of " << mNumSpritesCreated << " "
                         << "sprites were created during this sprite "
                         << "manager's life";
    LOG_INFO("Graphics") << "Unloaded " << freedSprites  << " sprite templates";
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
