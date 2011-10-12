#include "graphics/spritemanager.h"
#include "graphics/sprite.h"
#include "common/utils.h"

#include <string>
#include <map>
#include <vector>
#include <SDL.h>
#include <SDL_image.h>
#include <boost/checked_delete.hpp>

#include <iostream>
#include <cassert>

typedef std::map<std::string, Sprite*>::iterator SpriteCacheItr;
typedef std::map<std::string, Sprite*>::const_iterator SpriteCacheConstItr;
typedef std::pair<std::string, Sprite*> SpriteCacheEntry;

typedef std::map<std::string, SDL_Surface*>::iterator SurfaceListItr;
typedef std::map<std::string, SDL_Surface*>::const_iterator SurfaceListConstItr;
typedef std::pair<std::string, SDL_Surface*> SurfaceListEntry;

/**
 * Constructor
 */
SpriteManager::SpriteManager()
    : mImageRoot("data/")
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
 * Instructs the sprite manager to preload an image. Right now all images
 * must be preloaded before being used in a sprite, but in the future
 * the sprite manager class will gain the ability to manually load sprite
 * definitions
 */
void SpriteManager::preloadImage( const std::string& filepath )
{

}

/**
 * Instructs the sprite manager to create a new sprite using the given
 * image as a source and caching it for further use.
 * 
 * \param  spriteName  Name to give to the newly created sprite
 * \param  filepath    Path to the image that this sprite will use
 */
void SpriteManager::addSprite( const std::string& spriteName,
                               const std::string& filepath )
{

    // Has this sprite already been loaded once before? If so, don't
    // load anything
    SpriteCacheItr itr = mSpriteCache.find( spriteName );
 
    if ( itr == mSpriteCache.end() )
    {
        // This sprite needs to be loaded and then cached
        SDL_Surface *pSurface = loadImage( filepath );
        Sprite      *pSprite  = new Sprite( pSurface );

        mSpriteCache.insert( SpriteCacheEntry( spriteName, pSprite ) );
    }
    else
    {
        std::cerr << "Sprite '" << spriteName << "' loaded twice" << std::endl;
    }
}

/**
 * Instructs the sprite manager to create a new sprite using the given
 * image as a source and caching it for further use.
 *
 * \param  spriteName Name to give to the sprite
 * \param  imagepath  Path to the sprite image
 * \param  xOffset    Sprite's upper left x coordinate in the spritesheet
 * \param  yOffset    Sprite's upper left y coordinate in the spritesheet
 * \param  width      Width of the sprite
 * \parma  height     Height of the sprite
 * \return            Pointer to the generated sprite
 */
void SpriteManager::addSprite( const std::string& spriteName,
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
        SDL_Surface *pSurface = loadImage( imagepath );
        Sprite      *pSprite  = new Sprite( pSurface,
                                            xOffset, yOffset,
                                            width, height );

        mSpriteCache.insert( SpriteCacheEntry( spriteName, pSprite ) );
    }
    else
    {
        std::cerr << "Sprite '" << spriteName << "' loaded twice" << std::endl;
    }
}

/**
 * Generates a copy of a load sprite template and returns a pointer to
 * the sprite.
 *
 * \param  spriteName  Name of the sprite to instantiate
 * \return             Pointer to a sprite object
 */
Sprite* SpriteManager::findSprite( const std::string& spriteName ) const
{
    // Look the sprite up. Hopefully it has been loaded, otherwise
    // we're going to be in trouble...
    SpriteCacheConstItr itr = mSpriteCache.find( spriteName );
 
    if ( itr == mSpriteCache.end() )
    {
        std::cerr << "Sprite '" << spriteName << "' does not exist"
                  << std::endl;
        assert( false );
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
SDL_Surface* SpriteManager::loadImage( const std::string& filename )
{
    SDL_Surface *rawSurface = NULL, *optimizedSurface = NULL;

    // Tack the content prefix on
    std::string imagepath = mImageRoot + filename;

    // Do not load the image if it has already been loaded
    SurfaceListItr itr = mLoadedSurfaces.find( imagepath );

    if ( itr != mLoadedSurfaces.end() )
    {
        std::cerr << "Image was already preloaded: '" << imagepath << std::endl;
        return itr->second;
    }

    // First load the image into a potentially unoptimized surface
    rawSurface = IMG_Load( imagepath.c_str() );

    if ( rawSurface == NULL )
    {
        std::cerr << "Failed while attempting to load image: " << imagepath << std::endl
                  << SDL_GetError() << std::endl;
        assert( false );
    }

    // Now convert it to be the same format as the back buffers
    optimizedSurface = SDL_DisplayFormat( rawSurface );
    assert( optimizedSurface != NULL );

    // Release the older surface, and cache the optimized version
    SDL_FreeSurface( rawSurface );
    mLoadedSurfaces.insert( SurfaceListEntry( imagepath, optimizedSurface ) );

    // Now return the loaded sprite
    return optimizedSurface;
}

/**
 * Unloads all loaded sprites and images
 */
void SpriteManager::unload()
{
    size_t freedSurfaces = 0, freedSprites = 0;

    // Destroy all sprites
    DeleteMapPointers( mSpriteCache );

    // Kill all surfaces
    SurfaceListItr itr;

    for ( itr = mLoadedSurfaces.begin(); itr != mLoadedSurfaces.end(); ++itr )
    {
        SDL_FreeSurface( itr->second );
        freedSurfaces++;
    }

    // Let 'em know how many things were unloaded
    std::cout << "Unloaded " << freedSprites << " sprites and "
              << freedSurfaces << " images " << std::endl;
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
    return mLoadedSurfaces.size();
}
