using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Data
{
    /// <summary>
    /// A rotatable rectangular bounding box that is used for most objects in dungeon crawler.
    /// </summary>
    public class BoundingRect
    {
        private Rectangle mDimensions;
        private float mRotation;
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
            UpdateCachedValues();
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation</param>
        public BoundingRect( Rectangle boundingBox, float rotation )
        {
            mDimensions = boundingBox;
            mRotation = rotation;

            // Set the box origin (the point to pivot around) as the center of the collision
            // box
            Origin = new Vector2( (int) Math.Round( mDimensions.Width / 2.0f ) + mDimensions.X,
                                   (int) Math.Round( mDimensions.Height / 2.0f ) + mDimensions.Y );

            UpdateCachedValues();
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="boundingBox">Original dimensions of bounding box</param>
        /// <param name="rotation">Amount of rotation</param>
        /// <param name="origin">Rotational pivot position. Top left is (0,0).</param>
        public BoundingRect( Rectangle boundingBox, float rotation, Vector2 origin )
        {
            mDimensions = boundingBox;
            mRotation = rotation;
            Origin = origin;

            UpdateCachedValues();
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
            float a0 = GenerateProjectionScalar( otherRect.UpperLeft, axis );
            float a1 = GenerateProjectionScalar( otherRect.UpperRight, axis );
            float a2 = GenerateProjectionScalar( otherRect.LowerLeft, axis );
            float a3 = GenerateProjectionScalar( otherRect.LowerRight, axis );

            // Project the four corners of our bounding rect onto the requested axis, and
            // get scalar values for those projections
            float b0 = GenerateProjectionScalar( UpperLeft, axis );
            float b1 = GenerateProjectionScalar( UpperRight, axis );
            float b2 = GenerateProjectionScalar( LowerLeft, axis );
            float b3 = GenerateProjectionScalar( LowerRight, axis );

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

        private float GenerateProjectionScalar( Vector2 rectCorner, Vector2 axis )
        {
            // Project the corner vector onto the generated axis
            float numerator = ( rectCorner.X * axis.X ) + ( rectCorner.Y * axis.Y );
            float denom = ( axis.X * axis.X ) + ( axis.Y * axis.Y );
            float result = numerator / denom;

            Vector2 projection = new Vector2( result * axis.X, result * axis.Y );

            // Get the scalar
            float scalar = ( axis.X * rectCorner.X ) + ( axis.Y * rectCorner.Y );
            return scalar;
        }

        /// <summary>
        /// Rotates the bounding box around our pivot axis, and then stores the rotated
        /// bounding points in Top/Left/Bottom/Right. This must be called anytime mRotation
        /// is changed!
        /// </summary>
        private void UpdateCachedValues()
        {
            // Find unrotated vertex points
            Vector2 oUpperLeft  = new Vector2( mDimensions.Left, mDimensions.Top );
            Vector2 oUpperRight = new Vector2( mDimensions.Right, mDimensions.Top );
            Vector2 oLowerLeft  = new Vector2( mDimensions.Left, mDimensions.Bottom );
            Vector2 oLowerRight = new Vector2( mDimensions.Right, mDimensions.Bottom );

            // Rotate and calculate our new rotated vertex points
            UpperLeft  = RotatePoint( oUpperLeft, oUpperLeft + Origin, mRotation );
            UpperRight = RotatePoint( oUpperRight, oUpperRight + new Vector2( -Origin.X, Origin.Y ), mRotation );
            LowerLeft  = RotatePoint( oLowerLeft, oLowerLeft + new Vector2( Origin.X, -Origin.Y ), mRotation );
            LowerRight = RotatePoint( oLowerRight, oLowerRight + new Vector2( -Origin.X, -Origin.Y ), mRotation );


            UpperLeft = RotatePoint( oUpperLeft, Origin, mRotation );
            UpperRight = RotatePoint( oUpperRight, Origin, mRotation );
            LowerLeft = RotatePoint( oLowerLeft, Origin, mRotation );
            LowerRight = RotatePoint( oLowerRight, Origin, mRotation );
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
