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
#include <googletest/googletest.h>
#include <string>
#include <sstream>
#include <iostream>
#include <stdlib.h>

namespace App {
    bool GIsUnitTesting        = false;
    bool GTestAssertShouldExit = false;

/**
 * Configures assertion handling for either normal application mode or
 * unit testing mode
 */
void setIsInUnitTestMode( bool isInUnitTesting )
{
    GIsUnitTesting = true;
}

void setTestAssertsShouldDie( bool shouldBlowUp )
{
    GTestAssertShouldExit = shouldBlowUp;
}

void resetTestAssertsShouldDie()
{
    GTestAssertShouldExit = false;
}

/**
 * Returns a human readable string for the given error enum type
 */
std::string getNameForError( EErrorType type )
{
    switch ( type )
    {
        case EERROR_WARNING:
            return "warning";
        case EERROR_ERROR:
            return "error";
        case EERROR_FATAL:
            return "fatal";
    }

    return "unknown error type";
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
    reportSoftwareError( message, details, EERROR_ERROR, 0, NULL, NULL );
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
    reportSoftwareError( message, details, EERROR_FATAL, 0, NULL, NULL );
    App::quit( EPROGRAM_FATAL_ERROR, message );
}

EAssertionStatus raiseAssertion( const char *pMessage,
                                 const char *pExpression,
                                 const char *pFilename,
                                 unsigned int line )
{
    // We need to handle a special case of being in unit test mode. A software
    // assertion should not cause kill the unit test runner
    if ( GIsUnitTesting )
    {
        ADD_FAILURE_AT( pFilename, line )
            << "Application assertion triggered: "
            << pExpression;

        if ( GTestAssertShouldExit )
        {
            std::cerr << "ASSERTION FAILED: "
                      << pExpression
                      << std::endl;

            quit( EPROGRAM_ASSERT_FAILED, "Assertion failed" );
        }

        return EAssertion_Continue;
    }
    
    // Account for any null strings
    if ( pMessage == NULL )
    {
        pMessage = "An internal software assertion has occurred";
    }

    if ( pExpression == NULL )
    {
        pMessage = "n/a";
    }

    if ( pFilename == NULL )
    {
        pFilename = "n/a";
    }

    // Call a platform specific reporting function, and check what it decides
    // to return to us
    EAssertionStatus ret = reportAssertion( std::string( pMessage ),
                                            std::string( pExpression ),
                                            std::string( pFilename ),
                                            line );

    // Let the engine know how to handle our function
    if ( ret == EAssertion_Default )
    {
        return GDefaultAssertionStatus;
    }
    else
    {
        return ret;
    }
}

}
