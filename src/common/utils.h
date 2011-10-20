#ifndef SCOTT_DUNGEON_COMMON_UTILS_H
#define SCOTT_DUNGEON_COMMON_UTILS_H

/////////////////////////////////////////////////////////////////////////////
// Includes
/////////////////////////////////////////////////////////////////////////////
#include <map>
#include <vector>
#include <boost/checked_delete.hpp>

#include "common/platform.h"

/**
 * Deletes a pointer and then sets the address to NULL to prevent accidental
 * dereferences
 *
 * \param  pointer  The pointer to delete
 */
template<typename T>
void Delete( T& pointer )
{
    boost::checked_delete( pointer );
    pointer = NULL;
}

/**
 * Deletes an array pointer, and then sets the address to NULL to prevent
 * accidental dereferences
 *
 * \param  arrayPointer  The array pointer to delete
 */
template<typename T>
void DeleteArray( T& arrayPointer )
{
    boost::checked_array_delete( arrayPointer );
    arrayPointer = NULL;
}

/**
 * Deletes a generic container of pointer values by iterating through the
 * list, deleting each pointer element and then setting the size of the container
 * to zero.
 *
 * \param  container  A generic container holding pointers
 */
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

/**
 * Deletes an STL container of vectors, and resizes the container to zero.
 *
 * \param  container  A STL vector holding pointers
 */
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

/**
 * Deletes an STL map of containers, and resizes the container to zero.
 *
 * \param  container  A STL map holding pointers
 */
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


/**
 * Returns a reference to the pointer's address, after performing a check
 * to ensure the pointer is not null.
 *
 * \param  ptr  The pointer to dereference
 * \return Reference to the pointer's address
 */
template<typename T>
inline const T& deref( const T* ptr )
{
    assert( ptr != NULL );
    return *ptr;
}

/**
 * Returns a reference to the pointer's address, after performing a check
 * to ensure the pointer is not null.
 *
 * \param  ptr  The pointer to dereference
 * \return Reference to the pointer's address
 */
template<typename T>
inline T& deref( T* ptr )
{
    assert( ptr != NULL );
    return *ptr;
}

/////////////////////////////////////////////////////////////////////////////
// Utility functions that don't belong anywhere else go in here
/////////////////////////////////////////////////////////////////////////////
namespace Utils
{
    // Generate a random number in the range [min, max).
    int random( int min, int max );
}

#endif
