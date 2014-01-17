using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall.Audio;
using Microsoft.Xna.Framework.Audio;

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
        public Dictionary<SoundEffectInstance, SoundEffectFrameValues> SoundEffectValues { get; set; }
    }
}