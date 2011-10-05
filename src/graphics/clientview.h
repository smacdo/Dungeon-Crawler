#ifndef SCOTT_DUNGEON_CLIENTVIEW_H
#define SCOTT_DUNGEON_CLIENTVIEW_H

#include <string>
#include <boost/noncopyable.hpp>

#include "graphics/spritemanager.h"

struct SDL_Surface;
class Sprite;

/**
 * Displays the game graphically
 */
class ClientView : boost::noncopyable
{
public:
    ClientView();
    ~ClientView();

    void start();
    void draw();

protected:
    void load();
    void unload();

    SDL_Surface* loadImage( const std::string& filename );
    void drawSprite( int x, int y, const Sprite& sprite );
    void createMainWindow();

private:
    bool mWasStarted;
    SDL_Surface *mpBackbuffer;
    SDL_Surface *mpAppIcon;
    SpriteManager mSpriteManager;
    Sprite *mpSprite;
};

#endif