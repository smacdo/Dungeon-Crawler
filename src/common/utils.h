/*
 * Copyright 2012 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#ifndef SCOTT_DUNGEON_COMMON_UTILS_H
#define SCOTT_DUNGEON_COMMON_UTILS_H

/////////////////////////////////////////////////////////////////////////////
// Includes
/////////////////////////////////////////////////////////////////////////////
#include <map>
#include <vector>
#include <memory>
#include <boost/checked_delete.hpp>
#include <algorithm>

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

    for ( itr = container.begin(); itr != container.end(); std::advance(itr,1) )
    {
        Delete<typename T::value_type>( *itr );
    }

    container.clear();
}

/**
 * Deletes an STL container of vectors, and resizes the container to zero.
 *
 * \param  container  A STL vector holding pointers
 */
template<typename T>
void DeleteVectorPointers( std::vector<T>& container )
{
    std::for_each( container.begin(), container.end(), Delete<T> );
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
 * Returns a non-null reference that refers to the given pointer's address,
 * after performing a checking that the pointer passed is not null.
 *
 * \param  ptr  The constant pointer to dereference
 * \return      A constant reference to the pointer's address
 */
template<typename T>
inline const T& deref( const T* ptr )
{
    assert( ptr != NULL );
    return *ptr;
}

/**
 * Returns a non-null reference that refers to the given pointer's address,
 * after performing a checking that the pointer passed is not null.
 *
 * \param  ptr  The pointer to dereference
 * \return      A reference to the pointer's address
 */
template<typename T>
inline T& deref( T* ptr )
{
    assert( ptr != NULL );
    return *ptr;
}

/**
 * Returns a non-null reference that refers to the given pointer's address,
 * after performing a checking that the pointer passed is not null.
 *
 * \param  sharedPtr  The shared pointer to dereference
 * \return            A reference to the pointer's address
 */
template<typename T>
inline T& deref( std::shared_ptr<T> sharedPtr )
{
    T* ptr = sharedPtr.get();
    assert( ptr != NULL );

    return *ptr;
}

#endif
