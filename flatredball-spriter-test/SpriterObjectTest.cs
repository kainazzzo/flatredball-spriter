using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace flatredball_spriter_test
{


    /// <summary>
    ///This is a test class for SpriterObjectTest and is intended
    ///to contain all SpriterObjectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpriterObjectTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetPercentageIntoFrame
        ///</summary>
        [TestMethod()]
        public void GetPercentageIntoFrameTest()
        {
            float secondsIntoAnimation = 1.99F; // TODO: Initialize to an appropriate value
            float currentKeyFrameTime = 1.0F; // TODO: Initialize to an appropriate value
            float nextKeyFrameTime = 2.0F; // TODO: Initialize to an appropriate value
            float expected = .99F; // TODO: Initialize to an appropriate value
            float actual;

            actual = SpriterObject.GetPercentageIntoFrame(secondsIntoAnimation, currentKeyFrameTime, nextKeyFrameTime);
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void AnimationEndsAfterLength()
        {
            var so = GetSimpleSpriterObject();
            Assert.IsFalse(so.Animating);
            so.StartAnimation();
            Assert.IsTrue(so.Animating);
            so.TimedActivity(.999f, 0f, 0f);
            Assert.IsTrue(so.Animating);
            Assert.AreEqual(0, so.CurrentKeyFrameIndex);
            so.TimedActivity(.013f, 0f, 0f);
            Assert.AreEqual(1.012f, so.SecondsIn);
            Assert.IsTrue(so.Animating);
            Assert.AreEqual(1, so.CurrentKeyFrameIndex);
            so.TimedActivity(.987f, 0f, 0f);
            Assert.IsTrue(so.Animating);
            Assert.AreEqual(1.999f, so.SecondsIn);
        }

        

        [TestMethod]
        public void AnimationLoops()
        {
            var so = GetSimpleSpriterObject(true);
            so.StartAnimation();
            Assert.IsTrue(so.Looping);
            so.TimedActivity(1.0f, 0f, 0f);
            so.TimedActivity(0.99f, 0f, 0f);
            Assert.AreEqual(1.99f, so.SecondsIn);
            so.TimedActivity(.3f, 0f, 0f);
            Assert.IsTrue(Math.Abs(.29f - so.SecondsIn) < .0001f);
            Assert.IsTrue(so.Animating);
        }

        private static SpriterObject GetSimpleSpriterObject(bool loops=false)
        {

            var so = new SpriterObject("Global", false);

            var sprite = new Sprite();
            var pivot = new PositionedObject();
            pivot.AttachTo(so, true);
            sprite.AttachTo(pivot, true);

            so.Looping = loops;

            so.AnimationTotalTime = 2.0f;
            var keyFrame = new KeyFrame()
                {
                    Time = 0
                };
            keyFrame.Values[pivot] = new KeyFrameValues()
            {
                Position = new Vector3(30f, 30f, 0f)
            };

            so.KeyFrameList.Add(keyFrame);

            keyFrame = new KeyFrame()
                {
                    Time = 1.0f
                };
            keyFrame.Values[pivot] = new KeyFrameValues()
            {
                Position = Vector3.Zero
            };

            so.KeyFrameList.Add(keyFrame);

            so.ObjectList.Add(sprite);
            so.ObjectList.Add(pivot);
            return so;
        }

        [TestMethod]
        public void Test2Objects()
        {
            var so = GetSimpleSpriterObjectWithTwoObjects(true);

            so.StartAnimation();
            so.TimedActivity(.5f, 0f, 0f);

            Assert.AreEqual(5f, so.ObjectList[1].RelativePosition.Y);
            Assert.AreEqual(5f, so.ObjectList[3].RelativePosition.X);

            so.TimedActivity(.25f, 0f, 0f);
            Assert.AreEqual(7.5f, so.ObjectList[1].RelativePosition.Y);
            Assert.AreEqual(7.5f, so.ObjectList[3].RelativePosition.X);

        }

        private static SpriterObject GetSimpleSpriterObjectWithTwoObjects(bool loops = false)
        {

            var so = new SpriterObject("Global", false);

            var sprite = new Sprite();
            var pivot = new PositionedObject();
            var sprite2 = new Sprite();
            var pivot2 = new PositionedObject();

            pivot.AttachTo(so, true);
            sprite.AttachTo(pivot, true);

            pivot2.AttachTo(so, true);
            sprite2.AttachTo(pivot2, true);

            so.Looping = loops;

            so.AnimationTotalTime = 2.0f;
            var keyFrame = new KeyFrame()
            {
                Time = 0
            };
            keyFrame.Values[pivot] = new KeyFrameValues()
            {
                Position = Vector3.Zero
            };
            keyFrame.Values[pivot2] = new KeyFrameValues()
            {
                Position = Vector3.Zero
            };

            so.KeyFrameList.Add(keyFrame);

            keyFrame = new KeyFrame()
            {
                Time = 1.0f
            };
            keyFrame.Values[pivot] = new KeyFrameValues()
            {
                Position = new Vector3(0f, 10f, 0f)
            };
            keyFrame.Values[pivot2] = new KeyFrameValues()
            {
                Position = new Vector3(10f, 0f, 0f)
            };

            so.KeyFrameList.Add(keyFrame);

            so.ObjectList.Add(sprite);
            so.ObjectList.Add(pivot);
            so.ObjectList.Add(sprite2);
            so.ObjectList.Add(pivot2);

            return so;
        }


        /// <summary>
        ///A test for GetPercentageIntoFrame
        ///</summary>
        [TestMethod()]
        public void GetPercentageIntoFrameTestInfinity()
        {
            float secondsIntoAnimation = 1.733335f; // TODO: Initialize to an appropriate value
            float currentKeyFrameTime = 1.722f; // TODO: Initialize to an appropriate value
            float nextKeyFrameTime = 1.722f; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = SpriterObject.GetPercentageIntoFrame(secondsIntoAnimation, currentKeyFrameTime, nextKeyFrameTime);
            Assert.AreEqual(expected, actual);
        }
    }
}
