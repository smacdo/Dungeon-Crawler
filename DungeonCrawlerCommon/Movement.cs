using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scott.dungeon
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class Movement
    {
        /// <summary>
        /// The game object that this component belongs to
        /// </summary>
        private GameObject mGameObject;

        /// <summary>
        /// The speed (in units/sec) that the movement is occurring
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The direction that the movement is occurring
        /// </summary>
        public Direction Heading { get; set; }

        public bool IsMoving
        {
            get
            {
                return ( Speed < 1 );
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Movement( GameObject owner )
        {
            mGameObject = owner;
            Speed = 0;
            Heading = Direction.South;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update( GameTime gameTime )
        {
            if ( Speed > 0 )
            {
                float timeDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;

                // Determine axis of motion
                Vector2 movementAxis = Vector2.Zero;

                switch ( Heading )
                {
                    case Direction.North:
                        movementAxis = new Vector2( 0, -1 );
                        break;

                    case Direction.South:
                        movementAxis = new Vector2( 0, 1 );
                        break;

                    case Direction.West:
                        movementAxis = new Vector2( -1, 0 );
                        break;

                    case Direction.East:
                        movementAxis = new Vector2( 1, 0 );
                        break;
                }

                // Update the movement information
                mGameObject.Facing = Heading;
                mGameObject.Position += movementAxis * Speed * timeDelta;
            }
        }
    }
}
