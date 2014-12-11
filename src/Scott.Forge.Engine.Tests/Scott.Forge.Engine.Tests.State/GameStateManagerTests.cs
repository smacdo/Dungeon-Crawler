#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.State.Tests
{
    [TestClass]
    internal class GameStateManagerTests
    {
        [TestMethod]
        public void New()
        {
            GameStateManager gsm = new GameStateManager();
        }

        [TestMethod]
        public void PauseUnpause()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.False( tgs.IsPaused );

            tgs.Pause();
            Assert.True( tgs.IsPaused );

            tgs.Unpause();
            Assert.False( tgs.IsPaused );
        }

        [TestMethod]
        public void EnableDisable()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.True( tgs.Enabled );

            tgs.Enabled = false;
            Assert.False( tgs.Enabled );

            tgs.Enabled = true;
            Assert.True( tgs.Enabled );
        }

        [TestMethod]
        public void ActiveState()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState a = new TestGameState( gsm );
            TestGameState b = new TestGameState( gsm );
            TestGameState c = new TestGameState( gsm );

            Assert.IsNull( gsm.ActiveState );

            gsm.Push( a );
            Assert.AreSame( a, gsm.ActiveState );

            gsm.Push( b );
            Assert.AreSame( b, gsm.ActiveState );

            gsm.Push( c );
            Assert.AreSame( c, gsm.ActiveState );

            gsm.Pop();
            Assert.AreSame( b, gsm.ActiveState );

            gsm.Pop();
            Assert.AreSame( a, gsm.ActiveState );

            gsm.Pop();
            Assert.IsNull( gsm.ActiveState );
        }


        [TestMethod]
        public void PushStateMakesPreviousStatePaused()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState a = new TestGameState( gsm );

            Assert.IsFalse( a.IsPaused );
            gsm.Push( a );

            TestGameState b = new TestGameState( gsm );

            Assert.IsFalse( b.IsPaused );
            gsm.Push( b );

            Assert.IsTrue( a.IsPaused );
            Assert.IsFalse( b.IsPaused );
        }

        [TestMethod]
        public void PausePausesTopState()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState a = new TestGameState( gsm );

            gsm.Push( a );
            gsm.Pause();

            Assert.IsTrue( a.IsPaused );
        }

        [TestMethod]
        public void TestIfDisposingLeavesActiveState()
        {
            using ( GameStateManager gsm = new GameStateManager() )
            {

            }
        }

        class TestGameState : GameState
        {
            public bool EnterCalled = false;
            public bool LeaveCalled = false;
            public bool PauseCalled = false;
            public bool UnpauseCalled = false;

            public TestGameState( GameStateManager manager )
                : base( manager, false, false )
            {

            }


            public TestGameState( GameStateManager manager, bool xDraw, bool xUpdate )
                : base( manager, xDraw, xUpdate )
            {

            }

            public override void OnEnter()
            {
                EnterCalled = true;
            }

            public override void OnLeave()
            {
                LeaveCalled = true;
            }

            public override void OnPause()
            {
                PauseCalled = true;
            }

            public override void OnUnpaused()
            {
                UnpauseCalled = true;
            }
        }
    }
}
