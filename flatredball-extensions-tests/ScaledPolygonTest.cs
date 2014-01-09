using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall;
using FlatRedBall.Math.Geometry;
using FlatRedBallExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flatredball_extensions_tests
{
    [TestClass]
    public class ScaledPolygonTest
    {
        [TestMethod]
        public void ParentScaleAffectsPoints()
        {
            var parent = new ScaledPositionedObject {ScaleX = .5f, ScaleY = .5f};
            var polygon = new ScaledPolygon
            {
                X = 10,
                Y = 10,
                Points = new List<Point>
                {
                    new Point(0, 0),
                    new Point(32, 0),
                    new Point(32, 32),
                    new Point(0, 32),
                    new Point(0, 0)
                }
            };

            polygon.AttachTo(parent, true);

            polygon.UpdateDependencies(1.0);
            polygon.UpdateDependencies(2.0);

            Assert.IsTrue(Math.Abs(polygon.Points[0].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].X - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].X - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[0].Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].Y - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].Y - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].Y) < Single.Epsilon);
        }

        [TestMethod]
        public void ChangingScale()
        {
            var parent = new ScaledPositionedObject { ScaleX = .5f, ScaleY = .5f };
            var polygon = new ScaledPolygon
            {
                X = 10,
                Y = 10,
                Points = new List<Point>
                {
                    new Point(0, 0),
                    new Point(32, 0),
                    new Point(32, 32),
                    new Point(0, 32),
                    new Point(0, 0)
                }
            };

            polygon.AttachTo(parent, true);

            polygon.UpdateDependencies(1.0);
            polygon.RelativeScaleX = .5f;
            polygon.RelativeScaleY = .5f;
            polygon.UpdateDependencies(2.0);

            Assert.IsTrue(Math.Abs(polygon.Points[0].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].X - 8f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].X - 8f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[0].Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].Y - 8f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].Y - 8f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].Y) < Single.Epsilon);
        }

        [TestMethod]
        public void ParentScaleAffectsPosition()
        {
            var parent = new ScaledPositionedObject { ScaleX = .5f, ScaleY = .5f };
            var polygon = new ScaledPolygon
            {
                X = 10,
                Y = 15,
                Points = new List<Point>
                {
                    new Point(0, 0),
                    new Point(32, 0),
                    new Point(32, 32),
                    new Point(0, 32),
                    new Point(0, 0)
                }
            };

            polygon.AttachTo(parent, true);

            polygon.UpdateDependencies(1.0);
            polygon.UpdateDependencies(2.0);

            Assert.IsTrue(Math.Abs(polygon.X - 5f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Y - 7.5f) < Single.Epsilon);
        }

        [TestMethod]
        public void CreateRectangleWithCenterPivotTest()
        {
            var polygon = ScaledPolygon.CreateRectangleWithPivot(0, 0, 32, 32, .5f, .5f);

            Assert.IsTrue(Math.Abs(polygon.X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[0].X - (-16f)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[0].Y - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].X - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].Y - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].X - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].Y - -16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].X - -16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].Y - -16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].X - (-16f)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].Y - 16f) < Single.Epsilon);
        }

        [TestMethod]
        public void CreateRectangleWithBottomLeftPivotTest()
        {
            var polygon = ScaledPolygon.CreateRectangleWithPivot(0, 0, 32, 32, 0, 0);

            Assert.IsTrue(Math.Abs(polygon.X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[0].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[0].Y - 32f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].X - 32f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[1].Y - 32f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].X - 32) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[2].Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[3].Y) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].X) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(polygon.Points[4].Y - 32f) < Single.Epsilon);
        }

        [TestMethod]
        public void NegativeHeightWidth()
        {
            var polygon = ScaledPolygon.CreateRectangleWithPivot(0, 0, -32, -32, 0, 0);
            var absolutePoints = polygon.Points.Select(p => new Point(p.X + polygon.X, p.Y + polygon.Y)).ToList();
            var expectedAbsolutePoints = new[]
            {
                new Point(0, -32),
                new Point(-32, -32),
                new Point(-32, 0),
                new Point(0, 0),
                new Point(0, -32)
            };
            CollectionAssert.AreEquivalent(expectedAbsolutePoints, absolutePoints);
        }

        [TestMethod]
        public void NegativeHeightWidthNegativeScale()
        {
            var polygon = ScaledPolygon.CreateRectangleWithPivot(0, 0, -32, -32, 0, 0);
            
            var expectedAbsolutePoints = new[]
            {
                new Point(0, 32),
                new Point(32, 32),
                new Point(32, 0),
                new Point(0, 0),
                new Point(0, 32)
            };
            polygon.ScaleX = -1f;
            polygon.ScaleY = -1f;
            polygon.UpdateDependencies(1.0);
            var absolutePoints = polygon.Points.Select(p => new Point(p.X + polygon.X, p.Y + polygon.Y)).ToList();
            CollectionAssert.AreEquivalent(expectedAbsolutePoints, absolutePoints);
        }
    }
}
