###
### Dungeon Crawler unit tests
###

### List of unit test files
set(test_srcs
        test_alwaystrue.cpp
        test_fixedgrid.cpp
        test_point.cpp
        test_rect.cpp
        test_tilegrid.cpp
        test_utils.cpp
)

### List of required files from the game
set(test_req_srcs
        ${GAME_SRC_PATH}/common/rect.cpp
        ${GAME_SRC_PATH}/common/point.cpp
        ${GAME_SRC_PATH}/common/platform_unix.cpp
        ${GAME_SRC_PATH}/tile.cpp
        ${GAME_SRC_PATH}/tilegrid.cpp
)

###########################################################################
### Build script
###########################################################################
project(DungeonCrawler-Tests)

if(COMPILER_GCC)
    set(cxx_flags "${cxx_flags} -g -W -Wall -Werror -std=c++0x")
    set(cxx_flags "${cxx_flags} -Wno-unused-parameter")
endif()

set_source_files_properties( ${test_srcs} ${test_req_srcs}
    PROPERTIES
    COMPILE_FLAGS "${cxx_flags}"
    )

include_directories(
    ${GAME_SRC_PATH}
    ${GTEST_ROOT}/include
)

add_executable(
    test-dungeoncrawler
    googletestrunner.cpp
    ${test_srcs}
    ${test_req_srcs}
)

target_link_libraries(test-dungeoncrawler gtest)

add_custom_target(tests ALL test-dungeoncrawler
                  DEPENDS test-dungeoncrawler
                  WORKING_DIRECTORY ${GAME_PATH}
                  COMMENT Runs dungeon crawler unit tests)

