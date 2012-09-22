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
#include "dungeoncrawler.h"
#include "inputmanager.h"
#include "engine/appconfig.h"
#include "engine/optionsparser.h"
#include "game/world.h"
#include "game/gameplayengine.h"
#include "engine/playerinputcontroller.h"
#include "common/platform.h"
#include "common/utils.h"

#include <string>

#include <QApplication>
#include "client/dungeonclientwindow.h"

///////////////////////////////////////////////////////////////////////////////
// Dungeon Crawler Application entry point
///////////////////////////////////////////////////////////////////////////////
int main( int argc , char** argv )
{
    Q_INIT_RESOURCE( dungeoncrawler );
    QApplication app( argc, argv );

    // Game start up. Parse any requested command line arguments and initialize
    // the platform before starting the game up
    OptionsParser optParser;

    optParser.parseCommandLine( argc, argv );
    optParser.process();

    // Platform specific start up
    App::startup();

    // Create the game's subsystems
    InputManager input;
    PlayerInputController inputController;

    // Start the game simulation
    GamePlayEngine gamePlayEngine( inputController );
    gamePlayEngine.createNewWorld();

    // Main game loop
    DungeonClientWindow clientWindow;
    clientWindow.show();

//    while (! input.didUserPressQuit() )
//    {
        // make sure all user input is taken into account before simulation
        // and rendering
//        input.process();
//        inputController.update( input );

        // simulate the world for a tiny timeslice
//        gamePlayEngine.simulate();

        // and now draw the world
//        clientView.draw( gamePlayEngine.activeWorld() );
//    }

    return app.exec();
}
