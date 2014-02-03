using System;
using System.Collections.Generic;
using FlatRedBall;
using FlatRedBallExtensions;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace flatredball_spriter_test
{
    [TestClass]
    public class SpriterObjectReverseTests
    {
        [TestMethod]
        public void two_key_frames()
        {
            var so = GetSimpleSpriterObject(false);
            so.StartAnimation();
            TimeManager.CurrentTime = .9;
            so.TimedActivity(0.9f, 0.405, 0.9f);
            so.Reverse = true;
            TimeManager.CurrentTime = 1.0f;
            so.TimedActivity(0.4f, 0.005, .1f);

            var pivot = so.ObjectList[1];

            Assert.IsTrue(Math.Abs(pivot.X - 15f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Y - 15f) < Single.Epsilon);
        }

        [TestMethod]
        public void backwards_loop_test()
        {
            var so = GetSimpleSpriterObject(true);
            var soforward = GetSimpleSpriterObject(true);

            so.StartAnimation();
            soforward.StartAnimation();
            so.Reverse = true;
            TimeManager.CurrentTime = .5;
            so.TimedActivity(.5f, .125, .5f);

            TimeManager.CurrentTime = 1.5;
            soforward.TimedActivity(1.5f, 1.125, 1.5f);

            var pivot = so.ObjectList[1];
            var forwardpivot = soforward.ObjectList[1];
            Assert.IsTrue(Math.Abs(pivot.X - forwardpivot.X) < .00001f);
            Assert.IsTrue(Math.Abs(pivot.Y - forwardpivot.Y) < .00001f);
        }

        private static SpriterObject GetSimpleSpriterObject(bool loops = false)
        {
            var so = new SpriterObject("Global", false);

            var sprite = new ScaledSprite();
            var pivot = new ScaledPositionedObject();
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
                RelativePosition = new Vector3(30f, 30f, 0f)
            };
            // Sprite is just there to connect to the pivot
            keyFrame.Values[sprite] = new KeyFrameValues
            {
                Alpha = 1.0f,
                Parent = pivot,
                RelativeScaleX = 1.0f,
                RelativeScaleY = 1.0f
            };

            so.Animations[""].KeyFrames.Add(keyFrame);


            keyFrame = new KeyFrame
            {
                Time = 1.0f
            };
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                RelativePosition = Vector3.Zero
            };

            keyFrame.Values[sprite] = new KeyFrameValues
            {
                Alpha = 1.0f,
                Parent = pivot,
                RelativeScaleX = 1.0f,
                RelativeScaleY = 1.0f,
                RelativePosition = Vector3.Zero
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            so.ObjectList.Add(sprite);
            so.ObjectList.Add(pivot);

            //so.AddToManagers(null);
            return so;
        }
    }
}
