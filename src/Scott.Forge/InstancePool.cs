﻿/*
 * Copyright 2012-2014 Scott MacDonald
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
using System;
using System.Collections.Generic;

namespace Scott.Forge
{
    /// <summary>
    /// Instance pool
    /// TODO: Make this more efficient... it currently allocates a new node each time an
    /// object is retrieved from the pool. Make it so that the linked list nodes can be
    /// re-used.
    /// </summary>
    public class InstancePool<T> : IEnumerable<T>
        where T : IRecyclable, new()
    {
        /// <summary>
        ///  Total number of objects in the instance pool.
        /// </summary>
        public int TotalCount
        {
            get
            {
                return mCount;
            }
        }

        /// <summary>
        ///  Number of objects that are available for use.
        /// </summary>
        public int FreeCount
        {
            get
            {
                return mFreeList.Count;
            }
        }

        /// <summary>
        ///  Number of objects that are in use.
        /// </summary>
        public int ActiveCount
        {
            get
            {
                return mActiveList.Count;
            }
        }

        private int mCount = 0;

        private Queue<T> mFreeList;
        private LinkedList<T> mActiveList;
        private Dictionary<T, LinkedListNode<T>> mActiveNodeTable;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="count">Number of instances available to allocate</param>
        public InstancePool( int count )
        {
            Allocate( count );
        }

        /// <summary>
        /// Allocates the instance pool, and sets up the instance mapping
        /// </summary>
        private void Allocate( int count )
        {
            mCount = count;

            mFreeList = new Queue<T>( count );
            mActiveList = new LinkedList<T>();
            mActiveNodeTable = new Dictionary<T, LinkedListNode<T>>( count );

            // Allocate all of the instances and put them into our pool
            for ( int index = 0; index < mCount; ++index )
            {
                T instance = new T();
                mFreeList.Enqueue( instance );
            }
        }

        /// <summary>
        /// Retrieves an unused object instance from the pool
        /// </summary>
        public T Take()
        {
            T instance = default(T);

            // Grab a free instance from the pool of instances
            if ( mFreeList.Count > 0 )
            {
                // Get a free instance
                instance = mFreeList.Dequeue();

                // Move the taken instance to the active list, and record the object's
                // location in the linked list
                LinkedListNode<T> node     = mActiveList.AddLast( instance );
                mActiveNodeTable[instance] = node;
            }
            else
            {
                throw new OverflowException( "No more free instances are available for allocation from this pool" );
            }

            // Make sure the instance has been "recycled" so it is starting from a clean state
            instance.Recycle();

            // Let the caller have it
            return instance;
        }

        /// <summary>
        /// Returns an instance to the pool so that we can hand it out again later
        /// </summary>
        /// <param name="instance">The instance to recycle</param>
        public void Return( T instance )
        {
            // Take the instance being returned and find it's position in the active list
            // (so we can remove it properly).
            LinkedListNode<T> node = null;

            try
            {
                node = mActiveNodeTable[instance];
            }
            catch ( Exception ex )
            {
                throw new ArgumentException( "The object was not allocated from this pool", ex );
            }

            // Ensure the object was not already returned to this pool
            if ( node == null )
            {
                throw new ArgumentException( "The object was already returned to the pool" );
            }

            // Remove it from the active list
            mActiveNodeTable[instance] = null;
            mActiveList.Remove( node );
           
            // Make sure it's back in the free list
            mFreeList.Enqueue( instance );
        }

        /// <summary>
        /// Returns an enumerator that contains all of the active instances in the pool.
        /// </summary>
        /// <returns>Enumerator over all active instances in the pool</returns>
        public LinkedList<T>.Enumerator GetActiveListEnumerator()
        {
            return mActiveList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that contains all of the active instances in the pool.
        /// </summary>
        /// <returns>Enumerator over all active instances in the pool</returns>
        public LinkedList<T>.Enumerator GetEnumerator()
        {
            return mActiveList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that contains all of the active instances in the pool.
        /// </summary>
        /// <returns>Enumerator over all active instances in the pool</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return mActiveList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that contains all of the active instances in the pool.
        /// </summary>
        /// <returns>Enumerator over all active instances in the pool</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mActiveList.GetEnumerator();
        }
    }
}