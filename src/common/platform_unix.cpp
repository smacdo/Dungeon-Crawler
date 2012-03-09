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
#include "common/platform.h"
#include "common/utils.h"
#include "version.h"
#include <string>
#include <sstream>
#include <iostream>
#include <stdlib.h>

namespace App {

/**
 * Generates a assertion reporting dialog (or console output) to show to the
 * player, before exiting the application
 *
 * \param  message     An accompanying assertion description (if provided)
 * \param  expression  String containing the expression text
 * \param  filename    Name of the file that generated the assertion
 * \param  lineNumber  Line that generated the assertion
 */
EAssertionStatus reportAssertion( const std::string& message,
                                  const std::string& expression,
                                  const std::string& filename,
                                  unsigned int lineNumber )
{
    std::cerr
        << "---------- ASSERTION FAILED! ---------- " << std::endl
        << "MESSAGE   : "  << message                 << std::endl
        << "EXPRESSION: "  << expression              << std::endl
        << "FILENAME  : "  << filename                << std::endl
        << "LINE      : "  << lineNumber              << std::endl
        << "---------------------------------------"  << std::endl
        << std::endl;

    // Now return and let the caller know that they should abort
    return EAssertion_Default;
}

/**
 * Platform specific error reporting utility. This method is called by the
 * game engine whenever there is an error (or warning) to be reported. The
 * method is responsible for providing this information to the user and 
 * allowing the player to take appropriate action based on it.
 *
 * \param  message       A terse description of the error that occurred
 * \param  details       Optional contextual details of the error
 * \param  type          What type of error is this
 * \param  lineNumber    Line number that this error occurred on
 * \param  functionName  Name of the function where error occurred
 */
void reportSoftwareError( const std::string& message,
                          const std::string& details,
                          EErrorType type,
                          unsigned int lineNumber,
                          const char * filename,
                          const char * functionName )
{
    // Error header
    std::cerr
        << std::endl
        << "########################################################################"
        << std::endl
        << "# A(n) " << getNameForError( type ) << " has occurred. Details follow. "
        << std::endl
        << "#"
        << std::endl;

    // Print the message body
    std::cerr << "# MESSAGE: " << message << std::endl;

    if ( filename != NULL )
    {
        std::cerr << "#   FILE: " << filename << std::endl;
    }

    if ( lineNumber > 0 )
    {
        std::cerr << "#   LINE: " << lineNumber << std::endl;
    }

    if ( functionName != NULL )
    {
        std::cerr << "#   FUNC: " << functionName << std::endl;
    }

    // If there were extra details, print them at the bottom of the error
    // output
    if (! details.empty() )
    {
        std::cerr << "# DETAILS: "   << std::endl
                  << "# -------- "   << std::endl
                  << "# " << details << std::endl;
    }

    // Message bottom
    std::cerr
        << "########################################################################"
        << std::endl
        << std::endl;
}

/**
 * Performs UNIX specific start up tasks
 */
void startup()
{
}

/**
 * Quit the program with the requested status and reason
 */
void quit( EProgramStatus programStatus, const std::string& message )
{
    if (! message.empty() )
    {
        std::cerr << "EXITING: " << message << std::endl;
    }

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
    ss << Version::VERSION_S << " "
       << sse         << " "
       << platform    << " "
       << __DATE__    << " "
       << __TIME__    << " "
        ;

    // Return the build string
    return ss.str();
}

}
