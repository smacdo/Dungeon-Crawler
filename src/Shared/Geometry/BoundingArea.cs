using Microsoft.Xna.Framework;
using Scott.Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Geometry
{
    /// <summary>
    /// Game component that detects, resolves and handles rotatable, rectangular collisions
    /// between game objects.
    /// 
    /// TODO: Do profiling to see if we should use a fitting rectangle as an early exit test
    /// (A circle would probably be even better)
    /// 
    /// TODO: Transition this to local space collision detection (currently in world space)
    /// TODO: Unit test
    /// TODO: Make this cleaner.
    /// 
    /// HUGE HELP: http://www.metanetsoftware.com/technique/tutorialA.html
    /// 
    /// TESTS NEEDED:
    ///   Rotation
    ///   LocalOffset
    /// </summary>
    public class BoundingArea
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Rotation { get; private set; }
        public Vector2 WorldPosition { get; private set; }
        public Vector2 LocalOffset { get; private set; }

        /// <summary>
        /// A rectangle that tightly encompasses the rotated rectangle.
        /// </summary>
        public Rectangle BroadPhaseRectangle;

        public Vector2 Size { get { return new Vector2( Width, Height ); } }

        /// <summary>
        /// The rotated rectangle's upper left vertex. This is the original upper left vertex from
        /// the unrotated rectangle, so this value may or may not actually be the upper left most
        /// point.
        /// </summary>
        public Vector2 UpperLeft { get; private set; }

        /// <summary>
        /// The rotated rectangle's upper right vertex. This is the original upper right vertex from
        /// the unrotated rectangle, so this value may or may not actually be the upper right most
        /// point.
        /// </summary>
        public Vector2 UpperRight { get; private set; }

        /// <summary>
        /// The rotated rectangle's lower left vertex. This is the original lower left vertex from
        /// the unrotated rectangle, so this value may or may not actually be the lower left most
        /// point.
        /// </summary>
        public Vector2 LowerLeft { get; private set; }

        /// <summary>
        /// The rotated rectangle's lower right vertex. This is the original lower right vertex from
        /// the unrotated rectangle, so this value may or may not actually be the lower right most
        /// point.
        /// </summary>
        public Vector2 LowerRight { get; private set; }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="dimensions">Bounding box dimensions</param>
        public BoundingArea( Vector2 topLeft, Vector2 dimensions )
            : this( topLeft, dimensions, Vector2.Zero )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="dimensions">Original dimensions of bounding box</param>
        public BoundingArea( Vector2 topLeft, Vector2 dimensions, Vector2 offset )
        {
            Width         = dimensions.X;
            Height        = dimensions.Y;
            Rotation      = 0.0f;
            WorldPosition = topLeft + offset;
            LocalOffset   = offset;

            RecalculateCachedCorners( Rotation, dimensions / 2.0f );
        }

        /// <summary>
        ///  Moves the bounding box.
        /// </summary>
        /// <param name="delta">Distance to move the bounding box.</param>
        public void Move( Vector2 delta )
        {
            WorldPosition += delta;

            BroadPhaseRectangle.Y = (int) WorldPosition.Y;
            BroadPhaseRectangle.X = (int) WorldPosition.X;
        }

        /// <summary>
        /// Check if the bounding rectangle intersects this bounding rectangle.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool Intersects( BoundingArea area )
        {
            // Quick check
            if ( BroadPhaseRectangle.Intersects( area.BroadPhaseRectangle ) )
            {
                return true;
            }

            // Generate the potential seperating axis vectors between our bounding rect
            // and the provided rect. We avoid the use of arrays here so we can avoid
            // garbage collection
            Vector2 v0 = UpperRight - UpperLeft + WorldPosition;
            Vector2 v1 = UpperRight - LowerRight + WorldPosition;
            Vector2 v2 = area.UpperLeft - area.LowerLeft + WorldPosition;
            Vector2 v3 = area.UpperLeft - area.UpperRight + WorldPosition;

            return ( IsAxisCollision( area, v0 ) &&
                     IsAxisCollision( area, v1 ) &&
                     IsAxisCollision( area, v2 ) &&
                     IsAxisCollision( area, v3 ) );
        }

        private bool IsAxisCollision( BoundingArea otherRect, Vector2 axis )
        {
            // Project the four corners of the bounding box we are checking onto the axis,
            // and get a scalar value of that projection for comparison.
            Vector2 axisNormalized = Vector2.Normalize( axis );

            float a0 = Vector2.Dot( otherRect.UpperLeft, axisNormalized );
            float a1 = Vector2.Dot( otherRect.UpperRight, axisNormalized );
            float a2 = Vector2.Dot( otherRect.LowerLeft, axisNormalized );
            float a3 = Vector2.Dot( otherRect.LowerRight, axisNormalized );

            // Project the four corners of our bounding rect onto the requested axis, and
            // get scalar values for those projections
            float b0 = Vector2.Dot( UpperLeft, axisNormalized );
            float b1 = Vector2.Dot( UpperRight, axisNormalized );
            float b2 = Vector2.Dot( LowerLeft, axisNormalized );
            float b3 = Vector2.Dot( LowerRight, axisNormalized );

            // Get the maximum and minimum scalar values for each of the rectangles
            float aMin = Math.Min( Math.Min( a0, a1 ), Math.Min( a2, a3 ) );
            float aMax = Math.Max( Math.Min( a0, a1 ), Math.Max( a2, a3 ) );
            float bMin = Math.Min( Math.Min( b0, b1 ), Math.Min( b2, b3 ) );
            float bMax = Math.Max( Math.Min( b0, b1 ), Math.Max( b2, b3 ) );

            // Test if there is an overlap between the minimum of a and the maximum
            // values of the two rectangles
            return ( bMin <= aMax && bMax >= aMax ) ||
                   ( aMin <= bMax && aMax >= bMax );
        }

        /// <summary>
        /// Rotates the bounding box around our pivot axis, and then stores the rotated
        /// bounding points in Top/Left/Bottom/Right. This must be called anytime mRotation
        /// is changed!
        /// </summary>
        /// <param name="radians">Amount to rotate the rectangular bounding box</param>
        /// <param name="pivot">Position (in world coordintes) of the rotational pivot point</param>
        private void RecalculateCachedCorners( float radians, Vector2 pivot )
        {
            Rotation    = MathHelper.WrapAngle( radians );

            // Find unrotated vertex points
            Vector2 oUpperLeft  = new Vector2( WorldPosition.X,         WorldPosition.Y );
            Vector2 oUpperRight = new Vector2( WorldPosition.X + Width, WorldPosition.Y );
            Vector2 oLowerLeft  = new Vector2( WorldPosition.X,         WorldPosition.Y + Height );
            Vector2 oLowerRight = new Vector2( WorldPosition.X + Width, WorldPosition.Y + Height );

            // Rotate and calculate our new rotated vertex points
            UpperLeft  = RotatePoint( oUpperLeft, pivot, radians );
            UpperRight = RotatePoint( oUpperRight, pivot, radians );
            LowerLeft  = RotatePoint( oLowerLeft, pivot, radians );
            LowerRight = RotatePoint( oLowerRight, pivot, radians );

            // Calculate a broad phase bounding box
            //  XXX: Do this better
            BroadPhaseRectangle = new Rectangle( (int) Math.Round( WorldPosition.X ),
                                                 (int) Math.Round( WorldPosition.Y ),
                                                 (int) Math.Round( Width ),
                                                 (int) Math.Round( Height ) );
        }

        /// <summary>
        /// Rotates a vector around a pivot point
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="origin">Pivot to rotate around</param>
        /// <param name="amount">Amount of rotation to apply</param>
        /// <returns>Rotated vector</returns>
        private Vector2 RotatePoint( Vector2 vector, Vector2 origin, float amount )
        {
            if ( amount == 0 )
            {
                return vector;
            }
            else
            {
                return new Vector2( (float) ( origin.X + ( vector.X - origin.X ) * Math.Cos( amount ) - ( vector.Y - origin.Y ) * Math.Sin( amount ) ),
                                    (float) ( origin.Y + ( vector.Y - origin.Y ) * Math.Cos( amount ) + ( vector.X - origin.X ) * Math.Sin( amount ) ) );
            }
        }

        public override string ToString()
        {
            return String.Format( "{{ x: {0}, y: {1}, w: {2}, h: {3} }}",
                                  BroadPhaseRectangle.X,
                                  BroadPhaseRectangle.Y,
                                  BroadPhaseRectangle.Width,
                                  BroadPhaseRectangle.Height );
        }
    }
}
