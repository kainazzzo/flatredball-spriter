using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall.IO;
using FlatRedBall_Spriter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scurvy.Test;

namespace spritertestgame
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

        [TestMethod]
        public void SpritesClearTexturesAtStart()
        {
            var so = GetSimpleSpriterObjectWithTwoObjects(true);

            so.StartAnimation();
            so.TimedActivity(.5f, 0f, 0f);
            Assert.IsTrue(((Sprite)so.ObjectList[0]).Texture != null);
            Assert.IsTrue(((Sprite)so.ObjectList[2]).Texture == null);

            so.TimedActivity(1.0f, 0f, 0f);
            Assert.IsTrue(((Sprite)so.ObjectList[0]).Texture != null);
            Assert.IsTrue(((Sprite)so.ObjectList[2]).Texture != null);

            so.TimedActivity(.49f, 0f, 0f);
            Assert.IsTrue(((Sprite)so.ObjectList[0]).Texture != null);
            Assert.IsTrue(((Sprite)so.ObjectList[2]).Texture != null);

            so.TimedActivity(.01f, 0f, 0f);
            Assert.IsTrue(((Sprite)so.ObjectList[0]).Texture != null);
            Assert.IsTrue(((Sprite)so.ObjectList[2]).Texture == null);
        }

        private static SpriterObject GetSimpleSpriterObjectWithTwoObjects(bool loops = false)
        {
            var goodTexture =
                FlatRedBallServices.Load<Texture2D>(
                    "c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png");
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

            var keyFrame = new KeyFrame()
            {
                Time = 0
            };
            keyFrame.Values[pivot] = new KeyFrameValues()
            {
                Position = Vector3.Zero,
            };
            keyFrame.Values[pivot2] = new KeyFrameValues()
            {
                Position = Vector3.Zero,
            };
            keyFrame.Values[sprite] = new KeyFrameValues()
            {
                Texture = goodTexture
            };
            keyFrame.Values[sprite2] = new KeyFrameValues()
            {
                Texture = null
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            keyFrame = new KeyFrame()
            {
                Time = 1.0f
            };
            keyFrame.Values[pivot] = new KeyFrameValues()
            {
                Position = new Vector3(0f, 10f, 0f),
            };
            keyFrame.Values[pivot2] = new KeyFrameValues()
            {
                Position = new Vector3(10f, 0f, 0f),
            };
            keyFrame.Values[sprite] = new KeyFrameValues()
            {
                Texture = goodTexture
            };
            keyFrame.Values[sprite2] = new KeyFrameValues()
            {
                Texture = goodTexture
            };

            so.Animations[""].KeyFrames.Add(keyFrame);

            so.ObjectList.Add(sprite);
            so.ObjectList.Add(pivot);
            so.ObjectList.Add(sprite2);
            so.ObjectList.Add(pivot2);

            return so;
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
            
            var oldDir = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory =
                FileManager.GetDirectory(
                    @"c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png");            
            var so = sos.ToRuntime();
            FileManager.RelativeDirectory = oldDir;

            so.StartAnimation();
            Assert.AreEqual(2, so.ObjectList.Count, "1 Object");
            Assert.AreEqual(6, so.KeyFrameList.Count, "5 Keys");

            Assert.AreEqual(0.0f, so.KeyFrameList[0].Time, "KeyFrame 0 Time = 0ms");
            Assert.AreEqual(0.2f, so.KeyFrameList[1].Time, "KeyFrame 1 Time = 200ms");
            Assert.AreEqual(0.4f, so.KeyFrameList[2].Time, "KeyFrame 2 Time = 400ms");
            Assert.AreEqual(0.6f, so.KeyFrameList[3].Time, "KeyFrame 3 Time = 600ms");
            Assert.AreEqual(0.8f, so.KeyFrameList[4].Time, "KeyFrame 4 Time = 800ms");

            PositionedObject pivot = so.ObjectList[1];
            PositionedObject sprite = so.ObjectList[0];

            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[pivot].Position.X, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[pivot].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[pivot].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[pivot].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[pivot].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[pivot].Rotation.Z, "Rotation Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[0].Values[sprite].ScaleX, "Scale Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[0].Values[sprite].ScaleY, "Scale Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[0].Values[sprite].Position.Y, "Y value relative to pivot");
            Assert.AreEqual(0.0f, so.KeyFrameList[0].Values[sprite].Position.X, "X value relative to pivot");
            Assert.AreEqual("c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png", so.KeyFrameList[0].Values[sprite].Texture.Name, "Texture test");

            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[pivot].Position.X, "Position Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[1].Values[pivot].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[pivot].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[pivot].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[pivot].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[pivot].Rotation.Z, "Rotation Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[1].Values[sprite].ScaleX, "Scale Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[1].Values[sprite].ScaleY, "Scale Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[1].Values[sprite].Position.Y, "Y value relative to pivot");
            Assert.AreEqual(0.0f, so.KeyFrameList[1].Values[sprite].Position.X, "X value relative to pivot");
            Assert.AreEqual("c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png", so.KeyFrameList[1].Values[sprite].Texture.Name, "Texture test");

            Assert.AreEqual(-128.0f, so.KeyFrameList[2].Values[pivot].Position.X, "Position Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[2].Values[pivot].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[pivot].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[pivot].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[pivot].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[pivot].Rotation.Z, "Rotation Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[2].Values[sprite].ScaleX, "Scale Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[2].Values[sprite].ScaleY, "Scale Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[2].Values[sprite].Position.Y, "Y value relative to pivot");
            Assert.AreEqual(0.0f, so.KeyFrameList[2].Values[sprite].Position.X, "X value relative to pivot");
            Assert.AreEqual("c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png", so.KeyFrameList[2].Values[sprite].Texture.Name, "Texture test");

            Assert.AreEqual(-128.0f, so.KeyFrameList[3].Values[pivot].Position.X, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[pivot].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[pivot].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[pivot].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[pivot].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[pivot].Rotation.Z, "Rotation Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[3].Values[sprite].ScaleX, "Scale Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[3].Values[sprite].ScaleY, "Scale Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[3].Values[sprite].Position.Y, "Y value relative to pivot");
            Assert.AreEqual(0.0f, so.KeyFrameList[3].Values[sprite].Position.X, "X value relative to pivot");

            Assert.AreEqual("c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png", so.KeyFrameList[3].Values[sprite].Texture.Name, "Texture test");

            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[pivot].Position.X, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[pivot].Position.Y, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[pivot].Position.Z, "Position Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[pivot].Rotation.X, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[pivot].Rotation.Y, "Rotation Test");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[pivot].Rotation.Z, "Rotation Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[4].Values[sprite].ScaleX, "Scale Test");
            Assert.AreEqual(64.0f, so.KeyFrameList[4].Values[sprite].ScaleY, "Scale Test");
            Assert.AreEqual(128.0f, so.KeyFrameList[4].Values[sprite].Position.Y, "Y value relative to pivot");
            Assert.AreEqual(0.0f, so.KeyFrameList[4].Values[sprite].Position.X, "X value relative to pivot");

            Assert.AreEqual("c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png", so.KeyFrameList[4].Values[sprite].Texture.Name, "Texture test");
        }

        /// <summary>
        /// This test renders a texture and waits for manual approval that the test is finished.
        /// </summary>
        //[TestMethod]
        //public void ManualContentVerificationTest(TestContext context)
        //{
        //    context.ExitCriteria = new ContentVerificationExitCriteria(this.content, context);
        //}

        //private class ContentVerificationExitCriteria : ExitCriteria
        //{
        //    private readonly SpriteBatch _batch;
        //    private readonly Texture2D _texture;
        //    private readonly SpriteFont _font;
        //    private float _xPosition;

        //    public ContentVerificationExitCriteria(ContentManager content, TestContext context)
        //        : base(context)
        //    {
        //        var graphics = (IGraphicsDeviceService)this.Context.Services.GetService(typeof(IGraphicsDeviceService));
        //        this._batch = new SpriteBatch(graphics.GraphicsDevice);
        //        this._texture = content.Load<Texture2D>("scurvy_logo_big");
        //        this._font = content.Load<SpriteFont>("font");
        //    }

        //    public override void Update(TimeSpan elapsedTime)
        //    {
        //        KeyboardState keyState = Keyboard.GetState();
        //        GamePadState padState = GamePad.GetState(PlayerIndex.One);

        //        _xPosition += elapsedTime.Milliseconds * .01f;

        //        if (keyState.IsKeyDown(Keys.Enter) || keyState.IsKeyDown(Keys.Space) || padState.IsButtonDown(Buttons.A))
        //        {
        //            this.IsFinished = true;
        //        }
        //    }

        //    public override void Draw()
        //    {
        //        _batch.Begin();
        //        _batch.DrawString(_font, "Press Enter when finished verifying this test", new Vector2(50, 50), Color.White);
        //        _batch.Draw(this._texture, new Vector2((float)Math.Sin(_xPosition) * 100, 100), Color.White);
        //        _batch.End();
        //    }
        //}
    }
}
