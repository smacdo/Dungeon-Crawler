/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#include "worldgen/dungeongenerator.h"
#include "graphics/clientview.h"
#include "common/utils.h"
#include "level.h"
#include "inputmanager.h"
#include "dungeoncrawler.h"

#include <string>
#include <iostream>

#include <time.h>
#include <stdlib.h>
#include <SDL.h>

#if defined(_WIN32)
#pragma comment(linker, "\"/manifestdependency:type='Win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='X86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif

///////////////////////////////////////////////////////////////////////////////
// Dungeon Crawler Application entry point
///////////////////////////////////////////////////////////////////////////////
int main( int , char*[] )
{
#if defined(_WIN32)
    _putenv("SDL_VideoDriver=directx");
#endif

    // Print out information
    std::cout << App::getBuildString() << std::endl;
    
    // Create the world
    DungeonGenerator generator( 76, 50 );
    Level *level = generator.generateLevel();

    // Game components
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


