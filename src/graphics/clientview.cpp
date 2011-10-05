#include "graphics/clientview.h"
#include "graphics/spritemanager.h"
#include "graphics/sprite.h"
#include "utils.h"

#include <SDL.h>
#include <SDL_image.h>
#include <boost/checked_delete.hpp>

#include <iostream>
#include <string>
#include <cassert>


namespace Config
{
    const int DefaultScreenWidth  = 640;
    const int DefaultScreenHeight = 480;
    const int DefaultScreenDepth  = 32;
}

/**
 * Constructor. Sets the client view into a default
 * uninitialized state. Make sure to call start() before
 * using the class
 */
ClientView::ClientView()
    : mWasStarted( false ),
      mpBackbuffer( NULL ),
      mpAppIcon( NULL ),
      mSpriteManager(),
      mpSprite( NULL )
{
}

/**
 * Destructor. Frees up anything allocated by the client view
 */
ClientView::~ClientView()
{
//    SDL_FreeSurface( mpAppIcon );
    SDL_FreeSurface( mpBackbuffer );
    
    boost::checked_delete( mpSprite );
}

/**
 * Starts up the client view, which will create the view window, load any
 * required art assets and then show a preliminary display
 *
 * This must be called prior to any drawing calls, otherwise bad things
 * will happen
 */
void ClientView::start()
{
    createMainWindow();
    load();
    mWasStarted = true;

    mpSprite = mSpriteManager.createSprite( "tiles" );
}

void ClientView::load()
{
    SDL_Init( SDL_INIT_EVERYTHING );
    mSpriteManager.preloadImage( "tiles", "dg_dungeon32.png" );
}


void ClientView::unload()
{
    SDL_Quit();
}

void ClientView::createMainWindow()
{
    // Init the backbuffer surface
    mpBackbuffer = SDL_SetVideoMode( Config::DefaultScreenWidth,
                                     Config::DefaultScreenHeight,
                                     Config::DefaultScreenDepth,
                                     SDL_HWSURFACE | SDL_DOUBLEBUF );

    if ( mpBackbuffer == NULL )
    {
        std::cerr << "Failed to set video mode: " << SDL_GetError() << std::endl;
        assert( false );
    }

    // Make ourselves look pretty by setting a caption and then a window icon
    SDL_WM_SetCaption( "The Dungeon Demo", "Dungeon Demo" );

    SDL_Surface *pIcon    = loadImage( "data/gameicon.png" );
    Uint32       colorkey = SDL_MapRGB( pIcon->format, 255, 0, 255 );

    SDL_SetColorKey( pIcon, SDL_SRCCOLORKEY, colorkey );
    SDL_WM_SetIcon( loadImage( "data/gameicon.png" ), NULL );
}

void ClientView::draw()
{
    assert( mWasStarted );

    drawSprite( 300, 150, deref(mpSprite) );
    SDL_Flip( mpBackbuffer );

    SDL_Delay( 30 );
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
SDL_Surface* ClientView::loadImage( const std::string& filename )
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

    // Release the older surface, and return the optimized version
    SDL_FreeSurface( rawSurface );
    return optimizedSurface;
}

void ClientView::drawSprite( int x, int y, const Sprite& sprite )
{
    // Make sure the position being drawn to makes sense
    assert( x >= 0 );
    assert( y >= 0 );

    // Construct a rectangle that encompasses only the area of the image
    // that the sprite wants to pull from (for spritesheet sprites)
    SDL_Rect clip   = { sprite.x(), sprite.y(),
                        sprite.width(), sprite.height() };
    SDL_Rect offset = { x, y, 0, 0 };

    // Now blit the sprite image onto the backbuffer
    const SDL_Surface *pSurface = sprite.surface();

    SDL_BlitSurface( const_cast<SDL_Surface*>(pSurface),
                     &clip,
                     mpBackbuffer,
                     &offset );
}