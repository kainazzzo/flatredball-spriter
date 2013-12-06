using FlatRedBall;

namespace FlatRedBallExtensions
{
    public class ScaledPositionedObject : PositionedObject
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
        }

        public ScaledPositionedObject()
        {
            ScaleX = ScaleY = ScaleZ = 1.0f;
        }


        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }

    }
}
