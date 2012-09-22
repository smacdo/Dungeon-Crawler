###
### Dungeon Crawler unit tests
###
enable_testing()

### List of unit test files
set(test_srcs
        test_alwaystrue.cpp
        test_fixedgrid.cpp
        test_pathfinder.cpp
        test_point.cpp
        test_rect.cpp
        test_tilegrid.cpp
        test_utils.cpp
)

### List of required files from the game
set(test_req_srcs
        ${GAME_SRC_PATH}/common/rect.cpp
        ${GAME_SRC_PATH}/common/point.cpp
        ${GAME_SRC_PATH}/common/platform.cpp
        ${GAME_SRC_PATH}/common/platform_unix.cpp
        ${GAME_SRC_PATH}/game/pathfinder.cpp
        ${GAME_SRC_PATH}/game/tile.cpp
        ${GAME_SRC_PATH}/game/tilegrid.cpp
        ${GAME_SRC_PATH}/game/tiletype.cpp
)

###########################################################################
### Build script
###########################################################################
project(DungeonCrawler-Tests)

set(Boost_USE_STATIC_LIBS true)
set(Boost_DEBUG true)

find_package(Boost COMPONENTS system filesystem program_options REQUIRED)
find_package(Qt4 REQUIRED)

include_directories(
    ${GAME_SRC_PATH}
	${Boost_INCLUDE_DIRS}
    ${THIRDPARTY_ROOT}/googletest/include
)

add_executable(
    test-dungeoncrawler
    googletestrunner.cpp
    ${test_srcs}
    ${test_req_srcs}
)

target_link_libraries(test-dungeoncrawler gtest)

add_custom_target( tests test-dungeoncrawler
                   DEPENDS test-dungeoncrawler
                   WORKING_DIRECTORY ${GAME_OUTPUT_PATH}
                   COMMENT Run dungeon crawler unit tests)


add_test(
    NAME test-dungeoncrawler
    COMMAND test-dungeoncrawler )
