using System.Collections.Generic;
using System.Linq;

namespace FlatRedBall_Spriter
{
    public class SpriterObjectAnimation
    {
        public SpriterObjectAnimation(string name, bool looping, float totalTime, IEnumerable<KeyFrame> keyFrameList)
        {
            Name = name;
            TotalTime = totalTime;
            Looping = looping;
            KeyFrames = new List<KeyFrame>(keyFrameList.ToList());
        }

        public string Name { get; private set; }

        public List<KeyFrame> KeyFrames { get; private set; } 

        public bool Looping { get; private set; }

        public float TotalTime { get; private set; }
    }
}