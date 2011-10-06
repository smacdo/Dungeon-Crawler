#include "spritemanager.h"
#include "sprite.h"

#include <string>
#include <map>
#include <SDL.h>
#include <SDL_image.h>
#include <boost/checked_delete.hpp>

#include <iostream>
#include <cassert>

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
void SpriteManager::preloadImage( const std::string& imagename,
                                  const std::string& filepath )
{
    // Make sure the image hasn't already been loaded
    if ( mImageMap.find( imagename ) != mImageMap.end() )
    {
        std::cerr << "Preloaded image '" << imagename
                  << "' already exists in image cache" << std::endl;
        assert( false );
    }
    else
    {
        std::string fullpath   = mImageRoot + filepath;
        mImageMap[ imagename ] = loadImage( fullpath );
    }

    std::cout << "Preloaded image: " << imagename << std::endl;
}

/**
 * Creates a new standalone sprite
 * 
 * \param  imagename  Name of the image to use for the sprite
 * \return            Pointer to the generated sprite
 */
Sprite* SpriteManager::createSprite( const std::string& imagename ) const
{
    Sprite *pSprite = NULL;

    // Make sure the requested image has been loaded
    if ( mImageMap.find( imagename ) == mImageMap.end() )
    {
        std::cerr << "Cannot create sprite because image '" << imagename
                  << "' was not loaded" << std::endl;
        assert( false );
    }
    else
    {
        pSprite = new Sprite( mImageMap.find(imagename)->second );
    }

    return pSprite;
}

/**
 * Creates a new sprite cut from a spritesheet image
 * 
 * \param  imagename  Name of the image to use for the sprite
 * \param  xOffset    Sprite's upper left x coordinate in the spritesheet
 * \param  yOffset    Sprite's upper left y coordinate in the spritesheet
 * \param  width      Width of the sprite
 * \parma  height     Height of the sprite
 * \return            Pointer to the generated sprite
 */
Sprite* SpriteManager::createSprite( const std::string& imagename,
                        int xOffset,
                        int yOffset,
                        int width,
                        int height ) const
{
    Sprite *pSprite = NULL;

    // Make sure the requested image has been loaded
    if ( mImageMap.find( imagename ) == mImageMap.end() )
    {
        std::cerr << "Cannot create sprite because image '" << imagename
                  << "' was not loaded" << std::endl;
        assert( false );
    }
    else
    {
        pSprite = new Sprite( mImageMap.find(imagename)->second,
                              xOffset, yOffset,
                              width, height );
    }

    return pSprite;
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

    // First load the image into a potentially unoptimized surface
    rawSurface = IMG_Load( filename.c_str() );

    if ( rawSurface == NULL )
    {
        std::cerr << "Failed to load image: " << SDL_GetError() << std::endl;
        assert( false );
    }

    // Now convert it to be the same format as the back buffers
    optimizedSurface = SDL_DisplayFormat( rawSurface );
    assert( optimizedSurface != NULL );

    // Add the surface to our list of loaded surfaces
    mSurfaces.push_back( optimizedSurface );

    // Release the older surface, and return the optimized version
    SDL_FreeSurface( rawSurface );
    return optimizedSurface;
}

/**
 * Unloads all loaded sprites and images
 */
void SpriteManager::unload()
{
    size_t freedCount = 0;
    std::vector<SDL_Surface*>::iterator itr;

    for ( itr = mSurfaces.begin(); itr != mSurfaces.end(); ++itr )
    {
        SDL_FreeSurface( *itr );
        freedCount++;
    }

    std::cout << "Unloaded " << freedCount << " images" << std::endl;
}