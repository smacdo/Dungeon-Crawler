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

set(engine_srcs
        engine/actor.cpp
        engine/actordata.cpp
        engine/appconfig.cpp
        engine/camera.cpp
        engine/optionsparser.cpp
        engine/playerinputcontroller.cpp
)

set(engine_headers
        engine/actor.h
        engine/actordata.h
        engine/appconfig.h
        engine/camera.h
        engine/optionsparser.h
        engine/playerinputcontroller.h
)

set(game_srcs
        game/gameplayengine.cpp
        game/dungeon.cpp
        game/level.cpp
        game/tile.cpp
        game/tilefactory.cpp
        game/tilegrid.cpp
        game/tiletype.cpp
        game/pathfinder.cpp
        game/world.cpp
)

set(game_headers
        game/gameplayengine.h
        game/dungeon.h
        game/level.h
        game/tile.h
        game/tilefactory.h
        game/tileflags.h
        game/tilegrid.h
        game/tiletype.h
        game/pathfinder.h
        game/world.h
)

set(client_srcs
        dungeoncrawler.cpp
        inputmanager.cpp
		client/dungeonclientwindow.cpp
        client/spritemanager.cpp
        client/sprite.cpp
        client/spritedata.cpp
        client/spriteloader.cpp
        client/clientview.cpp
)

set(client_headers
        dungeoncrawler.h
        inputmanager.h
        version.h.in
        version.h
		client/dungeonclientwindow.h
        client/clientview.h
        client/sprite.h
        client/spritedata.h
        client/spriteloader.h
        client/spritemanager.h
)

set(client_ui
		client/dungeonclientwindow.ui
)

###########################################################################
# Main build script
###########################################################################
set(srcs
        ${client_srcs}
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
source_group( client\\src       FILES ${client_srcs}      )
source_group( client\\headers   FILES ${client_headers}   )
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

#=======================================================================#
# libboost support                                                      #
#=======================================================================#
SET(Boost_ADDITIONAL_VERSIONS "1.48")
set(Boost_USE_STATIC_LIBS ON)
#set(Boost_DEBUG ON)

find_package(Boost COMPONENTS system filesystem program_options REQUIRED)

message("BOOST: ${Boost_LIBRARIES}")

#=======================================================================#
# Qt4 support                                                           #
#=======================================================================#
find_package(Qt4 REQUIRED)
include(${QT_USE_FILE})
add_definitions(${QT_DEFINITIONS})

set(QT_USE_QTOPENGL TRUE)
set(QT_USE_QTXML TRUE)
set(QT_USE_QTSCRIPT TRUE)

QT4_WRAP_CPP(client_HEADERS_MOC ${client_headers})
QT4_WRAP_UI(client_FORMS_HEADERS ${client_ui})
QT4_ADD_RESOURCES(client_RESOURCES_RCC "dungeoncrawler.qrc")

#
# set up configuration variables
#
configure_file( ${CMAKE_CURRENT_SOURCE_DIR}/version.h.in
                ${CMAKE_CURRENT_SOURCE_DIR}/version.h )

#
# Generate the executable and link required libraries
#
include_directories(
	${CMAKE_CURRENT_BINARY_DIR}
    ${CMAKE_SOURCE_DIR}/src
    ${Boost_INCLUDE_DIRS}
    ${THIRDPARTY_ROOT}/tinyxml/include
    ${THIRDPARTY_ROOT}/googletest/include
)

add_executable( dungeon MACOSX_BUNDLE
	${srcs}
	${client_HEADERS_MOC}
	${client_FORMS_HEADERS}
	${client_RESOURCES_RCC}
)
enable_extra_compiler_warnings( dungeon )

target_link_libraries( dungeon
    tinyxml
	${QT_LIBRARIES}
    ${Boost_LIBRARIES}
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
add_doxygen( Doxyfile
             OUTPUT_DIRECTORY doc
             PROJECT_NUMBER "${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_PATH}"
             DOC_TARGET doxygen
             NO_WARNINGS
             NO_PDF )
             
