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
    public class AnimationSpeedTests
    {
        [TestMethod]
        public void half_speed()
        {
            var so = GetSimpleSpriterObject();

            so.StartAnimation();
            so.AnimationSpeed = .5f;

            TimeManager.CurrentTime += 1f;
            so.TimedActivity(1f, .5, 1f);

            Assert.AreEqual(15f, so.ObjectList[1].X);
            Assert.AreEqual(15f, so.ObjectList[1].Y);
        }

        [TestMethod]
        public void double_speed()
        {
            var so = GetSimpleSpriterObject();
            so.StartAnimation();
            so.AnimationSpeed = 2f;

            TimeManager.CurrentTime += .25;

            so.TimedActivity(.25f, .125, .25f);

            Assert.AreEqual(15f, so.ObjectList[1].X);
            Assert.AreEqual(15f, so.ObjectList[1].Y);
        }

        [TestMethod]
        public void negative_speed()
        {
            var so = GetSimpleSpriterObject();
            so.StartAnimation();
            TimeManager.CurrentTime += 1;
            so.TimedActivity(1, .5, 1);
            so.AnimationSpeed = -1;

            TimeManager.CurrentTime += .5;
            so.TimedActivity(.5f, .125, .5f);

            Assert.AreEqual(15f, so.ObjectList[1].X);
            Assert.AreEqual(15f, so.ObjectList[1].Y);
        }

        [TestMethod]
        public void negative_past_first_keyframe_withloop()
        {
            var so = GetSimpleSpriterObject(true);
            so.StartAnimation();
            so.AnimationSpeed = -1;
            TimeManager.CurrentTime += 1.5;
            so.TimedActivity(1.5f, 1.125, 1.5f);

            Assert.AreEqual(15f, so.ObjectList[1].X);
            Assert.AreEqual(15f, so.ObjectList[1].Y);
        }

        [TestMethod]
        public void negative_past_first_keyframe_withoutloop()
        {
            var so = GetSimpleSpriterObject();
            so.StartAnimation();
            so.AnimationSpeed = -1;
            TimeManager.CurrentTime += 1.5;
            bool pass = false;

            so.AnimationFinished += animation => pass = true;

            so.TimedActivity(1.5f, 1.125, 1.5f);
            
            Assert.IsTrue(pass);
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
