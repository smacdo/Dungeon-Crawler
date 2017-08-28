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
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Content;
using System.Threading.Tasks;

namespace Scott.Forge.Tests.Content
{
    [TestClass]
    public class ForgeContentManagerTests
    {
        // TODO: // TEST: .XNB fallback loading.

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
        public async Task Load_Assets_From_Asset_Containers_As_Expected_Types()
        {
            var teaAsset = await _contentManager.Load<FooAsset>("tea.foo");
            Assert.AreEqual(_teaAsset, teaAsset);

            var teaAsset2 = await _contentManager.Load<FooAsset>("tea.foox");
            Assert.AreEqual(_teaAssetX, teaAsset2);

            var coffeeAsset = await _contentManager.Load<BarAsset>("drinks\\coffee.bar");
            Assert.AreEqual(_coffeeAsset, coffeeAsset);
        }

        [TestMethod]
        public async Task Load_Will_Search_Containers_Until_Asset_Is_Located()
        {
            var a = await _contentManager.Load<FooAsset>("secondContentContainerAsset.foo");
            Assert.AreEqual(_secondContentContainerAsset, a);
        }

        [TestMethod]
        public async Task Load_Will_Normalize_Directory_Separator_Chars()
        {
            var a = await _contentManager.Load<BarAsset>("drinks\\coffee.bar");
            Assert.AreEqual(_coffeeAsset, a);

            var b = await _contentManager.Load<BarAsset>("drinks/coffee.bar");
            Assert.AreEqual(_coffeeAsset, b);
        }
        
        [TestMethod]
        public async Task Load_Will_Cache_Assets_For_Future_Loads()
        {
            var a = await _contentManager.Load<FooAsset>("tea.foo");
            Assert.AreEqual(_teaAsset, a);

            var b = await _contentManager.Load<FooAsset>("tea.foo");
            Assert.AreSame(a, b);
        }

        [TestMethod]
        [ExpectedException(typeof(ContentReaderNotFoundException))]
        public async Task Load_Will_Throw_Exception_If_Type_Does_Not_Match_Reader_Extensions()
        {
            await _contentManager.Load<BarAsset>("tea.foo");
        }
        
        [TestMethod]
        public async Task Check_If_Asset_Is_Cached_After_Loading()
        {
            Assert.IsFalse(_contentManager.IsLoaded("tea.foo"));

            var a = await _contentManager.Load<FooAsset>("tea.foo");
            Assert.IsTrue(_contentManager.IsLoaded("tea.foo"));
        }

