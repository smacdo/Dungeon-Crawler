###
### Dungeon Crawler unit tests
###

### List of unit test files
set(test_srcs
        test_alwaystrue.cpp
)

### List of required files from the game
set(test_req_srcs
)

###########################################################################
### Build script
###########################################################################
project(DungeonCrawler-Tests)

include_directories(
    ${PROJECT_SOURCE_DIR}/src
    ${GTEST_ROOT}/include
)

add_executable(
    test-dungeoncrawler
    ${test_srcs}
    ${test_req_srcs}
    ${GTEST_ROOT}/googletestrunner.cpp
)

target_link_libraries(test-dungeoncrawler gtest)

add_custom_target(tests ALL test-dungeoncrawler
                  DEPENDS test-dungeoncrawler
                  WORKING_DIRECTORY ${GAME_PATH}
                  COMMENT Runs dungeon crawler unit tests)

