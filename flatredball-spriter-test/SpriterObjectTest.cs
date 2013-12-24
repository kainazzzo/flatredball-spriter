using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using FlatRedBall;
using FlatRedBallExtensions;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;

namespace flatredball_spriter_test
{


    /// <summary>
    ///This is a test class for SpriterObjectTest and is intended
    ///to contain all SpriterObjectTest Unit Tests
    ///</summary>
    [TestClass]
    public class SpriterObjectTest
    {
        /// <summary>
        ///A test for GetPercentageIntoFrame
        ///</summary>
        [TestMethod]
        public void GetPercentageIntoFrameTest()
        {
            const float secondsIntoAnimation = 1.99F;
            const float currentKeyFrameTime = 1.0F;
            const float nextKeyFrameTime = 2.0F;
            const float expected = .99F;

            float actual = SpriterObject.GetPercentageIntoFrame(secondsIntoAnimation, currentKeyFrameTime, nextKeyFrameTime);
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
            Assert.IsTrue(Math.Abs(so.KeyFrameList[0].Values.First().Value.RelativeScaleX - clone.KeyFrameList[0].Values.First().Value.RelativeScaleX) < .00001f);
            so.KeyFrameList[0].Time = 12345f;
            so.KeyFrameList[0].Values.First().Value.RelativeScaleX = 12345f;

            Assert.IsFalse(Math.Abs(clone.KeyFrameList[0].Time - 12345f) < .00001f);
            Assert.IsFalse(Math.Abs(clone.KeyFrameList[0].Values.First().Value.RelativeScaleX - 12345f) < .00001f);

            var sokeyframeObjects = so.KeyFrameList.SelectMany(k => k.Values.Keys).ToList();
            var clonekeyframeObjects = clone.KeyFrameList.SelectMany(k => k.Values.Keys).ToList();

            CollectionAssert.AreNotEquivalent(sokeyframeObjects, clonekeyframeObjects);
            CollectionAssert.AreNotEquivalent(so.ObjectList, clone.ObjectList);
        }

        [TestMethod]
        public void PositionIsOffsetFromMainObject()
        {
            var so = GetSimpleSpriterObject();
            so.Position.X = 100;
            so.Position.Y = 200;

            var sprite = (ScaledSprite)so.ObjectList.Single(o => o.Name == "sprite");
            var pivot = so.ObjectList.Single(o => o.Name == "pivot");

            so.StartAnimation();
            so.TimedActivity(0, 0, 0);

            Assert.IsTrue(Math.Abs(sprite.Position.X - 130f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Position.Y - 230f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 130f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 230f) < Single.Epsilon);
            TimeManager.CurrentTime += .5;
            so.Position.X += 100;
            so.Position.Y += 100;
            so.TimedActivity(.5f, 0f, 0f);

            

            Assert.IsTrue(Math.Abs(sprite.Position.X - 215f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Position.Y - 315f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 215f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 315f) < Single.Epsilon);

            TimeManager.CurrentTime += .5f;
            so.TimedActivity(.5f, 0f, 0f);

            Assert.IsTrue(Math.Abs(sprite.Position.X - 200f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Position.Y - 300f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 200f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 300f) < Single.Epsilon);

        }

        [TestMethod]
        public void SpriterObjectScaleTest()
        {
            var so = GetSimpleSpriterObject();
            var sprite = (ScaledSprite) so.ObjectList.Single(o => o.Name == "sprite");
            var pivot = so.ObjectList.Single(o => o.Name == "pivot");
            so.ScaleX = .5f;
            so.ScaleY = .25f;
            so.StartAnimation();
            
            Assert.IsTrue(Math.Abs(sprite.Height - 8f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Width - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 7.5f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 15f) < Single.Epsilon);

            TimeManager.CurrentTime += .5;
            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 3.75f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 7.5f) < Single.Epsilon);

            TimeManager.CurrentTime += .5;
            so.TimedActivity(.5f, 0f, 0f);

