using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common.Tests
{
    /// <summary>
    ///  Tests for the instance pool.
    /// </summary>
    [TestFixture]
    [Category( "Common" )]
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
        [Test]
        public void CreateSmallInstancePool()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );

            Assert.AreEqual( 3, pool.TotalCount );
            Assert.AreEqual( 3, pool.FreeCount );
            Assert.AreEqual( 0, pool.ActiveCount );
        }

        /// <summary>
        /// </summary>
        [Test]
        public void TakeOneInstance()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );
            TestNode node = pool.Take();

            Assert.AreEqual( 3, pool.TotalCount );
            Assert.AreEqual( 2, pool.FreeCount );
            Assert.AreEqual( 1, pool.ActiveCount );
        }

        /// <summary>
        /// </summary>
        [Test]
        public void ReturnRecycle()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 1 );

            TestNode node = pool.Take();
            Assert.AreEqual( 1, node.RecycleCount );

            pool.Return( node );
            Assert.AreEqual( 1, node.RecycleCount );

            node = pool.Take();
            Assert.AreEqual( 2, node.RecycleCount );
        }

        /// <summary>
        /// </summary>
        [Test]
        public void TakeAndReturnOneInstance()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );
            TestNode node = pool.Take();

            pool.Return( node );

            Assert.AreEqual( 3, pool.TotalCount );
            Assert.AreEqual( 3, pool.FreeCount );
            Assert.AreEqual( 0, pool.ActiveCount );
        }

        /// <summary>
        /// </summary>
        [Test]
        public void TakeAndReturnInDifferentOrder()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );

            TestNode a = pool.Take();
            TestNode b = pool.Take();
            TestNode c = pool.Take();

            pool.Return( b );
            pool.Return( c );
            pool.Return( a );
        }

        /// <summary>
        /// </summary>
        [Test]
        public void TakeAndReturnAndTakeAgain()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );

            TestNode a = pool.Take();
            TestNode b = pool.Take();
            TestNode c = pool.Take();

            Assert.AreNotSame( a, b );
            Assert.AreNotSame( b, c );

            pool.Return( a );
            pool.Return( b );
            pool.Return( c );

            TestNode a2 = pool.Take();
            TestNode b2 = pool.Take();
            TestNode c2 = pool.Take();

            Assert.AreSame( a2, a );
            Assert.AreSame( b2, b );
            Assert.AreSame( c2, c );
        }

        /// <summary>
        /// </summary>
        [Test]
        [ExpectedException( typeof( OverflowException ), ExpectedMessage = "No more free instances are available for allocation from this pool" )]
        public void TakeTooManyInstances()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );
            TestNode node = pool.Take();

            for ( int i = 0; i < pool.TotalCount + 1; ++i )
            {
                pool.Take();
            }
        }

        /// <summary>
        /// </summary>
        [Test]
        [ExpectedException( typeof( ArgumentException ), ExpectedMessage = "The object was not allocated from this pool" )]
        public void ReturnTheWrongInstance()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );
            TestNode node = pool.Take();

            pool.Return( new TestNode() );
        }

        /// <summary>
        /// </summary>
        [Test]
        [ExpectedException( typeof( ArgumentException ), ExpectedMessage = "The object was already returned to the pool" )]
        public void ReturnTheInstanceTwice()
        {
            InstancePool<TestNode> pool = new InstancePool<TestNode>( 3 );
            TestNode a = pool.Take();
            TestNode b = pool.Take();

            pool.Return( a );
            pool.Return( b );
            pool.Return( a );
        }

        // TODO: Test the enumerators
    }
}
