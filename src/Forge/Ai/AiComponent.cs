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
using Forge.GameObjects;

namespace Forge.Ai
{
    /// <summary>
    ///  AI state.
    /// </summary>
    public enum AiState
    {
        Idle,
        Walking
    }

    /// <summary>
    ///  Artifical intelligence.
    ///  TODO: Move this class to the Game itself.
    /// </summary>
    public class AiComponent : Component
    {
        /// <summary>
        ///  The number of seconds
        /// </summary>
        public double SecondsSinceLastStateChange { get; set; }

        /// <summary>
        ///  Get or set the current AI state.
        /// </summary>
        public AiState CurrentState { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">Game object who owns this AI controller</param>
        public AiComponent()
        {
            SecondsSinceLastStateChange = 0.0f;
        }
    }
}
