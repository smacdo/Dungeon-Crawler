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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Scott.Forge.Engine.Movement;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Actors
{
    /// <summary>
    ///  Actor.
    /// </summary>
    public class ActorComponent : Component
    {
        public IActorAction CurrentAction { get; set; }
        public IActorAction RequestedAction { get; set; }

        /// <summary>
        ///  Direction the actor is facing.
        /// </summary>
        public DirectionName Direction { get; set; }
        public Vector2 RequestedMovement { get; set; }

        public float MaxSpeed { get; set;}

        /// <summary>
        ///  Get or set the seconds that have elapsed since the actor started the current walk cycle.
        /// </summary>
        public double WalkAccelerationSeconds { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ActorComponent()
        {
            Direction = Constants.DefaultDirection;
            MaxSpeed = 100;
            WalkAccelerationSeconds = 0;
        }

        /// <summary>
        ///  Requests the actor move in a given direction and speed.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void Move(DirectionName direction, float speed)
        {
            RequestedMovement = DirectionNameHelper.ToVector(direction) * speed;
        }
    }
}
