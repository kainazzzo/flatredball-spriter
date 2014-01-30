using FlatRedBall;

namespace FlatRedBallExtensions
{
    public class ScaledPositionedObject : PositionedObject, IRelativeScalable
    {
        private bool _parentScaleChangesPosition = true;

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
            ScaleX =
                ScaleY =
                ScaleZ =
                RelativeScaleX =
                RelativeScaleY =
                RelativeScaleZ = 1.0f;
        }


        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }

        public bool ParentScaleChangesPosition
        {
            get { return _parentScaleChangesPosition; }
            set { _parentScaleChangesPosition = value; }
        }

        public float RelativeScaleX { get; set; }
        public float RelativeScaleY { get; set; }
        public float RelativeScaleZ { get; set; }
    }
}
