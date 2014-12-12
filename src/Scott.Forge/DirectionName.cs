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

namespace Scott.Forge
{
    /// <summary>
    ///  Represents the four cardinal directions.
    /// </summary>
    public enum DirectionName
    {
        North,
        West,
        South,
        East
    }

    /// <summary>
    ///  Convenience methods to make dealing with the Direction enum a little easier.
    /// </summary>
    public static class DirectionNameHelper
    {
        /// <summary>
        ///  Convert an angular rotation (in degrees) into a Directional enum. This will wrap the
        ///  angle to the closest direction. Note that rotation must be between [0, 360].
        /// </summary>
        /// <param name="rotation">The angular rotation to convert.</param>
        /// <returns>A direction corresponding to the angular rotation.</returns>
        public static DirectionName FromRotationAngle(double rotation)
        {
            if (rotation < 0.0 || rotation > 360.0)
            {
                throw new ArgumentException("Rotation must be [0, 360]", "rotation");
            }

            if (rotation < 45.0)
            {
                return DirectionName.East;
            }
            else if (rotation < 135.0)
            {
                return DirectionName.North;
            }
            else if (rotation < 225.0)
            {
                return DirectionName.West;
            }
            else if (rotation < 315.0)
            {
                return DirectionName.South;
            }
            else
            {
                return DirectionName.East;
            }
        }

        public static DirectionName FromVector(Vector2 vector)
        {
            // TODO: Fixme.
            float angle = (float) Math.Tan(vector.X/vector.Y);
            float radians = MathHelper.RadianToDegree(angle);
            return FromRotationAngle(MathHelper.Wrap(radians, 0.0f, 360.0f));
        }

        /// <summary>
        ///  Convert a Direction enum into a circular rotation.
        /// </summary>
        /// <remarks>
        ///  Rotations are as follows:
        ///           90 
        ///     180        0
        ///          270
        /// </remarks>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static float ToRotationAngle(this DirectionName direction)
        {
            switch (direction)
            {
                case DirectionName.North:
                    return 90.0f;

                case DirectionName.South:
                    return 270.0f;

                case DirectionName.West:
                    return 180.0f;

                case DirectionName.East:
                    return 90.0f;

                default:
                    throw new ArgumentException("Why on earth is there a fifth direction?");
            }
        }

        /// <summary>
        /// Returns a unit vector that is oriented in the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 ToVector(this DirectionName direction)
        {
            switch (direction)
            {
                case DirectionName.North:
                    return new Vector2(0, -1);

                case DirectionName.South:
                    return new Vector2(0, 1);

                case DirectionName.West:
                    return new Vector2(-1, 0);

                case DirectionName.East:
                    return new Vector2(1, 0);

                default:
                    throw new ArgumentException("Why on earth is there a fifth direction?");
            }
        }

        /// <summary>
        /// Returns a unit vector that is 90 degrees right of the direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 ToRightVector(this DirectionName direction)
        {
            switch (direction)
            {
                case DirectionName.North:       // right faces east
                    return new Vector2(1, 0);

                case DirectionName.South:       // right faces west
                    return new Vector2(-1, 0);

                case DirectionName.West:        // right faces north
                    return new Vector2(0, -1);

                case DirectionName.East:        // right faces south
                    return new Vector2(0, 1);

                default:
                    throw new ArgumentException("Why on earth is there a fifth direction?");
            }
        }

        public static DirectionName Parse(string directionText)
        {
            if (string.IsNullOrWhiteSpace(directionText))
            {
                throw new ArgumentNullException("directionText");
            }

            return (DirectionName) Enum.Parse(typeof(DirectionName), directionText);
        }
    }
}
