#include "worldgen/dungeongenerator.h"
#include "common/utils.h"
#include "level.h"

#include "graphics/clientview.h"
#include "inputmanager.h"

#include <string>
#include <iostream>
#include <cassert>
#include <time.h>
#include <stdlib.h>

#pragma comment(linker, "\"/manifestdependency:type='Win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='X86' publicKeyToken='6595b64144ccf1df' language='*'\"")

#define USE_SDL_MAIN_MAGIC 1
#include "common/platform.h"        // let SDL redefine our main function

int main( int argc, char* argv[] )
{
    putenv("SDL_VideoDriver=directx");
    srand( time(NULL) );
    
    // Create the world
    DungeonGenerator generator( 76, 50 );
    Level *level = generator.generateLevel();
//    level->print();

    //
    // Game components
    //
    ClientView   clientView;
    InputManager input;

    clientView.start();

    //
    // Main game loop
    //
    while (! input.didUserPressQuit() )
    {
        // make sure all user input is taken into account before simulation
        // and rendering
        input.processInput();

        // move the character
        if ( input.didUserMove() )
        {
            clientView.moveCamera( input.userMoveXAxis(),
                                   input.userMoveYAxis() );
        }

        // simulate the world for a tiny timeslice

        // and now draw the world
        clientView.draw( deref(level) );
    }

    // all done. it worked!
    return 0;
}


