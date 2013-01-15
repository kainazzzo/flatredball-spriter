using System.Collections.Generic;
using System.Linq;
using FlatRedBall;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;

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

        public class SpriterObjectSaveNullTexture : SpriterObjectSave
        {
            public override Microsoft.Xna.Framework.Graphics.Texture2D LoadTexture(SpriterDataFolderFile file)
            {
                return null;
            }
        }

        [TestMethod]
        public void LoopingDefaultsToTrue()
        {

            var sos =
                SpriterObjectSave.FromFile(
                    @"C:\flatredballprojects\flatredball-spriter\spriterfiles\monsterexample\mytest.scml");
            Assert.IsTrue(sos.Entity[0].Animation[0].Looping);

        }

        [TestMethod]
        public void SpritesUseHalfPixelSize()
        {
            var sos = GetSimpleSpriterObjectSaveNullTexture();

            var so = sos.ToRuntime();

            Assert.IsTrue(so.ObjectList.OfType<Sprite>().All(s => Math.Abs(s.PixelSize - .5f) < .0001f));
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
            return new SpriterObjectSaveNullTexture
                {
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
                                                                            new Key()
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
            CollectionAssert.AllItemsAreInstancesOfType(so.ObjectList, typeof(PositionedObject));
            Assert.IsNotNull(so.Animations.First().Value.KeyFrames[0].Values.FirstOrDefault());
            var bone = so.ObjectList[0];
            Assert.IsTrue(Math.Abs(so.Animations.First().Value.KeyFrames.ElementAt(1).Values[bone].Position.X - 100f) < .0001f);
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

            var pivot = so.ObjectList.Single(o => o.Name == "pivot");
            var sprite = so.ObjectList.Single(o => o.Name == "sprite");
            var bone1 = so.ObjectList.Single(o => o.Name == "bone0");
            var bone2 = so.ObjectList.Single(o => o.Name == "bone1");

            Assert.AreSame(bone1, so.Animations.First().Value.KeyFrames.First().Values[bone2].Parent);
        }

        [TestMethod]
        public void ObjectAttachedToPivotIfNoParentSet()
        {
            var sos = GetSimpleSpriterObjectSaveNullTexture();
            var so = sos.ToRuntime();
            var pivot = so.ObjectList.Single(p => p.Name == "pivot");
            var sprite = (Sprite)so.ObjectList.Single(p => p.Name == "sprite");

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

            var pivot = so.ObjectList.Single(p => p.Name == "pivot");
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

            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values.ElementAt(0).Value.Position.Z);
            Assert.AreNotEqual(0.0f, so.KeyFrameList[0].Values.ElementAt(3).Value.Position.Z);
            Assert.IsTrue(Math.Abs(so.KeyFrameList[0].Values.ElementAt(3).Value.Position.Z - .0001f) < .00001);
        }

        
        public void UnreferencedKeysGetAddedStill()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWith2ObjectRefsAndExtraKey();
        }

        [TestMethod]
        public void BoneRotation()
        {
            var sos = GetSimpleSpriterObjectSaveNullTextureWithSingleBoneRotated();
            var so = sos.ToRuntime();

            Assert.IsTrue(
                Math.Abs(0f - so.Animations.First().Value.KeyFrames.ElementAt(0).Values.First().Value.Rotation.Z) < .0001f);

            Assert.IsTrue(
    Math.Abs(45f -
             so.Animations.First().Value.KeyFrames.ElementAt(1).Values.First().Value.Rotation.Z) < .0001f);

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

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithMultipleAnimations()
        {
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys=new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>()
                                                                        {
                                                                            new Key()
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
                                                            Keys=new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>()
                                                                        {
                                                                            new Key()
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
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys=new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
                                                                                        {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0
                                                                                        },
                                                                                    new KeyObjectRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Object = new KeyObject(),
                                                                                    Id=0, 
                                                                                    Spin=1,
                                                                                    Time=300
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id=1,
                                                                                    Object = new KeyObject(),
                                                                                    Spin=1,
                                                                                    Time=300
                                                                                },
                                                                                new Key()
                                                                                    {
                                                                                        Id=2,
                                                                                        Object = new KeyObject()
                                                                                            {
                                                                                                File = 0,
                                                                                                Folder = 0,
                                                                                                Angle = 0f,
                                                                                                X = 1,
                                                                                                Y = 1
                                                                                            },
                                                                                            
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
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys=new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
                                                                                        {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0
                                                                                        },
                                                                                    new KeyObjectRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Object = new KeyObject(),
                                                                                    Id=0, 
                                                                                    Spin=1,
                                                                                    Time=300
                                                                                },
                                                                            new Key()
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
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys = new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
                                                                                        {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef()
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
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
                                                                                        {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        },
                                                                                    new KeyBoneRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id = 0,
                                                                    Name = "bone_000",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            X = 100f  
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 500
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=1,
                                                                    Name="bone_001",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Id=0,
                                                                                    Spin=0,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            Y=100f
                                                                                        }
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                        Id=1,
                                                                                        Time=500,
                                                                                        Spin=0,
                                                                                        Bone=new KeyBone()
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

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithBonesAsObjectParent()
        {
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys = new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {

                                                                                    new KeyBoneRef()
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
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
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
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
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
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id = 0,
                                                                    Name = "bone1",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            X = 100f
                                                                                                
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id = 2,
                                                                    Name = "bone2",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            X = 100f
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=1,
                                                                    Name="object",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Id=0,
                                                                                    Object = new KeyObject(),
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
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
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys = new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
                                                                                        {
                                                                                            Id=0,
                                                                                            Key=0,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
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
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
                                                                                        {
                                                                                            Id=0,
                                                                                            Key=1,
                                                                                            Timeline = 0
                                                                                        }
                                                                                },
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id = 0,
                                                                    Name = "bone",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            X = 100f
                                                                                                
                                                                                        },
                                                                                    Spin = 0,
                                                                                    Time = 1000
                                                                                }
                                                                        }
                                                                },
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=1,
                                                                    Name="object",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Id=0,
                                                                                    Object = new KeyObject(),
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
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
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys = new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
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
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id = 0,
                                                                    Name = "",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            X = 100f,
                                                                                            Angle = 45f
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

        private static SpriterObjectSave GetSimpleSpriterObjectSaveNullTextureWithSingleBone()
        {
            return new SpriterObjectSaveNullTexture
                {
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
                                                            Keys = new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
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
                                                                    new Key()
                                                                        {
                                                                            BoneRef = new List<KeyBoneRef>()
                                                                                {
                                                                                    new KeyBoneRef()
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
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id = 0,
                                                                    Name = "",
                                                                    Key = new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Bone = new KeyBone(),
                                                                                    Id = 0,
                                                                                    Spin = 0,
                                                                                    Time = 0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Id = 1,
                                                                                    Bone = new KeyBone()
                                                                                        {
                                                                                            X = 100f
                                                                                                
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
            return new SpriterObjectSaveNullTexture
            {
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
                                                            Keys=new List<Key>()
                                                                {
                                                                    new Key()
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
                                                                                        {
                                                                                            Id = 0, Key=0,
                                                                                            Timeline = 0, ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=0,
                                                                            Spin=1,
                                                                            Time = 0
                                                                        },
                                                                    new Key()
                                                                        {
                                                                            ObjectRef = new List<KeyObjectRef>()
                                                                                {
                                                                                    new KeyObjectRef()
                                                                                        {
                                                                                            Id=0, Key=1,
                                                                                            Timeline = 0,ZIndex = 0
                                                                                        }
                                                                                },
                                                                            Id=1,
                                                                            Time=500
                                                                        }
                                                                }
                                                        },
                                                    Name="First Animation",
                                                    Timeline = new List<SpriterDataEntityAnimationTimeline>()
                                                        {
                                                            new SpriterDataEntityAnimationTimeline()
                                                                {
                                                                    Id=0, 
                                                                    Name="",
                                                                    Key=new List<Key>()
                                                                        {
                                                                            new Key()
                                                                                {
                                                                                    Object = new KeyObject(), Id=0, Spin=1,
                                                                                    Time=0
                                                                                },
                                                                            new Key()
                                                                                {
                                                                                    Object = new KeyObject()
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
            Assert.IsTrue(Math.Abs(so.NextKeyFrame.Values[so.ObjectList[1]].Position.X - 50.0f) < .0001f);
        }
        [TestMethod]
        public void FromFileTest()
        {
            var sos = SpriterObjectSave.FromFile(@"C:\flatredballprojects\flatredball-spriter\spriterfiles\monsterexample\mytest.scml");

            #region spriter_data
            Assert.AreEqual("1.0", sos.ScmlVersion);
            Assert.AreEqual("BrashMonkey Spriter", sos.Generator);
            Assert.AreEqual("a4.1", sos.GeneratorVersion);

            Assert.AreEqual(2, sos.Folder.Count);
            #endregion

            #region folder[0]
            Assert.AreEqual(0, sos.Folder[0].Id);
            Assert.AreEqual("mon_arms", sos.Folder[0].Name);
            Assert.AreEqual(2, sos.Folder[0].File.Count);
            #region folder[0].file[0]

            var file = sos.Folder[0].File[0];
            Assert.AreEqual(0, file.Id);
            Assert.AreEqual("mon_arms/shoulder_a.png", file.Name);
            Assert.AreEqual(71, file.Width);
            Assert.AreEqual(115, file.Height);
            #endregion

            #region folder[0].file[1]
            file = sos.Folder[0].File[1];
            Assert.AreEqual(1, file.Id);
            Assert.AreEqual("mon_arms/forearm_a.png", file.Name);
            Assert.AreEqual(99, file.Width);
            Assert.AreEqual(78, file.Height);
            #endregion
            #endregion
            #region folder[1]
            Assert.AreEqual(1, sos.Folder[1].Id);
            Assert.AreEqual("mon_torso", sos.Folder[1].Name);
            Assert.AreEqual(1, sos.Folder[1].File.Count);
            #region folder[1].file[0]

            file = sos.Folder[1].File[0];
            Assert.AreEqual(0, file.Id);
            Assert.AreEqual("mon_torso/torso_0.png", file.Name);
            Assert.AreEqual(180, file.Width);
            Assert.AreEqual(197, file.Height);

            #endregion

            #endregion
            #region entity[0]
            var entity = sos.Entity[0];
            Assert.AreEqual(0, entity.Id);
            Assert.AreEqual("", entity.Name);
            Assert.AreEqual(1, entity.Animation.Count);
            #region entity[0].animation[0]
            var animation = sos.Entity[0].Animation[0];
            Assert.AreEqual(0, animation.Id);
            Assert.AreEqual("First Animation", animation.Name);
            Assert.AreEqual(1000, animation.Length);
            Assert.AreEqual(true, animation.Looping);
            Assert.AreEqual(9, animation.Timeline.Count);
            #region entity[0].animation[0].mainline
            Assert.AreEqual(3, animation.Mainline.Keys.Count);
            #region entity[0].animation[0].mainline.key[0]
            var key = animation.Mainline.Keys[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(0, key.Time);
            Assert.AreEqual(6, key.BoneRef.Count);
            Assert.AreEqual(2, key.ObjectRef.Count);
            #region entity[0].animation[0].mainline.key[0].bone_ref[0]
            // ReSharper disable InconsistentNaming
            var bone_ref = key.BoneRef[0];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(0, bone_ref.Id);
            Assert.AreEqual(0, bone_ref.Timeline);
            Assert.AreEqual(0, bone_ref.Key);
            Assert.IsTrue(!bone_ref.Parent.HasValue);
            #endregion
            #region entity[0].animation[0].mainline.key[0].bone_ref[1]
            bone_ref = entity.Animation[0].Mainline.Keys[0].BoneRef[1];
            Assert.AreEqual(1, bone_ref.Id);
            Assert.AreEqual(4, bone_ref.Timeline);
            Assert.AreEqual(0, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);
            #endregion
            #region entity[0].animation[0].mainline.key[0].bone_ref[2]

            bone_ref = entity.Animation[0].Mainline.Keys[0].BoneRef[2];
            Assert.AreEqual(2, bone_ref.Id);
            Assert.AreEqual(3, bone_ref.Timeline);
            Assert.AreEqual(0, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[0].bone_ref[3]

            bone_ref = entity.Animation[0].Mainline.Keys[0].BoneRef[3];
            Assert.AreEqual(3, bone_ref.Id);
            Assert.AreEqual(2, bone_ref.Timeline);
            Assert.AreEqual(0, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[0].bone_ref[4]

            bone_ref = entity.Animation[0].Mainline.Keys[0].BoneRef[4];
            Assert.AreEqual(4, bone_ref.Id);
            Assert.AreEqual(1, bone_ref.Timeline);
            Assert.AreEqual(0, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[0].bone_ref[5]

            bone_ref = entity.Animation[0].Mainline.Keys[0].BoneRef[5];
            Assert.AreEqual(5, bone_ref.Id);
            Assert.AreEqual(6, bone_ref.Timeline);
            Assert.AreEqual(0, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[0].object_ref[0]

            // ReSharper disable InconsistentNaming
            var object_ref = key.ObjectRef[0];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(0, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(5, object_ref.Parent.Value);
            Assert.AreEqual(7, object_ref.Timeline);
            Assert.AreEqual(0, object_ref.Key);
            Assert.AreEqual(0, object_ref.ZIndex);

            #endregion
            #region entity[0].animation[0].mainline.key[0].object_ref[1]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[1];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(1, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(0, object_ref.Parent.Value);
            Assert.AreEqual(5, object_ref.Timeline);
            Assert.AreEqual(0, object_ref.Key);
            Assert.AreEqual(1, object_ref.ZIndex);

            #endregion
            #endregion
            #region entity[0].animation[0].mainline.key[1]
            key = animation.Mainline.Keys[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Time);
            Assert.AreEqual(6, key.BoneRef.Count);
            Assert.AreEqual(3, key.ObjectRef.Count);
            #region entity[0].animation[0].mainline.key[1].bone_ref[0]
            // ReSharper disable InconsistentNaming
            bone_ref = key.BoneRef[0];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(0, bone_ref.Id);
            Assert.AreEqual(0, bone_ref.Timeline);
            Assert.AreEqual(1, bone_ref.Key);
            Assert.IsTrue(!bone_ref.Parent.HasValue);
            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[1]
            bone_ref = key.BoneRef[1];
            Assert.AreEqual(1, bone_ref.Id);
            Assert.AreEqual(4, bone_ref.Timeline);
            Assert.AreEqual(1, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);
            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[2]

            bone_ref = key.BoneRef[2];
            Assert.AreEqual(2, bone_ref.Id);
            Assert.AreEqual(2, bone_ref.Timeline);
            Assert.AreEqual(1, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[3]

            bone_ref = key.BoneRef[3];
            Assert.AreEqual(3, bone_ref.Id);
            Assert.AreEqual(1, bone_ref.Timeline);
            Assert.AreEqual(1, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[4]

            bone_ref = key.BoneRef[4];
            Assert.AreEqual(4, bone_ref.Id);
            Assert.AreEqual(6, bone_ref.Timeline);
            Assert.AreEqual(1, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[5]

            bone_ref = key.BoneRef[5];
            Assert.AreEqual(5, bone_ref.Id);
            Assert.AreEqual(3, bone_ref.Timeline);
            Assert.AreEqual(2, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(4, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].object_ref[0]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[0];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(0, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(4, object_ref.Parent.Value);
            Assert.AreEqual(7, object_ref.Timeline);
            Assert.AreEqual(1, object_ref.Key);
            Assert.AreEqual(0, object_ref.ZIndex);

            #endregion
            #region entity[0].animation[0].mainline.key[1].object_ref[1]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[1];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(1, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(0, object_ref.Parent.Value);
            Assert.AreEqual(5, object_ref.Timeline);
            Assert.AreEqual(1, object_ref.Key);
            Assert.AreEqual(1, object_ref.ZIndex);

            #endregion
            #region entity[0].animation[0].mainline.key[1].object_ref[2]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[2];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(2, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(5, object_ref.Parent.Value);
            Assert.AreEqual(8, object_ref.Timeline);
            Assert.AreEqual(0, object_ref.Key);
            Assert.AreEqual(2, object_ref.ZIndex);

            #endregion
            #endregion
            #region entity[0].animation[0].mainline.key[2]
            key = animation.Mainline.Keys[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(687, key.Time);
            Assert.AreEqual(6, key.BoneRef.Count);
            Assert.AreEqual(3, key.ObjectRef.Count);
            #region entity[0].animation[0].mainline.key[2].bone_ref[0]
            // ReSharper disable InconsistentNaming
            bone_ref = key.BoneRef[0];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(0, bone_ref.Id);
            Assert.AreEqual(0, bone_ref.Timeline);
            Assert.AreEqual(2, bone_ref.Key);
            Assert.IsTrue(!bone_ref.Parent.HasValue);
            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[1]
            bone_ref = key.BoneRef[1];
            Assert.AreEqual(1, bone_ref.Id);
            Assert.AreEqual(4, bone_ref.Timeline);
            Assert.AreEqual(2, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);
            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[2]

            bone_ref = key.BoneRef[2];
            Assert.AreEqual(2, bone_ref.Id);
            Assert.AreEqual(2, bone_ref.Timeline);
            Assert.AreEqual(2, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[3]

            bone_ref = key.BoneRef[3];
            Assert.AreEqual(3, bone_ref.Id);
            Assert.AreEqual(1, bone_ref.Timeline);
            Assert.AreEqual(2, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[4]

            bone_ref = key.BoneRef[4];
            Assert.AreEqual(4, bone_ref.Id);
            Assert.AreEqual(6, bone_ref.Timeline);
            Assert.AreEqual(2, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(0, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].bone_ref[5]

            bone_ref = key.BoneRef[5];
            Assert.AreEqual(5, bone_ref.Id);
            Assert.AreEqual(3, bone_ref.Timeline);
            Assert.AreEqual(3, bone_ref.Key);
            Assert.IsTrue(bone_ref.Parent.HasValue);
            Assert.AreEqual(4, bone_ref.Parent.Value);

            #endregion
            #region entity[0].animation[0].mainline.key[1].object_ref[0]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[0];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(0, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(4, object_ref.Parent.Value);
            Assert.AreEqual(7, object_ref.Timeline);
            Assert.AreEqual(2, object_ref.Key);
            Assert.AreEqual(0, object_ref.ZIndex);

            #endregion
            #region entity[0].animation[0].mainline.key[1].object_ref[1]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[1];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(1, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(0, object_ref.Parent.Value);
            Assert.AreEqual(5, object_ref.Timeline);
            Assert.AreEqual(2, object_ref.Key);
            Assert.AreEqual(1, object_ref.ZIndex);

            #endregion
            #region entity[0].animation[0].mainline.key[1].object_ref[2]

            // ReSharper disable InconsistentNaming
            object_ref = key.ObjectRef[2];
            // ReSharper restore InconsistentNaming
            Assert.AreEqual(2, object_ref.Id);
            Assert.IsTrue(object_ref.Parent.HasValue);
            Assert.AreEqual(5, object_ref.Parent.Value);
            Assert.AreEqual(8, object_ref.Timeline);
            Assert.AreEqual(1, object_ref.Key);
            Assert.AreEqual(2, object_ref.ZIndex);

            #endregion
            #endregion


            #endregion
            #region entity[0].animation[0].timeline[0]

            var timeline = animation.Timeline[0];
            Assert.AreEqual(0, timeline.Id);
            Assert.AreEqual("bone_000", timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[0].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[0].key[0].bone[0]

            var bone = key.Bone;
            Assert.AreEqual(6.0f, bone.X);
            Assert.AreEqual(130.0f, bone.Y);
            Assert.AreEqual(254.53166f, bone.Angle);
            Assert.AreEqual(0.920014f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[0].key[1]

            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[0].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(6.0f, bone.X);
            Assert.AreEqual(130.0f, bone.Y);
            Assert.AreEqual(254.53166f, bone.Angle);
            Assert.AreEqual(0.920014f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[0].key[1]

            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[0].key[2].bone[0]

            bone = key.Bone;
            Assert.AreEqual(6.0f, bone.X);
            Assert.AreEqual(130.0f, bone.Y);
            Assert.AreEqual(254.53166f, bone.Angle);
            Assert.AreEqual(0.920014f, bone.ScaleX);

            #endregion

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[1]

            timeline = animation.Timeline[1];
            Assert.AreEqual(1, timeline.Id);
            Assert.AreEqual("bone_001", timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[1].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[1].key[0].bone[0]

            bone = key.Bone;
            Assert.AreEqual(229.033836f, bone.X);
            Assert.AreEqual(9.058467f, bone.Y);
            Assert.AreEqual(315.764376f, bone.Angle);
            Assert.AreEqual(0.61112f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[1].key[1]

            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[1].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(229.033836f, bone.X);
            Assert.AreEqual(9.058467f, bone.Y);
            Assert.AreEqual(315.764376f, bone.Angle);
            Assert.AreEqual(0.61112f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[0].key[1]

            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[1].key[2].bone[0]

            bone = key.Bone;
            Assert.AreEqual(229.033836f, bone.X);
            Assert.AreEqual(9.058467f, bone.Y);
            Assert.AreEqual(315.764376f, bone.Angle);
            Assert.AreEqual(0.61112f, bone.ScaleX);

            #endregion

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[2]

            timeline = animation.Timeline[2];
            Assert.AreEqual(2, timeline.Id);
            Assert.AreEqual("bone_002", timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[2].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[2].key[0].bone[0]

            bone = key.Bone;
            Assert.AreEqual(198.046441f, bone.X);
            Assert.AreEqual(52.481557f, bone.Y);
            Assert.AreEqual(50.18893f, bone.Angle);
            Assert.AreEqual(0.640867f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[2].key[1]

            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[2].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(198.046441f, bone.X);
            Assert.AreEqual(52.481557f, bone.Y);
            Assert.AreEqual(50.18893f, bone.Angle);
            Assert.AreEqual(0.640867f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[2].key[2]

            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[2].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(198.046441f, bone.X);
            Assert.AreEqual(52.481557f, bone.Y);
            Assert.AreEqual(50.18893f, bone.Angle);
            Assert.AreEqual(0.640867f, bone.ScaleX);

            #endregion

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[3]

            timeline = animation.Timeline[3];
            Assert.AreEqual(3, timeline.Id);
            Assert.AreEqual("bone_003", timeline.Name);
            Assert.AreEqual(4, timeline.Key.Count);
            #region entity[0].animation[0].timeline[3].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(-1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[3].key[0].bone[0]

            bone = key.Bone;
            Assert.AreEqual(141.520962f, bone.X);
            Assert.AreEqual(111.118524f, bone.Y);
            Assert.AreEqual(71.248503f, bone.Angle);
            Assert.AreEqual(0.842114f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[3].key[1]

            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[3].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(141.520962f, bone.X);
            Assert.AreEqual(111.118524f, bone.Y);
            Assert.AreEqual(108.632633f, bone.Angle);
            Assert.AreEqual(0.407474f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[3].key[2]

            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(-1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[3].key[2].bone[0]

            bone = key.Bone;
            Assert.AreEqual(280.733104f, bone.X);
            Assert.AreEqual(-6.194428f, bone.Y);
            Assert.AreEqual(53.874886f, bone.Angle);
            Assert.AreEqual(1.055106f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[3].key[3]

            key = timeline.Key[3];
            Assert.AreEqual(3, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[3].key[3].bone[0]

            bone = key.Bone;
            Assert.AreEqual(280.733104f, bone.X);
            Assert.AreEqual(-6.194428f, bone.Y);
            Assert.AreEqual(15.762352f, bone.Angle);
            Assert.AreEqual(1.055106f, bone.ScaleX);

            #endregion

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[4]

            timeline = animation.Timeline[4];
            Assert.AreEqual(4, timeline.Id);
            Assert.AreEqual("bone_004", timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[4].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[4].key[0].bone[0]

            bone = key.Bone;
            Assert.AreEqual(89.716259f, bone.X);
            Assert.AreEqual(-83.45729f, bone.Y);
            Assert.AreEqual(306.847927f, bone.Angle);
            Assert.AreEqual(0.442374f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[4].key[1]

            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[1].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(89.716259f, bone.X);
            Assert.AreEqual(-83.45729f, bone.Y);
            Assert.AreEqual(306.847927f, bone.Angle);
            Assert.AreEqual(0.442374f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[4].key[2]

            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[4].key[2].bone[0]

            bone = key.Bone;
            Assert.AreEqual(89.716259f, bone.X);
            Assert.AreEqual(-83.45729f, bone.Y);
            Assert.AreEqual(306.847927f, bone.Angle);
            Assert.AreEqual(0.442374f, bone.ScaleX);

            #endregion

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[5]

            timeline = animation.Timeline[5];
            Assert.AreEqual(5, timeline.Id);
            Assert.IsNull(timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[5].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[5].key[0].object[0]

            var @object = key.Object;
            Assert.AreEqual(40.784198f, @object.X);
            Assert.AreEqual(-135.952228f, @object.Y);
            Assert.AreEqual(112.219344f, @object.Angle);
            Assert.AreEqual(1.08694f, @object.ScaleX);
            Assert.AreEqual(1, @object.Folder);
            Assert.AreEqual(0, @object.File);
            Assert.AreEqual(0.0f, @object.PivotX);
            Assert.AreEqual(1.0f, @object.PivotY);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[5].key[1]
            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[5].key[0].object[0]

            @object = key.Object;
            Assert.AreEqual(40.784198f, @object.X);
            Assert.AreEqual(-135.952228f, @object.Y);
            Assert.AreEqual(112.219344f, @object.Angle);
            Assert.AreEqual(1.08694f, @object.ScaleX);
            Assert.AreEqual(1, @object.Folder);
            Assert.AreEqual(0, @object.File);

            #endregion
            #endregion
            #region entity[0].animation[0].timeline[5].key[2]
            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[5].key[0].object[0]

            @object = key.Object;
            Assert.AreEqual(40.784198f, @object.X);
            Assert.AreEqual(-135.952228f, @object.Y);
            Assert.AreEqual(112.219344f, @object.Angle);
            Assert.AreEqual(1.08694f, @object.ScaleX);
            Assert.AreEqual(1, @object.Folder);
            Assert.AreEqual(0, @object.File);

            #endregion
            #endregion

            #endregion
            #region entity[0].animation[0].timeline[6]

            timeline = animation.Timeline[6];
            Assert.AreEqual(6, timeline.Id);
            Assert.AreEqual("bone_005", timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[6].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[6].key[0].bone[0]

            bone = key.Bone;
            Assert.AreEqual(73.46162f, bone.X);
            Assert.AreEqual(33.229125f, bone.Y);
            Assert.AreEqual(54.757747f, bone.Angle);
            Assert.AreEqual(0.386192f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[6].key[1]

            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[6].key[1].bone[0]

            bone = key.Bone;
            Assert.AreEqual(73.46162f, bone.X);
            Assert.AreEqual(33.229125f, bone.Y);
            Assert.AreEqual(54.757747f, bone.Angle);
            Assert.AreEqual(0.386192f, bone.ScaleX);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[6].key[2]

            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[6].key[2].bone[0]

            bone = key.Bone;
            Assert.AreEqual(73.46162f, bone.X);
            Assert.AreEqual(33.229125f, bone.Y);
            Assert.AreEqual(54.757747f, bone.Angle);
            Assert.AreEqual(0.386192f, bone.ScaleX);
            #endregion
            #endregion
            #endregion
            #region entity[0].animation[0].timeline[7]

            timeline = animation.Timeline[7];
            Assert.AreEqual(7, timeline.Id);
            Assert.IsNull(timeline.Name);
            Assert.AreEqual(3, timeline.Key.Count);
            #region entity[0].animation[0].timeline[7].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(0, key.Time);
            #region entity[0].animation[0].timeline[7].key[0].object[0]

            @object = key.Object;
            Assert.AreEqual(-57.230045f, @object.X);
            Assert.AreEqual(-16.956701f, @object.Y);
            Assert.AreEqual(68.506582f, @object.Angle);
            Assert.AreEqual(2.814504f, @object.ScaleX);
            Assert.AreEqual(0, @object.Folder);
            Assert.AreEqual(0, @object.File);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[7].key[1]
            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[7].key[0].object[0]

            @object = key.Object;
            Assert.AreEqual(-57.230045f, @object.X);
            Assert.AreEqual(-16.956701f, @object.Y);
            Assert.AreEqual(68.506582f, @object.Angle);
            Assert.AreEqual(2.814504f, @object.ScaleX);
            Assert.AreEqual(0, @object.Folder);
            Assert.AreEqual(0, @object.File);

            #endregion
            #endregion
            #region entity[0].animation[0].timeline[7].key[2]
            key = timeline.Key[2];
            Assert.AreEqual(2, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[5].key[0].object[0]

            @object = key.Object;
            Assert.AreEqual(-57.230045f, @object.X);
            Assert.AreEqual(-16.956701f, @object.Y);
            Assert.AreEqual(68.506582f, @object.Angle);
            Assert.AreEqual(2.814504f, @object.ScaleX);
            Assert.AreEqual(0, @object.Folder);
            Assert.AreEqual(0, @object.File);

            #endregion
            #endregion

            #endregion
            #region entity[0].animation[0].timeline[8]

            timeline = animation.Timeline[8];
            Assert.AreEqual(8, timeline.Id);
            Assert.IsNull(timeline.Name);
            Assert.AreEqual(2, timeline.Key.Count);
            #region entity[0].animation[0].timeline[8].key[0]

            key = timeline.Key[0];
            Assert.AreEqual(0, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(1, key.Time);
            #region entity[0].animation[0].timeline[8].key[0].object[0]

            @object = key.Object;
            Assert.AreEqual(-94.666417f, @object.X);
            Assert.AreEqual(40.900994f, @object.Y);
            Assert.AreEqual(358.756205f, @object.Angle);
            Assert.AreEqual(2.667509f, @object.ScaleX);
            Assert.AreEqual(0, @object.Folder);
            Assert.AreEqual(1, @object.File);

            #endregion

            #endregion
            #region entity[0].animation[0].timeline[8].key[1]
            key = timeline.Key[1];
            Assert.AreEqual(1, key.Id);
            Assert.AreEqual(1, key.Spin);
            Assert.AreEqual(687, key.Time);
            #region entity[0].animation[0].timeline[8].key[1].object[0]

            @object = key.Object;
            Assert.AreEqual(-94.666417f, @object.X);
            Assert.AreEqual(40.900994f, @object.Y);
            Assert.AreEqual(358.756205f, @object.Angle);
            Assert.AreEqual(2.667509f, @object.ScaleX);
            Assert.AreEqual(0, @object.Folder);
            Assert.AreEqual(1, @object.File);

            #endregion
            #endregion
            #endregion

            #endregion

            #endregion


        }

        /// <summary>
        ///A test for GetSpriteRelativePosition
        ///</summary>
        [TestMethod()]
        public void GetSpriteRelativePositionTest()
        {
            float width = 128F;
            float height = 128F;
            float pivotX = .75F;
            float pivotY = .25F;
            int zIndex = 1;
            Vector3 expected = new Vector3(-32f, 32f, .0001f); // TODO: Initialize to an appropriate value
            Vector3 actual;
            actual = SpriterObjectSave.GetSpriteRelativePosition(width, height, pivotX, pivotY, zIndex);
            Assert.AreEqual(expected, actual);
        }
    }
}