            Assert.IsTrue(Math.Abs(sprite.Height - 8f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Width - 16f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.Y - 0.0f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Position.X - 0.0f) < Single.Epsilon);
            
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
            TimeManager.CurrentTime += 1.0;
            so.TimedActivity(1.0f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[1].Position.X - 0.0f) < .0001f);
            Assert.IsTrue(Math.Abs(so.ObjectList[1].Position.Y - 0.0f) < .0001f);


            TimeManager.CurrentTime += 0.5;
            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[1].Position.X - 15.0f) < .0001f);


            so = GetSimpleSpriterObject();
            so.StartAnimation();
            Assert.IsFalse(so.Looping);

            TimeManager.CurrentTime += 1.0;
            so.TimedActivity(1.0f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[0].Position.X - 0.0f) < .0001f);
            Assert.IsTrue(Math.Abs(so.ObjectList[0].Position.Y - 0.0f) < .0001f);

            TimeManager.CurrentTime += 0.5;
            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(Math.Abs(so.ObjectList[0].Position.X) < .0001f);
        }

        private static SpriterObject GetSimpleSpriterObject(bool loops=false)
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

        [TestMethod]
        public void Test2Objects()
        {
            var so = new SpriterObject("Global", false);

            var sprite = new ScaledSprite();
            var pivot = new ScaledPositionedObject();
            var sprite2 = new ScaledSprite();
            var pivot2 = new ScaledPositionedObject();

            pivot.AttachTo(so, true);
            sprite.AttachTo(pivot, true);

            pivot2.AttachTo(so, true);
            sprite2.AttachTo(pivot2, true);
            so.Animations.Add("", new SpriterObjectAnimation("", true, 2.0f, new List<KeyFrame>()));

            var keyFrame = new KeyFrame
            {
                Time = 0
            };
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                RelativePosition = Vector3.Zero
            };
            keyFrame.Values[pivot2] = new KeyFrameValues
            {
                RelativePosition = Vector3.Zero
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            keyFrame = new KeyFrame
            {
                Time = 1.0f
            };
            keyFrame.Values[pivot] = new KeyFrameValues
            {
                RelativePosition = new Vector3(0f, 10f, 0f)
            };
            keyFrame.Values[pivot2] = new KeyFrameValues
            {
                RelativePosition = new Vector3(10f, 0f, 0f)
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            so.ObjectList.Add(sprite);
            so.ObjectList.Add(pivot);
            so.ObjectList.Add(sprite2);
            so.ObjectList.Add(pivot2);

            so.StartAnimation();
            TimeManager.CurrentTime += .5;
            so.TimedActivity(.5f, 0f, 0f);

            Assert.AreEqual(5f, so.ObjectList[1].Position.Y);
            Assert.AreEqual(5f, so.ObjectList[3].Position.X);

            TimeManager.CurrentTime += .25;
            so.TimedActivity(.25f, 0f, 0f);
            Assert.AreEqual(7.5f, so.ObjectList[1].Position.Y);
            Assert.AreEqual(7.5f, so.ObjectList[3].Position.X);
        }

        [TestMethod]
        public void BoneReparenting()
        {
            var so = new SpriterObject("Global", false);
            var bone1 = new ScaledPositionedObject();
            var bone2 = new ScaledPositionedObject();

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

            keyFrame.Values[bone1] = new KeyFrameValues { RelativePosition = new Vector3(100f, 0f, 0f), Parent = so };
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

        /// <summary>
        ///A test for GetPercentageIntoFrame
        ///</summary>
        [TestMethod]
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
        [TestMethod]
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

        [TestMethod]
        public void ThreeConnectedBonesAndObjectsPositionedTogether()
        {
            #region xml
            var sos =
                TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b5"">
    <folder id=""0"">
        <file id=""0"" name=""/square.png"" width=""32"" height=""32"" pivot_x=""0"" pivot_y=""1""/>
    </folder>
    <entity id=""0"" name=""entity_000"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <bone_ref id=""0"" timeline=""0"" key=""0""/>
                    <bone_ref id=""1"" parent=""0"" timeline=""1"" key=""0""/>
                    <bone_ref id=""2"" parent=""1"" timeline=""2"" key=""0""/>
                    <object_ref id=""0"" parent=""0"" name=""square1"" folder=""0"" file=""0"" abs_x=""10"" abs_y=""0"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""3"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" parent=""1"" name=""square2"" folder=""0"" file=""0"" abs_x=""10"" abs_y=""0"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""4"" key=""0"" z_index=""1""/>
                    <object_ref id=""2"" parent=""2"" name=""square3"" folder=""0"" file=""0"" abs_x=""10"" abs_y=""0"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""5"" key=""0"" z_index=""2""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""bone1"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""100"" angle=""0"" scale_x=""0.5""/>
                </key>
            </timeline>
            <timeline id=""1"" name=""bone2"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""200"" y=""-0""/>
                </key>
            </timeline>
            <timeline id=""2"" name=""bone3"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""200"" y=""-0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""3"" name=""square1"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-180"" y=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""4"" name=""square2"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-380"" y=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""5"" name=""square3"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-290"" y=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
");
#endregion
            var so = sos.ToRuntime();
            var pivot1 = so.ObjectList[1];
            var pivot2 = so.ObjectList[3];
            var pivot3 = so.ObjectList[5];
            var bone1 = so.ObjectList[6];
            var bone2 = so.ObjectList[7];
            var bone3 = so.ObjectList[8];
            var sprite1 = so.ObjectList[0];
            var sprite2 = so.ObjectList[2];
            var sprite3 = so.ObjectList[4];

            pivot1.Name = "pivot1";
            pivot2.Name = "pivot2";
            pivot3.Name = "pivot3";

            bone1.Name = "bone1";
            bone2.Name = "bone2";
            bone3.Name = "bone3";

            sprite1.Name = "sprite1";
            sprite2.Name = "sprite2";
            sprite3.Name = "sprite3";

            so.StartAnimation();
            
            Assert.IsTrue(Math.Abs(bone1.Position.X - 100f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone2.Position.X - 200f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone3.Position.X - 300f) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(pivot1.Position.X - 10f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot2.Position.X - 10f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot3.Position.X - 10f) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(sprite1.Position.X - 26f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite2.Position.X - 26f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite3.Position.X - 26f) < Single.Epsilon);
        }

        [TestMethod]
        public void KeyFrameValuesScaleDefaultsTo1()
        {
            var kfv = new KeyFrameValues();

            Assert.IsTrue(Math.Abs(kfv.RelativeScaleX - 1.0f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(kfv.RelativeScaleY - 1.0f) < Single.Epsilon);
        }

        [TestMethod]
        public void ThreeScaledBonesRotating()
        {
            #region xml

            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b5"">
    <folder id=""0"">
        <file id=""0"" name=""square.png"" width=""32"" height=""32"" pivot_x=""0"" pivot_y=""1""/>
    </folder>
    <entity id=""0"" name=""entity_000"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <bone_ref id=""0"" timeline=""0"" key=""0""/>
                    <bone_ref id=""1"" parent=""0"" timeline=""1"" key=""0""/>
                    <bone_ref id=""2"" parent=""1"" timeline=""2"" key=""0""/>
                    <object_ref id=""0"" parent=""0"" name=""square1"" folder=""0"" file=""0"" abs_x=""10"" abs_y=""-0"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""3"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" parent=""1"" name=""square2"" folder=""0"" file=""0"" abs_x=""10"" abs_y=""-0"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""4"" key=""0"" z_index=""1""/>
                    <object_ref id=""2"" parent=""2"" name=""square3"" folder=""0"" file=""0"" abs_x=""10"" abs_y=""-0"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""5"" key=""0"" z_index=""2""/>
                </key>
                <key id=""1"" time=""500"">
                    <bone_ref id=""0"" timeline=""0"" key=""1""/>
                    <bone_ref id=""1"" parent=""0"" timeline=""1"" key=""1""/>
                    <bone_ref id=""2"" parent=""1"" timeline=""2"" key=""1""/>
                    <object_ref id=""0"" parent=""0"" timeline=""3"" key=""1"" z_index=""0""/>
                    <object_ref id=""1"" parent=""1"" timeline=""4"" key=""1"" z_index=""1""/>
                    <object_ref id=""2"" parent=""2"" timeline=""5"" key=""1"" z_index=""2""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""bone1"" object_type=""bone"">
                <key id=""0"">
                    <bone x=""100"" angle=""0"" scale_x=""0.5""/>
                </key>
                <key id=""1"" time=""500"" spin=""-1"">
                    <bone x=""100"" angle=""90"" scale_x=""0.5""/>
                </key>
            </timeline>
            <timeline id=""1"" name=""bone2"" object_type=""bone"">
                <key id=""0"">
                    <bone x=""200"" angle=""0""/>
                </key>
                <key id=""1"" time=""500"">
                    <bone x=""200""/>
                </key>
            </timeline>
            <timeline id=""2"" name=""bone3"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""200"" angle=""0"" scale_x=""2""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <bone x=""200"" angle=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""3"" name=""square1"">
                <key id=""0"">
                    <object folder=""0"" file=""0"" x=""-180"" angle=""0"" scale_x=""2""/>
                </key>
                <key id=""1"" time=""500"">
                    <object folder=""0"" file=""0"" x=""-180"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""4"" name=""square2"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-380"" angle=""0"" scale_x=""2""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-380"" angle=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""5"" name=""square3"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-290"" angle=""0""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-290"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";
            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);
            var so = sos.ToRuntime();

            so.StartAnimation();

            TimeManager.CurrentTime += .5;
            so.TimedActivity(.5f, 0.25, .5f);

            var pivot1 = so.ObjectList[1];
            var pivot2 = so.ObjectList[3];
            var pivot3 = so.ObjectList[5];
            var bone1 = so.ObjectList[6];
            var bone2 = so.ObjectList[7];
            var bone3 = so.ObjectList[8];

            Assert.IsTrue(Math.Abs(bone1.Position.X - 100f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone1.Position.Y) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(bone2.Position.X - 100f) < .00001f);
            Assert.IsTrue(Math.Abs(bone2.Position.Y - 100f) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(bone3.Position.X - 100f) < .001f);
            Assert.IsTrue(Math.Abs(bone3.Position.Y - 200f) < .001f);

            Assert.IsTrue(Math.Abs(bone1.RotationZ - MathHelper.ToRadians(90)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone2.RotationZ - MathHelper.ToRadians(90)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone2.RotationZ - MathHelper.ToRadians(90)) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(pivot1.Position.X - 100f) < .001f);
            Assert.IsTrue(Math.Abs(pivot1.Position.Y - -90f) < .001f);
            Assert.IsTrue(Math.Abs(pivot2.Position.X - 100f) < .001f);
            Assert.IsTrue(Math.Abs(pivot2.Position.Y - -90f) < .001f);
            Assert.IsTrue(Math.Abs(pivot3.Position.X - 100f) < .001f);
            Assert.IsTrue(Math.Abs(pivot3.Position.Y - -90f) < .001f);

            Assert.IsTrue(Math.Abs(pivot1.RotationZ - MathHelper.ToRadians(90)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot2.RotationZ - MathHelper.ToRadians(90)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot2.RotationZ - MathHelper.ToRadians(90)) < Single.Epsilon);

            #endregion
        }

        [TestMethod]
        public void BasicPivotTest()
        {
            #region xml
            var sos =
                TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <folder id=""0"">
        <file id=""0"" name=""square.png"" width=""32"" height=""32"" pivot_x=""0"" pivot_y=""1""/>
    </folder>
    <entity id=""0"" name=""entity_000"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"" looping=""false"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" timeline=""1"" key=""0"" z_index=""1""/>
                    <object_ref id=""2"" timeline=""2"" key=""0"" z_index=""2""/>
                    <object_ref id=""3"" timeline=""3"" key=""0"" z_index=""3""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""square"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" scale_x=""0.25"" scale_y=""0.25""/>
                </key>
            </timeline>
            <timeline id=""1"" name=""square_000"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""8"" scale_x=""0.5"" scale_y=""0.5""/>
                </key>
            </timeline>
            <timeline id=""2"" name=""square_001"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""24"" scale_x=""0.75"" scale_y=""0.75""/>
                </key>
            </timeline>
            <timeline id=""3"" name=""square_002"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""48""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
");
            #endregion

            var so = sos.ToRuntime();

            var x = 0;

            
            var pivot1Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(0).Value;
            var pivot1 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(0).Key;
            var sprite1Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(1).Value;
            var sprite1 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(1).Key;
            var pivot2Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(2).Value;
            var pivot2 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(2).Key;
            var sprite2Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(3).Value;
            var sprite2 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(3).Key;
            var pivot3Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(4).Value;
            var pivot3 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(4).Key;
            var sprite3Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(5).Value;
            var sprite3 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(5).Key;
            var pivot4Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(6).Value;
            var pivot4 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(6).Key;
            var sprite4Values = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(7).Value;
            var sprite4 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(7).Key;

            pivot1.Name = "pivot1";
            pivot2.Name = "pivot2";
            pivot3.Name = "pivot3";
            pivot4.Name = "pivot4";
            sprite1.Name = "sprite1";
            sprite2.Name = "sprite2";
            sprite3.Name = "sprite3";
            sprite4.Name = "sprite4";

            
            

            Assert.AreEqual(Vector3.Zero, pivot1Values.RelativePosition);
            Assert.AreEqual(new Vector3(16, -16, 0), sprite1Values.RelativePosition);

            so.StartAnimation();

            TimeManager.CurrentTime += .5;
            so.TimedActivity(.5f, 0.25, .5f);

            Assert.AreEqual(Vector3.Zero, pivot1.Position);
            Assert.AreEqual(new Vector3(4, -4, 0), sprite1.Position);

            Assert.AreEqual(new Vector3(8, 0, 0), pivot2.Position);
            Assert.AreEqual(new Vector3(16, -8, .0001f), sprite2.Position);

            Assert.AreEqual(new Vector3(24, 0, 0), pivot3.Position);
            Assert.AreEqual(new Vector3(36, -12, .0002f), sprite3.Position);
            
            Assert.AreEqual(new Vector3(48, 0, 0), pivot4.Position);
            Assert.IsTrue((new Vector3(64, -16, 0.0003f) - sprite4.Position).Length() < .0001f);
            
        }
    }
}
