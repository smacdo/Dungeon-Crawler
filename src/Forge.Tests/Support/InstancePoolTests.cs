using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Support.Tests
{
    /// <summary>
    ///  Tests for the instance pool.
    /// </summary>
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class InstancePoolTests
    {
        /// <summary>
        ///  Test dummy
        /// </summary>
        public class TestNode : IRecyclable
        {
            public int RecycleCount = 0;

            public void Recycle()
            {
                RecycleCount++;
            }
        }

        /// <summary>
        ///  Create a small instance pool, see if everything looks OK.
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void CreateSmallInstancePool()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);

            Assert.AreEqual(3, pool.TotalCount);
            Assert.AreEqual(3, pool.FreeCount);
            Assert.AreEqual(0, pool.ActiveCount);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void TakeOneInstance()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);
            TestNode node = pool.Take();

            Assert.AreEqual(3, pool.TotalCount);
            Assert.AreEqual(2, pool.FreeCount);
            Assert.AreEqual(1, pool.ActiveCount);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void ReturnRecycle()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(1);

            TestNode node = pool.Take();
            Assert.AreEqual(1, node.RecycleCount);

            pool.Return(node);
            Assert.AreEqual(1, node.RecycleCount);

            node = pool.Take();
            Assert.AreEqual(2, node.RecycleCount);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void TakeAndReturnOneInstance()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);
            TestNode node = pool.Take();

            pool.Return(node);

            Assert.AreEqual(3, pool.TotalCount);
            Assert.AreEqual(3, pool.FreeCount);
            Assert.AreEqual(0, pool.ActiveCount);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void TakeAndReturnInDifferentOrder()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);

            TestNode a = pool.Take();
            TestNode b = pool.Take();
            TestNode c = pool.Take();

            pool.Return(b);
            pool.Return(c);
            pool.Return(a);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void TakeAndReturnAndTakeAgain()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);

            TestNode a = pool.Take();
            TestNode b = pool.Take();
            TestNode c = pool.Take();

            Assert.AreNotSame(a, b);
            Assert.AreNotSame(b, c);

            pool.Return(a);
            pool.Return(b);
            pool.Return(c);

            TestNode a2 = pool.Take();
            TestNode b2 = pool.Take();
            TestNode c2 = pool.Take();

            Assert.AreSame(a2, a);
            Assert.AreSame(b2, b);
            Assert.AreSame(c2, c);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TakeTooManyInstances()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);
            TestNode node = pool.Take();

            for (int i = 0; i < pool.TotalCount + 1; ++i)
            {
                pool.Take();
            }
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        [ExpectedException(typeof(ArgumentException))]
        public void ReturnTheWrongInstance()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);
            TestNode node = pool.Take();

            pool.Return(new TestNode());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        [ExpectedException(typeof(ArgumentException))]
        public void ReturnTheInstanceTwice()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>(3);
            TestNode a = pool.Take();
            TestNode b = pool.Take();

            pool.Return(a);
            pool.Return(b);
            pool.Return(a);
        }

        [TestMethod]
        [TestCategory("Forge/InstancePool")]
        public void GetEnumeratorRepresentsActiveInstances()
        {
            // No active values at the start.
            var pool = new InstancePool<TestNode>(3);

            Assert.AreEqual(0, AsEnumerable(pool.GetActiveListEnumerator()).Count());
            Assert.AreEqual(0, AsEnumerable(pool.GetEnumerator()).Count());
            Assert.AreEqual(0, AsEnumerable(((IEnumerable<TestNode>)pool).GetEnumerator()).Count());

            // Add one active node.
            var a = pool.Take();

            Assert.AreEqual(1, AsEnumerable(pool.GetActiveListEnumerator()).Count());
            Assert.AreEqual(1, AsEnumerable(pool.GetEnumerator()).Count());
            Assert.AreEqual(1, AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).Count());

            CollectionAssert.AreEquivalent(
                new List<TestNode>() { a },
                AsEnumerable(pool.GetActiveListEnumerator()).ToList());
            CollectionAssert.AreEquivalent(
                new List<TestNode>() { a },
                AsEnumerable(pool.GetEnumerator()).ToList());
            CollectionAssert.AreEquivalent(
                new List<TestNode>() { a },
                AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).ToList());

            // Add two more active nodes.
            var b = pool.Take();
            var c = pool.Take();

            Assert.AreEqual(3, AsEnumerable(pool.GetActiveListEnumerator()).Count());
            Assert.AreEqual(3, AsEnumerable(pool.GetEnumerator()).Count());
            Assert.AreEqual(3, AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).Count());

            CollectionAssert.AreEquivalent(
                new List<TestNode>() { a, b, c },
                AsEnumerable(pool.GetActiveListEnumerator()).ToList());
            CollectionAssert.AreEquivalent(
                new List<TestNode>() { a, b, c },
                AsEnumerable(pool.GetEnumerator()).ToList());
            CollectionAssert.AreEquivalent(
                new List<TestNode>() { a, b, c },
                AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).ToList());

            // Return an active node.
            pool.Return(a);

            Assert.AreEqual(2, AsEnumerable(pool.GetActiveListEnumerator()).Count());
            Assert.AreEqual(2, AsEnumerable(pool.GetEnumerator()).Count());
            Assert.AreEqual(2, AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).Count());

            CollectionAssert.AreEquivalent(
                new List<TestNode>() { b, c },
                AsEnumerable(pool.GetActiveListEnumerator()).ToList());
            CollectionAssert.AreEquivalent(
                new List<TestNode>() { b, c },
                AsEnumerable(pool.GetEnumerator()).ToList());
            CollectionAssert.AreEquivalent(
                new List<TestNode>() { b, c },
                AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).ToList());

            // Return two active nodes.
            pool.Return(b);
            pool.Return(c);

            Assert.AreEqual(0, AsEnumerable(pool.GetActiveListEnumerator()).Count());
            Assert.AreEqual(0, AsEnumerable(pool.GetEnumerator()).Count());
            Assert.AreEqual(0, AsEnumerable(((IEnumerable<TestNode>) pool).GetEnumerator()).Count());
        }

        private static IEnumerable<T> AsEnumerable<T>(IEnumerator<T> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }
}
