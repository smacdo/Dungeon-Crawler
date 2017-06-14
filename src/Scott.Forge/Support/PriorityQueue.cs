/*
 * Copyright 2017 Scott MacDonald
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Scott.Forge.Support
{
    /// <summary>
    ///  Priority queue is a min heap queue that uses a standard array for high performance operations.
    /// </summary>
    /// <remarks>
    ///  The priority queue uses a default minimum value ordering such that the smallest values are removed first.
    ///  Users of this class can create a custom comparer class that changes this ordering to their liking.
    /// </remarks>
    /// <typeparam name="TValue">Type of item stored in priority queue.</typeparam>
    [DataContract]
    [DebuggerDisplay("Count = {Count}")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class PriorityQueue<TValue, TScore> : IEnumerable<TValue> where TScore : IComparable<TScore>
    {
        private const int DefaultCapacity = 15; // Full binary tree of height 4.

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [DataMember(Name = "Heap", IsRequired = true, Order = 0)]
        private HeapNode[] mHeap;

        /// <summary>
        ///  Initialize a new instance of the priority queue class.
        /// </summary>
        public PriorityQueue()
            : this(DefaultCapacity)
        {
        }
        
        /// <summary>
        ///  Initialize a new instance of the priority queue class.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        public PriorityQueue(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Capacity cannot be less than zero", nameof(capacity));
            }

            mHeap = new HeapNode[capacity];
        }
        
        /// <summary>
        ///  Initialize a new instance of the priority queue class by copying another priority queue.
        /// </summary>
        /// <remarks>
        ///  The priority queue head is cloned by the items are only shallow copied.
        /// </remarks>
        /// <param name="other">Priority queue to copy from.</param>
        public PriorityQueue(PriorityQueue<TValue, TScore> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            mHeap = new HeapNode[other.Capacity];
            Array.Copy(other.mHeap, mHeap, other.Count);

            Count = other.Count;
        }

        /// <summary>
        ///  Get the queue's current capacity.
        /// </summary>
        public int Capacity { [DebuggerStepThrough] get { return mHeap.Length; } }
        
        /// <summary>
        ///  Get a count of the number of items in the priority queue.
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        ///  Get if the priority queue is empty or not.
        /// </summary>
        public bool IsEmpty { [DebuggerStepThrough] get { return Count == 0; } }

        /// <summary>
        ///  Get if the priority queue is full or not.
        /// </summary>
        public bool IsFull { [DebuggerStepThrough] get { return Count == Capacity; } }

        /// <summary>
        ///  Get if this priority queue supports concurrent access from multiple threads.
        /// </summary>
        [DebuggerHidden]
        public bool IsSynchronized { [DebuggerStepThrough] get { return false; } }

        /// <summary>
        ///  Get if the priority queue is read only.
        /// </summary>
        [DebuggerHidden]
        public bool IsReadOnly { [DebuggerStepThrough] get { return false; } }

        /// <summary>
        ///  Get a number that tracks changes to this priority queue.
        /// </summary>
        internal int Version { get; private set; } = 0;

        /// <summary>
        ///  Add an item to the priority queue.
        /// </summary>
        /// <remarks>
        ///  Inserts the item into the correct position in the heap with a runtime of O(log n).
        /// </remarks>
        /// <param name="value">The value to insert.</param>
        /// <param name="score">The score of the value to insert.</param>
        public void Add(TValue value, TScore score)
        {
            // If there is no more space to store new items then automatically grow the heap.
            if (Count == Capacity)
            {
                GrowHeap();
            }

            // Add the item and resort the heap to maintain consistency. 
            Count++;
            HeapifyUp(Count - 1, value, score);

            // Update the version to invalidate any active iterators.
            Version++;
        }
        
        /// <summary>
        ///  Clear the priority queue of values.
        /// </summary>
        /// <remarks>
        ///  This method simply sets the item count to zero to clear the priority queue. The runtime for this is O(1).
        /// </remarks>
        public void Clear()
        {
            Clear(false);
        }

        /// <summary>
        ///  Clear the priority queue of values.
        /// </summary>
        /// <remarks>
        ///  If the clear heap flag is set to true then each item in the heap will be reset to its default value. This
        ///  changes the expected runtime from O(1) to O(n).
        /// </remarks>
        public void Clear(bool clearHeapValues)
        {
            if (clearHeapValues)
            {
                Array.Clear(mHeap, 0, Count);
            }

            Count = 0;
            Version++;
        }

        /// <summary>
        ///  Check if the priority queue contains the given item.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the item is in the priority queue, false otherwise.</returns>
        public bool Contains(TValue item)
        {
            var score = default(TScore);
            return TryFindScore(item, ref score);
        }

        /// <summary>
        ///  Copy the priority queue heap to an array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">Index in destination array to start writing at.</param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length < arrayIndex + Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array start index out of bounds");
            }

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = mHeap[i].Value;
            }
        }

        /// <summary>
        ///  Copy the priority queue heap to an array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">Index in destination array to start writing at.</param>
        public void CopyTo(Pair<TValue, TScore>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length < arrayIndex + Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array start index out of bounds");
            }

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = new Pair<TValue, TScore>(mHeap[i].Value, mHeap[i].Score);
            }
        }

        /// <summary>
        ///  Remove the smallest item from the priority queue and return it.
        /// </summary>
        /// <remarks>
        ///  Runs in O(log n) time.
        /// </remarks>
        /// <returns>The minimum value in the queue.</returns>
        public TValue Remove()
        {
            // Cannot do anything if there are not values in the heap.
            if (Count == 0)
            {
                throw new InvalidOperationException("No items in priority queue");
            }

            // Extract the smallest value from the heap.
            var result = mHeap[0];

            // Replace the heap root with the last element from the heap, and then reduce the heap's size.
            mHeap[0] = mHeap[Count - 1];
            Count -= 1;

            // Restore the heap's min ordering property by iteratively performing min-heapify until everything looks
            // nice and shiny again.
            if (Count > 0)
            {
                MinHeapify(0);
            }

            // Update the version to invalidate any active iterators.
            Version++;

            // Return to the caller the extracted minimum value from the priority queue.
            return result.Value;
        }

        /// <summary>
        ///  Get an enumerator for the priority queue.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            // Take a copy of the version to check before each yield if the collection was modified.
            int copiedModificationTracker = Version;

            for (int i = 0; i < Count; ++i)
            {
                if (copiedModificationTracker != Version)
                {
                    throw new InvalidOperationException("Priority queue iterator was invalidated");
                }

                yield return mHeap[i].Value;
            }
        }

        /// <summary>
        ///  Get an enumerator for the priority queue.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///  Grow the storage heap to accomodate new items in the priority queue.
        /// </summary>
        /// <remarks>
        ///  This will grow the priority queue heap by a size large enough to accomodate the next full level in the
        ///  heap's binary tree.
        /// </remarks>
        private void GrowHeap()
        {
            GrowHeap(GetNextCapacity(Capacity));
        }

        /// <summary>
        ///  Grow the storage heap to accomodate new items in the priority queue.
        /// </summary>
        /// <remarks>
        ///  The new capacity must be larger than zero and cannot be smaller than the current item count.
        /// </remarks>
        /// <param name="newCapacity">The new capacity for the priority queu.</param>
        private void GrowHeap(int newCapacity)
        {
            if (newCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newCapacity), "New capacity cannot be less than zero");
            }
            else if (newCapacity < Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(newCapacity),
                    "Capacity must at least as large as item count");
            }

            // Create a new heap and copy the old heap's items in.
            var newHeap = new HeapNode[newCapacity];
            Array.Copy(mHeap, 0, newHeap, 0, Count);

            // Switch to the larger heap and discard the smaller one.
            mHeap = newHeap;
        }

        /// <summary>
        ///  Calculates a size large enough to accomodate the next full level in the heap's binary tree.
        /// </summary>
        /// <param name="newCapacity">The new capacity to set.</param>
        /// <returns>The new capacity rounded to the optimized size.</returns>
        private static int GetNextCapacity(int newCapacity)
        {
            // This gets the next power of two, then subtracts one from it. Also has minimum floor value
            // of DefaultCapacity.
            // See http://stackoverflow.com/a/364993 for implementation details.
            if (newCapacity > DefaultCapacity)
            {
                return (int)Math.Ceiling(Math.Log(newCapacity)) - 1;
            }

            return DefaultCapacity;
        }

        /// <summary>
        ///  Add an item to the heap at the given index, and recursively apply heapify-up until the heap's parent -
        ///  child relationship is restored.
        /// </summary>
        /// <remarks>
        ///  Run time is O(log n).
        /// </remarks>
        /// <param name="startIndex">Index to insert the item at.</param>
        /// <param name="itemValue">Value of the item to insert.</param>
        /// <param name="itemScore">Score of the item to insert.</param>
        private void HeapifyUp(int startIndex, TValue itemValue, TScore itemScore)
        {
            if (startIndex < 0 || startIndex >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            int currentIndex = startIndex;
            int parentIndex = (currentIndex - 1) / 2;

            // Locate the slot where this value should be placed. Start at a blank slot in the heap, and then
            // iteratively check and swap the parent node into the slot if it is smaller than the value to insert.
            // Once this property is no longer true, add the value to insert.
            while (currentIndex > 0 && itemScore.CompareTo(mHeap[parentIndex].Score) < 0)
            {
                // Swap the parent value into the current slot.
                mHeap[currentIndex] = mHeap[parentIndex];

                // Move to the parent slot, and get its parent.
                currentIndex = parentIndex;
                parentIndex = (currentIndex - 1) / 2;
            }

            // Insert the value at the correct location in the heap.
            mHeap[currentIndex] = new HeapNode(itemValue, itemScore);

            // Update the version to invalidate any active iterators.
            Version++;
        }

        /// <summary>
        ///  Enforce the min heap property starting at the given index location.
        /// </summary>
        /// <remarks>
        ///  This method assumes that the children of the given node index (left and right) are valid min-heaps, but
        ///  that the specified node may be smaller than its children. MinHeapify will recursively swap the node's
        ///  value with one of the children until it the node is correctly positioned. This has the effect of
        ///  "floating down" a node until it is in the correct position.
        /// </remarks>
        /// <param name="index">Index to start min heapify at.</param>
        private void MinHeapify(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            bool keepHeapifying = true;

            while (index < Count && keepHeapifying)
            {
                int smallestIndex = index;
                int leftIndex = 2 * index + 1;
                int rightIndex = 2 * index + 2;

                if (leftIndex < Count && mHeap[leftIndex].Score.CompareTo(mHeap[smallestIndex].Score) < 0)
                {
                    smallestIndex = leftIndex;
                }

                if (rightIndex < Count && mHeap[rightIndex].Score.CompareTo(mHeap[smallestIndex].Score) < 0)
                {
                    smallestIndex = rightIndex;
                }

                if (smallestIndex != index)
                {
                    var swapTemp = mHeap[index];
                    mHeap[index] = mHeap[smallestIndex];
                    mHeap[smallestIndex] = swapTemp;

                    index = smallestIndex;
                }
                else
                {
                    keepHeapifying = false;
                }
            }

            // Update the version to invalidate any active iterators.
            Version++;
        }

        /// <summary>
        ///  Remove an item from the priority queue.
        /// </summary>
        /// <remarks>
        ///  Run time O(n).
        /// </remarks>
        /// <param name="item">The item to be removed.</param>
        /// <returns>True if the item removed from the priority queue, false otherwise.</returns>
        public bool Remove(TValue item)
        {
            // Removing the items from the array is a little tricky because the heap stores its values in an array.
            // Go through the array and remove the items by overwriting them with the remaining values in the array.
            int writeIndex = 0;
            int itemsRemoved = 0;

            var comparer = EqualityComparer<TValue>.Default;

            for (int readIndex = 0; readIndex < Count; ++readIndex)
            {
                if (comparer.Equals(item, mHeap[readIndex].Value))
                {
                    itemsRemoved++;
                }
                else
                {
                    mHeap[writeIndex++] = mHeap[readIndex];
                }
            }

            Count -= itemsRemoved;

            // Update the version to invalidate any active iterators.
            Version++;

            // Return true if at least one item was removed from the priority queue.
            return (itemsRemoved > 0);
        }

        /// <summary>
        ///  Peek at the lowest priority item in the priority queue.
        /// </summary>
        /// <returns>The next item to be removed.</returns>
        public TValue Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("No items in priority queue");
            }

            return mHeap[0].Value;
        }

        /// <summary>
        ///  Find the given item in the priority queue and retrieve the score associated with it.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <param name="score">Reeceives the item score if the item is located.</param>
        /// <returns>True if the item is in the priority queue, false otherwise.</returns>
        public bool TryFindScore(TValue item, ref TScore score)
        {
            bool didFind = false;

            // Search each entry in the priority queue to see if any match.
            var comparer = EqualityComparer<TValue>.Default;

            for (int currentIndex = 0; currentIndex < Count && !didFind; ++currentIndex)
            {
                if (comparer.Equals(item, mHeap[currentIndex].Value))
                {
                    score = mHeap[currentIndex].Score;
                    didFind = true;
                }
            }

            return didFind;
        }

        /// <summary>
        ///  Try to remove the lowest priority item in the priority queue.
        /// </summary>
        /// <param name="result">Receives the value of the lowest priority item.</param>
        /// <returns>True if an item was removed, false otherwise.</returns>
        public bool TryRemove(TValue result)
        {
            if (Count == 0)
            {
                result = default(TValue);
                return false;
            }

            result = Remove();
            return true;
        }

        /// <summary>
        ///  Try to peek at the lowest priority item in the priority queue.
        /// </summary>
        /// <param name="result">Receives the value of the lowest priority item.</param>
        /// <returns>True if there was an item to peek, false otherwise.</returns>
        public bool TryPeek(out TValue result)
        {
            if (Count == 0)
            {
                result = default(TValue);
                return false;
            }

            result = Peek();
            return true;
        }

        private struct HeapNode
        {
            public HeapNode(TValue value, TScore score)
            {
                Value = value;
                Score = score;
            }

            public readonly TValue Value;
            public readonly TScore Score;
        }
    }
}