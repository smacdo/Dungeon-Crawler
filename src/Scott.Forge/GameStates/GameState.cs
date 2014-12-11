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

namespace Scott.Forge.GameStates
{
    /// <summary>
    ///  Represents a game state.
    /// </summary>
	public abstract class GameState
	{
		private bool mPaused;
        private bool mActive;
        private bool mEnabled = true;
		private bool mExclusiveDraw;
		private bool mExclusiveUpdate;
        private GameStateManager mManager;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="manager">Manager managing this game state.</param>
        /// <param name="exclusiveDraw">If this game state has exclusive draw.</param>
        /// <param name="exclusiveUpdate">If this game state has exlusive update.</param>
        public GameState( GameStateManager manager, bool exclusiveDraw, bool exclusiveUpdate )
        {
            mManager = manager;
            mExclusiveDraw = exclusiveDraw;
            mExclusiveUpdate = exclusiveUpdate;
            mPaused = false;
        }
	
        /// <summary>
        ///  Get the manager.
        /// </summary>
		protected GameStateManager Manager { get { return mManager; } }

        /// <summary>
        ///  Get if state is paused.
        /// </summary>
		public bool IsPaused { get { return mPaused; } }

        /// <summary>
        ///  Get if state has exclusive draw.
        /// </summary>
		public bool HasExclusiveDraw { get { return mExclusiveDraw; } }

        /// <summary>
        ///  Get if state has exclusive update.
        /// </summary>
		public bool HasExclusiveUpdate { get { return mExclusiveDraw; } }

        /// <summary>
        ///  Get or set if this game state is enabled.
        /// </summary>
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; } }
		
        /// <summary>
        ///  Pause the state.
        /// </summary>
		public void Pause()
		{
			if (! mPaused )
			{
                mPaused = true;
				OnPause();
			}
		}
		
        /// <summary>
        ///  Unpause the state.
        /// </summary>
		public void Unpause()
		{
			if ( mPaused )
			{
				OnUnpaused();
				mPaused = false;
			}
		}
		
        /// <summary>
        ///  Enter the state. This game state is now the actively executing game state.
        /// </summary>
		internal void Enter()
		{
            if ( !mActive )
            {
                mActive = true;
                OnEnter();
            }
			
		}
		
        /// <summary>
        ///  Leave the state. This game state is no longer active.
        /// </summary>
		internal void Leave()
		{
            if ( mActive )
            {
                OnLeave();
                mActive = false;
            }
		}
	
        /// <summary>
        ///  Update the game state using the current simulation time.
        /// </summary>
        /// <param name="gameTime">The current simulation time.</param>
        public virtual void Update(double currentTime, double deltaTime)
		{
		}
		
        /// <summary>
        ///  Draw the game state using the current simulation time.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(double currentTime, double deltaTime)
		{
		}
		
        /// <summary>
        ///  Called when the game state becomes active.
        /// </summary>
		public virtual void OnEnter()
		{
		}
		
        /// <summary>
        ///  Called when the game state is no longer active.
        /// </summary>
		public virtual void OnLeave()
		{
		}
		
        /// <summary>
        ///  Called when the game state is paused.
        /// </summary>
		public virtual void OnPause()
		{
		}

		/// <summary>
		///  Called when the game state is unpaused.
		/// </summary>
		public virtual void OnUnpaused()
		{
		}
	}
}
