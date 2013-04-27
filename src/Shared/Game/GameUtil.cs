using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scott.Game
{
    /// <summary>
    ///  Misc game utilies.
    /// </summary>
    public static class GameUtil
    {
        /// <summary>
        ///  Convert an angular rotation (in degrees) into a Directional enum. This will wrap the
        ///  angle to the closest direction. Note that rotation must be between [0, 360].
        /// </summary>
        /// <param name="rotation">The angular rotation to convert.</param>
        /// <returns>A direction corresponding to the angular rotation.</returns>
        public static Direction GetDirectionFromRotation( float rotation )
        {
            Debug.Assert( rotation >= 0.0f && rotation <= 360.0f, "Rotation must be [0,360]" );

            if ( rotation < 45.0f )
            {
                return Direction.East;
            }
            else if ( rotation < 135.0f )
            {
                return Direction.North;
            }
            else if ( rotation < 225.0f )
            {
                return Direction.West;
            }
            else if ( rotation < 315.0f )
            {
                return Direction.South;
            }
            else
            {
                return Direction.East;
            }
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
        public static float GetRotationFromDirection( Direction direction )
        {
            switch ( direction )
            {
                case Direction.North:
                    return 90.0f;

                case Direction.South:
                    return 270.0f;

                case Direction.West:
                    return 180.0f;

                case Direction.East:
                    return 90.0f;

                default:
                    Debug.Fail( "Why on earth is there a fifth direction?" );
                    return 0.0f;
            }           
        }

        /// <summary>
        /// Returns a unit vector that is oriented in the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 GetDirectionVector( Direction direction )
        {
            switch ( direction )
            {
                case Direction.North:
                    return new Vector2( 0, -1 );

                case Direction.South:
                    return new Vector2( 0, 1 );

                case Direction.West:
                    return new Vector2( -1, 0 );

                case Direction.East:
                    return new Vector2( 1, 0 );

                default:
                    Debug.Fail( "Why on earth is there a fifth direction?" );
                    return Vector2.Zero;
            }
        }

        /// <summary>
        /// Returns a unit vector that is oriented in the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 GetDirectionRightVector( Direction direction )
        {
            switch ( direction )
            {
                case Direction.North:       // right faces east
                    return new Vector2( 1, 0 );

                case Direction.South:       // right faces west
                    return new Vector2( -1, 0 );

                case Direction.West:        // right faces north
                    return new Vector2( 0, -1 );

                case Direction.East:        // right faces south
                    return new Vector2( 0, 1 );

                default:
                    Debug.Fail( "Why on earth is there a fifth direction?" );
                    return Vector2.Zero;
            }
        }
    }
}
