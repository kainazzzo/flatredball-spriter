using System.Collections.Generic;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void TestAnimationTotalLength()
        {
            var sos = new SpriterObjectSaveNullTexture
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
                                                                                    Spin=1
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
            var so = sos.ToRuntime();
            Assert.IsNotNull(so);
            Assert.AreEqual(1, so.KeyFrameList.Count);
            Assert.AreEqual(2000, so.AnimationTotalTime);
        }

        public class SpriterObjectSaveNullTexture : SpriterObjectSave
        {
            public override Microsoft.Xna.Framework.Graphics.Texture2D LoadTexture(SpriterDataFolderFile file)
            {
                return null;
            }
        }
    }
}
