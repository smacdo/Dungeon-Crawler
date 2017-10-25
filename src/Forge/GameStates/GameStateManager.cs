/*
 * Copyright 2013-2014 Scott MacDonald
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
using Forge.GameStates;

namespace Forge.GameStates
{
    /// <summary>
    ///  Manages the game state stack.
    /// </summary>
    public class GameStateManager : IDisposable
    {
        const int DefaultMaxStackCount = 16;
        private List<GameState> mStates = new List<GameState>( DefaultMaxStackCount );
        private bool mDisposeStates = false;
        private bool mPaused = false;
        private bool mEnabled = true;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public GameStateManager()
        {
            // Empty
        }

        /// <summary>
        ///  Get or set if this game state manager is enabled.
        /// </summary>
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; } }

        /// <summary>
        ///  Get how many states are pushed onto the game state stack.
        /// </summary>
        public int Count { get { return mStates.Count; } }

        /// <summary>
        ///  Get the current active state.
        /// </summary>
        public GameState ActiveState
        {
            get
            {
                if ( Count < 1 )
                {
                    return null;
                }
                else
                {
                    return mStates[mStates.Count - 1];
                }
            }
        }

        /// <summary>
        ///  Dispose the game state manager.
        /// </summary>
        public void Dispose()
        {
            // Get out NAOW.
            LeaveAllGameStates();
        }

        /// <summary>
        ///  Pause the game state manager and all running states.
        /// </summary>
        public void Pause()
        {
            if ( !mPaused )
            {
                mPaused = true;

                foreach ( GameState state in mStates )
                {
                    state.Pause();
                }
            }
        }

        /// <summary>
        ///  Unpause the game state manager.
        /// </summary>
        public void Unpause()
        {
            if ( mPaused )
            {
                mPaused = false;

                foreach ( GameState state in mStates )
                {
                    state.Unpause();
                }
            }
        }

        /// <summary>
        ///  Push a new game state onto the game state stack, and make it active.
        /// </summary>
        /// <param name="state">New active state.</param>
        public void Push( GameState state )
        {
            // Pause the current state and push a new one onto the stack.
            if ( Count > 0 )
            {
                mStates[Count - 1].Pause();
            }

            mStates.Add( state );

            // Let the state know that it is active now.
            state.Enter();
        }
        
        /// <summary>
        ///  Pop the topmost game state and make the game state underneath it the active state.
        /// </summary>
        public void Pop()
        {
            // We can't pop if there are no game states to pop off.
            if ( Count < 1 )
            {
                throw new GameStateException( "There are no states to pop" );
            }

            // Get the newest state placed on the stack and remove it from the state stack.
            GameState oldState = mStates[Count - 1];
            mStates.RemoveAt( Count - 1 );

            oldState.Leave();
            TryDispose( oldState );

            // Unpause the topmost state to make it the new active state.
            if ( Count > 0 )
            {
                mStates[Count - 1].Unpause();
            }
        }


        /// <summary>
        ///  Leave the current active game state, and switch it to the provided game state.
        /// </summary>
        /// <param name="state">The state to switch to.</param>
        public void SwitchTo( GameState state )
        {
            if ( Count < 1 )
            {
                Push( state );
            }
            else
            {
                // Pop the top state off the stack.
                GameState oldState = mStates[Count - 1];

                mStates.RemoveAt( Count - 1 );
                oldState.Leave();

                TryDispose( oldState );

                // Push the new state onto the stack and make it the active state.
                mStates.Add( state );
                state.Enter();
            }
        }


        /// <summary>
        ///  Leave all current games states.
        /// </summary>
        public void LeaveAllGameStates()
        {
            while ( mStates.Count > 0 )
            {
                Pop();
            }
        }

        /// <summary>
        ///  Update the game state manager.
        /// </summary>
        /// <param name="gameTime">Current simulation time.</param>
        public void Update(double currentTime, double deltaTime)
        {
            // Update all of our active game states, starting from the top of the stack. Stop
            // calling update once we hit a game state that has exclusive update. (That way we
            // won't update any states underneath the exclusive state).
            for ( int i = Count - 1; i >= 0; --i )
            {
                GameState state = mStates[i];

                // Is the game state enabled? Only update it if it is enabled.
                if ( state.Enabled )
                {
                    state.Update(currentTime, deltaTime);
                }

                // Is this an exclusive update state? If so we should bail out of the update loop.
                if ( state.HasExclusiveUpdate )
                {
                    break;
                }
            }
        }

        /// <summary>
        ///  Let the game state manager draw on the screen.
        /// </summary>
        /// <param name="gameTime">The current simulation time.</param>
        public void Draw(double currentTime, double deltaTime)
        {
            // Draw all of our active game states, starting from the top of the stack. Stop
            // calling draw once we hit a game state that has exclusive update. (That way we
            // won't draw any states underneath the exclusive state).
            for ( int i = Count - 1; i >= 0; --i )
            {
                GameState state = mStates[i];

                // Is the game state enabled? Only draw it if it is enabled.
                if ( state.Enabled )
                {
                    state.Draw(currentTime, deltaTime);
                }

                // Is this an exclusive draw state? If so we should bail out of the draw loop.
                if ( state.HasExclusiveDraw )
                {
                    break;
                }
            }
        }

        /// <summary>
        ///  If the state extends IDisposable and requires disposal, this method will invoke
        ///  Dispose on that instance.
        /// </summary>
        /// <param name="state">Game state to dispose of.</param>
        private void TryDispose( GameState state )
        {
            if ( mDisposeStates && state is IDisposable )
            {
                ( (IDisposable) state ).Dispose();
            }
        }
    }
}
