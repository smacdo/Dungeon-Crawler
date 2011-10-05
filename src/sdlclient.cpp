#include <SDL.h>
#include <SDL_image.h>

#include <string>
#include <iostream>
#include <cassert>

namespace Config
{
    const int DefaultScreenWidth  = 640;
    const int DefaultScreenHeight = 480;
    const int DefaultScreenDepth  = 32;
}

bool GKeepRunning = true;
SDL_Surface * GBackbuffer = NULL;

SDL_Surface* loadImage( const std::string& filename );
SDL_Surface* createWindow();
void blitImage( int x, int y, SDL_Surface* source, SDL_Surface* dest );
void processInput();

int main( int argc, char* argv[] )
{
    SDL_Init( SDL_INIT_EVERYTHING );
    
    // Create the client window
    GBackbuffer = createWindow();
    
    // Load a simple test image
    SDL_Surface *pImage = loadImage( "data/dg_dungeon32.png" );

    // Draw it
    while ( GKeepRunning )
    {
        processInput();

        blitImage( 90, 90, pImage, GBackbuffer );
        SDL_Flip( GBackbuffer );

        SDL_Delay( 30 );
    }

    // all done. it worked!
    SDL_Quit();
    return 0;
}

void processInput()
{
    SDL_Event event;

    while ( SDL_PollEvent( &event ) )
    {
        switch ( event.type )
        {
            case SDL_QUIT:
                GKeepRunning = false;
                break;

            case SDL_KEYDOWN:
            {
                if  ( event.key.keysym.sym = SDLK_ESCAPE )
                {
                    GKeepRunning = false;
                }
            }
        }
    }
}

SDL_Surface* loadImage( const std::string& filename )
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

void blitImage( int x, int y, SDL_Surface* source, SDL_Surface* dest )
{
    // precondition sanity check
    assert( x > 0 );
    assert( y > 0 );
    assert( source != NULL );
    assert( dest   != NULL );

    // Now blit the sprites
    SDL_Rect destRect = { x, y, 0, 0 };
    SDL_BlitSurface( source, NULL, dest, &destRect );
}

SDL_Surface* createWindow()
{
    SDL_Surface *pScreen = NULL;
    
    // Init the backbuffer surface
    pScreen = SDL_SetVideoMode( Config::DefaultScreenWidth,
                                Config::DefaultScreenHeight,
                                Config::DefaultScreenDepth,
                                SDL_HWSURFACE | SDL_DOUBLEBUF );

    if ( pScreen == NULL )
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


    return pScreen;
}