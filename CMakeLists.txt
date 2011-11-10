cmake_minimum_required(VERSION 2.8)

message( "WHAT IS ${CMAKE_CURRENT_BINARY_DIR}" )

###
### Set a default build type if there was none provided
###  - this has to be done before project()
###
if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Debug CACHE STRING "Choose the type of build (Debug or Release)" FORCE)
endif()

### Project name
#project(DungeonCrawler)


### CMake includes
set(CMAKE_MODULE_PATH "${CMAKE_SOURCE_DIR}/cmake")
include(${CMAKE_CURRENT_SOURCE_DIR}/cmake/config.cmake)
include(${CMAKE_CURRENT_SOURCE_DIR}/cmake/shared.cmake)

### Version information
set(VERSION_MAJOR 0)
set(VERSION_MINOR 1)
set(VERSION_PATCH 1)

### Additional settings
set(GAME_PATH              "${CMAKE_SOURCE_DIR}/game")
set(GAME_SRC_PATH          "${CMAKE_SOURCE_DIR}/src")
set(GTEST_ROOT             "${PROJECT_SOURCE_DIR}/thirdparty/googletest")
set(DESTDIR                "${GAME_PATH}")
set(EXECUTABLE_OUTPUT_PATH "${GAME_PATH}")
set(LIBRARY_OUTPUT_PATH    "${GAME_PATH}")

### Build options
set(BUILD_DOC FALSE CACHE BOOL "TRUE to generate API docs, FALSE otherwise")

### Mac OSX support
if(MACOSX)
    set(CMAKE_OSX_ARCHITECTURES "i386;x86_64")
    set(CMAKE_OSX_SYSROOT "/Developer/SDKs/MacOSX10.6.sdk")
    set(CMAKE_OSX_DEPLOYMENT_TARGET "10.6")
endif()

### Allow code to build
add_subdirectory(thirdparty)
add_subdirectory(src)
add_subdirectory(tests)
