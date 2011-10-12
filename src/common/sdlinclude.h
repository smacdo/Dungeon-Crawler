#ifndef SCOTT_DUNGEON_COMMON_PLATFORM_H
#define SCOTT_DUNGEON_COMMON_PLATFORM_H

// Include windows.h properly on windows
#if defined(WIN32) || defined(_WINDOWS)
#   define WIN32_LEAN_AND_MEAN
#   define NOMINMAX
#   include <windows.h>
#endif

// Stop SDL from polluting the global namespace with its main
// macro (only want this magic in the file containing our main())
#ifndef USE_SDL_MAIN_MAGIC
#   undef main
#endif

#endif
