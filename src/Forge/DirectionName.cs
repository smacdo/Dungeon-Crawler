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

namespace Forge
{
    /// <summary>
    ///  Represents the four cardinal directions in screen space.
    /// </summary>
    /// <remarks>
    ///  Forge assumes the world origin is in the top left of the screen with the Y value increasing as you travel down
    ///  from the origin. This makes the North unit vector (0, -1) which can be a bit counter-intutitive until you
    ///  realize that North (up) is actually -Y, and the South vector (0, 1).
    ///  
    ///  Additionally, the initial direction (a rotation of "zero") is assumed to be East. The traditional orientation
    ///  of angle zero in trig is east so we take the same meaning. Keep in mind that due to the inverted Y axis
    ///  rotations appear to go in a clockwise direction rather than the expected counter clockwise direction.
    ///  
    ///          North                     270                     (0, -1)
    ///    West         East     ==>  180        0   ==>  (-1, 0)           (+1, 0)
    ///          South                      90                     (0, +1)
    /// </remarks>
    public enum DirectionName
    {
        East,
        South,
        West,
        North
    }

    /// <summary>
    ///  Helper and extension methods for the DirectionName enumeration.
    /// </summary>
    public static class DirectionNameHelper
    {
        /// <summary>
        ///  Convert an angular rotation (in degrees) into a Directional enum.
        /// </summary>
        /// <remarks>
        ///  This method matches the angle to its closest DirectionName counterpart.
        /// </remarks>
        /// <param name="rotation">The angular rotation in degrees.</param>
        /// <returns>DirectionName value that is closest to the provided angle.</returns>
        public static DirectionName FromRotationDegrees(double rotation)
        {
            if (rotation < 0.0 || rotation > 360.0)
            {
                throw new ArgumentException("Rotation must be [0, 360]", nameof(rotation));
            }

            if (rotation < 45.0f)
            {
                return DirectionName.East;
            }
            else if (rotation < 135.0)
            {
                return DirectionName.South;
            }
            else if (rotation < 225.0)
            {
                return DirectionName.West;
            }
            else if (rotation < 315.0)
            {
                return DirectionName.North;
            }
            else
            {
                return DirectionName.East;
            }
        }

        /// <summary>
        ///  Convert an angular rotation (in radians) into a Directional enum.
        /// </summary>
        /// <remarks>
        ///  This method matches the angle to its closest DirectionName counterpart.
        /// </remarks>
        /// <param name="rotation">The angular rotation in degrees.</param>
        /// <returns>DirectionName value that is closest to the provided angle.</returns>
        public static DirectionName FromRotationRadians(float rotation)
        {
            const float FirstQuarterWedge = (float)Math.PI / 4;
            const float SecondQuarterWedge = (float)Math.PI * 3 / 4;
            const float ThirdQuarterWedge = (float)Math.PI * 5 / 4;
            const float FourthQuarterWedge = (float)Math.PI * 7 / 4;

            rotation = MathHelper.NormalizeAngleTwoPi(rotation);

            if (rotation < FirstQuarterWedge)
            {
                return DirectionName.East;
            }
            else if (rotation < SecondQuarterWedge)
            {
                return DirectionName.South;
            }
            else if (rotation < ThirdQuarterWedge)
            {
                return DirectionName.West;
            }
            else if (rotation < FourthQuarterWedge)
            {
                return DirectionName.North;
            }
            else
            {
                return DirectionName.East;
            }
        }

        /// <summary>
        ///  Convert a rotational vector into a Direction enumeration value.
        /// </summary>
        /// <remarks>
        ///  TODO: Does this work with non-unit vectors? Zero?
        /// </remarks>
        /// <param name="vector">Rotational vector to convert.</param>
        /// <returns>DirectionName value that is closest to the provided vector.</returns>
        public static DirectionName FromVector(Vector2 vector)
        {
            float angle = (float) Math.Atan2(vector.Y, vector.X);
            float degrees = MathHelper.RadianToDegree(angle);
            return FromRotationDegrees(MathHelper.Wrap(degrees, 0.0f, 360.0f));
        }

        /// <summary>
        ///  Convert a Direction enumeration value to a rotational angle in degrees.
        /// </summary>
        /// <param name="direction">Direction value to convert.</param>
        /// <returns>Degree angle of rotation.</returns>
        public static float ToRotationDegrees(this DirectionName direction)
        {
            switch (direction)
            {
                case DirectionName.North:
                    return 270.0f;

                case DirectionName.South:
                    return 90.0f;

                case DirectionName.West:
                    return 180.0f;

                case DirectionName.East:
                    return 0.0f;

                default:
                    throw new InvalidOperationException("Unknown direction name");
            }
        }

        /// <summary>
        ///  Convert a Direction enumeration value to a rotational angle in radians.
        /// </summary>
        /// <param name="direction">Direction value to convert.</param>
        /// <returns>Radian angle of rotation.</returns>
        public static float ToRotationRadians(this DirectionName direction)
        {
            switch (direction)
            {
                case DirectionName.North:
                    return (float)Math.PI * 1.5f;

                case DirectionName.South:
                    return (float)Math.PI * 0.5f;

                case DirectionName.West:
                    return (float)Math.PI;

                case DirectionName.East:
                    return 0.0f;

                default:
                    throw new InvalidOperationException("Unknown direction name");
            }
        }

        /// <summary>
        ///  Convert a Direction enumeration value to a rotational vector.
        /// </summary>
        /// <param name="direction">Direction value to convert.</param>
        /// <returns>Vector oriented in same direction.</returns>
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
                    throw new InvalidOperationException("Unknown direction name");
            }
        }

        /// <summary>
        ///  Get a unit vector that is 90 degrees right of the direction.
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
                    throw new InvalidOperationException("Unknown direction name");
            }
        }

        /// <summary>
        ///  Parse string containing Direction value.
        /// </summary>
        /// <param name="directionText">String value with direction.</param>
        /// <returns>Matching direction.</returns>
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
