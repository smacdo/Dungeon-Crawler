#include "worldgen/dungeongenerator.h"
#include "level.h"
#include "utils.h"

#include "graphics/clientview.h"
#include "inputmanager.h"

#include <string>
#include <iostream>
#include <cassert>
#include <time.h>
#include <stdlib.h>

int main( int argc, char* argv[] )
{
    srand( time(NULL) );
    
    // Create the world
    DungeonGenerator generator( 76, 50 );
    Level *level = generator.generateLevel();
    level->print();

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


