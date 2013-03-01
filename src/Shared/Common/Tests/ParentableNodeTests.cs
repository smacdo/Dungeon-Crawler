using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common.Tests
{
    [TestFixture]
    [Category( "Common" )]
    class ParentableNodeTests
    {
        class TestObject : ParentableNode<TestObject>
        {
            public string Value { get; set; }

            public TestObject( string v )
                : base()
            {
                Value = v;
            }

            public TestObject( string v, TestObject obj )
                : base( obj )
            {
                Value = v;
            }
        }

        [Test]
        public void TestDefaultConstructor()
        {
            TestObject a = new TestObject( "a" );

            Assert.AreEqual( "a", a.Value );

            Assert.IsNull( a.Parent );
            Assert.IsNull( a.FirstChild );
            Assert.IsNull( a.LastChild );
            Assert.IsNull( a.PreviousSibling );
            Assert.IsNull( a.NextSibling );

            Assert.AreEqual( 0, a.ChildrenCount );
        }

        [Test]
        public void TestConstructorWithParent()
        {
            TestObject a = new TestObject( "a" );
            TestObject b = new TestObject( "b", a );

            // Test A
            Assert.IsNull( a.Parent );
            Assert.IsNotNull( a.FirstChild );
            Assert.IsNotNull( a.LastChild );
            Assert.IsNull( a.PreviousSibling );
            Assert.IsNull( a.NextSibling );

            Assert.AreSame( b, a.FirstChild );
            Assert.AreSame( b, a.LastChild );

            Assert.AreEqual( 1, a.ChildrenCount );

            // Test B
            Assert.AreSame( a, b.Parent );

            Assert.IsNotNull( b.Parent );
            Assert.IsNull( b.FirstChild );
            Assert.IsNull( b.LastChild );
            Assert.IsNull( b.PreviousSibling );
            Assert.IsNull( b.NextSibling );

            Assert.AreEqual( 0, b.ChildrenCount );
        }

        [Test]
        public void TestParentProperty()
        {
            TestObject a = new TestObject( "a" );
            TestObject b = new TestObject( "b" );

            // b has no parent yet
            Assert.IsNull( b.Parent );

            // now a has a parent
            b.SetParent( a );

            Assert.IsNotNull( b.Parent );
        }

        [Test]
        public void TestNextPreviousSiblingProperty()
        {
            TestObject a = new TestObject( "a" );
            TestObject b = new TestObject( "b" );

            // b has no siblings yet
            Assert.IsNull( b.NextSibling );
            Assert.IsNull( b.PreviousSibling );

            // b still has no siblings
            b.SetParent( a );
            Assert.IsNull( b.NextSibling );
            Assert.IsNull( b.PreviousSibling );

            // add another child 'c' to 'a'.
            //    a
            //   / \
            //  b - c
            TestObject c = new TestObject( "c", a );

            Assert.IsNotNull( b.NextSibling );
            Assert.AreSame( c, b.NextSibling );
            Assert.IsNull( b.PreviousSibling );

            Assert.IsNull( c.NextSibling );
            Assert.IsNotNull( c.PreviousSibling );
            Assert.AreSame( b, c.PreviousSibling );

            // add another child 'd' to 'a'
            //    a
            //   /
            //   b - c - d
            TestObject d = new TestObject( "d", a );

            Assert.AreSame( c, b.NextSibling );
            Assert.IsNull( b.PreviousSibling );

            Assert.AreSame( d, c.NextSibling );
            Assert.AreSame( b, c.PreviousSibling );

            Assert.IsNull( d.NextSibling );
            Assert.AreSame( c, d.PreviousSibling );
        }

        [Test]
        public void TestFirstChildProperty()
        {
            TestObject a = new TestObject( "a" );
            Assert.IsNull( a.FirstChild );

            TestObject b = new TestObject( "b", a );
            Assert.AreSame( b, a.FirstChild );

            TestObject c = new TestObject( "c", a );
            Assert.AreSame( b, a.FirstChild );

            TestObject d = new TestObject( "d", a );
            Assert.AreSame( b, a.FirstChild );
        }

        [Test]
        public void TestLastChildProperty()
        {
            TestObject a = new TestObject( "a" );
            Assert.IsNull( a.LastChild );

            TestObject b = new TestObject( "b", a );
            Assert.AreSame( b, a.LastChild );

            TestObject c = new TestObject( "c", a );
            Assert.AreSame( c, a.LastChild );

            TestObject d = new TestObject( "d", a );
            Assert.AreSame( d, a.LastChild );
        }

        [Test]
        public void TestChildrenProperty()
        {
 /*           List<TestObject> list;

            TestObject a = new TestObject( "a" );
            list = new List<TestObject>( a.Children );

            Assert.AreEqual( 0, list.Count );

            TestObject b = new TestObject( "b", a );
            list = new List<TestObject>( a.Children );

            Assert.AreEqual( 1, list.Count );
            Assert.AreSame( a, list[0] ); ;

            TestObject c = new TestObject( "c", a );
            Assert.AreSame( c, a.LastChild );

            TestObject d = new TestObject( "d", a );
            Assert.AreSame( d, a.LastChild );*/
        }
    }
}
