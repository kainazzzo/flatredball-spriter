using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Instructions;
using FlatRedBall.Math;

namespace FlatRedBall_Spriter
{
    public class SpriterObject : PositionedObject
    {
        public SpriterObject()
        {
            SpriteList = new PositionedObjectList<Sprite>();
        }

        public PositionedObjectList<Sprite> SpriteList { get; private set; }
    }
}
