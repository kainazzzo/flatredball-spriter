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
            KeyFrameList = new List<KeyFrame>();
            ObjectList = new PositionedObjectList<PositionedObject>();
        }

        public PositionedObjectList<PositionedObject> ObjectList { get; private set; }
        public List<KeyFrame> KeyFrameList { get; private set; }
    }
}
