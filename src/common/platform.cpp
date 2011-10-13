#include "common/platform.h"
#include <SDL.h>
#include <SDL_syswm.h>
#include <string>
#include <iostream>
#include <cassert>

#if defined(_WIN32)
#   define WIN32_LEAN_AND_MEAN
#   define NOMINMAX
#   include <Windows.h>
#   include <Commctrl.h>
#endif

#if defined(_WIN32)
namespace
{
    /**
     * Return the main rendering window's HWND
     */
    HWND appGetWindowHandle()
    {
        SDL_SysWMinfo sysInfo;
        HWND hwnd;

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
}

#endif

void appRaiseAssertion( const char *expression,
                        const char *file,
                        size_t lineNumber )
{
}

void appRaiseError( const std::string& message )
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