using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scott.dungeon.pipeline
{
    /// <summary>
    /// Holds information on a xml sprite file
    /// </summary>
    public class SpriteFile
    {
        public class AnimationInfo
        {
            public string Name { get; set; }
            public List<Rectangle> FrameOffsets { get; set; }

            public AnimationInfo( string name )
            {
                Name = name;
                FrameOffsets = new List<Rectangle>();
            }
        }

        public string Name { get; set; }
        public string TexturePath { get; set; }
        public List<AnimationInfo> Animations { get; set; }

        public SpriteFile( string name, string texture )
        {
            Name = name;
            TexturePath = texture;
            Animations = new List<AnimationInfo>();
        }
    }
}
