using FlatRedBall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatRedBall_Spriter
{
    public class KeyFrameValues
    {
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public Texture2D Texture { get; set; }
        public int Spin { get; set; }
        public PositionedObject Parent { get; set; }
    }
}