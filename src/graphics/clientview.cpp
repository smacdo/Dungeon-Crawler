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
#include "graphics/clientview.h"
#include "graphics/spritemanager.h"
#include "graphics/sprite.h"
#include "graphics/spriteloader.h"
#include "common/rect.h"
#include "common/utils.h"
#include "common/platform.h"
#include "common/logging.h"

#include "game/tiletype.h"
#include "game/level.h"
#include "game/world.h"
#include "game/dungeon.h"

#include <SDL2/SDL.h>
#include <SDL2/SDL_image.h>
#include <boost/checked_delete.hpp>

#include <iostream>
#include <sstream>
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
      mpWindow( NULL ),
      mpRenderer( NULL ),
      mSpriteManager(),
      mCamera( 0, 0, Config::DefaultScreenWidth, Config::DefaultScreenHeight )
{
}

/**
 * Destructor. Frees up anything allocated by the client view
 */
ClientView::~ClientView()
{
    SDL_DestroyWindow( mpWindow );
    SDL_DestroyRenderer( mpRenderer );
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
//    mSpriteManager.loadSpritesFromXml( "sprites.xml" );
    SpriteLoader loader( mSpriteManager );

    loader.loadSpritesFromXml( "data/sprites/tiles.xml" );

    if ( loader.hasErrors() )
    {
        App::raiseFatalError( "Loading sprites", loader.errorText() );
    }

    //
    // Load all of our tile sprites
    //   (I need to refactor this and make it load terrain types out of
    //    an XML file instead)
    //
    mTileSprites.resize( ETILETYPE_COUNT );
    mTileSprites[ ETILETYPE_VOID ]            = mSpriteManager.createSprite( "tile_void" );
    mTileSprites[ ETILETYPE_GRANITE ]         = mSpriteManager.createSprite( "tile_blocked" );
    mTileSprites[ ETILETYPE_DUNGEON_WALL ]    = mSpriteManager.createSprite( "stone2_floor"  );
    mTileSprites[ ETILETYPE_DUNGEON_FLOOR ]   = mSpriteManager.createSprite( "dcss_rl_lit_corridor" );
    mTileSprites[ ETILETYPE_DUNGEON_DOORWAY ] = mSpriteManager.createSprite( "stone2_door_open" );
    mTileSprites[ ETILETYPE_FILLER_STONE  ]   = mSpriteManager.createSprite( "tile_void" );
}

/**
 * Unloads the client view. Called before the application quits
 */
void ClientView::unload()
{
    SDL_Quit();
}

/**
 * Called to create the main viewing window
 */
void ClientView::createMainWindow()
{
    LOG_DEBUG("ClientView") << "Creating main window";

    // Make sure SDL's video subsystem is initialized before proceeding with
    // window creation
    if ( SDL_VideoInit( NULL ) != 0 )
    {
        App::raiseFatalError( "Failed to initialize SDL video",
                              SDL_GetError() );
    }


    // Create the main game window that will display the game to the
    // player
    mpWindow = SDL_CreateWindow( "Dungeon Crawler",
                                 SDL_WINDOWPOS_CENTERED,
                                 SDL_WINDOWPOS_CENTERED,
                                 Config::DefaultScreenWidth,
                                 Config::DefaultScreenHeight,
                                 SDL_WINDOW_SHOWN );
  
    if ( mpWindow == NULL )
    {
        App::raiseFatalError( "Failed to set video mode when creating window",
                              SDL_GetError() );
    }

    // Start the SDL renderer
    mpRenderer = SDL_CreateRenderer( mpWindow,
                                     -1,    // first available context
                                     SDL_RENDERER_ACCELERATED   |
                                     SDL_RENDERER_PRESENTVSYNC );

    if ( mpRenderer == NULL )
    {
        App::raiseFatalError( "Failed to create SDL renderer", SDL_GetError() );
    }

    // Make sure there were no uncaught errors before we proceed with
    // the game
    verifySDL();

    // Configure the sprite manager before loading any sprites
    mSpriteManager.setRenderer( mpRenderer );
}

/**
 * Processes input, and returns a list of commands to be executed
 */
void ClientView::processInput()
{
    mInput.processInput();

    // If the user moved, make sure to move the camera
    //  (move this into a command!)
    if ( mInput.didUserMove() )
    {
        moveCamera( mInput.userMoveXAxis(),
                    mInput.userMoveYAxis() );
    }
}

