using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Scott.Game
{
    /// <summary>
    ///  Misc game utilies.
    /// </summary>
    public static class GameUtil
    {

        /// <summary>
        /// Returns a unit vector that is oriented in the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 GetMovementVector( Direction direction )
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
                    return Vector2.Zero;
            }
        }
    }
}
