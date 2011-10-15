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
#define UNICODE
#define _UNICODE

#include "common/platform.h"
#include "common/logging.h"
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
    HWND window = GetActiveWindow();

    // Write out an error message if none was given
    if ( message == NULL )
    {
        message = "Unlucky for you, an internal assertion check has failed. Please "
                  "contact the maintainer to let him know the game broke. ";
    }

    // Spit the assertion information out to the log
    LOG_ERROR("SYSTEM") << "ASSERTION FAILED in file "
                        << filename << ":" << lineNumber << "; "
                        << "assert( " << expression << " )";

    // Properly format the assertion message, and also convert the expression
    // and filename into wstring for display with windows
    std::ostringstream ss;

    ss << "ASSERTION DETAILS: \n"
       << "assert( " << expression << " )\n"
       << filename   << ":"        << lineNumber;

    // Convert the error message and assertion expression into Windows friendly
    // unicode wstring objects
    std::wstring wExpression = WinNTStringToWideString( ss.str() );
    std::wstring wMessage    = WinNTStringToWideString( message );

    // Set up variables required to display the task dialog
    TASKDIALOGCONFIG tc = { 0 };

    const TASKDIALOG_BUTTON cb[] =
    {
        { 0, L"View crash information\nThat way the developers can try to fix your bug"    },
        { 1, L"Fire up the debugger\nThis won't do anything unless you're a developer, sorry!" },
        { 2, L"Quit the game\nOh no! We promise not to crash again..."        }
    };

    tc.cbSize = sizeof( tc );
    tc.hwndParent = window;
    tc.hInstance  = (HINSTANCE) GetModuleHandle(NULL); //GetWindowLong( window, GWL_HINSTANCE );
    tc.dwFlags    = TDF_USE_HICON_MAIN | TDF_USE_HICON_FOOTER | TDF_EXPAND_FOOTER_AREA |
                    TDF_EXPANDED_BY_DEFAULT | TDF_USE_COMMAND_LINKS;

    LoadIconWithScaleDown( NULL, IDI_ERROR,
                           GetSystemMetrics(SM_CXICON),
                           GetSystemMetrics(SM_CYICON),
                           &tc.hMainIcon );
    LoadIconWithScaleDown( NULL, IDI_INFORMATION,
                           GetSystemMetrics(SM_CXSMICON),
                           GetSystemMetrics(SM_CYSMICON),
                           &tc.hFooterIcon);

    tc.pszWindowTitle      = L"Dungeon Crawler Internal Error";
    tc.pszMainInstruction  = L"An internal assertion check failed!";
    tc.pszContent          = L"Sorry to ruin your fun, but it looks like you found a bug in our code. Aren't you the lucky one?";
    tc.pszFooter           = wExpression.c_str();


    tc.cButtons       = ARRAYSIZE(cb);
    tc.pButtons       = cb;
    tc.nDefaultButton = 0;

    // Display the task dialog
    int buttonPressed        = 0;
    int commandPressed       = 0;
    BOOL verificationChecked = false;

    HRESULT hResult = TaskDialogIndirect( &tc,
                                          &buttonPressed, &commandPressed, 
                                          &verificationChecked );

    if (! SUCCEEDED(hResult) )
    {
        // huh?
        quit( EPROGRAM_ASSERT_FAILED, "Task dialog failed to display" );
    }

    // Now interpret the results of the task dialog
    switch ( buttonPressed )
    {
        case 0:     // view bug information
            quit( EPROGRAM_ASSERT_FAILED, "Assertion failed" );
            break;

        case 1:     // fire up debugger
            return EAssertion_Halt;
            
        case 2:     // quit
            quit( EPROGRAM_ASSERT_FAILED, "Assertion failed" );
            break;

        default:
            break;
    }

#else
    std::cerr << "[FATAL ERROR]: " << message << std::endl;
    //    exit( 1 );
#endif

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
#if defined(_WIN32)
    // Convert STL strings into wstrings for windows
    std::wstring wMessage     = WinNTStringToWideString( message );
    std::wstring wDetails     = WinNTStringToWideString( details );

    /////////////////////////////////////////////////////////////////////////
    // Windows Vista and Windows 7 Task Dialog                             //
    /////////////////////////////////////////////////////////////////////////
    TASKDIALOGCONFIG tc = { 0 };

    const TASKDIALOG_BUTTON cb[] =
    {
        { 0, L"Keep going\nContinue playing, but please be aware the game could become instable"    },
        { 1, L"Quit the game\nRather than continuing onward, quit and fix the problem" }
    };

    tc.cbSize = sizeof( tc );
    tc.hwndParent = GetActiveWindow();
    tc.hInstance  = (HINSTANCE) GetModuleHandle(NULL);
    tc.dwFlags    = TDF_USE_HICON_MAIN | TDF_USE_COMMAND_LINKS |
                    TDF_EXPAND_FOOTER_AREA ;

    tc.cButtons       = ARRAYSIZE(cb);
    tc.pButtons       = cb;
    tc.nDefaultButton = 1;

    LoadIconWithScaleDown( NULL, IDI_ERROR,
        GetSystemMetrics(SM_CXICON),
        GetSystemMetrics(SM_CYICON),
        &tc.hMainIcon );

    tc.pszWindowTitle      = L"Dungeon Crawler Game Error";
    tc.pszMainInstruction  = L"The game has encountered a problem";
    tc.pszContent          = wMessage.c_str();

    if ( wDetails.size() > 0 )
    {
        tc.pszExpandedInformation  = wDetails.c_str();
        tc.pszExpandedControlText  = L"Hide error details";
        tc.pszCollapsedControlText = L"Show error details";
    }

    // Display the task dialog
    int buttonPressed        = 0;
    int commandPressed       = 0;
    BOOL verificationChecked = false;

    HRESULT hResult = TaskDialogIndirect( &tc,
        &buttonPressed, &commandPressed, 
        &verificationChecked );

    // Now interpret the results of the task dialog
    switch ( buttonPressed )
    {
    case 1:     // view bug information
        quit( EPROGRAM_USER_ERROR, "User chose to quit after error" );
        break;

    default:
        break;
    }

#else
    std::cerr << "[FATAL ERROR]: " << message << std::endl;
//    exit( 1 );
#endif
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
#if defined(_WIN32)
    // Convert STL strings into wstrings for windows
    std::wstring wMessage     = WinNTStringToWideString( message );
    std::wstring wDetails     = WinNTStringToWideString( details );

    /////////////////////////////////////////////////////////////////////////
    // Windows Vista and Windows 7 Task Dialog                             //
    /////////////////////////////////////////////////////////////////////////
    TaskDialog( GetActiveWindow(),
                GetModuleHandle(NULL),
                L"Dungeon Crawler Fatal Error",
                wMessage.c_str(),
                wDetails.c_str(),
                TDCBF_OK_BUTTON,
                TD_ERROR_ICON,
                NULL
    );

    quit( EPROGRAM_FATAL_ERROR, message );
#else
    std::cerr << "[FATAL ERROR]: " << message << std::endl;
//    exit( 1 );
#endif
}

/**
 * Quit the program with the requested status and reason
 */
void quit( EProgramStatus programStatus, const std::string& message )
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