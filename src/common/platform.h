#ifndef SCOTT_DUNGEON_COMMON_PLATFORM_H
#define SCOTT_DUNGEON_COMMON_PLATFORM_H

#include "dungeoncrawler.h"
#include <string>
#define ASSERTS_ENABLED 1

#define STRINGIFY(x) #x

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

    void quit( EProgramStatus quitStatus, const std::string& message );

    EAssertionStatus raiseAssertion( const char* message,
                                     const char* expression,
                                     const char* filename,
                                     unsigned int linenumber );

    void raiseError( const std::string& message,
                     const char* filename = NULL,
                     unsigned int linenumber = 0 );

    void raiseFatalError( const std::string& message,
                          const char* filename = NULL,
                          unsigned int linenumber = 0 );

    // Returns a "build string", which is a long string containing information
    // about the settings under which the game was built
    std::string getBuildString();
}

void appRaiseError( const std::string& message );

#endif
