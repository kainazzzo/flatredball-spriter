using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Math.Geometry;
using FlatRedBallExtensions;

namespace FlatRedBall_Spriter
{
    public class SpriterBone : ScaledPositionedObject
    {
        public float Length { get; set; }

        public bool Visible { get; set; }

        private Line _line = null;

        private bool _added = false;

        public override void UpdateDependencies(double currentTime)
        {
            base.UpdateDependencies(currentTime);

            if (Visible)
            {
                if (_line == null)
                {
                    _line = new Line();
                    _line.AttachTo(this, false);
                    _line.RelativePoint1 = new Point3D(0, 0);
                    _line.RelativePoint2 = new Point3D(Length * ScaleX, 0);
                }

                if (!_added)
                {
                    ShapeManager.AddLine(_line);
                    _added = true;
                }
                if (Math.Abs(_line.RelativePoint2.X - Length * ScaleX) > Double.Epsilon)
                {
                    _line.RelativePoint2.X = Length;
                }
            }

            if (!Visible && _line != null)
            {
                ShapeManager.Remove(_line);
                _added = false;
            }
        }
    }
}
