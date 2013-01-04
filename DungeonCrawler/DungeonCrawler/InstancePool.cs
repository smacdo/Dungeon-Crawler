using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon
{
    /// <summary>
    /// Instance pool
    /// TODO: Make this more efficient... it currently allocates a new node each time an
    /// object is retrieved from the pool. Make it so that the linked list nodes can be
    /// re-used.
    /// </summary>
    public class InstancePool<T> : IEnumerable<T>
        where T : new()
    {
        private int mCount;

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
