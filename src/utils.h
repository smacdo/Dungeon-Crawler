#ifndef SCOTT_DUNGEON_UTILS_H
#define SCOTT_DUNGEON_UTILS_H

#include <cassert>

template<typename T>
inline const T& deref( const T* ptr )
{
    assert( ptr != NULL );
    return *ptr;
}

template<typename T>
inline T& deref( T* ptr )
{
    assert( ptr != NULL );
    return *ptr;
}

namespace Utils
{
    int random( int min, int max );
}
#endif
