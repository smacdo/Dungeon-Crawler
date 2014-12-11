#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.State.Tests
{
    [TestClass]
    internal class GameStateTests
    {
        [TestMethod]
        public void CreateGameState()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm, true, true );

            Assert.True( tgs.HasExclusiveDraw );
            Assert.True( tgs.HasExclusiveUpdate );
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
        public void PauseCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.False( tgs.PauseCalled );
            
            tgs.Pause();
            Assert.True( tgs.PauseCalled );
        }

        [TestMethod]
        public void UnpauseCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.False( tgs.UnpauseCalled );

            tgs.Unpause();
            Assert.False( tgs.UnpauseCalled );

            tgs.Pause();
            tgs.Unpause();
            Assert.True( tgs.UnpauseCalled );
        }


        [TestMethod]
        public void EnterCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.False( tgs.EnterCalled );

            tgs.Enter();
            Assert.True( tgs.EnterCalled );
        }

        [TestMethod]
        public void LeaveCalled()
        {
            GameStateManager gsm = new GameStateManager();
            TestGameState tgs = new TestGameState( gsm );

            Assert.False( tgs.LeaveCalled );

            tgs.Leave();
            Assert.False( tgs.LeaveCalled );

            tgs.Enter();
            tgs.Leave();
            Assert.True( tgs.LeaveCalled );
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
