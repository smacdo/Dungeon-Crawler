using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common
{
    /// <summary>
    ///  For objects that can be reset to their default state.
    /// </summary>
    public interface IRecyclable
    {
        /// <summary>
        ///  Return the object to the object pool from whence it came.
        /// </summary>
        void Recycle();
    }
}
