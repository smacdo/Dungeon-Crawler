###########################################################################
### Dungeon CMake buildscript for crossplatform Linux/POSIX systems      ##
###########################################################################
project(DungeonCrawler)

#==========================================================================
#  Dungeon Crawler source code
#==========================================================================
set(common_srcs
        common/logging.cpp
        common/platform_unix.cpp
        common/point.cpp
        common/random.cpp
        common/rect.cpp
)

set(common_headers
        common/fixedgrid.h
        common/logging.h
        common/platform.h
        common/point.h
        common/random.h
        common/rect.h
        common/sdlinclude.h
        common/utils.h
)

set(worldgen_srcs
        worldgen/levelgenerator.cpp
        worldgen/dungeongenerator.cpp
        worldgen/roomgenerator.cpp
        worldgen/hallgenerator.cpp
)

set(worldgen_headers
        worldgen/dungeongenerator.h
        worldgen/hallgenerator.h
        worldgen/levelgenerator.h
        worldgen/roomgenerator.h
)

set(graphics_srcs 
        graphics/spritemanager.cpp
        graphics/sprite.cpp
        graphics/clientview.cpp
)

set(graphics_headers
        graphics/clientview.h
        graphics/sprite.h
        graphics/spritemanager.h
)

set(game_srcs
        appconfig.cpp
        camera.cpp
        dungeon.cpp
        dungeoncrawler.cpp
        inputmanager.cpp
        level.cpp
        tile.cpp
        tilegrid.cpp
        world.cpp
)

set(game_headers
        appconfig.h
        camera.h
        config.h
        door.h
        dungeon.h
        dungeoncrawler.h
        inputmanager.h
        level.h
        tilegrid.h
        tile.h
        tiletype.h
        world.h
)

###########################################################################
# Main build script
###########################################################################
set(srcs ${common_srcs} ${worldgen_srcs} ${game_srcs} ${graphics_srcs})
set(cxx_flags "")

###
### Visual Studio (or other IDEs) file groupings
###
source_group( common\\src       FILES ${common_srcs}      )
source_group( common\\headers   FILES ${common_headers}   )
source_group( gameplay\\src     FILES ${game_srcs}        )
source_group( gameplay\\headers FILES ${game_headers}     )
source_group( graphics\\src     FILES ${graphics_srcs}    )
source_group( graphics\\headers FILES ${graphics_headers} )
source_group( worldgen\\src     FILES ${worldgen_srcs}    )
source_group( worldgen\\headers FILES ${worldgen_headers} )
###
### Additional compiler settings and definitions
###
if(COMPILER_GCC)
    set(cxx_flags "${cxx_flags} -g -W -Wall -Werror -Wextra -std=c++0x")
    set(cxx_flags "${cxx_flags} -Wno-unused-parameter")
endif()

if(MSVC)
    set(cxx_flags "${cxx_flags} /Wp64")
    add_definitions(-D_CRT_SECURE_NO_DEPRECATE)
    add_definitions(-DWIN32_LEAN_AND_MEAN)
endif()

###
### Required packages
###
find_package(Boost COMPONENTS system filesystem program_options REQUIRED)
find_package(SDL REQUIRED)
find_package(SDL_image REQUIRED)

include_directories(${Boost_INCLUDE_DIRS})
# find_package(OpenGL REQUIRED)

# on OS X we also have to add '-framework Cocoa' as library.  This is
# # actually a bit of an hack but it's easy enough and reliable.
set(EXTRA_LIBS "")
if (APPLE)
    set(EXTRA_LIBS ${EXTRA_LIBS} "-framework Cocoa -framework SDL")

    # I really do not like building on the mac
    add_library( SDLmain SDLMain.m)
    set_target_properties( SDLmain PROPERTIES COMPILE_FLAGS "" )
endif()


###
### Make sure compile flags are applied to the source code files
###
set_source_files_properties( ${srcs}
    PROPERTIES
    COMPILE_FLAGS "${cxx_flags}"
)

###
### Generate the executable and link required libraries
###
include_directories(${CMAKE_SOURCE_DIR}/src 
                    ${CMAKE_SOURCE_DIR}/thirdparty/sdl/include)
add_executable(dungeon MACOSX_BUNDLE ${srcs})
target_link_libraries(dungeon
    ${Boost_LIBRARIES}
    ${EXTRA_LIBS}
    ${SDL_LIBRARY}
    ${SDLIMAGE_LIBRARY}
    ${OPENGL_LIBRARIES}
    SDLmain                 # apparently not included on some systems?
)

set_target_properties( dungeon
    PROPERTIES
    PROJECT_LABEL "DungeonCrawler-Client"
)

###
### Useful targets
###
if(NOT MSVC)
    add_custom_target(
        run dungeon DEPENDS dungeon
        WORKING_DIRECTORY ${GAME_PATH}
        COMMENT "Starts the game"
    )

    add_custom_target(debug
        gdb `pwd`/dungeon -d ${PROJECT_SOURCE_DIR}/src
        ${GAME_PATH}
        DEPENDS dungeon
        COMMENT "Runs the game with gdb support"
    )

    add_custom_target(doxygen
        doxygen ${CMAKE_CURRENT_BINARY_DIR}/Doxyfile
        WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}
        COMMENT "Generates doxygen documentation"
    )
endif()

