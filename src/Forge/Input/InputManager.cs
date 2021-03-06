﻿/*
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
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Forge.Input
{
    /// <summary>
    ///  Manages input mappings and input reading.
    /// </summary>
    /// <remarks>
    ///  http://stackoverflow.com/a/948985
    /// </remarks>
    public class InputManager<TAction> where TAction : struct, IComparable, IFormattable
    {
        private const int MaxPlayerCount = 4;

        /// <summary>
        ///  Defines a mapping from an input action to input buttons that trigger it.
        /// </summary>
        private class InputActionData
        {
            /// <summary>
            ///  Action.
            ///  TODO: Check if used.
            /// </summary>
            public TAction InputAction = default(TAction);

            /// <summary>
            ///  Get if action has a directional axis or not.
            /// </summary>
            /// <remarks>
            ///  If an input action is directional then up to four keyboard keys can be assigned otherwise only the
            ///  first key will be checked.
            /// </remarks>
            public bool IsDirectional = false;

            /// <summary>
            ///  Keyboard keys mapped to this action.
            /// </summary>
            /// <remarks>
            ///  Each index corresponds to the Direction enumeration.
            /// </remarks>
            public Keys[] KeyboardKeys = new Keys[Constants.DirectionCount];

            /// <summary>
            ///  Non-directional keyboard input constructor.
            /// </summary>
            /// <param name="type">Input action.</param>
            /// <param name="k">Keyboard button.</param>
            public InputActionData(TAction type, Keys k)
            {
                InputAction = type;
                KeyboardKeys[0] = k;
            }

            /// <summary>
            ///  Directional keyboard input constructor.
            /// </summary>
            /// <param name="type">Input action.</param>
            /// <param name="k">Keyboard button.</param>
            /// <param name="direction">First direction to configure.</param>
            public InputActionData(TAction type, Keys k, DirectionName direction)
            {
                InputAction = type;
                IsDirectional = true;
                KeyboardKeys[(int) direction] = k;
            }
        }

        /// <summary>
        ///  List of registered actions.
        /// </summary>
        private Dictionary<TAction, InputActionData> mActions = new Dictionary<TAction, InputActionData>();

        private GamePadState[] mLastGamePadState = new GamePadState[MaxPlayerCount];
        private GamePadState[] mCurrentGamePadState = new GamePadState[MaxPlayerCount];
        private KeyboardState mLastKeyboardState;
        private KeyboardState mCurrentKeyboardState;
        private MouseState mLastMouseState;
        private MouseState mCurrentMouseState;

        private PlayerIndex[] mPlayerIndices = new PlayerIndex[1];

        /// <summary>
        ///  Constructor.
        /// </summary>
        public InputManager()
        {
            mPlayerIndices[0] = PlayerIndex.One;
        }

        /// <summary>
        ///  Adds a non-directional keyboard action binding.
        /// </summary>
        /// <param name="action">Action to bind.</param>
        /// <param name="key">Keyboard button to bind.</param>
        public void AddAction(TAction actionType, Keys key)
        {
            // Don't bind the action more than once.
            InputActionData action = null;

            if (mActions.TryGetValue(actionType, out action))
            {
                throw new ArgumentException("Input action was already bound");
            }
            
            // The action did not exist. Create it now.
            mActions.Add(actionType, new InputActionData(actionType, key));
        }

        /// <summary>
        ///  Adds a directional keyboard binding that corresponds to an input action.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="key"></param>
        /// <param name="dir"></param>
        public void AddDirectionalAction(TAction actionType, Keys key, DirectionName dir)
        {
            // Check if the input binding already exists.
            InputActionData action = null;

            if (mActions.TryGetValue(actionType, out action))
            {
                // Make sure this is a directional action. No sense on combining the two concepts together... it'll
                // only end in sadness.
                if (!action.IsDirectional)
                {
                    throw new ArgumentException("Tried to add directional key to non directional action");
                }

                // Add the new keyboard binding.
                action.KeyboardKeys[(int) dir] = key;
            }
            else
            {
                action = new InputActionData(actionType, key, dir);
                mActions.Add(actionType, action);
            }
        }

        /// <summary>
        ///  Checks if the requested input action was triggered on this update.
        /// </summary>
        /// <param name="action">Input action to check.</param>
        /// <returns>True if it was triggered this frame, false otherwise.</returns>
        public bool WasTriggered(TAction actionType)
        {
            DirectionName temp;
            return WasTriggered(actionType, out temp);
        }
        
        /// <summary>
        ///  Checks if the requested input action was triggered on this update.
        ///  TODO: USe a vector instead of DirectionName to allow diagonal movement.
        /// </summary>
        /// <param name="action">Input action to check.</param>
        /// <param name="direction">The direction that was triggered</param>
        /// <returns>True if it was triggered this frame, false otherwise.</returns>
        public bool WasTriggered(TAction action, out DirectionName direction)
        {
            InputActionData actionData = null;
            bool wasTriggered = false;

            direction = Constants.DefaultDirection;

            if (mActions.TryGetValue(action, out actionData))
            {
                // Check keyboard keys.
                int keyCount = (actionData.IsDirectional ? Constants.DirectionCount : 1);

                for (int i = 0; i < keyCount; i++)
                {
                    var key = actionData.KeyboardKeys[i];

                    if (mCurrentKeyboardState.IsKeyDown(key) && !mLastKeyboardState.IsKeyDown(key))
                    {
                        wasTriggered = true;
                        direction = (DirectionName) i;
                        break;
                    }
                }
            }
            else
            {
                throw new InputException("There is no such action {0} to check".With(action));
            }

            return wasTriggered;
        }

        /// <summary>
        ///  Get input action as a movement vector.
        /// </summary>
        /// <param name="action">Action to check.</param>
        /// <returns>Unit vector representing movement vector.</returns>
        public Vector2 GetAxis(TAction action)
        {
            InputActionData actionData = null;
            var axisValue = Vector2.Zero;

            if (mActions.TryGetValue(action, out actionData))
            {
                // TODO: Check gamepad and mouse.

                // Check keyboard keys and combine multiple presses into a vector. Only consider the keyboard if
                // there was no valid input from the gamepad.
                if (axisValue == Vector2.Zero)
                {
                    int keyCount = (actionData.IsDirectional ? Constants.DirectionCount : 1);

                    for (int i = 0; i < keyCount; i++)
                    {
                        var key = actionData.KeyboardKeys[i];

                        if (mCurrentKeyboardState.IsKeyDown(key))
                        {
                            axisValue += ((DirectionName) i).ToVector();
                        }
                    }

                    // Normalize movement vector to prevent diagonal movement from moving faster than max speed.
                    if (axisValue.LengthSquared > 0.0f)
                    {
                        axisValue.Normalize();
                    }
                }
            }
            else
            {
                throw new InputException("There is no such action {0} to check".With(action));
            }

            return axisValue;
        }

        /// <summary>
        ///  Update the input manager by reading input devices and updating associated actions.
        /// </summary>
        public void Update(double currentTimeSeconds, double deltaSeconds)
        {
            // Copy and update gamepad state for each connected player.
            for (int rawPlayerIndex = 0; rawPlayerIndex < mPlayerIndices.Length; rawPlayerIndex++)
            {
                var playerIndex = mPlayerIndices[rawPlayerIndex];
                
                mLastGamePadState[rawPlayerIndex] = mCurrentGamePadState[rawPlayerIndex];
                mCurrentGamePadState[rawPlayerIndex] = GamePad.GetState(playerIndex);
            }

            // Copy and update mouse and keyboard state.
            mLastMouseState = mCurrentMouseState;
            mCurrentMouseState = Mouse.GetState();

            mLastKeyboardState = mCurrentKeyboardState;
            mCurrentKeyboardState = Keyboard.GetState();

            var keyboard = Keyboard.GetState();
            var keyboardKeys = keyboard.GetPressedKeys();

            // Iterate through all input actions and see which ones have been activated.
            foreach (var action in mActions.Values)
            {
                // Check if the keyboard triggered this action.
                foreach (var item in action.KeyboardKeys)
                {
                    // TODO: Trigger associated event.
                }
            }
        }

        /// <summary>
        ///  Clear input state. This should be called once the input has been fully processed for
        ///  a simulation update.
        /// </summary>
        public void ClearState()
        {

        }

        /// <summary>
        ///  Locates the action data corresponding to the requested action type. If it cannot be
        ///  located this method will return null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private InputActionData FindActionForType( TAction type )
        {
            InputActionData action = null;
            mActions.TryGetValue( type, out action );

            return action;
        }
    }
}
