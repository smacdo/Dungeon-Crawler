﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.Forge.Engine.Sprites
{
    public class AnimationNotFoundException : GameEngineException
    {
        public string AnimationName { get; private set; }
        public AnimationNotFoundException(string animationName)
            : base(string.Format("Animation not found: {0}", animationName))
        {
            AnimationName = animationName;
        }
    }

    public class AnimationDirectionNotFoundException : GameEngineException
    {
        public string AnimationName { get; private set; }
        public DirectionName Direction { get; private set; }

        public AnimationDirectionNotFoundException(string animationName, DirectionName direction)
            : base(string.Format("Animation '{0}' does not include direction '{1}'", animationName, direction))
        {
            AnimationName = animationName;
            Direction = direction;
        }
    }
}