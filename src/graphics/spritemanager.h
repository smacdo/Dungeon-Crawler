#ifndef SCOTT_DUNGEON_GRAPHICS_SPRITEMANAGER_H
#define SCOTT_DUNGEON_GRAPHICS_SPRITEMANAGER_H

#include <map>
#include <vector>

class Sprite;
struct SDL_Surface;

class SpriteManager
{
public:
    SpriteManager();
    ~SpriteManager();

    // Instruct sprite manager to preload an image
    void preloadImage( const std::string& imagename,
                       const std::string& filepath );

    // Creates a new sprite
    Sprite* createSprite( const std::string& imagename ) const;

    // Creates a new sprite in a spritesheet
    Sprite* createSprite( const std::string& imagename,
                          int xOFfset,
                          int yOffset,
                          int width,
                          int height ) const;

protected:
    SDL_Surface* loadImage( const std::string& filepath );
    void unload();

private:
    // Image content directory
    std::string mImageRoot;

    // Association of image names to loaded texture files
    std::map<std::string, SDL_Surface*> mImageMap;

    // List of loaded sdl surfaces
    std::vector<SDL_Surface*> mSurfaces;
};

#endif