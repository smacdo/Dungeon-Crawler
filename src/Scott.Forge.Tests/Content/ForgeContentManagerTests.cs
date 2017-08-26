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
    public class ForgeContentManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Creating_Content_Manager_With_Null_Content_Directories_Throws_Exception()
        {
            new ForgeContentManager(null, _contentHandlerDirectories);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Creating_Content_Manager_With_Null_Content_Handler_Directories_Throws_Exception()
        {
            new ForgeContentManager(_contentContainers, null);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidAssetNameException))]
        public async void Load_Asset_Throws_Exception_If_Name_Is_Null()
        {
            var c = new ForgeContentManager(_contentContainers, _contentHandlerDirectories);
            await c.Load<TestableAsset>(null);
        }

        [TestMethod]
        //[ExpectedException(typeof(InvalidAssetNameException))]
        public void Load_Asset_Throws_Exception_If_Name_Is_Not_Found()
        {
            var c = new ForgeContentManager(_contentContainers, _contentHandlerDirectories);
            //c.Load<TestableAsset>(@"i\do\not\exist.file");
        }

        [TestMethod]
        public void Is_Xnb_File_For_Asset_Paths()
        {
            Assert.IsTrue(ForgeContentManager.IsXnbFile("a.xnb"));
            Assert.IsTrue(ForgeContentManager.IsXnbFile("b.xnb"));
            Assert.IsTrue(ForgeContentManager.IsXnbFile("c.foo.xnb"));

            Assert.IsFalse(ForgeContentManager.IsXnbFile("c.axnb"));
            Assert.IsFalse(ForgeContentManager.IsXnbFile("c.xnbc"));
            Assert.IsFalse(ForgeContentManager.IsXnbFile("xnb"));
            Assert.IsFalse(ForgeContentManager.IsXnbFile("a.png"));
            Assert.IsFalse(ForgeContentManager.IsXnbFile("a"));
            Assert.IsFalse(ForgeContentManager.IsXnbFile(""));
            Assert.IsFalse(ForgeContentManager.IsXnbFile(null));
        }

        [TestMethod]
        public void Get_Asset_Path_Without_Extension_For_Asset_Paths()
        {
            Assert.AreEqual("foobar", ForgeContentManager.GetAssetPathWithoutExtension("foobar.png"));
            Assert.AreEqual("barfoo", ForgeContentManager.GetAssetPathWithoutExtension("barfoo.png"));
            Assert.AreEqual("foobar", ForgeContentManager.GetAssetPathWithoutExtension("foobar"));
            Assert.AreEqual("foobar.x", ForgeContentManager.GetAssetPathWithoutExtension("foobar.x.png"));
            Assert.AreEqual(@"a\foobar", ForgeContentManager.GetAssetPathWithoutExtension(@"a\foobar.png"));
            Assert.AreEqual(@"ab\c\foobar", ForgeContentManager.GetAssetPathWithoutExtension(@"ab\c\foobar.png"));

            Assert.AreEqual(null, ForgeContentManager.GetAssetPathWithoutExtension(null));
            Assert.AreEqual("", ForgeContentManager.GetAssetPathWithoutExtension(""));
            Assert.AreEqual("  ", ForgeContentManager.GetAssetPathWithoutExtension("  "));
        }

        private IList<IContentHandlerDirectory> _contentHandlerDirectories =
            new List<IContentHandlerDirectory>() { new ContentHandlerDirectory() };

        private IList<IContentContainer> _contentContainers =
            new List<IContentContainer>()
            {
                new TestableContentContainer(new Dictionary<string, TestableAsset>()
                {
                })
            };


        private class TestableAsset
        {
            public TestableAsset(string value)
            {
                Value = value;
            }

            public string Value { get; set; }
            
            public byte[] ToBytes()
            {
                // TODO: Implement me.
                return null;
            }
        }

        private class TestableAssetContentReader : IContentReader<TestableAsset>
        {
            public Task<TestableAsset> Read(Stream inputStream, string assetPath, IContentManager content)
            {
                // TODO: Implement me.
                throw new NotImplementedException();
            }
        }

        private class TestableContentContainer : IContentContainer
        {
            public IDictionary<string, byte[]> Assets { get; set; }

            public TestableContentContainer(IDictionary<string, TestableAsset> assets)
            {
                Assets = new Dictionary<string, byte[]>();

                foreach (var entry in assets)
                {
                    Assets.Add(entry.Key, entry.Value.ToBytes());
                }
            }

            public Task<bool> TryReadItem(string assetName, ref Stream readStream)
            {
                if (Assets.ContainsKey(assetName))
                {
                    // TODO: Return stream reading bytes.
                }

                return Task.FromResult(false);
            }
        }
    }
}
