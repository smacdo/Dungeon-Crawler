#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common.Tests
{
    [TestFixture]
    public class TypeExtensionMethodsTests
    {
        [Test]
        public void HasDefaultConstructor()
        {
            Assert.IsTrue( typeof( Foo ).HasDefaultConstructor() );
            Assert.IsFalse( typeof( Bar ).HasDefaultConstructor() );
        }

        [Test]
        public void HasAttributeGenric()
        {
            Assert.IsTrue( typeof( Foo ).HasAttribute<TestAttributeA>() );
            Assert.IsTrue( typeof( Foo ).HasAttribute<TestAttributeB>() );
            Assert.IsTrue( typeof( Bar ).HasAttribute<TestAttributeB>() );
            Assert.IsFalse( typeof( Bar ).HasAttribute<TestAttributeA>() );
        }

        [Test]
        public void HasAttribute()
        {
            Assert.IsTrue( typeof( Foo ).HasAttribute( typeof( TestAttributeA ) ) );
            Assert.IsTrue( typeof( Foo ).HasAttribute( typeof( TestAttributeB ) ) );
            Assert.IsTrue( typeof( Bar ).HasAttribute( typeof( TestAttributeB ) ) );
            Assert.IsFalse( typeof( Bar ).HasAttribute( typeof( TestAttributeA ) ) );
        }

        [Test]
        public void TryFindAttribute()
        {
            TestAttributeA a = null;
            bool result = typeof( Foo ).TryFindAttribute<TestAttributeA>( ref a );

            Assert.IsTrue( result );
            Assert.IsNotNull( a );

            // C# does not promise ordering, so it could either be 1 or 42. Lets check...
            bool areValuesPresent = false;

            if ( a.Val == 1 || a.Val == 42 )
            {
                areValuesPresent = true;
            }

            Assert.IsTrue( areValuesPresent );

            Assert.IsFalse( typeof( Bar ).TryFindAttribute<TestAttributeA>( ref a ) );
        }

        [Test]
        public void TryFindAttributes()
        {
            TestAttributeA[] a = null;
            bool result = typeof( Foo ).TryFindAttributes<TestAttributeA>( ref a );

            Assert.IsTrue( result );
            Assert.IsNotNull( a );
            Assert.AreEqual( 2, a.Length );

            // are the two values there?
            bool areValuesPresent = false;

            if ( ( a[0].Val == 1 && a[1].Val == 42 ) || ( a[0].Val == 42 && a[1].Val == 1 ) )
            {
                areValuesPresent = true;
            }

            Assert.IsTrue( areValuesPresent );

            Assert.IsFalse( typeof( Bar ).TryFindAttributes<TestAttributeA>( ref a ) );
        }

        [System.AttributeUsage(System.AttributeTargets.Class,AllowMultiple=true)]
        class TestAttributeA : System.Attribute
        {
            public int Val { get; private set; }
            public TestAttributeA( int v )
            {
                Val = v;
            }
        }

        [System.AttributeUsage( System.AttributeTargets.Class, AllowMultiple = true )]
        class TestAttributeB : System.Attribute
        {
            public TestAttributeB()
            {
                // Empty
            }
        }

        [TestAttributeA(1)]
        [TestAttributeA(42)]
        [TestAttributeB]
        class Foo
        {
            public Foo()
            {
                // Empty
            }
        }

        [TestAttributeB]
        class Bar
        {
            public Bar( int x, int y )
            {
                // Empty
            }
        }

    }
}
