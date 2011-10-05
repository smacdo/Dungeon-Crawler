###########################################################################
### Dungeon CMake buildscript for crossplatform Linux/POSIX systems      ##
###########################################################################
# Project source code files
set(srcs bsplevelgen.cpp 
         level.cpp       
         room.cpp        
         utils.cpp       
         dungeon.cpp     
         rect.cpp        
         roomgenerator.cpp
)

# Compiler definitions
ADD_DEFINITIONS(-g -W -Wall -Wextra -Werror)

#
# required packages
#
find_package(Boost COMPONENTS system filesystem REQUIRED)
find_package(SDL REQUIRED)
find_package(SDL_image REQUIRED)

#
# Generate the executable and link required libraries
#
add_executable(dungeon MACOSX_BUNDLE ${srcs})
target_link_libraries(dungeon
    ${Boost_FILESYSTEM_LIBRARY}
    ${Boost_SYSTEM_LIBRARY}
    ${SDL_LIBRARY}
    ${SDLIMAGE_LIBRARY}
    SDLmain                 # apparently not included on some systems?
)
