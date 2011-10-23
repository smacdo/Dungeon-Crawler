###########################################################################
### Dungeon CMake buildscript for crossplatform Linux/POSIX systems      ##
###########################################################################
# Project source code files
set(common_srcs   common/logging.cpp
                  common/platform.cpp
                  common/rect.cpp
)

set(worldgen_srcs worldgen/levelgenerator.cpp
                  worldgen/dungeongenerator.cpp
                  worldgen/roomgenerator.cpp)

set(graphics_srcs graphics/spritemanager.cpp
                  graphics/sprite.cpp
                  graphics/clientview.cpp
)

set(game_srcs     camera.cpp
                  dungeon.cpp
                  inputmanager.cpp
                  level.cpp
                  room.cpp
                  sdlclient.cpp
                  tilegrid.cpp
)
				  
set(srcs ${common_srcs} ${worldgen_srcs} ${game_srcs} ${graphics_srcs})

# Compiler definitions
ADD_DEFINITIONS(-g -W -Wall -Wextra -Werror -std=c++0x)

#
# required packages
#
find_package(Boost COMPONENTS system filesystem REQUIRED)
find_package(SDL REQUIRED)
find_package(SDL_image REQUIRED)

# find_package(OpenGL REQUIRED)

# on OS X we also have to add '-framework Cocoa' as library.  This is
# # actually a bit of an hack but it's easy enough and reliable.
set(EXTRA_LIBS "")
if (APPLE)
    set(EXTRA_LIBS ${EXTRA_LIBS} "-framework Cocoa")
endif()


#
# Generate the executable and link required libraries
#
include_directories(${CMAKE_SOURCE_DIR}/src 
                    ${CMAKE_SOURCE_DIR}/thirdparty/sdl/include)
add_executable(dungeon MACOSX_BUNDLE ${srcs})
target_link_libraries(dungeon
    ${Boost_FILESYSTEM_LIBRARY}
    ${Boost_SYSTEM_LIBRARY}
    ${SDL_LIBRARY}
    ${SDLIMAGE_LIBRARY}
    ${OPENGL_LIBRARIES}
    ${EXTRA_LIBS}
    SDLmain                 # apparently not included on some systems?
)
