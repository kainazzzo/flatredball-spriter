using System;
using FlatRedBall;
using FlatRedBallExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace flatredball_extensions_tests
{
    [TestClass]
    public class ScaledPositionedObjectTests
    {
        [TestMethod]
        public void RelativePositionAffectedByParentScale()
        {
            var spo = new ScaledPositionedObject { Position = new Vector3(100, 200, 300) };
            var parent = new ScaledPositionedObject();

            spo.AttachTo(parent, true);

            parent.ScaleX = .5f;
            parent.ScaleY = .5f;
            parent.ScaleZ = .5f;

            spo.UpdateDependencies(0);


            Assert.IsTrue(Math.Abs(spo.Position.X - 50f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Y - 100f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Z - 150f) < Single.Epsilon);
        }

        [TestMethod]
        public void RelativeScalingDoesNotBreakPositioning()
        {
            var parent = new ScaledPositionedObject
            {
                Position = new Vector3
                {
                    X = 100f,
                    Y = 200f,
                    Z = 300f
                },
                ScaleX = .5f,
                ScaleY = .5f,
                ScaleZ = .5f
            };

            var spo = new ScaledPositionedObject();
            spo.AttachTo(parent, true);
            spo.RelativePosition = new Vector3(10f, 20f, 30f);

            spo.UpdateDependencies(0);
            
            Assert.IsTrue(Math.Abs(spo.Position.X - 105f) < Single.Epsilon);      
            Assert.IsTrue(Math.Abs(spo.Position.Y - 210f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Z - 315f) < Single.Epsilon);
        }
    }
}
