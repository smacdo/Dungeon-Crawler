/*
 * Copyright 2012-2017 Scott MacDonald.
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

namespace Scott.Forge.Sprites
{
    /// <summary>
    ///  A data driven event that occurs when animating a sprite. An event has a name and an optional list of named
    ///  argument values.
    /// </summary>
    public class AnimationEvent
    {
        private IDictionary<string, string> _args;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="args">Optional table of named argument values.</param>
        public AnimationEvent(string name, IDictionary<string, string> args)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            _args = args;
        }

        /// <summary>
        ///  Get the number of arguments associated with this animation event.
        /// </summary>
        public int ArgCount
        {
            get { return _args?.Count ?? 0; }
        }

        /// <summary>
        ///  Get the name of the event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///  Get the named argument value or throw an exception if the argument does not exist.
        /// </summary>
        /// <param name="argName">Name of the argument to get.</param>
        /// <returns>Value of the argument.</returns>
        public string GetArg(string argName)
        {
            if (!TryGetArg(argName, out string argValue))
            {
                throw new ArgumentException("Expected animation argument does not exist");
            }

            return argValue;
        }
        
        /// <summary>
        ///  Try to get the named argument value.
        /// </summary>
        /// <param name="argName">Argument name.</param>
        /// <param name="argValue">Receives the argument values/</param>
        /// <returns>True if argument name is valid false otherwise.</returns>
        public bool TryGetArg(string argName, out string argValue)
        {
            if (_args != null)
            {
                return _args.TryGetValue(argName, out argValue);
            }

            argValue = null;
            return false;
        }
    }
}