        [TestMethod]
        [ExpectedException(typeof(AssetWrongTypeException))]
        public async Task Load_Cached_Asset_As_Wrong_Type_Will_Throw_Exception()
        {
            var a = await _contentManager.Load<FooAsset>("tea.foo");
            await _contentManager.Load<BarAsset>("tea.foo");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAssetNameException))]
        public async Task Load_Asset_Throws_Exception_If_Name_Is_Null()
        {
            await _contentManager.Load<FooAsset>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAssetNameException))]
        public async Task Load_Asset_Throws_Exception_If_Name_Is_Empty()
        {
            await _contentManager.Load<FooAsset>("");
        }

        [TestMethod]
        [ExpectedException(typeof(AssetNotFoundException))]
        public async Task Load_Asset_Throws_Exception_If_Name_Is_Not_Found()
        {
            await _contentManager.Load<FooAsset>(@"i\do\not\exist.foo");
        }

        [TestMethod]
        [ExpectedException(typeof(ContentReaderNotFoundException))]
        public async Task Load_Asset_Throws_Exception_If_Extension_Is_Not_Registered()
        {
            await _contentManager.Load<FooAsset>(@"i\do\not\exist.blah");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task Load_Asset_Throws_Exception_If_Loader_Is_Disposed()
        {
            _contentManager.Dispose();
            await _contentManager.Load<FooAsset>("tea.foo");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Is_Loaded_Throws_Exception_If_Loader_Is_Disposed()
        {
            _contentManager.Dispose();
            _contentManager.IsLoaded("tea.foo");
        }

        [TestMethod]
        public async Task Unload_Asset_Removes_Object_From_Cache()
        {
            await _contentManager.Load<FooAsset>("tea.foo");
            Assert.IsTrue(_contentManager.IsLoaded("tea.foo"));

            _contentManager.Unload("tea.foo");
            Assert.IsFalse(_contentManager.IsLoaded("tea.foo"));
        }

        [TestMethod]
        public async Task Unload_Asset_Will_Call_Dispose_If_Object_Supports_IDiposable()
        {
            var a = await _contentManager.Load<BarAsset>("drinks\\coffee.bar");
            Assert.IsFalse(a.WasDisposed);

            _contentManager.Unload("drinks\\coffee.bar");
            Assert.IsTrue(a.WasDisposed);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Unload_Asset_Throws_Exception_If_Loader_Is_Disposed()
        {
            _contentManager.Dispose();
            _contentManager.Unload("drinks\\coffee.bar");
        }

        [TestMethod]
        public async Task Unload_All_Calls_Unload_On_All_Assets()
        {
            var a = await _contentManager.Load<FooAsset>("tea.foo");
            var b = await _contentManager.Load<FooAsset>("secondContentContainerAsset.foo");
            var c = await _contentManager.Load<BarAsset>("drinks\\coffee.bar");

            Assert.IsTrue(_contentManager.IsLoaded("tea.foo"));
            Assert.IsTrue(_contentManager.IsLoaded("secondContentContainerAsset.foo"));
            Assert.IsTrue(_contentManager.IsLoaded("drinks\\coffee.bar"));

            _contentManager.Unload();

            Assert.IsFalse(_contentManager.IsLoaded("tea.foo"));
            Assert.IsFalse(_contentManager.IsLoaded("secondContentContainerAsset.foo"));
            Assert.IsFalse(_contentManager.IsLoaded("drinks\\coffee.bar"));
        }

        // TEST: Unload all.

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Unload_All_Throws_Exception_If_Loader_Is_Disposed()
        {
            _contentManager.Dispose();
            _contentManager.Unload();
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

        private FooAsset _teaAsset = new FooAsset("tea.foo", "black tea yum");
        private FooAsset _teaAssetX = new FooAsset("tea.foox", "black tea yum");
        private BarAsset _coffeeAsset = new BarAsset("drinks\\coffee.bar", 42);
        private FooAsset _secondContentContainerAsset = new FooAsset("secondContentContainerAsset.foo", "blah");

        private IList<IContentHandlerDirectory> _contentHandlerDirectories;
        private IList<IContentContainer> _contentContainers;
        private ForgeContentManager _contentManager;
        
        [TestInitialize]
        public void InitializeContentManagerValues()
        {
            // Configure content readers.
            var firstContentHandlerDirectory = new ContentHandlerDirectory();
            firstContentHandlerDirectory.Add<FooAsset>(
                typeof(FooAssetContentReader),
                new string[] { ".foo", ".foox" }
            );

            var secondContentHandlerDirectory = new ContentHandlerDirectory();
            secondContentHandlerDirectory.Add<BarAsset>(
                typeof(BarAssetContentReader),
                new string[] { ".bar" }
            );
            
            _contentHandlerDirectories = new ContentHandlerDirectory[]
            {
                firstContentHandlerDirectory,
                secondContentHandlerDirectory
            };

            // Configure content containers.
            var defaultContentContainer = new TestableContentContainer();
            defaultContentContainer.Add(_teaAsset);
            defaultContentContainer.Add(_coffeeAsset);
            defaultContentContainer.Add(_teaAssetX);

            var secondaryContentContainer = new TestableContentContainer();
            secondaryContentContainer.Add(_secondContentContainerAsset);

            _contentContainers = new TestableContentContainer[]
            {
                defaultContentContainer,
                secondaryContentContainer
            };

            // Configure content manager.
            _contentManager = new ForgeContentManager(_contentContainers, _contentHandlerDirectories);
        }
    }
}
