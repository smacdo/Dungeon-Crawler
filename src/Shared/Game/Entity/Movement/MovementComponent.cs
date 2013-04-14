using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Scott.GameContent;
using Scott.Geometry;
using Scott.Game.Entity.Graphics;
using Scott.Game.Graphics;

namespace Scott.Game.Entity.Movement
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class MovementComponent : Component
    {
        private float mVelocity = 0.0f;
        private float mLastVelocity = 0.0f;
        private Direction mDirection = Direction.East;
        private Direction mLastDirection = Direction.East;
        private Vector2 mPosition = Vector2.Zero;
        private Vector2 mLastPosition = Vector2.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        public MovementComponent()
        {
            // Empty;
        }

        /// <summary>
        ///  The speed (in units/sec) that the movement is occurring
        /// </summary>
        public float Velocity { get { return mVelocity; } set { mVelocity = value; } }

        /// <summary>
        ///  The direction that this component is moving.
        /// </summary>
        public Direction Direction { get { return mDirection; } set { mDirection = value; } }

        /// <summary>
        ///  Rectangle defining which part of the game object is used for movement related
        ///  collision detection.
        /// </summary>
        public RectangleF MoveBox { get; set; }

        /// <summary>
        ///  Is this component moving?
        /// </summary>
        public bool IsMoving
        {
            get
            {
                return ( Math.Abs( mVelocity ) < 0.1f );
            }
        }

        public Vector2 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
            }
        }

        public Vector2 LastPosition { get { return mLastPosition; } }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool StartedMovingThisFrame { get; private set; }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool StoppedMovingThisFrame { get; private set; }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool ChangedDirectionThisFrame { get; private set; }

        /// <summary>
        /// Requests the actor to move in a specified direction for the duration of this update
        /// cycle
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        /// <param name="velocity">Speed at which to move</param>
        public void Move( Direction direction, float velocity )
        {
            mVelocity = velocity;
            mDirection = direction;
        }

        /// <summary>
        /// Stops a queued move
        /// </summary>
        public void CancelMove()
        {
            mVelocity = 0.0f;
        }

        public void ChangeDirection( Direction direction )
        {
            Direction = direction;
        }

        /// <summary>
        /// Update movement game components
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime time )
        {
            float timeDelta  = (float) time.ElapsedGameTime.TotalSeconds;
            Vector2 position = Owner.Transform.Position;

            // Calculate our new position.
            Vector2 movementAxis = GameUtil.GetMovementVector( Direction );
            Vector2 newPosition  = position + ( movementAxis * mVelocity * timeDelta );

            // Did we start or stop moving this frame?
            StartedMovingThisFrame =
                ( ( mVelocity > 0.0f || mVelocity < 0.0f ) && mLastVelocity == 0.0f );

            StoppedMovingThisFrame =
                ( mVelocity == 0.0f && ( mLastVelocity > 0.0f || mLastVelocity < 0.0f ) );

            // How about direction? Did we change direction this frame?
            ChangedDirectionThisFrame = ( mDirection != mLastDirection );

            // Update our new values and store our old ones.
            mLastVelocity  = mVelocity;
            mLastDirection = mDirection;
            mLastPosition  = mPosition;

            mPosition      = newPosition;
            mVelocity      = 0.0f;
        }
    }
}
