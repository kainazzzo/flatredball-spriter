using System.Security.Principal;
using FlatRedBall;

namespace FlatRedBallExtensions
{
    public class ScaledSprite : Sprite
    {
        public const int DefaultTextureWidth = 32;
        public const int DefaultTextureHeight = 32;

        public ScaledSprite()
        {
            RelativeScaleX = RelativeScaleY = OriginalScaleX = OriginalScaleY = ScaleX = ScaleY = 1.0f;
        }

        private int TextureWidth
        {
            get
            {
                return Texture == null ? DefaultTextureWidth : Texture.Width;
            }
        }

        private int TextureHeight
        {
            get
            {
                return Texture == null ? DefaultTextureHeight : Texture.Width;
            }
        }

        public override void UpdateDependencies(double currentTime)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            lock (this)
            {
                if (mLastDependencyUpdate == currentTime)
                {
                    return;
                }
                mLastDependencyUpdate = currentTime;
            }

            this.UpdateDependenciesHelper(currentTime);

            var scaledParent = Parent as ScaledPositionedObject;

            

            if (scaledParent == null)
            {
                Width = ScaleX*TextureWidth;
                Height = ScaleY*TextureHeight;
            }
            else
            {
                Width = RelativeScaleX * TextureWidth;
                Height = RelativeScaleY * TextureHeight;
                Width *= scaledParent.ScaleX;
                Height *= scaledParent.ScaleY;
            }
        }

        public float RelativeScaleX { get; set; }
        public float RelativeScaleY { get; set; }

        public float OriginalScaleX { get; set; }
        public float OriginalScaleY { get; set; }
    }
}