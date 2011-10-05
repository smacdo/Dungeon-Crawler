#ifndef SCOTT_DUNGEON_SPRITE_H
#define SCOTT_DUNGEON_SPRITE_H

#include <boost/noncopyable.hpp>

struct SDL_Surface;

/**
 * Defines a sprite that is located in a (potential) spritesheet
 */
class Sprite : boost::noncopyable
{
public:
    // Sprite constructor for a standalone sprite image
    Sprite( SDL_Surface *surface );

    // Sprite constructor a sprite located in a spritesheet
    Sprite( SDL_Surface *surface,
            int xOffset,
            int yOffset,
            int width,
            int height );

    ~Sprite();

    const SDL_Surface* surface() const;
    int x() const;
    int y() const;
    int width() const;
    int height() const;

private:
    SDL_Surface *mpSurface;
    int mX, mY, mW, mH;
};

#endif