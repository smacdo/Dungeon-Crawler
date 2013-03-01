using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.GameContent
{
    public static class Constants
    {
        public const int DIRECTION_COUNT = 4;
    }

    public enum Direction
    {
        North,
        West,
        South,
        East
    }

    /// <summary>
    /// The action to perform when an action has ended
    /// </summary>
    public enum AnimationEndingAction
    {
        Stop,         // Freeze on the last played animation frame
        Loop,         // Restart on animation frame zero and continue animating
        StopAndReset, // Freeze on the first animation frame
    }
}
