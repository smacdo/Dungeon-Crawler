using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Data
{
    /// <summary>
    /// A rotatable rectangular bounding box that is used for most objects in dungeon crawler.
    /// 
    /// TODO: Do profiling to see if we should use a fitting rectangle as an early exit test
    /// (A circle would probably be even better)
    /// 
    /// HUGE HELP: http://www.metanetsoftware.com/technique/tutorialA.html
    /// </summary>
    public class BoundingRect
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
        /// Amount the bounding rectangle is rotated.
        /// </summary>
        /// <remarks>
        /// We hold onto the unrotated bounding rectangle, and re-rotate each time the
        /// rotation is updated. Why? To prevent increasingly nasty floating point drift.
        /// </remarks>
        public Vector2 Origin { get; private set; }

        public Vector2 UpperLeft { get; private set; }
        public Vector2 UpperRight { get; private set; }
        public Vector2 LowerLeft { get; private set; }
        public Vector2 LowerRight { get; private set; }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Bounding box dimensions</param>
        public BoundingRect( Rectangle boundingBox )
            : this( boundingBox, 0.0f )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation (in radians)</param>
        public BoundingRect( Rectangle boundingBox, float rotation )
        {
            UnrotatedBoundingRect = boundingBox;

            // Use the center of the bounding box as the pivot point
            Vector2 origin = new Vector2( (int) Math.Round( UnrotatedBoundingRect.Width / 2.0f )  + UnrotatedBoundingRect.X,
                                          (int) Math.Round( UnrotatedBoundingRect.Height / 2.0f ) + UnrotatedBoundingRect.Y );

            RecalculateCachedCorners( rotation, origin );
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation</param>
        /// <param name="origin">Rotational pivot position. Top left is (0,0).</param>
        public BoundingRect( Rectangle boundingBox, float rotation, Vector2 origin )
        {
            UnrotatedBoundingRect = boundingBox;
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
            Origin = pivot;

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
    }
}
