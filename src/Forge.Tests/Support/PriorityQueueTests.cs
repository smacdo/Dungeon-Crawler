using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Support;
using System.Collections.Generic;
using Forge;

namespace Forge.Tests.Support
{
    [TestClass]
    public class PriorityQueueTests
    {
        [TestMethod]
        public void Create_Empty_Priority_Queue_With_Specified_Capacity()
        {
            var q = new PriorityQueue<char, int>(15);
            Assert.AreEqual(15, q.Capacity);
        }

        [TestMethod]
        public void Create_With_Zero_Capacity_OK()
        {
            var q = new PriorityQueue<char, int>(0);
            Assert.AreEqual(0, q.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_With_Negative_Capacity_Throws_Exception()
        {
            var q = new PriorityQueue<char, int>(-1);
        }

        [TestMethod]
        public void Create_With_Copy_Constructor_Copies_Elements()
        {
            var a = new PriorityQueue<char, int>(1);
            a.Add('x', 42);

            var b = new PriorityQueue<char, int>(a);
            Assert.AreEqual('x', b.Peek());
        }

        [TestMethod]
        public void Copy_Constructor_Deep_Copies_Heap_Array()
        {
            var a = new PriorityQueue<char, int>(1);
            a.Add('x', 42);

            var b = new PriorityQueue<char, int>(a);
            Assert.AreEqual('x', b.Peek());

            a.Add('y', 40);

            Assert.AreEqual('y', a.Peek());
            Assert.AreEqual('x', b.Peek());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Constructor_Throws_Exception_If_Other_Is_Null()
        {
            var a = new PriorityQueue<char, int>(null);
        }

        [TestMethod]
        public void Count_Reflects_Number_Of_Items_In_Queue()
        {
            var q = new PriorityQueue<char, int>();
            Assert.AreEqual(0, q.Count);

            q.Add('a', 22);
            Assert.AreEqual(1, q.Count);

            q.Add('b', 20);
            q.Add('c', 25);
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
            var q = new PriorityQueue<char, int>();
            Assert.IsTrue(q.IsEmpty);

            q.Add('a', 42);
            Assert.IsFalse(q.IsEmpty);

            q.Remove();
            Assert.IsTrue(q.IsEmpty);

            q.Add('a', 42);
            q.Add('b', 5);
            Assert.IsFalse(q.IsEmpty);

            q.Remove();
            Assert.IsFalse(q.IsEmpty);
        }

        [TestMethod]
        public void Is_Full_Only_When_Count_Equals_Capacity()
        {
            var q = new PriorityQueue<char, int>(2);
            Assert.IsFalse(q.IsFull);

            q.Add('a', 42);
            Assert.IsFalse(q.IsFull);

            q.Add('b', 40);
            Assert.IsTrue(q.IsFull);

            q.Add('c', 50);      // Triggers a resize so no longer full.
            Assert.IsFalse(q.IsFull);

            q.Remove();     // Back to previous capcaity but was resized so no longer full.
            Assert.IsFalse(q.IsFull);
        }

        [TestMethod]
        public void Add_Increments_Version_Tracker()
        {
            var q = new PriorityQueue<char, int>();
            int v0 = q.Version;
            Assert.AreEqual(0, v0);

            q.Add('a', 12);

            int v1 = q.Version;
            Assert.IsTrue(v1 > v0);

            q.Add('b', 5);
            q.Add('c', 6);

            int v2 = q.Version;
            Assert.IsTrue(v2 > v1);
        }

        [TestMethod]
        public void Add_Single_And_Removing_Returns_Values_In_Lowest_Priority_Order()
        {
            var q = new PriorityQueue<char, int>();

            // Add values [5, 3, 1, 6, 4] -> [1, 3, 4, 5, 6].
            q.Add('5', 5);
            q.Add('3', 3);
            q.Add('1', 1);
            q.Add('6', 6);
            q.Add('4', 4);

            // Remove and test ordering.
            Assert.AreEqual('1', q.Remove());
            Assert.AreEqual('3', q.Remove());
            Assert.AreEqual('4', q.Remove());
            Assert.AreEqual('5', q.Remove());
            Assert.AreEqual('6', q.Remove());

            Assert.AreEqual(0, q.Count);
        }
        
        [TestMethod]
        public void Add_With_Duplicate_Values_Is_Ok()
        {
            var q = new PriorityQueue<char, int>();

            // Add values [5, 3, 1, 3, 5] -> [1, 3, 3, 5, 5].
            q.Add('5', 5);
            q.Add('3', 3);
            q.Add('1', 1);
            q.Add('3', 3);
            q.Add('5', 5);

            // Remove and test ordering.
            Assert.AreEqual('1', q.Remove());
            Assert.AreEqual('3', q.Remove());
            Assert.AreEqual('3', q.Remove());
            Assert.AreEqual('5', q.Remove());
            Assert.AreEqual('5', q.Remove());
        }

        [TestMethod]
        public void Add_Single_Values_Exceeding_Capacity_Does_Not_Break_Ordering()
        {
            var q = new PriorityQueue<char, int>(2);
            Assert.AreEqual(2, q.Capacity);

            // Add values [5, 3, 1, 6, 4] -> [1, 3, 4, 5, 6].
            q.Add('5', 5);
            q.Add('3', 3);
            q.Add('1', 1);
            q.Add('6', 6);
            q.Add('4', 4);

            // Remove and test ordering.
            Assert.AreEqual('1', q.Remove());
            Assert.AreEqual('3', q.Remove());
            Assert.AreEqual('4', q.Remove());
            Assert.AreEqual('5', q.Remove());
            Assert.AreEqual('6', q.Remove());

            Assert.IsTrue(q.Capacity > 2);
        }
        
        [TestMethod]
        public void Remove_Increments_Version_Tracker()
        {
            var q = new PriorityQueue<char, int>();
            Assert.AreEqual(0, q.Version);

            q.Add('x', 5);
            int v1 = q.Version;

            q.Remove();
            int v2 = q.Version;

            Assert.IsTrue(v2 > v1);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Remove_Throws_Exception_When_No_Values_Are_In_Queue()
        {
            var q = new PriorityQueue<char, int>();
            q.Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Remove_Throws_Exception_When_Too_Many_Values_Are_Removed()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('x', 5);
            q.Add('y', 8);

            q.Remove();
            q.Remove();
            q.Remove();
        }

        [TestMethod]
        public void Try_Remove_Returns_True_And_Value_Only_If_Value_Present()
        {
            var q = new PriorityQueue<char, int>();

            Assert.IsFalse(q.TryRemove('*'));

            q.Add('y', 2);

            Assert.IsTrue(q.TryRemove('y'));
            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Try_Remove_Increments_Version_Tracker()
        {
            var q = new PriorityQueue<char, int>();
            Assert.AreEqual(0, q.Version);

            q.Add('i', 5);
            int v1 = q.Version;

            q.TryRemove('i');
            int v2 = q.Version;

            Assert.IsTrue(v2 > v1);
        }

        [TestMethod]
        public void Clear_Removes_Values_From_Queue()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('x', 2);
            q.Add('y', -1);

            Assert.AreEqual(2, q.Count);

            q.Clear();
            Assert.AreEqual(0, q.Count);
        }

        [TestMethod]
        public void Contains_Searches_Queue_For_Values()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('3', 3);
            q.Add('6', 6);
            q.Add('5', 5);
            q.Add('5', 5);
            q.Add('8', 8);

            Assert.IsTrue(q.Contains('3'));
            Assert.IsTrue(q.Contains('5'));
            Assert.IsTrue(q.Contains('6'));
            Assert.IsTrue(q.Contains('8'));

            Assert.IsFalse(q.Contains('0'));
            Assert.IsFalse(q.Contains('9'));

            // Remove two values (3, 5) and test again.
            Assert.AreEqual('3', q.Remove());
            Assert.AreEqual('5', q.Remove());

            Assert.IsFalse(q.Contains('3'));
            Assert.IsTrue(q.Contains('5'));       // 5 was duplicated.
        }

        [TestMethod]
        public void Can_Find_Scores_Associated_With_Values()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('3', 3);
            q.Add('6', 6);
            q.Add('x', 7);
            q.Add('x', 2);
            q.Add('8', 8);

            int score = 0;

            Assert.IsTrue(q.TryFindScore('3', ref score));
            Assert.AreEqual(3, score);

            Assert.IsTrue(q.TryFindScore('6', ref score));
            Assert.AreEqual(6, score);

            Assert.IsTrue(q.TryFindScore('8', ref score));
            Assert.AreEqual(8, score);

            // 'x' is associated with both 2 and 7 so either could be the result returned.
            // (This is to illustrate that adding a value twice is supported by not recommended).
            Assert.IsTrue(q.TryFindScore('x', ref score));
            Assert.IsTrue(score == 7 || score == 2);
        }

        [TestMethod]
        public void Copy_Values_To_Array_Have_Min_Ordering()
        {
            var q = new PriorityQueue<char, int>();
            
            q.Add('3', 3);
            q.Add('6', 6);
            q.Add('5', 5);
            q.Add('4', 4);
            q.Add('8', 8);

            var a = new char[q.Count];
            q.CopyTo(a, 0);

            // Start and end should be min/max values.
            Assert.AreEqual('3', a[0]);
            Assert.AreEqual('8', a[q.Count - 1]);

            // Each value in array should be larger than previous values. (min ordering).
            Assert.IsTrue(a[0] < a[1]);
            Assert.IsTrue(a[1] < a[2]);
            Assert.IsTrue(a[2] < a[3]);
            Assert.IsTrue(a[3] < a[4]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Values_Throws_Exception_If_Destination_Array_Is_Null()
        {
            var q = new PriorityQueue<char, int>();
            q.CopyTo((char[])null, 0);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Copy_Values_Throws_Exception_Destination_Array_Too_Small()
        {
            var q = new PriorityQueue<char, int>();
            var a = new char[2];

            q.Add('1', 1);
            q.Add('2', 2);

            q.CopyTo(a, 1);
        }

        [TestMethod]
        public void Copy_Heap_To_Array_Have_Min_Ordering()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('3', 3);
            q.Add('6', 6);
            q.Add('5', 5);
            q.Add('4', 4);
            q.Add('8', 8);

            var a = new Pair<char, int>[q.Count];
            q.CopyTo(a, 0);

            // Start and end should be min/max values.
            Assert.AreEqual('3', a[0].First);
            Assert.AreEqual('8', a[q.Count - 1].First);

            // Each value in array should be larger than previous values. (min ordering).
            Assert.IsTrue(a[0].Second < a[1].Second);
            Assert.IsTrue(a[1].Second < a[2].Second);
            Assert.IsTrue(a[2].Second < a[3].Second);
            Assert.IsTrue(a[3].Second < a[4].Second);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Heap_Throws_Exception_If_Destination_Array_Is_Null()
        {
            var q = new PriorityQueue<char, int>();
            q.CopyTo((Pair<char, int>[])null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Copy_Heap_Throws_Exception_Destination_Array_Too_Small()
        {
            var q = new PriorityQueue<char, int>();
            var a = new Pair<char, int>[2];

            q.Add('1', 1);
            q.Add('2', 2);

            q.CopyTo(a, 1);
        }

        [TestMethod]
        public void Enumerate_Queue_Values_With_Min_Ordering()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('3', 3);
            q.Add('6', 6);
            q.Add('5', 5);
            q.Add('4', 4);
            q.Add('8', 8);

            // Visit all values and copy.
            var a = new char[q.Count];
            int i = 0;

            foreach (var v in q)
            {
                a[i++] = v;
            }

            Assert.AreEqual(5, i);

            // Start and end should be min/max values.
            Assert.AreEqual('3', a[0]);
            Assert.AreEqual('8', a[q.Count - 1]);

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
            var q = new PriorityQueue<char, int>();

            q.Add('2', 2);
            q.Add('3', 3);
            q.Add('1', 1);
            q.Add('1', 1);
            q.Add('0', 0);
            
            // Peek at each before removing.
            Assert.AreEqual('0', q.Peek());
            q.Remove();

            Assert.AreEqual('1', q.Peek());
            q.Remove();

            Assert.AreEqual('1', q.Peek());
            q.Remove();

            Assert.AreEqual('2', q.Peek());
            q.Remove();

            Assert.AreEqual('3', q.Peek());
            q.Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Peek_Throws_Exception_If_No_Values_Present()
        {
            var q = new PriorityQueue<char, int>();
            q.Peek();
        }

        [TestMethod]
        public void Try_Peek_Returns_True_And_Value_Only_If_Value_Present()
        {
            var q = new PriorityQueue<char, int>();
            var o = '$';     // Specifically not default 0 to test if default is set.

            Assert.IsFalse(q.TryPeek(out o));
            Assert.AreEqual(0, o);

            q.Add('?', 2);

            Assert.IsTrue(q.TryPeek(out o));
            Assert.AreEqual('?', o);
        }
        
        [TestMethod]
        public void Priority_Queue_Internal_Heap_Is_Valid_After_Work()
        {
            var q = new PriorityQueue<char, int>();

            q.Add('3', 3);
            q.Add('7', 7);
            q.Add('2', 2);
            q.Add('1', 1);
            q.Add('3', 3);
            q.Add('2', 2);
            q.Add('4', 4);
            q.Add('5', 5);

            q.Remove();
            q.Add('6', 6);

            q.Remove();
            q.Remove();

            q.Add('4', 4);
            q.Add('1', 1);
            q.Add('2', 2);

            ValidateHeap(q);
        }

        protected void ValidateHeap<T, S>(PriorityQueue<T, S> q) where S : IComparable<S>
        {
            var heap = new Pair<T, S>[q.Count];
            q.CopyTo(heap, 0);

            ValidateHeapRecursive(heap, 0);
        }

        protected void ValidateHeapRecursive<T, S>(Pair<T, S>[] heap, int nodeIndex)
        {
            var comparer = Comparer<S>.Default;

            if (nodeIndex < heap.Length)
            {
                int leftIndex = 2 * nodeIndex + 1;
                int rightIndex = 2 * nodeIndex + 2;

                // Verify left child is in order (default larger) than the parent.
                if (leftIndex < heap.Length)
                {
                    Assert.IsTrue(
                        comparer.Compare(heap[nodeIndex].Second, heap[leftIndex].Second) <= 0,
                        string.Format("Light child '{1}' (#{0}) is out of order to parent '{3}' (#{2})",
                            leftIndex,
                            heap[leftIndex].First,
                            nodeIndex,
                            heap[nodeIndex].First));

                    ValidateHeapRecursive(heap, leftIndex);
                }

                // Verify right child is in order (default larger) than the parent.
                if (rightIndex < heap.Length)
                {
                    Assert.IsTrue(
                        comparer.Compare(heap[nodeIndex].Second, heap[leftIndex].Second) <= 0,
                        string.Format("Right child '{1}' (#{0}) is out of order to parent '{3}' (#{2})",
                            rightIndex,
                            heap[rightIndex].First,
                            nodeIndex,
                            heap[nodeIndex].First));

                    ValidateHeapRecursive(heap, rightIndex);
                }
            }
        }
    }
}
