using System;
using FlatRedBall.Math.Geometry;
using FlatRedBallExtensions;

namespace FlatRedBall_Spriter
{
    public class SpriterPoint : ScaledPositionedObject
    {
        public bool Visible { get; set; }

        private Line _line = null;
        private Circle _circle = null;

        private bool _added = false;

        private const float Radius = 5.0f;

        public override void UpdateDependencies(double currentTime)
        {
            base.UpdateDependencies(currentTime);

            if (Visible)
            {
                if (_line == null)
                {
                    _line = new Line();
                    _circle = new Circle {Radius = Radius};

                    _circle.AttachTo(this, false);
                    _line.AttachTo(_circle, false);
                    _line.RelativePoint1 = new Point3D(0, 0);
                    _line.RelativePoint2 = new Point3D(Radius, 0);
                }

                if (!_added)
                {
                    ShapeManager.AddLine(_line);
                    ShapeManager.AddCircle(_circle);
                    _added = true;
                }
            }

            if (!Visible && _line != null)
            {
                ShapeManager.Remove(_line);
                ShapeManager.Remove(_circle);
                _added = false;
            }
        }
    }
}