/**
 * This needs to be removed! User quitting should be a command that is
 * executed by the world engine, not us
 */
bool ClientView::didUserPressQuit()
{
    return mInput.didUserPressQuit();
}

/**
 * Draws the game world
 *   todo make this const
 */
void ClientView::draw( World& world )
{
    // Sanity checks first!
    assert( mpWindow != NULL );
    assert( mpRenderer != NULL );

    // Clear the background prior to any game rendering
    SDL_SetRenderDrawColor( mpRenderer, 255, 255, 255, 255 );
    SDL_RenderClear( mpRenderer );

    // Draw the main game level
    std::shared_ptr<Dungeon> dungeon = world.mainDungeon();
    std::shared_ptr<Level>   level   = dungeon->getLevel( 0 );
    
    drawGameLevel( deref(level.get()) );

    // Render the backbuffer to the actual display
    SDL_RenderPresent( mpRenderer );
}

/**
 * Draws a game level
 */
void ClientView::drawGameLevel( const Level& level )
{
    //
    // This is a pretty terrible way of rendering the level's tiles,
    // but I'm hungry, the hour is late and I would really like to see
    // this work before i finish for the day
    //
    for ( int y = 0; y < static_cast<int>(level.height()); ++y )
    {
        for ( int x = 0; x < static_cast<int>(level.width()); ++x )
        {
            Rect bounds( x * 32, y * 32, 32, 32 );

            // Is it visible? Sigh, I know its a bad hack
            if (! mCamera.contains( bounds ) )
            {
                continue;
            }

            // Get information on that tile
            const Tile& tile    = level.tileAt( Point( x, y ) );
            Sprite *pTileSprite = mTileSprites[ tile.tileid() ];

            drawSprite( bounds.x() - mCamera.x(),
                        bounds.y() - mCamera.y(),
                        deref( pTileSprite) );
        }
    }
}

void ClientView::drawSprite( int x, int y, const Sprite& sprite )
{
    // Make sure the position being drawn to makes sense
    assert( x >= 0 );
    assert( y >= 0 );

    // Construct a rectangle that encompasses only the area of the image
    // that the sprite wants to pull from (for sprite sheet sprites)
    SDL_Rect clip   = { static_cast<int16_t>(sprite.x()),
                        static_cast<int16_t>(sprite.y()),
                        static_cast<int16_t>(sprite.width()),
                        static_cast<int16_t>(sprite.height()) };

    SDL_Rect offset = { static_cast<int16_t>(x),
                        static_cast<int16_t>(y),
                        static_cast<int16_t>( sprite.width() ),
                        static_cast<int16_t>( sprite.height() ) };

    // Grab a copy of the texture. I don't like having to cast away the
    // const-ness of the texture pointer, but SDL doesn't support clean const
    // programming
    SDL_Texture * pTexture = const_cast<SDL_Texture*>( sprite.texture() );

    // Render the texture to the back buffer
    SDL_RenderCopy( mpRenderer, pTexture, &clip, &offset );
    SDL_SetRenderDrawColor( mpRenderer, 255, 0, 0, 255 );
  //  SDL_RenderFillRect( mpRenderer, &offset );

  //  SDL_SetRenderDrawColor( mpRenderer, 0, 0, 0, 255 );
  //  SDL_RenderDrawRect( mpRenderer, &offset );
}

/**
 * Moves the client view's camera by the requested amount of space. We should
 * move this logic into a camera class and return a reference to this class,
 * since camera movement is not a direct part of the client view
 */
void ClientView::moveCamera( int x, int y )
{
    // Move the camera
    mCamera = mCamera.translate( Point( x * 32 , y * 32 ) );
}

/**
 * Verifies that there were no SDL errors. We should strongly consider breaking
 * this out into a utility method, since this has nothing to do with the client
 * view
 */
void ClientView::verifySDL() const
{
    const char * pErrorText = SDL_GetError();

    if ( pErrorText != NULL && *pErrorText != '\0' )
    {
        // Printing a backtrace would be incredibly helpful... :)
        App::raiseError( pErrorText );
        SDL_ClearError();       // fat chance
    }
}
