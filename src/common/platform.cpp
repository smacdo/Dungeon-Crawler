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
#include <SDL.h>
#include <SDL_syswm.h>
#include <string>
#include <sstream>
#include <iostream>

#if defined(_WIN32)
#   define WIN32_LEAN_AND_MEAN
#   define NOMINMAX
#   include <Windows.h>
#   include <Commctrl.h>
#endif

/////////////////////////////////////////////////////////////////////////////
// Internal methods for the windows platform
/////////////////////////////////////////////////////////////////////////////
#if defined(_WIN32)
namespace
{
    /**
     * Return the SDL main window's HWND handle.
     *
     * \return HWND value of the SDL main window
     */
    HWND appGetWindowHandle()
    {
        SDL_SysWMinfo sysInfo;
        HWND hwnd;

        // Get the window handle out of SDL
        int result = SDL_GetWMInfo( &sysInfo );
        hwnd       = sysInfo.window;

        // Make sure the retrieval worked
        if ( result != 1 )
        {
            std::cerr << "[ERROR] Failed to retrieve window handle" << std::endl;
            hwnd = 0;
        }

        return hwnd;
    }

    /**
     * Converts an STL string into a WindowsNT wchar* wrapped inside of a
     * pretty wstring
     *
     * \param  str  The stl string you want to convert
     * \return A wstring representing the input
     */
    std::wstring WinNTStringToWideString( const std::string& str )
    {
        // Find the length of the soon to be allocated wstring
        size_t slen = str.length();
        int len     = MultiByteToWideChar( CP_ACP, 0,
                                           str.c_str(),
                                           static_cast<int>(slen) + 1,
                                           0,
                                           0 );

        // Allocate space for the new wstring, and then convert the input
        wchar_t *buffer = new wchar_t[len];

        MultiByteToWideChar( CP_ACP, 0,
                             str.c_str(),
                             static_cast<int>(slen) + 1,
                             buffer,
                             len );

        // Now create the wstring, copy the temporary wchar* buffer into it and
        // then destroy the buffer before returning
        std::wstring result( buffer );
        DeleteArray( buffer );

        return result;
    }
}

#endif

/////////////////////////////////////////////////////////////////////////////
// Application utility functions
/////////////////////////////////////////////////////////////////////////////
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
EAssertionStatus raiseAssertion( const char *message,
                                 const char *expression,
                                 const char *filename,
                                 unsigned int lineNumber )
{
#if defined(_WIN32)
    int result  = 0;
    HWND window = appGetWindowHandle();

    // Write out an error message if none was given
    if ( message == NULL )
    {
        message = "Unlucky for you, an internal assertion check has failed. Please "
            "contact the maintainer to let him know the game broke. ";
    }

    // Properly format the assertion message, and also convert the expression
    // and filename into wstring for display with windows
    std::ostringstream ss;

    ss << "EXPRESSION: " << expression << "\n"
       << "FILE: "       << filename 
       << ":"            << lineNumber;

    // Convert the error message and assertion expression into Windows friendly
    // unicode wstring objects
    std::wstring wExpression = WinNTStringToWideString( ss.str() );
    std::wstring wMessage    = WinNTStringToWideString( message );

    // Show the task dialog containing details on the failed assertion
    TaskDialog( window, NULL,
        L"Dungeon Crawler",
        wMessage.c_str(),
        wExpression.c_str(),
        TDCBF_OK_BUTTON,
        TD_ERROR_ICON ,
        &result );

#else
    std::cerr << "[FATAL ERROR]: " << message << std::endl;
    //    exit( 1 );
#endif

    // Now return and let the caller know that they should abort
    return EAssertion_Halt;
}

/**
 * Generates an informative error message that is displayed to the user. Note
 * that calling this method will (by default) cause the application to
 * terminate once the user clicked 'ok'
 *
 * \param  message    The message to display to the player
 * \param  filename   (optional) Name of the file that generated the error
 * \param  lineNumber (optional) Line number that generated the error
 */
void raiseError( const std::string& message,
                 const char * filename,
                 unsigned int lineNumber )
{
#if defined(_WIN32)
    HWND window = appGetWindowHandle();

    //
    // Windows XP error dialog
    //
//    MessageBox( window,
//                message.c_str(),
//                "A fatal error has occurred",
//                MB_OK | MB_ICONSTOP | MB_TASKMODAL | MB_TOPMOST );

    //
    // Windows Vista and Windows 7 Task Dialog
    //
    int buttonResult = 0;

    TaskDialog( window, NULL,
                L"Dungeon Crawler",
                L"This game has encountered a fatal error",
                L"(error details generally go here)",
                TDCBF_OK_BUTTON,
                TD_ERROR_ICON ,
                &buttonResult );

#else
    std::cerr << "[FATAL ERROR]: " << message << std::endl;
//    exit( 1 );
#endif
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

    // Retrieval of settings is compiler specific
#if defined(OS_WINDOWS)
    // Compiler
    compiler = std::string("MSVC") + STRINGIFY(_MSC_VER);

    // Release mode
#   ifdef _DEBUG
        releaseMode = "debug";
#   else
        releaseMode = "release";
#   endif

    // Always x86 at the moment
    processor = "x86";

    // Platform
#   ifdef USE_WINDOWS_VISTA
        platform = "Windows 6.x (Vista, 7)";
#   else
        platform = "Windows 5.1 (XP)";
#   endif

    // SSE
#   if _M_IX86_FP == 0
        sse = "no_sse";
#   elif _M_IX86_FP == 1
        sse = "sse";
#   elif _M_IX86_FP == 2
        sse = "sse2";
#   endif
#else
    // need to implement
#endif

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