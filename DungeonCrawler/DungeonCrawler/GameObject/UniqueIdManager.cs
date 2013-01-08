using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.ComponentModel
{
    /// <summary>
    /// Keeps track of and allocates unique identification numbers.
    /// 
    /// TODO: We can make this much better, including recycling unused values
    /// </summary>
    public class UniqueIdManager
    {
        /// <summary>
        /// The next Id to hand out
        /// </summary>
        private ulong mNextId;

        /// <summary>
        /// Constuctor that starts the id values at one.
        /// </summary>
        public UniqueIdManager()
            : this( 1 )
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextId">The next ID to hand out</param>
        public UniqueIdManager( ulong nextId )
        {
            mNextId = nextId;
        }

        /// <summary>
        /// Allocates a new id
        /// </summary>
        /// <returns>The new unique id to use</returns>
        public ulong AllocateId()
        {
            return mNextId++;
        }
    }
}
