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
            SpriteList = new PositionedObjectList<PositionedObject>();
            SpriteInstructions = new Dictionary<Sprite, KeyFrameValues>();
        }

        public PositionedObjectList<PositionedObject> SpriteList { get; private set; }

        public Dictionary<Sprite, KeyFrameValues> SpriteInstructions { get; private set; }
    }
}
