using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBallExtensions;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;
using Telerik.JustMock;

namespace flatredball_spriter_test
{


    /// <summary>
    ///This is a test class for SpriterObjectSaveTest and is intended
    ///to contain all SpriterObjectSaveTest Unit Tests
    ///</summary>
    [TestClass]
    public class SpriterObjectSaveTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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

        [TestMethod]
        public void LoopingDefaultsToTrue()
        {

            var sos =
                SpriterObjectSave.FromFile(
                    @"C:\flatredballprojects\flatredball-spriter\spriterfiles\monsterexample\mytest.scml");
            Assert.IsTrue(sos.Entity[0].Animation[0].Looping);

        }

        [TestMethod]
        public void OpacityConversion()
        {
            var sos = GetSimpleSpriterObjectSaveNullTexture();
            var so = sos.ToRuntime();
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames[0].Values.ElementAt(1).Value.Alpha - 1f) < .00001f);
            sos.Entity[0].Animation[0].Timeline[0].Key[0].Object.Alpha = .5f;
            so = sos.ToRuntime();
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames[0].Values.ElementAt(1).Value.Alpha - .5f) < .00001f);
        }

        [TestMethod]
        public void TestAnimationTotalLength()
        {
            var sos = GetSimpleSpriterObjectSaveNullTexture();
            var so = sos.ToRuntime();
            Assert.IsNotNull(so);
            so.StartAnimation();
            Assert.AreEqual(1, so.KeyFrameList.Count);
            Assert.AreEqual(2.0f, so.AnimationTotalTime);
        }

        [TestMethod]
        public void MultipleAnimationTotalLength()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithMultipleAnimations();
            var so = sos.ToRuntime();

            so.StartAnimation("0");
            Assert.AreEqual(1.0f, so.AnimationTotalTime);

            so.StartAnimation("1");
            Assert.AreEqual(2.0f, so.AnimationTotalTime);
        }

        [TestMethod]
        public void TestKeyFrameTime()
        {
            var sos = GetSimpleSpriterObjectSaveNullTexture();
            var so = sos.ToRuntime();
            so.StartAnimation();
            Assert.AreEqual(.3f, so.KeyFrameList[0].Time);
        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTexture()
        {
            return new SpriterObjectSave
                {
                    TextureLoader = Mock.Create<ITextureLoader>(),
                    Directory = "C:\\",
                    Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                    Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id=0, Name="",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length=2000,
                                                    Id=0, Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys=new List<Key>
                                                                {
                                                                    new Key
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                                {
                                                                                    new KeyObjectRef
                                                                                        {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=0,
                                                                            Spin=1,
                                                                            Time = 300
                                                                        }
                                                                }
                                                        },
                                                    Name="First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                        {
                                                            new SpriterDataEntityAnimationTimeline
                                                                {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>
                                                                        {
                                                                            new Key
                                                                            {
                                                                                    Object = new KeyObject(), Id=0, Spin=1,
                                                                                    Time=300
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
                };
        }

        [TestMethod]
        public void BonesGetAddedToKeyframes()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithSingleBone();
            var so = sos.ToRuntime();
            Assert.AreEqual(1, so.ObjectList.Count);
            CollectionAssert.AllItemsAreInstancesOfType(so.ObjectList, typeof(ScaledPositionedObject));
            Assert.IsNotNull(so.Animations.First().Value.KeyFrames[0].Values.FirstOrDefault());
            var bone = so.ObjectList[0];
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.ElementAt(1).Values[bone].RelativePosition.X - 100f) < .0001f);
        }

        [TestMethod]
        public void BoneParenting()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithTwoBones();
            var so = sos.ToRuntime();
            Assert.AreEqual(2, so.ObjectList.Count);
            Assert.AreSame(so, so.Animations.First().Value.KeyFrames[0].Values[so.ObjectList[0]].Parent);
            Assert.AreSame(so.ObjectList[0],
                so.Animations.First().Value.KeyFrames[0].Values[so.ObjectList[1]].Parent);
        }

        [TestMethod]
        public void DoubleBoneParenting()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithBonesAsObjectParent();
            var so = sos.ToRuntime();

            //var pivot = so.ObjectList.Single(o => o.Name == "pivot");
            //var sprite = so.ObjectList.Single(o => o.Name == "sprite");
            var bone1 = so.ObjectList.Single(o => o.Name == "bone0");
            var bone2 = so.ObjectList.Single(o => o.Name == "bone1");

            Assert.AreSame(bone1, so.Animations.First().Value.KeyFrames.First().Values[bone2].Parent);
        }

        [TestMethod]
        public void ObjectAttachedToPivotIfNoParentSet()
        {
            var sos = GetSimpleSpriterObjectSaveNullTexture();
            var so = sos.ToRuntime();
            var pivot = so.ObjectList.Single(p => p.Name.Contains("pivot"));
            var sprite = (Sprite)so.ObjectList.Single(p => p.Name.Contains("sprite"));

            Assert.AreSame(so, so.Animations.First().Value.KeyFrames[0].Values[pivot].Parent);
            Assert.AreSame(pivot, so.Animations.First().Value.KeyFrames[0].Values[sprite].Parent);
            Assert.AreSame(pivot, sprite.Parent);
            Assert.AreSame(so, pivot.Parent);
        }

        [TestMethod]
        public void ObjectWithBoneParent()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithSingleBoneAsObjectParent();
            var so = sos.ToRuntime();

            Assert.AreEqual(3, so.ObjectList.Count);

            var pivot = so.ObjectList.Single(p => p.Name.Contains("pivot"));
            var bone = so.ObjectList[2];

            Assert.AreSame(bone, so.Animations.First().Value.KeyFrames[0].Values[pivot].Parent);
            Assert.IsTrue(so.Animations.First().Value.KeyFrames.All(k => k.Values[pivot].Parent == bone));
        }

        [TestMethod]
        public void ConvertTwoObjects()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWith2ObjectRefs();
            var so = sos.ToRuntime();
            so.StartAnimation();
            Assert.AreEqual(1, so.KeyFrameList.Count);
            Assert.AreEqual(.3f, so.KeyFrameList[0].Time);
        }

        [TestMethod]
        public void ZIndexTest()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWith2ObjectRefs();
            var so = sos.ToRuntime();
            so.StartAnimation();

            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values.ElementAt(0).Value.RelativePosition.Z);
            Assert.AreNotEqual(0.0f, so.KeyFrameList[0].Values.ElementAt(3).Value.RelativePosition.Z);
            Assert.IsTrue(Math.Abs(so.KeyFrameList[0].Values.ElementAt(3).Value.RelativePosition.Z - .0001f) < .00001);
        }

        [TestMethod]
        public void UnreferencedKeysGetAddedStill()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWith2ObjectRefsAndExtraKey();
            var so = sos.ToRuntime();

            var pivot = so.ObjectList.Single(o => o.Name.Contains("pivot"));
            var bone = so.ObjectList.Single(o => !o.Name.Contains("pivot") && !o.Name.Contains("sprite"));

            Assert.AreEqual(4, so.Animations.First().Value.KeyFrames.Count);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.ElementAt(1).Time - .207f) < .0001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.ElementAt(2).Time - .207f) < .0001f);
            so.StartAnimation();
            so.TimedActivity(.205f, 0f, 0f);
            Assert.IsTrue(Math.Abs(pivot.RelativePosition.X - 220.0f) < .00001f);
            Assert.AreSame(bone, pivot.Parent);
            so.TimedActivity(.02f, 0f, 0f);
            Assert.IsTrue(Math.Abs(pivot.RelativePosition.X - 281f) < .00001f);
            Assert.AreSame(so, pivot.Parent);
        }

        [TestMethod]
        public void BoneRotation()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithSingleBoneRotated();
            var so = sos.ToRuntime();

            Assert.IsTrue(
                Math.Abs(0f - so.Animations.First().Value.KeyFrames.ElementAt(0).Values.First().Value.RelativeRotation.Z) < .0001f);

            Assert.IsTrue(
    Math.Abs(45f -
             so.Animations.First().Value.KeyFrames.ElementAt(1).Values.First().Value.RelativeRotation.Z) < .0001f);
            Assert.AreEqual(1, so.Animations.First().Value.KeyFrames.ElementAt(0).Values.First().Value.Spin);
            Assert.AreEqual(-1, so.Animations.First().Value.KeyFrames.ElementAt(1).Values.First().Value.Spin);
        }

        [TestMethod]
        public void AllAnimationsConvertToRuntimeObject()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithMultipleAnimations();
            var so = sos.ToRuntime();

            Assert.IsNotNull(so.Animations);
            Assert.AreEqual(2, so.Animations.Count);
            Assert.IsTrue(so.Animations.ContainsKey("0"));
            Assert.IsTrue(so.Animations.ContainsKey("1"));

            so.StartAnimation("0");
            Assert.IsTrue(so.Animating);
            Assert.IsTrue(Math.Abs(so.CurrentKeyFrame.Time - .3f) < .0001);
            so.StartAnimation();
            Assert.IsTrue(so.Animating);
            Assert.IsTrue(Math.Abs(so.CurrentKeyFrame.Time - .3f) < .0001);
            so.StartAnimation("1");
            Assert.IsTrue(so.Animating);
            Assert.IsTrue(Math.Abs(so.CurrentKeyFrame.Time - .1f) < .0001);
            so.StartAnimation();
            Assert.IsTrue(so.Animating);
            Assert.IsTrue(Math.Abs(so.CurrentKeyFrame.Time - .1f) < .0001);

            bool pass = false;
            try
            {
                so.StartAnimation("blah");
            }
            catch (ArgumentException)
            {
                pass = true;
            }

            Assert.IsTrue(pass, string.Format("{0}.StartAnimation didn't throw exception for an invalid animation name.", typeof(SpriterObject)));
        }


        [TestMethod]
        public void RelativeScaleWithObjectUnscaled()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithBonesAsObjectParentAndOneOrphanObject();
            var so = sos.ToRuntime();

            var sprite = so.ObjectList.First(p => p.Name .Contains("sprite"));
            var sprite2 = so.ObjectList.Where(p => p.Name.Contains("sprite")).ElementAt(1);
            var pivot = sprite.Parent;
            var pivot2 = sprite2.Parent;

            var bones =
                so.Animations.First()
                    .Value.KeyFrames.SelectMany(k => k.Values.Where(p => p.Key.Name.Contains("bone"))).ToList();

            Assert.IsTrue(bones.All(p => Math.Abs(p.Value.RelativeScaleX - 1.5f) < .0001f));
            Assert.IsTrue(bones.All(p => Math.Abs(p.Value.RelativeScaleY - 2.0f) < Single.Epsilon));
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[sprite].RelativeScaleY - 1f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[sprite].RelativeScaleX - 1f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[sprite2].RelativeScaleX - 1f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[sprite2].RelativeScaleY - 1f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[sprite].RelativePosition.Y - -64f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[sprite].RelativePosition.X - 64f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[pivot].RelativePosition.Y - 0.0f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[pivot].RelativePosition.X - 0.0f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[pivot2].RelativePosition.Y - 0.0f) < .00001f);
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.First().Values[pivot2].RelativePosition.X - 0.0f) < .00001f);

            Assert.AreSame(sprite2.Parent, so.Animations.First().Value.KeyFrames.First().Values[sprite2].Parent);
        }

        [TestMethod]
        public void ThreeConnectedBonesAndObjectsPositionedTogether()
        {
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
            var so = sos.ToRuntime();

            var pivot1 = so.Animations.First().Value.KeyFrames.First().Values.First();
            var sprite1 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(1);
            var pivot2 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(2);
            var sprite2 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(3);
            var pivot3 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(4);
            var sprite3 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(5);
            var bone1 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(6);
            var bone2 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(7);
            var bone3 = so.Animations.First().Value.KeyFrames.First().Values.ElementAt(8);

            Assert.IsTrue(Math.Abs(bone1.Value.RelativePosition.X - 100f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone2.Value.RelativePosition.X - 200f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(bone3.Value.RelativePosition.X - 200f) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(pivot1.Value.RelativePosition.X - (-180f)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot2.Value.RelativePosition.X - (-380f)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot3.Value.RelativePosition.X - (-290f)) < Single.Epsilon);

            Assert.IsTrue(Math.Abs(bone3.Value.RelativeScaleX - 2f) < Single.Epsilon);
        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithMultipleAnimations()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id=0, Name="",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length=1000,
                                                    Id=0, Looping = false,
                                                    Name="0",
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys=new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=0,
                                                                            Spin=1,
                                                                            Time = 300
                                                                        }
                                                                }
                                                        },
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Object = new KeyObject(), Id=0, Spin=1,
                                                                                    Time=300
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                },
                                                new SpriterDataEntityAnimation
                                                {
                                                    Length=2000,
                                                    Id=1, Looping = false,
                                                    Name="1",
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys=new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0,
                                                                                        }
                                                                                },
                                                                            Id=0,
                                                                            Spin=1,
                                                                            Time = 100,
                                                                        }
                                                                }
                                                        },
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Object = new KeyObject(), Id=0, Spin=1,
                                                                                    Time=100
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };
        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWith2ObjectRefsAndExtraKey()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id=0, Name="",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length=1000,
                                                    Id=0, Looping = true,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys=new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Timeline=0,
                                                                                            Key=0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Parent = 0,
                                                                                            Timeline=1,
                                                                                            Key=0,
                                                                                            ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            Id=1,
                                                                            Time=207,
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Timeline = 0,
                                                                                            Key = 1
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Timeline = 1,
                                                                                            Key = 2,
                                                                                            ZIndex = 0
                                                                                        }
                                                                                }
                                                                        },
                                                                    new Key
                                                                    {
                                                                            Id=2,
                                                                            Time=489,
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Timeline = 0,
                                                                                            Key = 2
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id = 0,
                                                                                            Timeline = 1,
                                                                                            Key = 3,
                                                                                            ZIndex = 0
                                                                                        }
                                                                                }
                                                                        }
                                                                }
                                                        },
                                                    Name="First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone(),
                                                                                    Id=0,
                                                                                    Spin=0,
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 61f,
                                                                                            Y = -1f
                                                                                        },
                                                                                    Spin=0,
                                                                                    Time=207
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=2,
                                                                                    Time=489,
                                                                                    Spin=0,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = -19,
                                                                                            Y = -1
                                                                                        }
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 1,
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Id=0,
                                                                                    Spin=0,
                                                                                    Object = new KeyObject
                                                                                    {
                                                                                            Folder=0,
                                                                                            File = 0,
                                                                                            X=220f,
                                                                                            Y=67f
                                                                                        }
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Time=207,
                                                                                    Object = new KeyObject
                                                                                    {
                                                                                            Folder = 0,
                                                                                            File = 0,
                                                                                            X = 220f,
                                                                                            Y = 67f
                                                                                        }
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=2,
                                                                                    Time = 207,
                                                                                    Spin = 0,
                                                                                    Object = new KeyObject
                                                                                    {
                                                                                            Folder = 0,
                                                                                            File = 0,
                                                                                            X = 281,
                                                                                            Y = 66
                                                                                        }
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=3,
                                                                                    Time=489,
                                                                                    Spin=0,
                                                                                    Object = new KeyObject
                                                                                    {
                                                                                            Folder = 0,
                                                                                            File = 0,
                                                                                            X = 281,
                                                                                            Y = 66
                                                                                        }
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };
        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWith2ObjectRefs()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id=0, Name="",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length=2000,
                                                    Id=0, Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys=new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0
                                                                                        },
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=1,
                                                                                            Key=1,
                                                                                            Timeline=0,
                                                                                            ZIndex = 1
                                                                                        }
                                                                                },
                                                                            Id=0,
                                                                            Spin=1,
                                                                            Time=300
                                                                        }
                                                                }
                                                        },
                                                    Name="First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Object = new KeyObject(),
                                                                                    Id=0, 
                                                                                    Spin=1,
                                                                                    Time=300
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Object = new KeyObject(),
                                                                                    Spin=1,
                                                                                    Time=300
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };
        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithTwoBones()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id = 0,
                                    Name = "",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length = 1000,
                                                    Id = 0,
                                                    Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys = new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=1,
                                                                                            Key=0,
                                                                                            Timeline = 1,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            Id = 0,
                                                                            Spin = 1,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=1,
                                                                                            Key=1,
                                                                                            Timeline = 1,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            Id = 1,
                                                                            Spin = 1,
                                                                            Time = 1000
                                                                        }
                                                                }
                                                        },
                                                    Name = "First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 0,
                                                                    Name = "bone_000",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 100f  
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 500
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=1,
                                                                    Name="bone_001",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Id=0,
                                                                                    Spin=0,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            Y=100f
                                                                                        }
                                                                                },
                                                                            new Key
                                                                            {
                                                                                        Id=1,
                                                                                        Time=500,
                                                                                        Spin=0,
                                                                                        Bone=new KeyBone
                                                                                        {
                                                                                                Y=100f,
                                                                                                X=100f
                                                                                            }
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };

        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithBonesAsObjectParentAndOneOrphanObject()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id = 0,
                                    Name = "",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length = 1000,
                                                    Id = 0,
                                                    Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys = new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {

                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef
                                                                                        {
                                                                                            Id=1,
                                                                                            Key = 0,
                                                                                            Timeline = 2,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 1,
                                                                                            Parent = 1
                                                                                        },
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=2,
                                                                                            Key=0,
                                                                                            Timeline = 3
                                                                                        }
                                                                                },
                                                                            Id = 0,
                                                                            Spin = 1,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef
                                                                                        {
                                                                                            Id = 1,
                                                                                            Key = 1,
                                                                                            Timeline = 2,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 1,
                                                                                            Parent = 1
                                                                                        },
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=1,
                                                                                            Key=1,
                                                                                            Timeline = 3
                                                                                        }
                                                                                },
                                                                            Id = 1,
                                                                            Spin = 1,
                                                                            Time = 1000
                                                                        }
                                                                }
                                                        },
                                                    Name = "First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 0,
                                                                    Name = "bone1",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone
                                                                                        {
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 100f,
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 2,
                                                                    Name = "bone2",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone
                                                                                        {
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                        {
                                                                                            X = 100f,
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=1,
                                                                    Name="object",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Id=0,
                                                                                    Object = new KeyObject(),
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Object = new KeyObject(),
                                                                                        Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                                new SpriterDataEntityAnimationTimeline
                                                                {
                                                                    Id=3,
                                                                    Name="object",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Id=0,
                                                                                    Object = new KeyObject(),
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Object = new KeyObject(),
                                                                                        Time = 1000
                                                                                }
                                                                        }
                                                                }

                                                        }
                                                }
                                        }
                                }
                        }
            };

        }
        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithBonesAsObjectParent()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id = 0,
                                    Name = "",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length = 1000,
                                                    Id = 0,
                                                    Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys = new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {

                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef
                                                                                        {
                                                                                            Id=1,
                                                                                            Key = 0,
                                                                                            Timeline = 2,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 1,
                                                                                            Parent = 1
                                                                                        }
                                                                                },
                                                                            Id = 0,
                                                                            Spin = 1,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef
                                                                                        {
                                                                                            Id = 1,
                                                                                            Key = 1,
                                                                                            Timeline = 2,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 1,
                                                                                            Parent = 1
                                                                                        }
                                                                                },
                                                                            Id = 1,
                                                                            Spin = 1,
                                                                            Time = 1000
                                                                        }
                                                                }
                                                        },
                                                    Name = "First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 0,
                                                                    Name = "bone1",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone
                                                                                        {
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 100f,
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 2,
                                                                    Name = "bone2",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone
                                                                                        {
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                        {
                                                                                            X = 100f,
                                                                                            ScaleY = 2f,
                                                                                            ScaleX = 1.5f
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=1,
                                                                    Name="object",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Id=0,
                                                                                    Object = new KeyObject(),
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Object = new KeyObject(),
                                                                                        Time = 1000
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };

        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithSingleBoneAsObjectParent()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id = 0,
                                    Name = "",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length = 1000,
                                                    Id = 0,
                                                    Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys = new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 1,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            Id = 0,
                                                                            Spin = 1,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 1,
                                                                                            Parent = 0
                                                                                        }
                                                                                },
                                                                            Id = 1,
                                                                            Spin = 1,
                                                                            Time = 1000
                                                                        }
                                                                }
                                                        },
                                                    Name = "First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 0,
                                                                    Name = "bone",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 100f
                                                                                                
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=1,
                                                                    Name="object",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Id=0,
                                                                                    Object = new KeyObject(),
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id=1,
                                                                                    Object = new KeyObject(),
                                                                                        Time = 1000
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };
        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithSingleBoneRotated()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id = 0,
                                    Name = "",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length = 1000,
                                                    Id = 0,
                                                    Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys = new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            Id = 0,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                        {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            Id = 1,
                                                                            Time = 1000
                                                                        }
                                                                }
                                                        },
                                                    Name = "First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 0,
                                                                    Name = "",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 1,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 100f,
                                                                                            Angle = 45f
                                                                                        },
                                                                                    Spin = -1,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };

        }

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithSingleBone()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id = 0,
                                    Name = "",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length = 1000,
                                                    Id = 0,
                                                    Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys = new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            Id = 0,
                                                                            Spin = 1,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            BoneRef = new List<KeyBoneRef>
                                                                            {
                                                                                    new KeyBoneRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            Id = 1,
                                                                            Spin = 1,
                                                                            Time = 1000
                                                                        }
                                                                }
                                                        },
                                                    Name = "First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id = 0,
                                                                    Name = "",
                                                                    Key = new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone
                                                                                    {
                                                                                            X = 100f,
                                                                                            ScaleX = .25f
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };
        }

        public static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithPositionChange()
        {
            return new SpriterObjectSave
            {
                TextureLoader = Mock.Create<ITextureLoader>(),
                Directory = "C:\\",
                Folder = new List<SpriterDataFolder>(1)
                        {
                            new SpriterDataFolder
                                {
                                    Id = 0,
                                    Name = "folder",
                                    File = new List<SpriterDataFolderFile>
                                        {
                                            new SpriterDataFolderFile
                                                {
                                                    Height = 128,
                                                    Width = 128,
                                                    Id = 0,
                                                    Name = "folder/test.png"
                                                }
                                        }
                                }
                        },
                Entity = new List<SpriterDataEntity>
                        {
                            new SpriterDataEntity
                                {
                                    Id=0, Name="",
                                    Animation = new List<SpriterDataEntityAnimation>
                                        {
                                            new SpriterDataEntityAnimation
                                                {
                                                    Length=2000,
                                                    Id=0, Looping = false,
                                                    Mainline = new SpriterDataEntityAnimationMainline
                                                        {
                                                            Keys=new List<Key>
                                                            {
                                                                    new Key
                                                                    {
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id = 0,
                                                                                            Key=0,
                                                                                            Timeline = 0,
                                                                                            ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=0,
                                                                            Spin=1,
                                                                            Time = 0
                                                                        },
                                                                    new Key
                                                                    {
                                                                            ObjectRef = new List<KeyObjectRef>
                                                                            {
                                                                                    new KeyObjectRef
                                                                                    {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0,
                                                                                            ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=1,
                                                                            Time=500
                                                                        }
                                                                }
                                                        },
                                                    Name="First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>
                                                    {
                                                            new SpriterDataEntityAnimationTimeline
                                                            {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>
                                                                    {
                                                                            new Key
                                                                            {
                                                                                    Object = new KeyObject(),
                                                                                    Id=0, 
                                                                                    Spin=1,
                                                                                    Time=0
                                                                                },
                                                                            new Key
                                                                            {
                                                                                    Object = new KeyObject
                                                                                    {
                                                                                        X = 50f
                                                                                    },
                                                                                    Id=1,
                                                                                    Time=500
                                                                                }
                                                                        }
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
            };

        }

        [TestMethod]
        public void ObjectPosition()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithPositionChange();
            var so = sos.ToRuntime();
            so.StartAnimation();
            Assert.IsTrue(Math.Abs(so.NextKeyFrame.Values[so.ObjectList[1]].RelativePosition.X - 50.0f) < Single.Epsilon);
        }

        /// <summary>
        ///A test for GetSpriteRelativePosition
        ///</summary>
        [TestMethod]
        public void GetSpriteRelativePositionTest()
        {
            float width = 128F;
            float height = 128F;
            float pivotX = .75F;
            float pivotY = .25F;
            int zIndex = 1;
            Vector3 expected = new Vector3(-32f, 32f, .0001f); // TODO: Initialize to an appropriate value
            Vector3 actual;
            actual = SpriterObjectSave.GetPivotedRelativePosition(width, height, pivotX, pivotY, zIndex);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Default_Pivot()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <folder id=""0"">
        <file id=""0"" name=""square.png"" width=""32"" height=""32"" pivot_x=""0.5"" pivot_y=""0.5"" />
    </folder>
    <entity id=""0"" name=""entity_000"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""square"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""16"" y=""-16"" angle=""0"" />
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>";
            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);
            var so = sos.ToRuntime();

            var x = 1;
        }
    }
}
