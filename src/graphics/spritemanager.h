#ifndef SCOTT_DUNGEON_GRAPHICS_SPRITEMANAGER_H
#define SCOTT_DUNGEON_GRAPHICS_SPRITEMANAGER_H

#include <map>
#include <vector>
#include <string>

class Sprite;
struct SDL_Surface;

class SpriteManager
{
public:
    SpriteManager();
    ~SpriteManager();

    // Creates a new sprite object and stores it in the sprite manager
    void addSpriteTemplate( const std::string& spriteName,
                            const std::string& filename );

    // Creates a new sprite object and store it in the sprite manager
    void addSpriteTemplate( const std::string& spriteName,
                            const std::string& filename,
                            int xOffset,
                            int yOffset,
                            int width,
                            int height );

    // Locates a loaded sprite and returns a copy of it
    Sprite* createSprite( const std::string& spriteName ) const;

    // Returns the number of sprites loaded
    size_t spriteCount() const;

    // Returns the number of images loaded
    size_t imageCount() const;

protected:
    // Loads an image file and returns a SDL surface
    SDL_Surface* loadImage( const std::string& filepath );

    // Destroys all loaded sprites and surfaces
    void unload();

private:
    // Image content directory
    std::string mImageRoot;

    // Association of image names to loaded texture files
    std::map<std::string, SDL_Surface*> mLoadedSurfaces;

    // List of loaded sprite definitions
    std::map<std::string, Sprite*> mSpriteCache;
};

#endif
