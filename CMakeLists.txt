cmake_minimum_required(VERSION 2.6)
project(DungeonCrawler)

set(GAME_PATH "${CMAKE_SOURCE_DIR}/game")
set(DESTDIR   "${GAME_PATH}")
set(EXECUTABLE_OUTPUT_PATH "${GAME_PATH}")
set(LIBRARY_OUTPUT_PATH "${GAME_PATH}")

message( "Game executables and libraries will be in: ${GAME_PATH}" )

add_subdirectory(src)
