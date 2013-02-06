using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common
{
    /// <summary>
    /// Common functions
    /// </summary>
    public static class Utils
    {
        public static double FindOneZeroWeighting( double current, double min, double max )
        {
            return 1.0 - ( max - current ) / ( max - min ); 
        }
    }
}
