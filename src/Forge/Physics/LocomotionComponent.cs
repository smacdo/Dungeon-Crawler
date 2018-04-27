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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Forge.GameObjects;

namespace Forge.Physics
{
    /// <summary>
    ///  The locomotor is the "doer" component for a game object. It is responsible for moving a game object,
    ///  and performing actions on behalf of a game object.
    /// </summary>
    /// <remarks>
    ///  Movement and performing action seem like two separate responsibilities that should be in two different
    ///  components, and you would be right. The problem is that movement and actions are deeply linked, and
    ///  moving them into two classes who create an interlinked mess. Consider for example these scenarios:
    ///  
    ///  1. Can I perform this action while I am moving?
    ///  2. Should the action be cancelled when I move? Or should I not move when an action is performed?
    ///  3. I want to perform an action when I get to a target position.
    ///  4. etc
    ///  
    /// </remarks>
    public class LocomotionComponent : Component
    {
        public Vector2 RequestedMovement { get; set; }

        public float MaxSpeed { get; set;}

        /// <summary>
        ///  Get or set the seconds that have elapsed since the actor started the current walk cycle.
        /// </summary>
        public TimeSpan WalkAccelerationTime { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LocomotionComponent()
        {
            MaxSpeed = 100;
            WalkAccelerationTime = TimeSpan.Zero;
        }

        /// <summary>
        ///  Requests the actor move in a given direction and speed.
        /// </summary>
        /// <remarks>This movement will be performed the next time Actor processor runs.</remarks>
        /// <param name="speed"></param>
        public void Move(Vector2 direction, float speed)
        {
            RequestedMovement = direction * speed;
        }

        /// <summary>
        ///  Requests the actor to move in the direction they are facing at the requested speed.
        /// </summary>
        /// <param name="speed">Speed to move the actor.</param>
        public void MoveForward(float speed)
        {
            Move(Owner.Transform.Forward, speed);
        }
    }
}