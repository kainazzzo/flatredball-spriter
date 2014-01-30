using System.Security.Principal;
using FlatRedBall;

namespace FlatRedBallExtensions
{
    public class ScaledSprite : Sprite, IRelativeScalable
    {
        private bool _parentScaleChangesPosition = true;
        public const int DefaultTextureWidth = 32;
        public const int DefaultTextureHeight = 32;

        public ScaledSprite()
        {
            RelativeScaleX = RelativeScaleY = RelativeScaleZ = 1.0f;
        }

        public int TextureWidth
        {
            get
            {
                return Texture == null ? DefaultTextureWidth : Texture.Width;
            }
        }

        public int TextureHeight
        {
            get
            {
                return Texture == null ? DefaultTextureHeight : Texture.Height;
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
        public float RelativeScaleZ { get; set; }
        public float ScaleZ { get; set; }

        public bool ParentScaleChangesPosition
        {
            get { return _parentScaleChangesPosition; }
            set { _parentScaleChangesPosition = value; }
        }
    }
}