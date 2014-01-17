using FlatRedBall;
using Microsoft.Xna.Framework.Audio;

namespace FlatRedBall_Spriter
{
    public class FlatRedBallSoundLoader : ISoundLoader
    {
        public SoundEffectInstance FromFile(string filename)
        {
            return FlatRedBallServices.Load<SoundEffect>(filename).CreateInstance();
        }
    }
}