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


            // (next - current) / (secondsin - current) / 100
            // secondsin - current / next
            /// currentKeyFrameTime / secondsIntoAnimation = expected
            actual = SpriterObject.GetPercentageIntoFrame(secondsIntoAnimation, currentKeyFrameTime, nextKeyFrameTime);
            Assert.AreEqual(expected, actual);
            
        }
    }
}
