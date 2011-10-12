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

namespace
{
    /**
     * Return the main rendering window's HWND
     */
    HWND appGetWindowHandle()
    {
        SDL_SysWMinfo sysInfo;
        int result = SDL_GetWMInfo( &sysInfo );

        assert( result == 0 );

        return sysInfo.window;
    }
}

#endif

void appRaiseError( const std::string& message )
{
#if defined(_WIN32)
    HWND window = appGetWindowHandle();
#else
    std::cerr << "[FATAL ERROR]: " << message << std::endl;
//    exit( 1 );
#endif
}