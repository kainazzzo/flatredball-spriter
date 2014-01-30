using Microsoft.Xna.Framework.Graphics;

namespace FlatRedBall_Spriter
{
    public interface ITextureLoader
    {
        Texture2D FromFile(string filename);
    }
}