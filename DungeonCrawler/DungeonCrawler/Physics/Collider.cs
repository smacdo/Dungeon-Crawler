using Microsoft.Xna.Framework;
using Scott.Dungeon.Data;
using Scott.Dungeon.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Physics
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
    /// 
    /// HUGE HELP: http://www.metanetsoftware.com/technique/tutorialA.html
    /// </summary>
    public class Collider : AbstractGameObjectComponent
    {
        /// <summary>
        /// The original unrotated bounding rectangle.
        /// </summary>
        /// <remarks>
        /// We hold onto the unrotated bounding rectangle, and re-rotate each time the
        /// rotation is updated. Why? To prevent increasingly nasty floating point drift.
        /// </remarks>
        public Rectangle UnrotatedBoundingRect;

        /// <summary>
        /// A  rectangle that tightly encompasses the rotated rectangle.
        /// </summary>
        public Rectangle AxisAlignedBoundingRect;

        /// <summary>
        /// Amount the bounding rectangle is rotated.
        /// </summary>
        public Vector2 PivotOrigin { get; private set; }

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
        /// Width of the rotated bounding rectangle
        /// </summary>
        public float Width
        {
            get
            {
                return UnrotatedBoundingRect.Width;
            }
        }

        /// <summary>
        /// Height of the rotated bounding rectangle
        /// </summary>
        public float Height
        {
            get
            {
                return UnrotatedBoundingRect.Height;
            }
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Bounding box dimensions</param>
        public Collider( Rectangle boundingBox )
            : this( boundingBox, 0.0f, CalculateCenterPoint( boundingBox ) )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation (in radians)</param>
        public Collider( Rectangle boundingBox, float rotation )
            : this( boundingBox, rotation, CalculateCenterPoint( boundingBox ) )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation</param>
        /// <param name="origin">Rotational pivot position. Top left is (0,0).</param>
        public Collider( Rectangle boundingBox, float rotation, Vector2 origin )
        {
            UnrotatedBoundingRect = boundingBox;
            PivotOrigin           = origin;

            RecalculateCachedCorners( rotation, origin );
        }

        /// <summary>
        /// Sets the collision bounding box with new values
        /// </summary>
        /// <param name="boundingBox"></param>
        public void Set( Rectangle boundingBox )
        {
            Set( boundingBox, 0.0f, CalculateCenterPoint( boundingBox ) );
        }

        /// <summary>
        /// Sets the collision bounding box with new values
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        public void Set( Rectangle boundingBox, float rotation, Vector2 origin )
        {
            UnrotatedBoundingRect = boundingBox;
            PivotOrigin = origin;

            RecalculateCachedCorners( rotation, origin );
        }

        /// <summary>
        /// Check if the bounding rectangle intersects this bounding rectangle.
        /// </summary>
        /// <param name="otherRect"></param>
        /// <returns></returns>
        public bool Intersects( BoundingRect otherRect )
        {
            // Generate the potential seperating axis vectors between our bounding rect
            // and the provided rect. We avoid the use of arrays here so we can avoid
            // garbage collection
            Vector2 v0 = UpperRight - UpperLeft;
            Vector2 v1 = UpperRight - LowerRight;
            Vector2 v2 = otherRect.UpperLeft - otherRect.LowerLeft;
            Vector2 v3 = otherRect.UpperLeft - otherRect.UpperRight;

            return ( IsAxisCollision( otherRect, v0 ) &&
                     IsAxisCollision( otherRect, v1 ) &&
                     IsAxisCollision( otherRect, v2 ) &&
                     IsAxisCollision( otherRect, v3 ) );
        }

        private bool IsAxisCollision( BoundingRect otherRect, Vector2 axis )
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
            PivotOrigin = pivot;

            // Find unrotated vertex points
            Vector2 oUpperLeft  = new Vector2( UnrotatedBoundingRect.Left, UnrotatedBoundingRect.Top );
            Vector2 oUpperRight = new Vector2( UnrotatedBoundingRect.Right, UnrotatedBoundingRect.Top );
            Vector2 oLowerLeft  = new Vector2( UnrotatedBoundingRect.Left, UnrotatedBoundingRect.Bottom );
            Vector2 oLowerRight = new Vector2( UnrotatedBoundingRect.Right, UnrotatedBoundingRect.Bottom );

            // Rotate and calculate our new rotated vertex points
            UpperLeft = RotatePoint( oUpperLeft, pivot, radians );
            UpperRight = RotatePoint( oUpperRight, pivot, radians );
            LowerLeft = RotatePoint( oLowerLeft, pivot, radians );
            LowerRight = RotatePoint( oLowerRight, pivot, radians );
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

        /// <summary>
        /// Takes an axis aligned rectangle, and returns it's center position
        /// </summary>
        /// <param name="unrotatedRect">Axis aligned rectangle</param>
        /// <returns>Center point of rectangle</returns>
        private static Vector2 CalculateCenterPoint( Rectangle unrotatedRect  )
        {
            return new Vector2( unrotatedRect.Width / 2.0f  + (float) unrotatedRect.X,
                                unrotatedRect.Height / 2.0f + (float) unrotatedRect.Y );
        }

        /// <summary>
        /// Perform update computations
        /// </summary>
        /// <param name="time"></param>
        public override void Update( GameTime time )
        {
            Rectangle worldCollisionRect = new Rectangle( (int) Owner.Position.X + UnrotatedBoundingRect.X,
                                                          (int) Owner.Position.Y + UnrotatedBoundingRect.Y,
                                                          (int) this.Width,
                                                          (int) this.Height );

            GameRoot.Debug.DrawRect( worldCollisionRect, Color.Green );
        }
    }
}
