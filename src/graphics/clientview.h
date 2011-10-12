#ifndef SCOTT_DUNGEON_CLIENTVIEW_H
#define SCOTT_DUNGEON_CLIENTVIEW_H

#include <string>
#include <boost/noncopyable.hpp>

#include "graphics/spritemanager.h"
#include "core/rect.h"

struct SDL_Surface;
struct SDL_Rect;
class Sprite;
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
    void draw( Level& level );
    void moveCamera( int x, int y );

    // Return a string containing information about the client's
    // video settings and capabilities
    std::string dumpInfo() const;

protected:
    void load();
    void unload();

    SDL_Surface* loadImage( const std::string& filename );
    void drawSprite( int x, int y, const Sprite& sprite );
    void createMainWindow();

    bool isInCameraBounds( const SDL_Rect& camera, int x, int y, int w, int h ) const;

private:
    bool mWasStarted;
    SDL_Surface *mpBackbuffer;
    SDL_Surface *mpAppIcon;
    SpriteManager mSpriteManager;
    std::vector<Sprite*> mTileSprites;
    Rect mCamera;
};

#endif