#include "graphics/clientview.h"
#include "graphics/spritemanager.h"
#include "graphics/sprite.h"
#include "core/rect.h"

#include "tiletype.h"
#include "level.h"
#include "utils.h"

#include <SDL.h>
#include <SDL_image.h>
#include <boost/checked_delete.hpp>

#include <iostream>
#include <string>
#include <cassert>


namespace Config
{
    const int DefaultScreenWidth  = 1280;
    const int DefaultScreenHeight = 1024;
    const int DefaultScreenDepth  = 32;
    const int TileWidth = 32;
    const int TileHeight = 32;
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
      mCamera( 0, 0, Config::DefaultScreenWidth, Config::DefaultScreenHeight )
{
}

/**
 * Destructor. Frees up anything allocated by the client view
 */
ClientView::~ClientView()
{
//    SDL_FreeSurface( mpAppIcon );
    SDL_FreeSurface( mpBackbuffer );
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
}

void ClientView::load()
{
    SDL_Init( SDL_INIT_EVERYTHING );

    // preload all of our images
    mSpriteManager.preloadImage( "blocked",      "tile_blocked.png" );
    mSpriteManager.preloadImage( "unallocated",  "tile_unallocated.png" );
    mSpriteManager.preloadImage( "void",         "tile_void.png" );
    mSpriteManager.preloadImage( "dungeontiles", "dg_dungeon32.png" );
    mSpriteManager.preloadImage( "terraintiles", "dg_grounds32.png" );
    mSpriteManager.preloadImage( "humans",       "dg_humans32.png" );
    mSpriteManager.preloadImage( "monsters",     "dg_monster132.png" );

    // load all of our tile sprites
    mTileSprites.resize( ETileType_Count );
    mTileSprites[ TILE_BLOCKED ]     = mSpriteManager.createSprite( "blocked" );
    mTileSprites[ TILE_UNALLOCATED ] = mSpriteManager.createSprite( "unallocated" );
    mTileSprites[ TILE_VOID ]        = mSpriteManager.createSprite( "void" );
    mTileSprites[ TILE_WALL ]        = mSpriteManager.createSprite( "dungeontiles", 0 * 32, 0 * 32, 32, 32 );
    mTileSprites[ TILE_FLOOR ]       = mSpriteManager.createSprite( "dungeontiles", 3 * 32, 6 * 32, 32, 32 );
    mTileSprites[ TILE_DOOR  ]       = mSpriteManager.createSprite( "dungeontiles", 1 * 32, 1 * 32, 32, 32 );

        /*
        TILE_BLOCKED,      // nothing can be placed here,
                 TILE_UNALLOCATED,  // this tile has not been allocated to anyone
                 TILE_VOID,         // there is literally nothing here
                 TILE_WALL,         // its a wall
                 TILE_FLOOR,        // floor
                 TILE_DOOR,         // door
                 ETileType_Count
        */
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

void ClientView::draw( Level& level )
{
    assert( mWasStarted );

    //
    // This is a pretty terrible way of rendering the level's tiles,
    // but I'm hungry, the hour is late and I would really like to see
    // this work before i finish for the day
    //
    for ( size_t row = 0; row < level.height(); ++row )
    {
        for ( size_t col = 0; col < level.width(); ++col )
        {
            Rect bounds( col * 32, row * 32, 32, 32 );

            // Is it visible? Sigh, I know its a bad hack
            if (! mCamera.contains( bounds ) )
            {
                continue;
            }

            // Get information on that tile
            Tile      tile = level.getTileAt( row, col );
            Sprite *pTileSprite = mTileSprites[ tile.type ];

            drawSprite( bounds.x() - mCamera.x(),
                        bounds.y() - mCamera.y(),
                        deref( pTileSprite) );
        }
    }

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

void ClientView::moveCamera( int x, int y )
{
    // Move the camera
    mCamera.translate( x * 32 , y * 32 );
}