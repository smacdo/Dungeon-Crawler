using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameStates;

namespace Scott.Forge.Tests.GameStates
{
    [TestClass]
    internal class GameStateTests
    {
        [TestMethod]
        public void CreateGameState()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm, true, true );

            Assert.IsTrue( tgs.HasExclusiveDraw );
            Assert.IsTrue(tgs.HasExclusiveUpdate);
        }

        [TestMethod]
        public void PauseUnpause()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.IsFalse(tgs.IsPaused);

            tgs.Pause();
            Assert.IsTrue(tgs.IsPaused);

            tgs.Unpause();
            Assert.IsFalse(tgs.IsPaused);
        }

        [TestMethod]
        public void EnableDisable()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.IsTrue(tgs.Enabled);

            tgs.Enabled = false;
            Assert.IsFalse(tgs.Enabled);

            tgs.Enabled = true;
            Assert.IsTrue(tgs.Enabled);
        }

        [TestMethod]
        public void PauseCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.IsFalse(tgs.PauseCalled);
            
            tgs.Pause();
            Assert.IsTrue(tgs.PauseCalled);
        }

        [TestMethod]
        public void UnpauseCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.IsFalse(tgs.UnpauseCalled);

            tgs.Unpause();
            Assert.IsFalse(tgs.UnpauseCalled);

            tgs.Pause();
            tgs.Unpause();
            Assert.IsTrue(tgs.UnpauseCalled);
        }

        [TestMethod]
        [Ignore]
        public void EnterCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.IsFalse(tgs.EnterCalled);

            //tgs.Enter();
            Assert.IsTrue(tgs.EnterCalled);
        }

        [TestMethod]
        [Ignore]
        public void LeaveCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.IsFalse(tgs.LeaveCalled);

            //tgs.Leave();
            Assert.IsFalse(tgs.LeaveCalled);

            //tgs.Enter();
            //tgs.Leave();
            Assert.IsTrue(tgs.LeaveCalled);
        }

        internal class TestGameState : GameState
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
