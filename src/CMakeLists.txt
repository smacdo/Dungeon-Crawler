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
        graphics/spriteloader.cpp
        graphics/clientview.cpp
)

set(graphics_headers
        graphics/clientview.h
        graphics/sprite.h
        graphics/spriteloader.h
        graphics/spritemanager.h
)

set(engine_srcs
        engine/camera.cpp
)

set(engine_headers
        engine/camera.h
)

set(game_srcs
        game/dungeon.cpp
        game/level.cpp
        game/tile.cpp
        game/tilefactory.cpp
        game/tilegrid.cpp
        game/pathfinder.cpp
        game/world.cpp
)

set(game_headers
        game/dungeon.h
        game/level.h
        game/tile.h
        game/tilefactory.h
        game/tiledata.h
        game/tileflags.h
        game/tilegrid.h
        game/pathfinder.h
        game/world.h
)

set(app_srcs
        appconfig.cpp
        dungeoncrawler.cpp
        inputmanager.cpp
)

set(app_headers
        appconfig.h
        config.h
        dungeoncrawler.h
        inputmanager.h
)

###########################################################################
# Main build script
###########################################################################
set(srcs
        ${app_srcs}
        ${common_srcs}
        ${engine_srcs}
        ${game_srcs}
        ${graphics_srcs}
        ${worldgen_srcs}
)

set(cxx_flags "")

###
### Visual Studio (or other IDEs) file groupings
###
source_group( app\\src          FILES ${app_srcs}         )
source_group( app\\headers      FILES ${app_headers}      )
source_group( common\\src       FILES ${common_srcs}      )
source_group( common\\headers   FILES ${common_headers}   )
source_group( engine\\src       FILES ${common_srcs}      )
source_group( engine\\headers   FILES ${engine_headers}   )
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
find_package(OpenGL REQUIRED)
#find_package(SDL2 REQUIRED)

include_directories(
    ${Boost_INCLUDE_DIRS}
    ${THIRDPARTY_ROOT}/tinyxml/include
)
link_directories( /usr/local/lib )

# on OS X we also have to add '-framework Cocoa' as library.  This is
# # actually a bit of an hack but it's easy enough and reliable.
set(EXTRA_LIBS )

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
    SDL2 SDL2_image SDL2_ttf
    tinyxml
    ${EXTRA_LIBS}
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
        COMMENT "Start the game"
    )

    add_custom_target(debug
        gdb `pwd`/dungeon -d ${PROJECT_SOURCE_DIR}
        WORKING_DIRECTORY ${GAME_PATH}
        DEPENDS dungeon
        COMMENT "Runs the game with GDB support"
    )

    add_custom_target(doxygen
        doxygen ${CMAKE_CURRENT_BINARY_DIR}/Doxyfile
        WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}
        COMMENT "Generates doxygen documentation"
    )
endif()

