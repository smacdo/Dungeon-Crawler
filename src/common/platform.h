/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#ifndef SCOTT_DUNGEON_PLATFORM_H
#define SCOTT_DUNGEON_PLATFORM_H

#include <string>
#define ASSERTS_ENABLED 1

#define STRINGIFY(x) #x

/////////////////////////////////////////////////////////////////////////////
// Platform enumerations
/////////////////////////////////////////////////////////////////////////////
enum EProgramStatus
{
    EPROGRAM_OK = 0,
    EPROGRAM_ASSERT_FAILED = 1,
    EPROGRAM_FATAL_ERROR   = 2,
    EPROGRAM_USER_ERROR    = 3
};

/////////////////////////////////////////////////////////////////////////////
// Platform detection and flag setting
/////////////////////////////////////////////////////////////////////////////
#if defined(_WIN32) || defined(__WIN32__)
#   if defined(_MSC_VER)
#       define OS_PLATFORM windows
#       define OS_WINDOWS  1
#       define USE_WINDOWS_VISTA 1
#   else
#       error "Non Visual Studio compilers not supported at the moment"
#   endif
#elif defined(__GNUG__)
#   define OS_PLATFORM posix
#   define OS_POSIX 1
#else
#   error "This platform isn't supported yet"
#endif

/////////////////////////////////////////////////////////////////////////////
// Platform specific debug trigger
/////////////////////////////////////////////////////////////////////////////
#if defined(_WIN32)
#   define app_break __debugbreak()
#else
#   define app_break __builtin_trap()
#endif

/////////////////////////////////////////////////////////////////////////////
// Custom assertion handling
/////////////////////////////////////////////////////////////////////////////
#ifdef ASSERTS_ENABLED
#   define scott_assert(msg,cond)           \
        do                                  \
        {                                   \
            if ( !(cond) )                  \
            {                               \
                if ( App::raiseAssertion(msg,#cond,__FILE__,__LINE__) == \
                     App::EAssertion_Halt ) \
                    app_break;              \
            }                               \
        } while( 0 )

# define assert_null(var) scott_assert("Pointer was expected to be null",#var##" == NULL")
# define assert_notNull(var) scott_assert("Pointer was expected to be non-null",#var##" != NULL")

    // Replace standard assert
#   ifdef assert
#       undef assert
#   endif
#   define assert(x) scott_assert(NULL,x)

#else
#   define scott_custom_assert(msg, x) \
        do { (void)sizeof(msg); (void)sizeof(x); } while(0)
#   define assert_null(x) \
    do { (void)sizeof(x); } while(0)
#   define assert_notNull(x) \
    do { (void)sizeof(x); } while(0)
#endif

/////////////////////////////////////////////////////////////////////////////
// Internal application utility functions
/////////////////////////////////////////////////////////////////////////////
namespace App
{
    enum EAssertionStatus
    {
        EAssertion_Halt = 0,
        EAssertion_Continue = 1
    };

    // Performs any needed platform specific work before starting the game
    void startup();
    void quit( EProgramStatus quitStatus, const std::string& message );

    EAssertionStatus raiseAssertion( const char* message,
                                     const char* expression,
                                     const char* filename,
                                     unsigned int linenumber );

    void raiseError( const std::string& message,
                     const std::string& details = "" );

    void raiseFatalError( const std::string& message,
                          const std::string& details = "" );

    // Returns a "build string", which is a long string containing information
    // about the settings under which the game was built
    std::string getBuildString();
}

#endif
