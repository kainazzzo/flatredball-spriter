using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FlatRedBall.Math.Geometry;

namespace FlatRedBallExtensions
{
    public class ScaledPolygon : Polygon, IRelativeScalable
    {
        private bool _parentScaleChangesPosition = true;

        public float RelativeScaleX { get; set; }

        public float RelativeScaleY { get; set; }

        public float RelativeScaleZ { get; set; }

        public float ScaleX
        {
            get { return _scaleX; }
            set
            {
                var oldValue = _scaleX;
                _scaleX = value;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != oldValue)
                {
                    RecalculateScale();
                }
            }
        }

        public float ScaleY
        {
            get { return _scaleY; }
            set
            {
                var oldValue = _scaleY;
                _scaleY = value;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != oldValue)
                {
                    RecalculateScale();
                }
            }
        }

        public float ScaleZ { get; set; }

        private IList<Point> _unscaledPoints;
        private bool _alreadyScaled;
        private float _scaleX;
        private float _scaleY;

        public ScaledPolygon()
        {
            ScaleX =
                ScaleY =
                ScaleZ =
                RelativeScaleX =
                RelativeScaleY =
                RelativeScaleZ = 1.0f;
        }

        private void RecalculateScale()
        {
            if (_alreadyScaled)
            {
                Points = new List<Point>(_unscaledPoints);
                ScaleBy(ScaleX, ScaleY);
            }
        }

        public void SetUnscaledPoints(IList<Point> points)
        {
            _unscaledPoints = new List<Point>(points);
            RecalculateScale();
        }

        public bool ParentScaleChangesPosition
        {
            get { return _parentScaleChangesPosition; }
            set { _parentScaleChangesPosition = value; }
        }

        public override void UpdateDependencies(double currentTime)
        {
            lock (this)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (mLastDependencyUpdate == currentTime)
                {
                    return;
                }
                mLastDependencyUpdate = currentTime;
            }

            this.UpdateDependenciesHelper(currentTime);

            if (!_alreadyScaled)
            {
                _alreadyScaled = true;
                SetUnscaledPoints(Points);
            }

            if (Visible)
            {
                // I am only calling this so it calls FillVertexArray();
                // The logic in PositionedObject.UpdateDependencies(double) should mean it doesn't do anything else
                UpdateDependencies(currentTime, true);
            }
        }

        public static ScaledPolygon CreateRectangle(float x, float y, int width, int height)
        {
            return CreateRectangleWithPivot(x, y, width, height, .5f, .5f);
        }

        public static ScaledPolygon CreateRectangleWithPivot(float x, float y, int width, int height, float pivotX, float pivotY)
        {
            var points = new Point[5];

            // clockwise
            points[0] = new Point(0, height);
            points[1] = new Point(width, height);
            points[2] = new Point(width, 0);
            points[3] = new Point(0, 0);
            points[4] = new Point(0, height);
            
            // Now shift over by the pivot value
            for (var i = 0; i < 5; ++i)
            {
                points[i].X -= Math.Abs(width*pivotX);
                points[i].Y -= Math.Abs(height*pivotY);
            }

            return new ScaledPolygon
            {
                X = x,
                Y = y,
                Points = points
            };
        }
    }
}
