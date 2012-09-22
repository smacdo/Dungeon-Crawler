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
#include "client/clientview.h"
#include "client/spritemanager.h"
#include "client/sprite.h"
#include "client/spritedata.h"
#include "client/spriteloader.h"
#include "common/rect.h"
#include "common/utils.h"
#include "common/platform.h"
#include "common/logging.h"

#include "game/tiletype.h"
#include "game/level.h"
#include "game/world.h"
#include "game/dungeon.h"
#include "engine/actor.h"

#include "inputmanager.h"

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
    : mSpriteManager(),
      mpPlayerSprite( NULL ),
      mTileSprites(),
      mCamera( 0, 0, Config::DefaultScreenWidth, Config::DefaultScreenHeight )
{
}

/**
 * Destructor. Frees up anything allocated by the client view
 */
ClientView::~ClientView()
{
    // Get rid of our sprites
    Delete( mpPlayerSprite );
    DeletePointerContainer( mTileSprites );
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
}

void ClientView::load()
{
    // preload all of our images
    SpriteLoader loader( mSpriteManager );

    loader.loadSpritesFromXml( "data/sprites/tiles.xml" );
    loader.loadSpritesFromXml( "data/sprites/players.xml" );

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
    mTileSprites[ ETILETYPE_STAIRS_UP ]       = mSpriteManager.createSprite( "stone2_stairs_up" );
    mTileSprites[ ETILETYPE_FILLER_STONE  ]   = mSpriteManager.createSprite( "tile_void" );

    mpPlayerSprite = mSpriteManager.createSprite( "hunter" );
}

/**
 * Unloads the client view. Called before the application quits
 */
void ClientView::unload()
{
}

/**
 * Called to create the main viewing window
 */
void ClientView::createMainWindow()
{
    LOG_DEBUG("ClientView") << "Creating main window";
}

/**
 * Draws the game world
 *   todo make this const
 */
void ClientView::draw( const World& world )
{
    // Clear the background prior to any game rendering
//    SDL_SetRenderDrawColor( mpRenderer, 255, 255, 255, 255 );
//    SDL_RenderClear( mpRenderer );

/*    if ( mInput.userMoveXAxis() != 0 || mInput.userMoveYAxis() != 0 )
    {
        moveCamera( mInput.userMoveXAxis(),
                    mInput.userMoveYAxis() );
    }*/

    // Load the world's main player, so that we can draw from his/her
    // perspective
    const Actor& player = world.activePlayer();

    // Query the player's actor for the current level. Once we have that,
    // render it to the screen
    drawGameLevel( player.activeLevel() );

    // Draw the player
    drawPlayer( player );

    // Render the backbuffer to the actual display
//    SDL_RenderPresent( mpRenderer );
}

/**
 * Draws a game level
 */
void ClientView::drawGameLevel( const Level& /*level*/ )
{
    //
    // This is a pretty terrible way of rendering the level's tiles,
    // but I'm hungry, the hour is late and I would really like to see
    // this work before i finish for the day
    //
/*    for ( int y = 0; y < static_cast<int>(level.height()); ++y )
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
//            SpriteData *pSprite = mTileSprites[ tile.tileid() ];
            Point position      = Point( bounds.x() - mCamera.x(),
                                         bounds.y() - mCamera.y() );

            Sprite *pSprite = mTileSprites[ tile.tileid() ];
            pSprite->setPosition( position );

            // Generate a sprite and draw it
            drawSprite( *pSprite );
        }
    }*/
}

void ClientView::drawPlayer( const Actor& /*player*/ )
{
/*    Point pos    = player.position();
    Point drawAt = Point( pos.x() * 32, pos.y() * 32 );

    // Only draw the player if they are on the screen
    if ( mCamera.contains( drawAt ) )
    {
        drawSprite( deref( mpPlayerSprite ) );
        
    }*/
}

void ClientView::drawSprite( const Sprite& /*sprite*/ )
{
/*    const SpriteData& data = deref( sprite.spriteData() );
    const Point pos        = sprite.position();

    // Construct a rectangle that encompasses only the area of the image
    // that the sprite wants to pull from (for sprite sheet sprites)
    SDL_Rect clip   = { static_cast<int16_t>(data.x()),
                        static_cast<int16_t>(data.y()),
                        static_cast<int16_t>(data.width()),
                        static_cast<int16_t>(data.height()) };

    SDL_Rect offset = { static_cast<int16_t>(pos.x()),
                        static_cast<int16_t>(pos.y()),
                        static_cast<int16_t>(data.width()),
                        static_cast<int16_t>(data.height()) };

    // Grab a copy of the texture. I don't like having to cast away the
    // const-ness of the texture pointer, but SDL doesn't support clean const
    // programming
    SDL_Texture * pTexture = const_cast<SDL_Texture*>( data.texture() );

    // Render the texture to the back buffer
    SDL_RenderCopy( mpRenderer, pTexture, &clip, &offset );
    SDL_SetRenderDrawColor( mpRenderer, 255, 0, 0, 255 );
  //  SDL_RenderFillRect( mpRenderer, &offset );

  //  SDL_SetRenderDrawColor( mpRenderer, 0, 0, 0, 255 );
  //  SDL_RenderDrawRect( mpRenderer, &offset );*/
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
