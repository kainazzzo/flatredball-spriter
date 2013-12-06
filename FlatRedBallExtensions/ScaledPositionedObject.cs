using FlatRedBall;

namespace FlatRedBallExtensions
{
    public class ScaledPositionedObject : PositionedObject
    {
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }

        public ScaledPositionedObject()
        {
            ScaleX = ScaleY = ScaleZ = 1.0f;
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
        }
    }

    public class ScaledSprite : Sprite
    {
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
                return;
            }
            
            ScaleX *= scaledParent.ScaleX;
            ScaleY *= scaledParent.ScaleY;
        }
    }
}
