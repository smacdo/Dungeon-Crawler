namespace Scott.Common
{
    /// <summary>
    ///  Common math utility functions.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        ///  Normalize a value in the [min,max] range to [0.0,1.0]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [0.0,1.0] range.</returns>
        public static float NormalizeToZeroOneRange( float v, float min, float max )
        {
            return ( v - min ) / ( max - min );
        }

        /// <summary>
        ///  Normalize a value in the [min,max] range to [0.0,1.0]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [0.0,1.0] range.</returns>
        public static double NormalizeToZeroOneRange( double v, double min, double max )
        {
            return ( v - min ) / ( max - min );
        }

        /// <summary>
        ///  Clamps a value that is in the [min,max] range to [-1,1]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [-1.0,1.0] range.</returns>
        public static float NormalizeToNegativeOneOneRange( float v, float min, float max )
        {
            return 2.0f * ( v - min ) / ( max - min ) + -1.0f;
        }

        /// <summary>
        ///  Clamps a value that is in the [min,max] range to [-1,1]. Range must be zero or greater.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <param name="min">Minimum value in the current range.</param>
        /// <param name="max">Maximum value in the current range.</param>
        /// <returns>Normalized value in the [-1.0,1.0] range.</returns>
        public static double NormalizeToNegativeOneOneRange( double v, double min, double max )
        {
            return 2.0 * ( v - min ) / ( max - min ) + -1.0;
        }
    }
}
