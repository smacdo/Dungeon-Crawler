###########################################################################
### Dungeon Crawler build script                                        ###
###  - Set CMAKE_BUILD_TYPE to Debug, Release, RelWithDebInfo           ###
###########################################################################
cmake_minimum_required(VERSION 2.8)

# Load extra special cmake modules
list(APPEND CMAKE_MODULE_PATH "${CMAKE_SOURCE_DIR}/cmake")

include( config )
include( shared )
include( GetGitRevisionDescription )
include( GetCompilerInfoString )
include( EnableExtraCompilerWarnings )

# Load the current revision for this project
get_git_head_revision(GIT_REFSPEC GIT_SHA1)

# Load the current compiler's id
get_compiler_info_string(COMPILER_ID)

### Version and build information
set(GAME_NAME    "Dungeon Crawler")
set(GAME_AUTHOR  "Scott MacDonald")
set(GAME_EMAIL   "scott@whitespaceconsideredharmful.com")
set(GAME_WEBSITE "http://www.whitespaceconsideredharmful.com/dungeoncrawler")

set(VERSION_MAJOR 0)
set(VERSION_MINOR 1)
set(VERSION_PATCH 2)
set(VERSION_RELEASE development)

### Additional settings
set(GAME_OUTPUT_PATH       "${CMAKE_CURRENT_BINARY_DIR}")
set(GAME_DATA_ROOT         "${PROJECT_SOURCE_DIR}/game")
set(GAME_SRC_PATH          "${CMAKE_SOURCE_DIR}/src")
set(THIRDPARTY_ROOT        "${PROJECT_SOURCE_DIR}/thirdparty")

# Take this out once we start detecting compilers for C++0x support
add_definitions(-std=c++0x)
add_definitions(-TIXML_USE_STL)

# Special  XCode (v4.0) Support
if(MACOSX)
    set(CMAKE_OSX_ARCHITECTURES "i386;x86_64")
    set(CMAKE_OSX_SYSROOT "/Developer/SDKs/MacOSX10.6.sdk")
    set(CMAKE_OSX_DEPLOYMENT_TARGET "10.6")

    # This seems like a better way of setting XCode attributes
    set(CMAKE_XCODE_ATTRIBUTE_GCC_VERSION "com.apple.compilers.llvm.clang.1_0")
    set(CMAKE_XCODE_ATTRIBUTE_CLANG_CXX_LANGUAGE_STANDARD "c++0x")
    set(CMAKE_XCODE_ATTRIBUTE_CLANG_CXX_LIBRARY "libc++")

    # Might not need this line?
    set(CMAKE_CXX_FLAGS "${CMAKE_C_FLAGS} -std=c++0x -stdlib=libc++")
endif()

### Allow code to build
add_subdirectory(thirdparty)
add_subdirectory(src)
add_subdirectory(game)
add_subdirectory(tests)
