###########################################################################
### Dungeon CMake buildscript for crossplatform Linux/POSIX systems      ##
###########################################################################
project(DungeonCrawler)

#==========================================================================
#  Dungeon Crawler source code
#==========================================================================
set(common_srcs
        common/logging.cpp
        common/platform.cpp
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
        worldgen/worldgenerator.cpp
)

set(worldgen_headers
        worldgen/dungeongenerator.h
        worldgen/hallgenerator.h
        worldgen/levelgenerator.h
        worldgen/roomgenerator.h
        worldgen/worldgenerator.h
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
        game/actor.cpp
        game/actorcontroller.cpp
        game/gameplayengine.cpp
        game/dungeon.cpp
        game/level.cpp
        game/tile.cpp
        game/tilefactory.cpp
        game/tilegrid.cpp
        game/tiletype.cpp
        game/pathfinder.cpp
        game/playerinputcontroller.cpp
        game/world.cpp
)

set(game_headers
        game/actor.h
        game/actorcontroller.h
        game/gameplayengine.h
        game/dungeon.h
        game/level.h
        game/tile.h
        game/tilefactory.h
        game/tileflags.h
        game/tilegrid.h
        game/tiletype.h
        game/pathfinder.h
        game/playerinputcontroller.cpp
        game/world.h
)

set(app_srcs
        appconfig.cpp
        dungeoncrawler.cpp
        inputmanager.cpp
)

set(app_headers
        appconfig.h
        dungeoncrawler.h
        inputmanager.h
        version.h.in
        version.h
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
        ${THIRDPARTY_ROOT}/googletest/googletest-all.cpp
)

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

#
# Required packages
#
find_package(Boost COMPONENTS system filesystem program_options REQUIRED)
find_package(OpenGL REQUIRED)
find_package(SDL2 REQUIRED)
find_package(SDL2_image REQUIRED)
find_package(SDL2_ttf REQUIRED)

add_definitions(-DTIXML_USE_STL)

#
# set up configuration variables
#
configure_file( ${CMAKE_CURRENT_SOURCE_DIR}/version.h.in
                ${CMAKE_CURRENT_SOURCE_DIR}/version.h )

#
# Make sure compile flags are applied to the source code files
#
set_source_files_properties( ${srcs}
    PROPERTIES
    COMPILE_FLAGS "${cxx_flags}"
)

#
# Generate the executable and link required libraries
#
include_directories(
    ${CMAKE_SOURCE_DIR}/src
    ${Boost_INCLUDE_DIRS}
    ${THIRDPARTY_ROOT}/tinyxml/include
    ${THIRDPARTY_ROOT}/googletest/include
    ${SDL2_INCLUDE_DIR}
)

add_executable( dungeon MACOSX_BUNDLE ${srcs})
enable_extra_compiler_warnings( dungeon )

target_link_libraries( dungeon
    tinyxml
    ${Boost_LIBRARIES}
    ${SDL2_LIBRARY}
    ${SDL2IMAGE_LIBRARY}
    ${SDL2TTF_LIBRARY}
    ${OPENGL_LIBRARIES}
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
        WORKING_DIRECTORY ${GAME_OUTPUT_PATH}
        COMMENT "Start the game"
    )

    add_custom_target(debug
        gdb `pwd`/dungeon -d ${PROJECT_SOURCE_DIR}
        WORKING_DIRECTORY ${GAME_OUTPUT_PATH}
        DEPENDS dungeon
        COMMENT "Runs the game with GDB support"
    )
endif()

include(DoxygenTargets)
add_doxygen( ${PROJECT_SOURCE_DIR}/Doxyfile
             OUTPUT_DIRECTORY ${GAME_OUTPUT_PATH}/doc
             PROJECT_NUMBER "${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATH}"
             DOC_TARGET doxygen
             NO_WARNINGS
             NO_PDF )
             
