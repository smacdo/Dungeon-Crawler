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

namespace Scott.Forge.Tests.Content
{
    [TestClass]
    public class ForgeContentManagerTests
    {
        private IList<IContentHandlerDirectory> mContentHandlerDirectories =
            new List<IContentHandlerDirectory>() { new ContentHandlerDirectory() };

        private IList<IContentContainer> mContentContainers =
            new List<IContentContainer>()
            {
                new TestableContentContainer(new Dictionary<string, TestableAsset>()
                {
                })
            };

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Creating_Content_Manager_With_Null_Content_Directories_Throws_Exception()
        {
            new ForgeContentManager(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAssetNameException))]
        public void Load_Asset_Throws_Exception_If_Name_Is_Null()
        {
            var c = new ForgeContentManager(mContentContainers, mContentHandlerDirectories);
            c.Load<TestableAsset>(null);
        }

        [TestMethod]
        //[ExpectedException(typeof(InvalidAssetNameException))]
        public void Load_Asset_Throws_Exception_If_Name_Is_Not_Found()
        {
            var c = new ForgeContentManager(mContentContainers, mContentHandlerDirectories);
            //c.Load<TestableAsset>(@"i\do\not\exist.file");
        }

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
            public TestableAsset Read(Stream inputStream, string assetPath, IContentManager content)
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

            public bool TryReadItem(string assetName, ref Stream readStream)
            {
                if (Assets.ContainsKey(assetName))
                {
                    // TODO: Return stream reading bytes.
                }

                return false;
            }
        }
    }
}
