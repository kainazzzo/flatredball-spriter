using FlatRedBall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatRedBall_Spriter
{
    public class KeyFrameValues
    {
        public KeyFrameValues()
        {
            RelativeScaleX = RelativeScaleY = 1.0f;
        }

        public Vector3 RelativeRotation { get; set; }
        public Vector3 RelativePosition { get; set; }
        public float RelativeScaleX { get; set; }
        public float RelativeScaleY { get; set; }
        public Texture2D Texture { get; set; }
        public int Spin { get; set; }
        public PositionedObject Parent { get; set; }

        public float Alpha { get; set; }
    }
}