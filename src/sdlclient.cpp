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
#include "common/platform.h"
#include "level.h"
#include "inputmanager.h"
#include "dungeoncrawler.h"
#include "appconfig.h"

#include <string>
#include <iostream>
#include <fstream>
#include <SDL.h>

#include <boost/program_options.hpp>

AppConfig parseCommandLineArgs( int argc, char** args );

namespace po = boost::program_options;

#if defined(_WIN32)
#pragma comment(linker, "\"/manifestdependency:type='Win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='X86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif

///////////////////////////////////////////////////////////////////////////////
// Dungeon Crawler Application entry point
///////////////////////////////////////////////////////////////////////////////
int main( int argc , char* argv[] )
{
    // Parse command line options first
    AppConfig config = parseCommandLineArgs( argc, argv );

#if defined(_WIN32)
    _putenv("SDL_VideoDriver=directx");
#endif

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
