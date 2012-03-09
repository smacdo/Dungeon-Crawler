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
#include "appconfig.h"
#include "game/world.h"
#include "game/gameplayengine.h"
#include "game/playerinputcontroller.h"
#include "graphics/clientview.h"
#include "inputmanager.h"
#include "common/platform.h"
#include "common/utils.h"

#include <string>
#include <iostream>
#include <fstream>

#include <boost/program_options.hpp>
#include <SDL2/SDL.h>

AppConfig parseCommandLineArgs( int argc, char** args );

namespace po = boost::program_options;

///////////////////////////////////////////////////////////////////////////////
// Dungeon Crawler Application entry point
///////////////////////////////////////////////////////////////////////////////
int main( int argc , char* argv[] )
{
    // Game start up. Parse any requested command line arguments and initialize
    // the platform before starting the game up
    AppConfig config = parseCommandLineArgs( argc, argv );
    App::startup();

    // Make sure SDL is up and running
    if ( SDL_Init( SDL_INIT_EVERYTHING ) != 0 )
    {
        App::raiseFatalError( "Failed to init SDL", SDL_GetError() );
    }

    //
    // Create the game's subsystems
    //
    InputManager input;
    PlayerInputController inputController;

    //
    // Start the game simulation
    //
    GamePlayEngine gamePlayEngine( inputController );
    gamePlayEngine.createNewWorld();

    //
    // Main game loop
    //
    ClientView clientView( input );
    clientView.start();

    while (! input.didUserPressQuit() )
    {
        // make sure all user input is taken into account before simulation
        // and rendering
        input.process();
        inputController.update( input );

        // simulate the world for a tiny timeslice
        gamePlayEngine.simulate();

        // and now draw the world
        clientView.draw( gamePlayEngine.activeWorld() );
    }

    // all done. it worked!
    return 0;
}

/**
 * Command line parsing. This function parses all options provided to the
 * dungeon crawler client when launched, and returns an AppConfig structure
 * that contains all parsed options
 */
AppConfig parseCommandLineArgs( int argc, char** argv )
{
    AppConfig appConfig;

    // Declare a group of options that will only be allowed from the command
    // line
    po::options_description generic("Command Line Options");
    generic.add_options()
        ( "version,v", "Print version information" )
        ( "help",      "Show command line options" )
        ( "help-all",  "Show all options" )
        ;

    // Declare a group of options that will be allowed from both the command
    // line and from a configuration file
    po::options_description config("Configuration");
    config.add_options()
        (
            "renderer.width,w",
            po::value<int>( &appConfig.rwWidth )->default_value( appConfig.rwWidth ),
            "Width of the main game window"
        )
        (
            "renderer.height,h",
            po::value<int>( &appConfig.rwHeight )->default_value( appConfig.rwHeight ),
            "Height of the main game window"
        )
        (
            "renderer.fullscreen,f",
            po::value<bool>( &appConfig.rwFullscreen )->default_value( false ),
            "Launch in full screen or windowed mode"
        )
        (
            "renderer.driver",
            po::value<std::string>( &appConfig.rwDriver )->default_value( "default" ),
            "Platform specific video driver to use"
        )
        (
            "game.randomseed,s",
            po::value<int>( &appConfig.randomSeed )->default_value( 0 ),
            "Value to seed the random number generator with"
        )
    ;

    // Combine our program options together
    po::options_description cmdlineOptions;
    cmdlineOptions.add(generic).add(config);

    po::options_description extraOptions;
    extraOptions.add(config);

    // Parse the command line first
    po::variables_map varmap;

    po::store( po::parse_command_line( argc, argv, cmdlineOptions ), varmap );

    // Now try to parse the config file (if it exists)
    std::ifstream configFile( "dungeonclient.cfg" );

    if ( configFile )
    {
        po::store( po::parse_config_file( configFile, extraOptions), varmap );
    }
    else
    {
        std::cerr << "Failed to open config file" << std::endl;
    }

    // All done. Process what the user provided us
    po::notify( varmap );

    if ( varmap.count("help") )
    {
        std::cout << App::getBuildString() << std::endl
                  << generic
                  << std::endl;
    }
    else if ( varmap.count("help-all") )
    {
        std::cout << App::getBuildString() << std::endl
                  << generic          << std::endl
                  << config
                  << std::endl;
    }
    else if ( varmap.count("version") )
    {
        std::cout << App::getBuildString() << std::endl;
    }

    // All done parsing
    return appConfig;
}
