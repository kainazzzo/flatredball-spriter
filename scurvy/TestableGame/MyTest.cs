using System;
using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall_Spriter;
using Microsoft.Xna.Framework.Content;
using Scurvy.Test;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestableGame
{
    [TestClass]
    public class MyTest
    {
        private ContentManager content;

        public MyTest()
        {
        }

        [TestSetup]
        public void Setup(TestContext context)
        {
            this.content = new ContentManager(context.Services, "Content");
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.content.Unload();
            this.content.Dispose();
        }

        ///// <summary>
        ///// This test makes sure that the scurvy_logo_big content loads successfully.
        ///// </summary>
        //[TestMethod]
        //public void ContentLoadTest()
        //{
        //    Texture2D texture = this.content.Load<Texture2D>("scurvy_logo_bigX");
        //    Assert.IsTrue(texture != null);
        //}
        /// <summary>
        ///A test for ToRuntime
        ///</summary>
        [TestMethod]
        public void ToRuntimeTest()
        {
            var sos =
                SpriterObjectSave.FromFile(
                    @"c:\flatredballprojects\flatredball-spriter\spriterfiles\simpleballanimation\simpleballanimation.scml");

            var so = sos.ToRuntime();

            Assert.AreEqual(1, so.ObjectList.Count, "1 Object");
            Assert.AreEqual(5, so.KeyFrameList.Count, "5 Keys");

            Assert.AreEqual(0.0, so.KeyFrameList[0].Time, "KeyFrame 0 Time = 0ms");
            Assert.AreEqual(0.2, so.KeyFrameList[1].Time, "KeyFrame 1 Time = 200ms");
            Assert.AreEqual(0.4, so.KeyFrameList[2].Time, "KeyFrame 2 Time = 400ms");
            Assert.AreEqual(0.6, so.KeyFrameList[3].Time, "KeyFrame 3 Time = 600ms");
            Assert.AreEqual(0.8, so.KeyFrameList[4].Time, "KeyFrame 4 Time = 800ms");

            PositionedObject positionedObject = so.ObjectList[0];

            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[positionedObject].Position.X, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[positionedObject].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[positionedObject].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[positionedObject].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[positionedObject].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[positionedObject].Rotation.Z, "Rotation Test");
            Assert.AreEqual(1.0f, so.KeyFrameList[0].Values[positionedObject].ScaleX, "Scale Test");
            //Assert.AreEqual(1.0f, so.KeyFrameList[0].Values[positionedObject].ScaleY, "Scale Test");
            //Assert.AreEqual("ball.png", so.KeyFrameList[0].Values[positionedObject].Texture.Name, "Texture test");

            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[positionedObject].Position.X, "Position Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[1].Values[positionedObject].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[positionedObject].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[positionedObject].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[positionedObject].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[positionedObject].Rotation.Z, "Rotation Test");
            Assert.AreEqual(1.0f, so.KeyFrameList[1].Values[positionedObject].ScaleX, "Scale Test");
            //Assert.AreEqual(1.0f, so.KeyFrameList[1].Values[positionedObject].ScaleY, "Scale Test");

            Assert.AreEqual(-128.0f, so.KeyFrameList[2].Values[positionedObject].Position.X, "Position Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[2].Values[positionedObject].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[positionedObject].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[positionedObject].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[positionedObject].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[positionedObject].Rotation.Z, "Rotation Test");
            Assert.AreEqual(1.0f, so.KeyFrameList[2].Values[positionedObject].ScaleX, "Scale Test");
            //Assert.AreEqual(1.0f, so.KeyFrameList[2].Values[positionedObject].ScaleY, "Scale Test");

            Assert.AreEqual(-128.0f, so.KeyFrameList[3].Values[positionedObject].Position.X, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[positionedObject].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[positionedObject].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[positionedObject].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[positionedObject].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[positionedObject].Rotation.Z, "Rotation Test");
            Assert.AreEqual(1.0f, so.KeyFrameList[3].Values[positionedObject].ScaleX, "Scale Test");
            //Assert.AreEqual(1.0f, so.KeyFrameList[3].Values[positionedObject].ScaleY, "Scale Test");

            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[positionedObject].Position.X, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[positionedObject].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[positionedObject].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[positionedObject].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[positionedObject].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[positionedObject].Rotation.Z, "Rotation Test");
            Assert.AreEqual(1.0f, so.KeyFrameList[4].Values[positionedObject].ScaleX, "Scale Test");
            //Assert.AreEqual(1.0f, so.KeyFrameList[4].Values[positionedObject].ScaleY, "Scale Test");
        }

        /// <summary>
        /// This test renders a texture and waits for manual approval that the test is finished.
        /// </summary>
        [TestMethod]
        public void ManualContentVerificationTest(TestContext context)
        {
            context.ExitCriteria = new ContentVerificationExitCriteria(this.content, context);
        }

        private class ContentVerificationExitCriteria : ExitCriteria
        {
            private SpriteBatch batch;
            private Texture2D texture;
            private SpriteFont font;
            private float xPosition;

            public ContentVerificationExitCriteria(ContentManager content, TestContext context)
                : base(context)
            {
                IGraphicsDeviceService graphics = (IGraphicsDeviceService)this.Context.Services.GetService(typeof(IGraphicsDeviceService));
                this.batch = new SpriteBatch(graphics.GraphicsDevice);
                this.texture = content.Load<Texture2D>("scurvy_logo_big");
                this.font = content.Load<SpriteFont>("font");
            }

            public override void Update(TimeSpan elapsedTime)
            {
                KeyboardState keyState = Keyboard.GetState();
                GamePadState padState = GamePad.GetState(PlayerIndex.One);

                xPosition += elapsedTime.Milliseconds * .01f;

                if (keyState.IsKeyDown(Keys.Enter) || keyState.IsKeyDown(Keys.Space) || padState.IsButtonDown(Buttons.A))
                {
                    this.IsFinished = true;
                }
            }

            public override void Draw()
            {
                batch.Begin();
                batch.DrawString(font, "Press Enter when finished verifying this test", new Vector2(50, 50), Color.White);
                batch.Draw(this.texture, new Vector2((float)Math.Sin(xPosition) * 100, 100), Color.White);
                batch.End();
            }
        }
    }
}
