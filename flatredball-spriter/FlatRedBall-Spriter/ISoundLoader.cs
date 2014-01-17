using Microsoft.Xna.Framework.Audio;

namespace FlatRedBall_Spriter
{
    public interface ISoundLoader
    {
        SoundEffectInstance FromFile(string filename);
    }
}