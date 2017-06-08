using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Support;
using System.Collections.Generic;

namespace Scott.Forge.Tests.Support
{
    [TestClass]
    public class PriorityQueueTests
    {
        [TestMethod]
        public void Create_Empty_Priority_Queue_With_Specified_Capacity()
        {
            var q = new PriorityQueue<int>(15);
            Assert.AreEqual(15, q.Capacity);
        }

        [TestMethod]
        public void Create_With_Zero_Capacity_OK()
        {
            var q = new PriorityQueue<int>(0);
            Assert.AreEqual(0, q.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_With_Negative_Capacity_Throws_Exception()
        {
            var q = new PriorityQueue<int>(-1);
        }

        [TestMethod]
        public void Create_With_Custom_Comparer()
        {
            var c = new MaxHeapComparer();
            var q = new PriorityQueue<int>(c);

            Assert.AreSame(c, q.Comparer);

            // TODO: Test if custom comparer is used correctly when removing items from queue.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_With_Null_Comparer_Throws_Exception()
        {
            var q = new PriorityQueue<int>(0, null);
        }

        [TestMethod]
        public void Create_With_Initial_Values_Will_Populate_Queue()
        {
            var q = new PriorityQueue<int>(new List<int> { 3, 5, 1, 2 });
            Assert.AreEqual(4, q.Count);

            Assert.AreEqual(1, q.Remove());
            Assert.AreEqual(2, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(5, q.Remove());
        }

        [TestMethod]
        public void Count_Reflects_Number_Of_Items_In_Queue()
        {
            var q = new PriorityQueue<int>();
            Assert.AreEqual(0, q.Count);

            q.Add(22);
            Assert.AreEqual(1, q.Count);

            q.Add(new List<int> { 20, 25 });
            Assert.AreEqual(3, q.Count);

            q.Remove();
            Assert.AreEqual(2, q.Count);

            q.Remove();
            Assert.AreEqual(1, q.Count);

            q.Remove();
            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Is_Empty_Only_True_When_Count_Is_Zero()
        {
            var q = new PriorityQueue<int>();
            Assert.IsTrue(q.IsEmpty);

            q.Add(42);
            Assert.IsFalse(q.IsEmpty);

            q.Remove();
            Assert.IsTrue(q.IsEmpty);

            q.Add(42);
            q.Add(5);
            Assert.IsFalse(q.IsEmpty);

            q.Remove();
            Assert.IsFalse(q.IsEmpty);
        }

        [TestMethod]
        public void Is_Full_Only_When_Count_Equals_Capacity()
        {
            var q = new PriorityQueue<int>(2);
            Assert.IsFalse(q.IsFull);

            q.Add(42);
            Assert.IsFalse(q.IsFull);

            q.Add(40);
            Assert.IsTrue(q.IsFull);

            q.Add(50);      // Triggers a resize so no longer full.
            Assert.IsFalse(q.IsFull);

            q.Remove();     // Back to previous capcaity but was resized so no longer full.
            Assert.IsFalse(q.IsFull);
        }

        [TestMethod]
        public void Add_Increments_Version_Tracker()
        {
            var q = new PriorityQueue<int>();
            int v0 = q.Version;
            Assert.AreEqual(0, v0);

            q.Add(12);

            int v1 = q.Version;
            Assert.IsTrue(v1 > v0);

            q.Add(new List<int> { 5, 7 });

            int v2 = q.Version;
            Assert.IsTrue(v2 > v1);
        }

        [TestMethod]
        public void Add_Single_And_Removing_Returns_Values_In_Lowest_Priority_Order()
        {
            var q = new PriorityQueue<int>();

            // Add values [5, 3, 1, 6, 4] -> [1, 3, 4, 5, 6].
            q.Add(5);
            q.Add(3);
            q.Add(1);
            q.Add(6);
            q.Add(4);

            // Remove and test ordering.
            Assert.AreEqual(1, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(4, q.Remove());
            Assert.AreEqual(5, q.Remove());
            Assert.AreEqual(6, q.Remove());

            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Add_Multiple_And_Removing_Returns_Values_In_Lowest_Priority_Order()
        {
            var q = new PriorityQueue<int>();

            // Add values [3, 2, 6, 5, 4] -> [2, 3, 4, 5, 6].
            q.Add(new HashSet<int> { 3, 2, 6, 5, 4 });

            // Remove and test ordering.
            Assert.AreEqual(2, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(4, q.Remove());
            Assert.AreEqual(5, q.Remove());
            Assert.AreEqual(6, q.Remove());

            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Add_Multiple_And_Removing_With_Custom_Ordering_Returns_Values_In_Highest_Priority_Order()
        {
            var q = new PriorityQueue<int>(new MaxHeapComparer());

            // Add values [3, 2, 6, 5, 4] -> [6, 5, 4, 3, 2].
            q.Add(new HashSet<int> { 3, 2, 6, 5, 4 });

            // Remove and test ordering.
            Assert.AreEqual(6, q.Remove());
            Assert.AreEqual(5, q.Remove());
            Assert.AreEqual(4, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(2, q.Remove());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Multiple_With_Null_Container_Throws_Exception()
        {
            var q = new PriorityQueue<int>();
            q.Add(null);
        }
        
        [TestMethod]
        public void Add_With_Duplicate_Values_Is_Ok()
        {
            var q = new PriorityQueue<int>();

            // Add values [5, 3, 1, 3, 5] -> [1, 3, 3, 5, 5].
            q.Add(5);
            q.Add(3);
            q.Add(1);
            q.Add(3);
            q.Add(5);

            // Remove and test ordering.
            Assert.AreEqual(1, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(5, q.Remove());
            Assert.AreEqual(5, q.Remove());
        }

        [TestMethod]
        public void Add_Single_Values_Exceeding_Capacity_Does_Not_Break_Ordering()
        {
            var q = new PriorityQueue<int>(2);
            Assert.AreEqual(2, q.Capacity);

            // Add values [5, 3, 1, 6, 4] -> [1, 3, 4, 5, 6].
            q.Add(5);
            q.Add(3);
            q.Add(1);
            q.Add(6);
            q.Add(4);

            // Remove and test ordering.
            Assert.AreEqual(1, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(4, q.Remove());
            Assert.AreEqual(5, q.Remove());
            Assert.AreEqual(6, q.Remove());

            Assert.IsTrue(q.Capacity > 2);
        }

        [TestMethod]
        public void Add_Multiple_Values_Exceeding_Capacit_Does_Not_Break_Ordering()
        {
            var q = new PriorityQueue<int>(2);
            Assert.AreEqual(2, q.Capacity);

            // Add values [3, 2, 6, 5, 4] -> [2, 3, 4, 5, 6].
            q.Add(new HashSet<int> { 3, 2, 6, 5, 4 });

            // Remove and test ordering.
            Assert.AreEqual(2, q.Remove());
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(4, q.Remove());
            Assert.AreEqual(5, q.Remove());
            Assert.AreEqual(6, q.Remove());

            Assert.IsTrue(q.Capacity > 2);
        }

        [TestMethod]
        public void Remove_Increments_Version_Tracker()
        {
            var q = new PriorityQueue<int>();
            Assert.AreEqual(0, q.Version);

            q.Add(5);
            int v1 = q.Version;

            q.Remove();
            int v2 = q.Version;

            Assert.IsTrue(v2 > v1);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Remove_Throws_Exception_When_No_Values_Are_In_Queue()
        {
            var q = new PriorityQueue<int>();
            q.Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Remove_Throws_Exception_When_Too_Many_Values_Are_Removed()
        {
            var q = new PriorityQueue<int>(new List<int>() { 3, 2 });
            q.Remove();
            q.Remove();
            q.Remove();
        }

        [TestMethod]
        public void Try_Remove_Returns_True_And_Value_Only_If_Value_Present()
        {
            var q = new PriorityQueue<int>();
            var o = -1;     // Specifically not default 0 to test if default is set.

            Assert.IsFalse(q.TryRemove(out o));
            Assert.AreEqual(0, o);

            q.Add(2);

            Assert.IsTrue(q.TryRemove(out o));
            Assert.AreEqual(2, o);
            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Try_Remove_Increments_Version_Tracker()
        {
            var q = new PriorityQueue<int>();
            Assert.AreEqual(0, q.Version);

            q.Add(5);
            int v1 = q.Version;

            int o = 0;
            q.TryRemove(out o);
            int v2 = q.Version;

            Assert.IsTrue(v2 > v1);
        }

        [TestMethod]
        public void Clear_Removes_Values_From_Queue()
        {
            var q = new PriorityQueue<int>(new List<int>() { 3, 2 });
            Assert.AreEqual(2, q.Count);

            q.Clear();
            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Contains_Searches_Queue_For_Values()
        {
            var q = new PriorityQueue<int>(new List<int>() { 3, 6, 5, 5, 8 });

            Assert.IsTrue(q.Contains(3));
            Assert.IsTrue(q.Contains(5));
            Assert.IsTrue(q.Contains(6));
            Assert.IsTrue(q.Contains(8));

            Assert.IsFalse(q.Contains(0));
            Assert.IsFalse(q.Contains(-3));

            // Remove two values (3, 5) and test again.
            Assert.AreEqual(3, q.Remove());
            Assert.AreEqual(5, q.Remove());

            Assert.IsFalse(q.Contains(3));
            Assert.IsTrue(q.Contains(5));       // 5 was duplicated.
        }

        [TestMethod]
        public void Copy_Values_To_Array_Have_Min_Ordering()
        {
            var q = new PriorityQueue<int>(new List<int>() { 3, 6, 5, 4, 8 });
            var a = new int[q.Count];

            q.CopyTo(a, 0);

            // Start and end should be min/max values.
            Assert.AreEqual(3, a[0]);
            Assert.AreEqual(8, a[q.Count - 1]);

            // Each value in array should be larger than previous values. (min ordering).
            Assert.IsTrue(a[0] < a[1]);
            Assert.IsTrue(a[1] < a[2]);
            Assert.IsTrue(a[2] < a[3]);
            Assert.IsTrue(a[3] < a[4]);
        }

        [TestMethod]
        public void Enumerate_Queue_Values_With_Min_Ordering()
        {
            var q = new PriorityQueue<int>(new List<int>() { 3, 6, 5, 4, 8 });
            var a = new int[q.Count];

            // Visit all values and copy.
            int i = 0;

            foreach (var v in q)
            {
                a[i++] = v;
            }

            Assert.AreEqual(5, i);

            // Start and end should be min/max values.
            Assert.AreEqual(3, a[0]);
            Assert.AreEqual(8, a[q.Count - 1]);

            // Each value in array should be larger than previous values. (min ordering).
            Assert.IsTrue(a[0] < a[1]);
            Assert.IsTrue(a[1] < a[2]);
            Assert.IsTrue(a[2] < a[3]);
            Assert.IsTrue(a[3] < a[4]);
        }

        [TestMethod]
        public void Peek_Shows_Min_Value_About_To_Be_Removed()
        {
            // Add values [2, 3, 1, 1, 0] -> [0, 1, 1, 2, 3].
            var q = new PriorityQueue<int>(new List<int> { 2, 3, 1, 1, 0 });
            
            // Peek at each before removing.
            Assert.AreEqual(0, q.Peek());
            q.Remove();

            Assert.AreEqual(1, q.Peek());
            q.Remove();

            Assert.AreEqual(1, q.Peek());
            q.Remove();

            Assert.AreEqual(2, q.Peek());
            q.Remove();

            Assert.AreEqual(3, q.Peek());
            q.Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Peek_Throws_Exception_If_No_Values_Present()
        {
            var q = new PriorityQueue<int>();
            q.Peek();
        }

        [TestMethod]
        public void Try_Peek_Returns_True_And_Value_Only_If_Value_Present()
        {
            var q = new PriorityQueue<int>();
            var o = -1;     // Specifically not default 0 to test if default is set.

            Assert.IsFalse(q.TryPeek(out o));
            Assert.AreEqual(0, o);

            q.Add(2);

            Assert.IsTrue(q.TryPeek(out o));
            Assert.AreEqual(2, o);
        }

        // TODO: Test custom comparer is used as expected.

        [TestMethod]
        public void Priority_Queue_Internal_Heap_Is_Valid_After_Work()
        {
            var q = new PriorityQueue<int>(new List<int> { 3, 7, 2, 1, 3, 2, 4, 5 });

            q.Remove();
            q.Add(6);

            q.Remove();
            q.Remove();

            q.Add(new List<int> { 4, 1, 2 });

            ValidateHeap(q);
        }

        protected void ValidateHeap<T>(PriorityQueue<T> q)
        {
            var heap = new T[q.Count];
            q.CopyTo(heap, 0);

            ValidateHeapRecursive(heap, q.Comparer, 0);
        }

        protected void ValidateHeapRecursive<T>(T[] heap, IComparer<T> comparer, int nodeIndex)
        {
            if (nodeIndex < heap.Length)
            {
                int leftIndex = 2 * nodeIndex + 1;
                int rightIndex = 2 * nodeIndex + 2;

                // Verify left child is in order (default larger) than the parent.
                if (leftIndex < heap.Length)
                {
                    Assert.IsTrue(
                        comparer.Compare(heap[nodeIndex], heap[leftIndex]) <= 0,
                        string.Format("Light child (#{0} '{1}') is out of order to parent (#{2} '{3}')",
                            leftIndex,
                            heap[leftIndex],
                            nodeIndex,
                            heap[nodeIndex]));

                    ValidateHeapRecursive(heap, comparer, leftIndex);
                }

                // Verify right child is in order (default larger) than the parent.
                if (rightIndex < heap.Length)
                {
                    Assert.IsTrue(
                        comparer.Compare(heap[nodeIndex], heap[leftIndex]) <= 0,
                        string.Format("Right child (#{0} '{1}') is out of order to parent (#{2} '{3}')",
                            rightIndex,
                            heap[rightIndex],
                            nodeIndex,
                            heap[nodeIndex]));

                    ValidateHeapRecursive(heap, comparer, rightIndex);
                }
            }
        }

        private class MaxHeapComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y) * -1;
            }

            public int GetHashCode(int obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
