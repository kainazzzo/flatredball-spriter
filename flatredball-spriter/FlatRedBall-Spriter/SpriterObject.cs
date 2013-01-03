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
            SpriteInstructions = new Dictionary<Sprite, InstructionList>();
        }

        public PositionedObjectList<Sprite> SpriteList { get; private set; }

        public Dictionary<Sprite, InstructionList> SpriteInstructions { get; private set; }
    }
}
