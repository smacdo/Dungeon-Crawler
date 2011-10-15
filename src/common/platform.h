#ifndef SCOTT_DUNGEON_COMMON_PLATFORM_H
#define SCOTT_DUNGEON_COMMON_PLATFORM_H

#include <string>
#define ASSERTS_ENABLED

#ifdef MSVC
#   define app_break __debugbreak()
#else
#   define app_break __builtin_trap()
#endif

#ifdef ASSERTS_ENABLED
#   define scott_assert(msg,cond)      \
        do                             \
        {                              \
            if ( !(cond) )             \
            {                          \
                if ( App::raiseAssertion(msg,#cond,__FILE__,__LINE__) == \
                     App::AssertHalt ) \
                    app_break;         \
            }                          \
        } while( 0 )

    // Useful short hand asserts
#   define assert_null(var)    scott_assert("Pointer is not null",#var##" == NULL")
#   define assert_notNull(var) scott_assert("Pointer is null",#var##" != NULL")

    // Replace standard assert
#   ifdef assert
#       undef assert
#   endif
#   define assert(x) scott_assert(NULL,x)

#else
#   define scott_custom_assert(x) \
        do { (void)sizeof(x); } while(0)
#endif

namespace App
{
    enum EAssertionStatus
    {
        AssertHalt = 0,
        AssertContinue = 1
    };

    void exit( unsigned int code = 1 );

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
}

void appRaiseError( const std::string& message );

#endif
