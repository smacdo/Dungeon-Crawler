###########################################################################
# Google Test custom cmake file
###########################################################################
project(gtest)

# Special flags for Visual C compiler
#  (these were taken from Google's cmake script
if(MSVC)
    set(cxx_flags "${cxx_flags} -GS -W4 -WX")
    set(cxx_flags "${cxx_flags} -wd4127 -wd4251 -wd4275 -nologo -J -Zi")
    set(cxx_flags "${cxx_flags} -DSTRICT -DWIN32_LEAN_AND_MEAN")
endif()

include_directories(include)

# Create a static library that the project can use called "gtest"
add_library(gtest STATIC googletest-all.cpp)

# Forcibly disable threading for the moment. This will re-added later on
# once tests actually need threading
add_definitions(-DGTEST_HAS_PTHREAD=0)
