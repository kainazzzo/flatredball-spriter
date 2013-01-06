using System.Collections.Generic;
using FlatRedBall;

namespace FlatRedBall_Spriter
{
    public class KeyFrame
    {
        public KeyFrame()
        {
            Values = new Dictionary<PositionedObject, KeyFrameValues>();
        }

        public float Time { get; set; }
        public Dictionary<PositionedObject, KeyFrameValues> Values { get; set; }
    }
}