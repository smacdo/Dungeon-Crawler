using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scott.Forge.Tests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class PairTests
    {
        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void ValueConstructorSetsProperties()
        {
            var a = new Pair<int, string>(1, "moo");
            Assert.AreEqual(1, a.First);
            Assert.AreEqual("moo", a.Second);
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void KeyValueConstructorSetsProperties()
        {
            var a = new Pair<int, string>(new KeyValuePair<int, string>(2, "baaaaah"));
            Assert.AreEqual(2, a.First);
            Assert.AreEqual("baaaaah", a.Second);
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void CanSetFirstAndSecondValues()
        {
            var a = new Pair<int, string>(1, "moo");

            a.First = 42;
            Assert.AreEqual(42, a.First);

            a.Second = "meow";
            Assert.AreEqual("meow", a.Second);
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void Comparable()
        {
            var a = new Pair<int, string>(1, "b");
            var b = new Pair<int, string>(1, "b");
            var c = new Pair<int, string>(2, "b");
            var d = new Pair<int, string>(2, "c");
            var e = new Pair<int, string>(0, "b");
            var f = new Pair<int, string>(0, "a");
            var g = new Pair<int, string>(1, "c");
            var h = new Pair<int, string>(1, "a");

            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(0, ((IComparable) a).CompareTo(a));

            Assert.AreEqual(0, a.CompareTo(b));
            Assert.AreEqual(0, ((IComparable) a).CompareTo(b));
            Assert.IsFalse(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsTrue(a >= b);

            // a < c
            Assert.AreEqual(-1, a.CompareTo(c));
            Assert.AreEqual(-1, ((IComparable) a).CompareTo(c));
            Assert.IsTrue(a < c);
            Assert.IsTrue(a <= c);
            Assert.IsFalse(a > c);
            Assert.IsFalse(a >= c);

            // a < d
            Assert.AreEqual(-1, a.CompareTo(d));
            Assert.AreEqual(-1, ((IComparable) a).CompareTo(d));
            Assert.IsTrue(a < d);
            Assert.IsTrue(a <= d);
            Assert.IsFalse(a > d);
            Assert.IsFalse(a >= d);

            // a > e
            Assert.AreEqual(1, a.CompareTo(e));
            Assert.AreEqual(1, ((IComparable) a).CompareTo(e));
            Assert.IsFalse(a < e);
            Assert.IsFalse(a <= e);
            Assert.IsTrue(a > e);
            Assert.IsTrue(a >= e);

            // a > f
            Assert.AreEqual(1, a.CompareTo(f));
            Assert.AreEqual(1, ((IComparable) a).CompareTo(f));
            Assert.IsFalse(a < f);
            Assert.IsFalse(a <= f);
            Assert.IsTrue(a > f);
            Assert.IsTrue(a >= f);

            // a < g
            Assert.AreEqual(-1, a.CompareTo(g));
            Assert.AreEqual(-1, ((IComparable) a).CompareTo(g));
            Assert.IsTrue(a < g);
            Assert.IsTrue(a <= g);
            Assert.IsFalse(a > g);
            Assert.IsFalse(a >= g);

            // a > h
            Assert.AreEqual(1, a.CompareTo(h));
            Assert.AreEqual(1, ((IComparable) a).CompareTo(h));
            Assert.IsFalse(a < h);
            Assert.IsFalse(a <= h);
            Assert.IsTrue(a > h);
            Assert.IsTrue(a >= h);
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void Equality()
        {
            var a = new Pair<int, string>(1, "b");
            var b = new Pair<int, string>(1, "b");
            var c = new Pair<int, string>(2, "b");
            var d = new Pair<int, string>(1, "c");
            var e = new Pair<int, string>(1, "x");

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object) a));

            // a == b
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object) b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);

            // a != c
            Assert.IsFalse(a.Equals(c));
            Assert.IsFalse(a.Equals((object) c));
            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);

            // a != d
            Assert.IsFalse(a.Equals(d));
            Assert.IsFalse(a.Equals((object) d));
            Assert.IsFalse(a == d);
            Assert.IsTrue(a != d);

            // a != e
            Assert.IsFalse(a.Equals(e));
            Assert.IsFalse(a.Equals((object) e));
            Assert.IsFalse(a == e);
            Assert.IsTrue(a != e);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void HashCode()
        {
            var a = new Pair<int, string>(1, "b");
            var b = new Pair<int, string>(1, "b");
            var c = new Pair<int, string>(2, "b");

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void ToKeyValuePair()
        {
            var a = new Pair<int, string>(1, "moo");
            var expected = new KeyValuePair<int, string>(1, "moo");
            Assert.AreEqual(expected, a.ToKeyValuePair());
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void DefaultToString()
        {
            var a = new Pair<int?, string>(1, "moo");
            var b = new Pair<int?, string>(null, "meow");
            var c = new Pair<int?, string>(42, null);
            var d = new Pair<int?, string>(null, null);

            Assert.AreEqual("1 moo", a.ToString());
            Assert.AreEqual("meow", b.ToString());
            Assert.AreEqual("42", c.ToString());
            Assert.AreEqual("", d.ToString());
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void ConvertToKeyValuePair()
        {
            var a = new Pair<int, string>(1, "moo");
            var expected = new KeyValuePair<int, string>(1, "moo");
            KeyValuePair<int, string> result = (KeyValuePair<int, string>) a;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Forge/Pair")]
        public void ConvertFromKeyValuePair()
        {
            var a = new KeyValuePair<int, string>(1, "moo");
            var expected = new Pair<int, string>(1, "moo");
            Pair<int, string> result = (Pair<int, string>) a;

            Assert.AreEqual(expected, result);
        }
    }
}
