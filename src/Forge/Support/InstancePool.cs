/*
 * Copyright 2012-2015 Scott MacDonald
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

namespace Forge.Support
{
    /// <summary>
    ///  Creates an efficient collection of pre-allocated objects for taking and returning to the pool.
    /// </summary>
    /// <remarks>
    ///  TODO: Make this more efficient... it currently allocates a new node each time an object is retrieved from the
    ///  pool. Let's try to get all operations with the instance pool to never allocate, which might require a custom
    ///  linked list implementation.
    /// </remarks>
    public class InstancePool<T> : IEnumerable<T>
        where T : IRecyclable, new()
    {
        /// <summary>
        ///  Get the total number of pre-allocated objects from this instance pool.
        /// </summary>
        public int TotalCount
        {
            get
            {
                return mCount;
            }
        }

        /// <summary>
        ///  Get the number of objects that are ready to be taken.
        /// </summary>
        public int FreeCount
        {
            get
            {
                return mFreeList.Count;
            }
        }

        /// <summary>
        ///  Get umber of objects that have been taken for use.
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
        ///  Constructor.
        /// </summary>
        /// <param name="count">Number of objects to pre-allocate when creating this instance pool.</param>
        public InstancePool(int count)
        {
            mCount = count;

            mFreeList = new Queue<T>(count);
            mActiveList = new LinkedList<T>();
            mActiveNodeTable = new Dictionary<T, LinkedListNode<T>>(count);

            // Allocate all of the instances and put them into our pool
            for (int index = 0; index < mCount; ++index)
            {
                var instance = new T();
                mFreeList.Enqueue(instance);
            }
        }

        /// <summary>
        ///  Get an unused pre-allocated object from the pool.
        /// </summary>
        /// <remarks>
        ///  This method takes an instance from the pool, marks it as being in-use and returns it. The caller is
        ///  responsible for ensuring that the instance is returned to the pool after it is finished being used.
        ///  If this does not happen then the instance pool will leak instances.
        /// </remarks>
        public T Take()
        {
            var instance = default(T);

            // Grab a free instance from the pool of instances.
            if ( mFreeList.Count > 0 )
            {
                // Get a free instance.
                instance = mFreeList.Dequeue();

                // Move the taken instance to the active list, and record the object's location in the linked list.
                var node = mActiveList.AddLast(instance);
                mActiveNodeTable[instance] = node;
            }
            else
            {
                throw new InvalidOperationException("No more free instances are available for allocation from this pool");
            }

            // Make sure the instance has been "recycled" so it is starting from a clean state.
            instance.Recycle();

            // Let the caller have it.
            return instance;
        }

        /// <summary>
        ///  Return an instance to the pool to make it available for future use.
        /// </summary>
        /// <remarks>
        ///  All objects taken from this pool must eventually be returned for the pool, otherwise the instance pool will
        ///  leaks instances. Additionally, only objects taken from this pool can be returned to it. Attempting to
        ///  return other objects will throw an exception.
        /// </remarks>
        /// <param name="instance">The instance to return.</param>
        public void Return(T instance)
        {
            // Take the instance being returned and find it's position in the active list. This gives the method the
            // chance to properly remove it.
            LinkedListNode<T> node = null;

            try
            {
                node = mActiveNodeTable[instance];
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentException("The object was not allocated from this pool", ex);
            }

            // Ensure the object was not already returned to this pool.
            if (node == null)
            {
                throw new ArgumentException("The object was already returned to the pool");
            }

            // Remove the node from the active list.
            mActiveNodeTable[instance] = null;
            mActiveList.Remove(node);
           
            // Make sure the node is placed back in the free list.
            mFreeList.Enqueue(instance);
        }

        /// <summary>
        ///  Returns an enumerator that contains all of the active instances in the pool.
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
