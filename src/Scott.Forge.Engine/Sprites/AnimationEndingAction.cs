/*
 * Copyright 2012-2015 Scott MacDonald
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

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    /// The action to perform when an action has ended
    /// </summary>
    public enum AnimationEndingAction
    {
        Stop,         // Freeze on the last played animation frame
        Loop,         // Restart on animation frame zero and continue animating
        StopAndReset, // Freeze on the first animation frame
    }
}
