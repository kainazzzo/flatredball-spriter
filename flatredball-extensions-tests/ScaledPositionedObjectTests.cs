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

        [TestMethod]
        public void RotationTest()
        {
            var parent = new ScaledPositionedObject
            {
                Position = Vector3.Zero,
                ScaleX = 1.0f,
                ScaleY = 1.0f,
                ScaleZ = 1.0f
            };

            var spo = new ScaledPositionedObject();
            spo.AttachTo(parent, true);
            spo.RelativePosition = new Vector3(10f, 0f, 0f);

            parent.RotationZ += MathHelper.ToRadians(90f);

            spo.UpdateDependencies(0);

            Assert.IsTrue(Math.Abs(spo.Position.X - 10f) >= Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Y - 10f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Z - 0f) < Single.Epsilon);
        }

        [TestMethod]
        public void ScaledRotatedPosition()
        {
            var parent = new ScaledPositionedObject
            {
                Position = Vector3.Zero,
                ScaleX = .5f,
                ScaleY = 1.0f,
                ScaleZ = 1f
            };

            var spo = new ScaledPositionedObject();
            spo.AttachTo(parent, true);
            spo.RelativePosition = new Vector3(10f, 0f, 0f);

            parent.RotationZ += MathHelper.ToRadians(90f);

            spo.UpdateDependencies(0);

            Assert.IsTrue(Math.Abs(spo.Position.X - 5f) >= Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Y - 5f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Z - 0f) < Single.Epsilon);
        }

        [TestMethod]
        public void ScaledRotationPositionOffOrigin()
        {
            var parent = new ScaledPositionedObject
            {
                Position = new Vector3(100f, 100f, 100f),
                ScaleX = .5f,
                ScaleY = 1.0f,
                ScaleZ = 1f
            };

            var spo = new ScaledPositionedObject();
            spo.AttachTo(parent, true);
            spo.RelativePosition = new Vector3(10f, 0f, 0f);

            parent.RotationZ += MathHelper.ToRadians(90f);

            spo.UpdateDependencies(0);

            Assert.IsTrue(Math.Abs(spo.Position.X - 105f) >= Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Y - 105f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(spo.Position.Z - 100f) < Single.Epsilon);
        }

        [TestMethod]
        public void RelativeRotationTest()
        {
            var parent = new ScaledPositionedObject
            {
                Position = new Vector3(100f, 100f, 100f),
            };
            
            var spo = new ScaledPositionedObject();
            spo.AttachTo(parent, true);
            spo.RelativePosition = new Vector3(10f, 0f, 0f);
            spo.RelativeRotationZ = MathHelper.ToRadians(90f);
            parent.RotationZ += MathHelper.ToRadians(90f);

            spo.UpdateDependencies(1);

            var rotationZ = MathHelper.ToDegrees(spo.RotationZ);
            Assert.IsTrue(Math.Abs(rotationZ - 180f) < .0001f);
        }

        [TestMethod]
        public void RelativeRotationScaledPositionTest()
        {
            var parent1 = new ScaledPositionedObject
            {
                Position = new Vector3(100f, 100f, 100f)
            };

            var parent2 = new ScaledPositionedObject
            {
                ScaleX = .5f,
                Position = new Vector3(100f, 100f, 100f)
            };

            var spo = new ScaledPositionedObject();
            parent2.AttachTo(parent1, true);
            spo.AttachTo(parent2, true);

            spo.RelativePosition = new Vector3(10f, 10f, 0f);
            parent2.RelativeRotationZ = MathHelper.ToRadians(90f);
            parent1.RotationZ += MathHelper.ToRadians(90f);

            spo.UpdateDependencies(1);

            var rotationZ = MathHelper.ToDegrees(spo.RotationZ);
            Assert.IsTrue(Math.Abs(rotationZ - 180f) < .0001f);
            Assert.IsTrue(Math.Abs(spo.Position.X - 95f) < .0001f);
            Assert.IsTrue(Math.Abs(spo.Position.Y - 90f) < .0001f);
        }

        [TestMethod]
        public void ScaleDefaultsTo1()
        {
            var spo = new ScaledPositionedObject();
            Assert.AreEqual(1.0f, spo.ScaleX);
            Assert.AreEqual(1.0f, spo.ScaleY);
            Assert.AreEqual(1.0f, spo.ScaleZ);
        }

        [TestMethod]
        public void ThreeChainTest()
        {
            var parent1 = new ScaledPositionedObject
            {
                Position = new Vector3(0f, 0f, 0f)
            };

            var parent2 = new ScaledPositionedObject
            {
                Position = new Vector3(200f, 0f, 0f)
            };

            var parent3 = new ScaledPositionedObject
            {
                Position = new Vector3(300f, 0f, 0f)
            };

            var spo = new ScaledPositionedObject();
            parent3.AttachTo(parent2, true);
            parent2.AttachTo(parent1, true);
            spo.AttachTo(parent3, true);

            spo.RelativePosition = new Vector3(10f, 0f, 0f);
            parent1.RotationZ += MathHelper.ToRadians(90f);
            parent2.RelativeRotationZ = MathHelper.ToRadians(90f);

            spo.UpdateDependencies(1);

            var rotationZ = MathHelper.ToDegrees(spo.RotationZ);

            Assert.IsTrue(Math.Abs(rotationZ - 180f) < .0001f);
            Assert.IsTrue(Math.Abs(spo.Position.X - -110f) < .0001f);
            Assert.IsTrue(Math.Abs(spo.Position.Y - 200f) < .0001f);
        }
    }
}
