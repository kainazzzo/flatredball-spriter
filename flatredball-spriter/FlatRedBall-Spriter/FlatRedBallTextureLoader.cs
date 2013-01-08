using FlatRedBall;
using Microsoft.Xna.Framework.Graphics;

namespace FlatRedBall_Spriter
{
    public class FlatRedBallTextureLoader : ITextureLoader
    {
        public Texture2D FromFile(string filename)
        {
            return FlatRedBallServices.Load<Texture2D>(filename);
        }
    }
}