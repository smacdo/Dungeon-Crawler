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
#include "inputmanager.h"

struct SDL_Window;
struct SDL_Renderer;
struct SDL_Rect;
class Sprite;
class World;
class Level;

/**
 * Displays the game graphically
 */
class ClientView : boost::noncopyable
{
public:
    ClientView();
    ~ClientView();

    void start();

    // todo make this const
    void draw( World& world );
    void moveCamera( int x, int y );

    // remove
    bool didUserPressQuit();
    void processInput();

protected:
    void load();
    void unload();

    void drawGameLevel( const Level& level );

    SDL_Surface* loadImage( const std::string& filename );
    void drawSprite( int x, int y, const Sprite& sprite );
    void createMainWindow();

    bool isInCameraBounds( const SDL_Rect& camera, int x, int y, int w, int h ) const;
    void verifySDL() const;

private:
    bool mWasStarted;
    SDL_Window *mpWindow;
    SDL_Renderer * mpRenderer;
    SpriteManager mSpriteManager;
    std::vector<Sprite*> mTileSprites;
    Rect mCamera;
    InputManager mInput;
};

#endif
