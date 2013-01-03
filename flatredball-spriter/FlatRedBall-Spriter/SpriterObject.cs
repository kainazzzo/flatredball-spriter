using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Math;

namespace FlatRedBall_Spriter
{
    public class SpriterObject : PositionedObject
    {
        public SpriterObject(PositionedObjectList<Sprite> spriteList)
        {
            SpriteList = spriteList;
        }

        public PositionedObjectList<Sprite> SpriteList { get; private set; } 
    }
}
