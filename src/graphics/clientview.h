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
#ifndef SCOTT_DUNGEON_CLIENTVIEW_H
#define SCOTT_DUNGEON_CLIENTVIEW_H

#include <string>
#include <boost/noncopyable.hpp>

#include "graphics/spritemanager.h"
#include "common/rect.h"

class InputManager;
struct SDL_Window;
struct SDL_Renderer;
struct SDL_Rect;
struct SDL_Surface;
class Sprite;
class World;
class Level;
class Actor;

/**
 * Displays the game graphically
 */
class ClientView : boost::noncopyable
{
public:
    ClientView( InputManager& inputManager );
    ~ClientView();

    void start();

    // todo make this const
    void draw( const World& world );
    void moveCamera( int x, int y );

    // remove
    bool didUserPressQuit();

protected:
    void load();
    void unload();

    void drawGameLevel( const Level& level );
    void drawPlayer( const Actor& actor );

    SDL_Surface* loadImage( const std::string& filename );
    void drawSprite( const Sprite& sprite );
    void createMainWindow();

    bool isInCameraBounds( const SDL_Rect& camera, int x, int y, int w, int h ) const;
    void verifySDL() const;

private:
    InputManager& mInput;
    bool mWasStarted;
    SDL_Window *mpWindow;
    SDL_Renderer * mpRenderer;
    SpriteManager mSpriteManager;
    Sprite * mpPlayerSprite;
    std::vector<Sprite*> mTileSprites;
    Rect mCamera;
};

#endif
