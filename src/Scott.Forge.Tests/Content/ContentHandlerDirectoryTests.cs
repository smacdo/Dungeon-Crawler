using System;
/*
 * Copyright 2012-2017 Scott MacDonald
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Graphics;
using Scott.Forge.Tilemaps;
using Scott.Forge.Content;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scott.Forge.Tests.Content
{
    [TestClass]
    public class ContentHandlerDirectoryTests
    {
        [TestMethod]
        public void Can_Find_Content_Reader_By_Extension_After_Adding()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { "foobar" });

            var result = dir.GetContentReaderFor<FooAsset>("foobar");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FooAssetContentReader));
        }

        [TestMethod]
        public void Can_Find_Content_Reader_By_Multiple_Extensions_After_Adding()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { "foo", "bar" });

            var result = dir.GetContentReaderFor<FooAsset>("bar");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FooAssetContentReader));

            result = dir.GetContentReaderFor<FooAsset>("foo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FooAssetContentReader));
        }

        [TestMethod]
        [ExpectedException(typeof(ContentReaderMissingException))]
        public void Cannot_Find_Content_Reader_Of_Wrong_Type_With_Same_Extension()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { "foobar" });

            dir.GetContentReaderFor<BarAsset>("foobar");
        }

        [TestMethod]
        [ExpectedException(typeof(ContentReaderMissingException))]
        public void Find_Content_Reader_Throws_Exception_If_Nothing_Found()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { "foobar" });

            dir.GetContentReaderFor<FooAsset>("barfoo");
        }

        [TestMethod]
        public void Try_Find_Content_Reader_Returns_True_If_Ok_False_Otherwise()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { "foobar" });

            IContentReader<FooAsset> f = null;

            Assert.IsTrue(dir.TryGetContentReaderFor<FooAsset>("foobar", ref f));
            Assert.IsFalse(dir.TryGetContentReaderFor<FooAsset>("barfoo", ref f));
        }

        [TestMethod]
        public void Readers_Are_Matched_To_Content_Type_They_Were_Registered_With()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { "foo", "foobar" });
            dir.Add(typeof(BarAsset), typeof(BarAssetContentReader), new string[] { "bar", "foobar" });

            // Foo and foobar are both valid.
            var result = dir.GetContentReaderFor<FooAsset>("foo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FooAssetContentReader));

            result = dir.GetContentReaderFor<FooAsset>("foobar");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FooAssetContentReader));

            // Bar and fooba are valid.
            var result2 = dir.GetContentReaderFor<BarAsset>("bar");

            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result2, typeof(BarAssetContentReader));

            result2 = dir.GetContentReaderFor<BarAsset>("foobar");

            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result2, typeof(BarAssetContentReader));

            // Bar is not valid for FooAsset.
            IContentReader<FooAsset> f = null;
            Assert.IsFalse(dir.TryGetContentReaderFor<FooAsset>("bar", ref f));

            // Foo is not valid for BarAsset.
            IContentReader<BarAsset> f1 = null;
            Assert.IsFalse(dir.TryGetContentReaderFor<BarAsset>("foo", ref f1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_With_Null_Content_Type_Throws_Exception()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(null, typeof(FooAssetContentReader), new string[] { "foobar" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_With_Null_Content_Reader_Type_Throws_Exception()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), null, new string[] { "foobar" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_With_Null_File_Extension_List_Throws_Exception()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_With_Empty_File_Extension_List_Throws_Exception()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] {});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_With_Whitespace_File_Extension_Name_Throws_Exception()
        {
            var dir = new ContentHandlerDirectory();
            dir.Add(typeof(FooAsset), typeof(FooAssetContentReader), new string[] { " " });
        }

        private class FooAsset
        {
        }

        private class FooAssetContentReader : IContentReader<FooAsset>
        {
            public Task<FooAsset> Read(Stream inputStream, string assetPath, IContentManager content)
            {
                throw new NotImplementedException();
            }
        }

        private class BarAsset
        {
        }

        private class BarAssetContentReader : IContentReader<BarAsset>
        {
            public Task<BarAsset> Read(Stream inputStream, string assetPath, IContentManager content)
            {
                throw new NotImplementedException();
            }
        }
    }
}