using System.Collections.Generic;
using System.Linq;
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
        public void CloneTest()
        {
            var so = GetSimpleSpriterObject();
            var clone = so.Clone();

            Assert.AreNotSame(so, clone);

            Assert.AreEqual(so.Looping, clone.Looping);
            Assert.AreEqual(so.Animating, clone.Animating);
            Assert.AreEqual(so.ObjectList.Count, clone.ObjectList.Count);
            Assert.AreEqual(so.Animations.Count, clone.Animations.Count);
            so.StartAnimation();
            Assert.AreNotEqual(so.Animating, clone.Animating);
            clone.StartAnimation();
            Assert.AreEqual(true, clone.Animating);
            Assert.AreEqual(so.Animating, clone.Animating);

            Assert.AreEqual(so.KeyFrameList.Count, clone.KeyFrameList.Count);

            Assert.IsTrue(Math.Abs(so.KeyFrameList[0].Time - clone.KeyFrameList[0].Time) < .00001f);
            Assert.IsTrue(Math.Abs(so.KeyFrameList[0].Values.First().Value.ScaleX - clone.KeyFrameList[0].Values.First().Value.ScaleX) < .00001f);
            so.KeyFrameList[0].Time = 12345f;
            so.KeyFrameList[0].Values.First().Value.ScaleX = 12345f;

            Assert.IsFalse(Math.Abs(clone.KeyFrameList[0].Time - 12345f) < .00001f);
            Assert.IsFalse(Math.Abs(clone.KeyFrameList[0].Values.First().Value.ScaleX - 12345f) < .00001f);

            
        }

        [TestMethod]
        public void SpriterObjectScaleTest()
        {
            var so = GetSimpleSpriterObject();
            var sprite = (Sprite) so.ObjectList.Single(o => o.Name == "sprite");
            var pivot = so.ObjectList.Single(o => o.Name == "pivot");
            so.ScaleX = .5f;
            so.ScaleY = .25f;
            so.StartAnimation();
            Assert.IsTrue(Math.Abs(sprite.ScaleX - .5f) < .00001f);
            Assert.IsTrue(Math.Abs(sprite.ScaleY - .25f) < .00001f);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 7.5f) < .00001f);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 15f) < .00001f);

            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 3.75f) < .00001f);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 7.5f) < .00001f);

            so.TimedActivity(.5f, 0f, 0f);

            Assert.IsTrue(Math.Abs(sprite.ScaleX - .5f) < .00001f);
            Assert.IsTrue(Math.Abs(sprite.ScaleY - .25f) < .00001f);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 0.0f) < .00001f);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 0.0f) < .00001f);
            
        }

        [TestMethod]
        public void AlphaTweeningTest()
        {
            var so = GetSimpleSpriterObject();
            var sprite = (Sprite)so.ObjectList.Single(o => o.Name == "sprite");
            so.Animations.First().Value.KeyFrames[1].Values[sprite].Alpha = .5f;
            so.StartAnimation();
            Assert.IsTrue(Math.Abs(sprite.Alpha - 1f) < .00001f);
            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(sprite.Alpha - .75f) < .00001f);
            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(sprite.Alpha - .5f) < .00001f);
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

        [TestMethod]
        public void OnlyLoopingAnimationTweensToFirstKeyFrameAfterLast()
        {
            var so = GetSimpleSpriterObject(true);
            so.StartAnimation();
            Assert.IsTrue(so.Looping);
            so.TimedActivity(1.0f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[1].RelativePosition.X - 0.0f) < .0001f);
            Assert.IsTrue(Math.Abs(so.ObjectList[1].RelativePosition.Y - 0.0f) < .0001f);

            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[1].RelativePosition.X - 15.0f) < .0001f);


            so = GetSimpleSpriterObject(false);
            so.StartAnimation();
            Assert.IsFalse(so.Looping);
            so.TimedActivity(1.0f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[0].RelativePosition.X - 0.0f) < .0001f);
            Assert.IsTrue(Math.Abs(so.ObjectList[0].RelativePosition.Y - 0.0f) < .0001f);

            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[0].RelativePosition.X) < .0001f);
        }

        private static SpriterObject GetSimpleSpriterObject(bool loops=false)
        {

            var so = new SpriterObject("Global", false);

            var sprite = new Sprite();
            var pivot = new PositionedObject();
            pivot.AttachTo(so, true);
            sprite.AttachTo(pivot, true);
            sprite.Name = "sprite";
            pivot.Name = "pivot";

            so.Animations.Add("", new SpriterObjectAnimation("", loops, 2.0f, new List<KeyFrame>()));

            // first keyframe (0ms)
            var keyFrame = new KeyFrame
            {
                Time = 0
            };

            // The pivot is at 30,30
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                Position = new Vector3(30f, 30f, 0f)
            };
            // Sprite is just there to connect to the pivot
            keyFrame.Values[sprite] = new KeyFrameValues
            {
                    Alpha = 1.0f,
                    Parent = pivot,
                    ScaleX = 1.0f,
                    ScaleY = 1.0f,
                    Position = new Vector3(30f, 30f, 0f)
                };

            so.Animations[""].KeyFrames.Add(keyFrame);


            keyFrame = new KeyFrame
            {
                    Time = 1.0f
                };
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                Position = Vector3.Zero
            };

            keyFrame.Values[sprite] = new KeyFrameValues
            {
                    Alpha = 1.0f,
                    Parent = pivot,
                    ScaleX = 1.0f,
                    ScaleY = 1.0f,
                    Position = Vector3.Zero
                };

            so.Animations[""].KeyFrames.Add(keyFrame);

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

        [TestMethod]
        public void BoneReparenting()
        {
            var so = new SpriterObject("Global", false);
            var bone1 = new PositionedObject();
            var bone2 = new PositionedObject();

            so.Animations.Add("", new SpriterObjectAnimation("", false, 2.0f, new List<KeyFrame>()));

            var keyFrame = new KeyFrame
            {
                Time = 0f
            };

            keyFrame.Values[bone1] = new KeyFrameValues
            {
                Parent = so
            };

            keyFrame.Values[bone2] = new KeyFrameValues { Parent = bone1 };
            so.Animations[""].KeyFrames.Add(keyFrame);

            keyFrame = new KeyFrame
            {
                Time = 1.0f
            };

            keyFrame.Values[bone1] = new KeyFrameValues { Position = new Vector3(100f, 0f, 0f), Parent = so };
            keyFrame.Values[bone2] = new KeyFrameValues
            {
                Parent = so
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            so.ObjectList.Add(bone1);
            so.ObjectList.Add(bone2);

            so.StartAnimation();

            so.TimedActivity(.5f, 0f, 0f);
            Assert.AreSame(so.ObjectList[0], so.ObjectList[1].Parent);
            Assert.AreSame(so, so.ObjectList[0].Parent);

            so.TimedActivity(.5f, 0f, 0f);
            Assert.AreSame(so, so.ObjectList[1].Parent);
            Assert.AreSame(so, so.ObjectList[0].Parent);
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
            so.Animations.Add("", new SpriterObjectAnimation("", loops, 2.0f, new List<KeyFrame>()));

            var keyFrame = new KeyFrame
            {
                Time = 0
            };
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                Position = Vector3.Zero
            };
            keyFrame.Values[pivot2] = new KeyFrameValues
            {
                Position = Vector3.Zero
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            keyFrame = new KeyFrame
            {
                Time = 1.0f
            };
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                Position = new Vector3(0f, 10f, 0f)
            };
            keyFrame.Values[pivot2] = new KeyFrameValues
            {
                Position = new Vector3(10f, 0f, 0f)
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

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

        /// <summary>
        ///A test for GetPercentageIntoFrame
        ///</summary>
        [TestMethod()]
        public void GetPercentageIntoFrameTestNaN()
        {
            float secondsIntoAnimation = 0F; // TODO: Initialize to an appropriate value
            float currentKeyFrameTime = 0F; // TODO: Initialize to an appropriate value
            float nextKeyFrameTime = 0F; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = SpriterObject.GetPercentageIntoFrame(secondsIntoAnimation, currentKeyFrameTime, nextKeyFrameTime);
            Assert.AreEqual(expected, actual);
        }
    }
}
