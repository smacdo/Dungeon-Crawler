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
#include "common/platform.h"
#include "common/utils.h"
#include "config.h"
#include <string>
#include <sstream>
#include <iostream>
#include <stdlib.h>

namespace App {
    bool GAppTestingMode = false;

/**
 * Configures assertion handling for either normal application mode or
 * unit testing mode
 */
void setTestingMode( bool inTestingMode )
{
    GAppTestingMode = inTestingMode;
}

/**
 * Generates a assertion reporting dialog (or console output) to show to the
 * player, before exiting the application
 *
 * \param  message     An accompanying assertion description (if provided)
 * \param  expression  String containing the expression text
 * \param  filename    Name of the file that generated the assertion
 * \param  lineNumber  Line that generated the assertion
 */
EAssertionStatus raiseAssertion( const char *message,
                                 const char *expression,
                                 const char *filename,
                                 unsigned int lineNumber )
{
    if ( message == NULL )
    {
        message = "Assertion failed";
    }

    if ( GAppTestingMode )
    {
        std::cerr << "ASSERTION FAILED: " << expression << std::endl;
        quit( EPROGRAM_ASSERT_FAILED, "ASSERTION FAILED" );      
    }

    std::cerr
        << "---------- ASSERTION FAILED! ---------- " << std::endl
        << "MESSAGE   : "  << message                 << std::endl
        << "EXPRESSION: "  << expression              << std::endl
        << "FILENAME  : "  << filename                << std::endl
        << "LINE      : "  << lineNumber              << std::endl
        << "---------------------------------------"  << std::endl
        << std::endl;

    // Now return and let the caller know that they should abort
    return EAssertion_Halt;
}

/**
 * Generates a non-fatal error message that is displayed to the player, and
 * the player is allowed to choose whether to continue or quit.
 *
 * \param  message  The main error message
 * \param  details  (optional) Details about the problem
 */
void raiseError( const std::string& message,
                 const std::string& details )
{
    std::cerr
        << "---------- AN ERROR OCCURRED! ---------- " << std::endl
        << "MESSAGE: " << message                      << std::endl
        << "DETAILS: " << details                      << std::endl
        << "----------------------------------------"  << std::endl
        << std::endl;
}

/**
 * Displays a fatal error message to the player before he/she is forced to
 * quit playing.
 *
 * \param  message  The main error message
 * \param  details  (optional) Details about the problem
 */
void raiseFatalError( const std::string& message,
                      const std::string& details )
{
    std::cerr
        << "---------- FATAL ERROR OCCURRED ---------- " << std::endl
        << "MESSAGE: " << message                      << std::endl
        << "DETAILS: " << details                      << std::endl
        << "------------------------------------------"  << std::endl
        << std::endl;

    App::quit( EPROGRAM_FATAL_ERROR, message );
}

void startup()
{
}

/**
 * Quit the program with the requested status and reason
 */
void quit( EProgramStatus programStatus, const std::string& )
{
    exit( programStatus );
}

/**
 * Returns a string describing the conditions under which the game was
 * built. Useful for troubleshooting, and that's about it
 */
std::string getBuildString()
{
    std::ostringstream ss;

    // All the settings we'll need to discover
    std::string releaseMode = "_?release?_";
    std::string processor   = "_?cpu?_";
    std::string platform    = "_?platform?_";
    std::string sse         = "_?sse?_";
    std::string compiler    = "_?compiler?_";

    // need to implement this

    // Properly format the build string according to the compiler
    ss << GAME_ID << " "
       << GAME_VERSION_MAJOR << "."
       << GAME_VERSION_MINOR << "."
       << GAME_VERSION_PATCH << GAME_VERSION_RELEASE << "-"
       << releaseMode << " "
       << sse         << " "
       << platform    << " "
       << __DATE__    << " "
       << __TIME__    << " "
        ;

    // Return the build string
    return ss.str();
}

}
