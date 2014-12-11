/*
 * Copyright 2012-2014 Scott MacDonald
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Scott.Forge.Engine.Input
{
    /// <summary>
    ///  Manages input mappings and input reading.
    /// </summary>
    public class InputManager<ActionType> where ActionType : struct, IComparable, IConvertible, IFormattable
    {
        private class ActionData
        {
            public ActionType InputAction = default(ActionType);
            public bool TriggeredThisFrame = false;
            public DirectionName DirectionThisFrame = DirectionName.East;
            public bool IsDirectional = false;
            public Dictionary<Keys, DirectionName> KeyboardKeys = new Dictionary<Keys, DirectionName>();

            public ActionData( ActionType type, Keys k )
            {
                InputAction = type;
                KeyboardKeys.Add(k, DirectionName.East);
            }

            public ActionData(ActionType type, Keys k, DirectionName d)
            {
                InputAction = type;
                IsDirectional = true;
                KeyboardKeys.Add( k, d );
            }
        }

        private Dictionary<ActionType, ActionData> mActions;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public InputManager()
        {
            mActions = new Dictionary<ActionType, ActionData>();
        }

        /// <summary>
        ///  Adds a keyboard binding that corresponds to an input action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        public void AddAction( ActionType actionType, Keys key )
        {
            // See if this action has already been created. Either update the existing action data,
            // or create a new entry.
            ActionData action = FindActionForType( actionType );

            if ( action != null )
            {
                // The action exists. Does it already contain this keyboard key?
                if ( action.KeyboardKeys.ContainsKey( key ) )
                {
                    throw new ArgumentException( "Input action already contains this input key" );
                }

                // Make sure this is a directional action. No sense on combining the two concepts
                // together... it'll only end in sadness.
                if ( action.IsDirectional )
                {
                    throw new ArgumentException( "Tried to add non directional key to directional action" );
                }

                // Register the new key press!
                action.KeyboardKeys.Add(key, DirectionName.East);
            }
            else
            {
                // The action did not exist. Create it now.
                mActions.Add( actionType, new ActionData( actionType, key ) );
            }
        }

        /// <summary>
        ///  Adds a directional keyboard binding that corresponds to an input action.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="key"></param>
        /// <param name="dir"></param>
        public void AddDirectionalAction(ActionType actionType, Keys key, DirectionName dir)
        {
            // See if this action has already been created. Either update the existing action data,
            // or create a new entry.
            ActionData action = FindActionForType( actionType );

            if ( action != null )
            {
                // The action exists. Does it already contain this keyboard key?
                if ( action.KeyboardKeys.ContainsKey( key ) )
                {
                    throw new ArgumentException( "Input action already contains this input key" );
                }

                // Make sure this is a directional action. No sense on combining the two concepts
                // together... it'll only end in sadness.
                if (! action.IsDirectional )
                {
                    throw new ArgumentException( "Tried to add directional key to non directional action" );
                }

                // Register the new key press!
                action.KeyboardKeys.Add( key, dir );
            }
            else
            {
                // The action did not exist. Create it now.
                mActions.Add( actionType, new ActionData( actionType, key, dir ) );
            }
        }

        /// <summary>
        ///  Checks if the requested input action was triggered on this frame.
        /// </summary>
        /// <param name="action">Input action to check.</param>
        /// <returns>True if it was triggered this frame, false otherwise.</returns>
        public bool WasTriggered( ActionType actionType )
        {
            // Ensure the action exists before going any further.
            ActionData action = null;

            if ( mActions.TryGetValue( actionType, out action ) )
            {
                return action.TriggeredThisFrame;
            }
            else
            {
                throw new InputException( "There is no such action {0} to check".With( actionType ) );
            }
        }


        /// <summary>
        ///  Checks if the requested input action was triggered on this frame.
        /// </summary>
        /// <param name="action">Input action to check.</param>
        /// <param name="direction">The direction that was triggered</param>
        /// <returns>True if it was triggered this frame, false otherwise.</returns>
        public bool WasTriggered(ActionType actionType, out DirectionName direction)
        {
            // Ensure the action exists before going any further.
            ActionData action = null;

            if ( mActions.TryGetValue( actionType, out action ) )
            {
                direction = action.DirectionThisFrame;
                return action.TriggeredThisFrame;
            }
            else
            {
                throw new InputException( "There is no such action {0} to check".With( actionType ) );
            }
        }

        /// <summary>
        ///  Update the input manager by instructing it to read the latest state information from
        ///  our input controllers.
        /// </summary>
        public void Update()
        {
            KeyboardState keyboard = Keyboard.GetState( PlayerIndex.One );
            Keys[] keyboardKeys = keyboard.GetPressedKeys();

            // Scan through all of the registered input actions to see if any of them have been
            // triggered on this frame.
            foreach ( ActionData action in mActions.Values )
            {
                // Check if the keyboard triggered this action.
                foreach (KeyValuePair<Keys,DirectionName> item in action.KeyboardKeys)
                {
                    if ( keyboard.IsKeyDown( item.Key ) )
                    {
                        action.TriggeredThisFrame = true;
                        action.DirectionThisFrame = item.Value;
                    }
                }
            }
        }

        /// <summary>
        ///  Clear input state. This should be called once the input has been fully processed for
        ///  a simulation update.
        /// </summary>
        public void ClearState()
        {
            foreach ( ActionData action in mActions.Values )
            {
                action.DirectionThisFrame = DirectionName.East;
                action.TriggeredThisFrame = false;
            }
        }

        /// <summary>
        ///  Locates the action data corresponding to the requested action type. If it cannot be
        ///  located this method will return null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ActionData FindActionForType( ActionType type )
        {
            ActionData action = null;
            mActions.TryGetValue( type, out action );

            return action;
        }
    }
}
