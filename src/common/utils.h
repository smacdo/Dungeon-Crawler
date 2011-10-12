#ifndef SCOTT_DUNGEON_COMMON_UTILS_H
#define SCOTT_DUNGEON_COMMON_UTILS_H

#include <map>
#include <vector>
#include <boost/checked_delete.hpp>

template<typename T>
void DeletePointerContainer( T& container )
{
    typename T::iterator itr;

    for ( itr = container.begin(); itr != container.end(); ++itr )
    {
        boost::checked_delete( *itr );
    }

    itr.clear();
}

template<typename T>
void DeleteVectorPointers( std::vector<T>& container )
{
    typename std::vector<T>::iterator itr;

    for ( itr = container.begin(); itr != container.end(); ++itr )
    {
        boost::checked_delete( *itr );
    }

    container.clear();
}

template<typename T, typename U>
void DeleteMapPointers( std::map<T,U>& container )
{
    typename std::map<T,U>::iterator itr;

    for ( itr = container.begin(); itr != container.end(); ++itr )
    {
        boost::checked_delete( itr->second );
    }

    container.clear();
}

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