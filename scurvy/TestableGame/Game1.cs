using System;
using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.IO;
using FlatRedBall.Screens;
using FlatRedBall_Spriter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Scurvy.Test;

namespace TestableGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TestRunner<Game1> runner;
        XnaTestReporter reporter;
        SpriteFont font;
        private SpriterObject so = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Renderer.UseRenderTargets = false;
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
			GlobalContent.Initialize();

			FlatRedBall.Screens.ScreenManager.Start(typeof(TestableGame.Screens.TestScreen));

            SpriteManager.Camera.BackgroundColor = Color.Black;

            IsMouseVisible = true;



            this.runner = new TestRunner<Game1>(this.Services);
            this.reporter = new XnaTestReporter();
            this.runner.Reporter(this.reporter);

            this.font = this.Content.Load<SpriteFont>("font");

            var sos =
                SpriterObjectSave.FromFile(
                    @"c:\flatredballprojects\flatredball-spriter\spriterfiles\simpleballanimation\simpleballanimation.scml");

            var oldDir = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory =
                FileManager.GetDirectory(
                    @"c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png");
            so = sos.ToRuntime();
            FileManager.RelativeDirectory = oldDir;

            so.X = 300f;
            so.Y = 300f;
            

            so.AddToManagers(null);
            SpriteManager.Camera.Position.Z += 1900;
            SpriteManager.Camera.Position.Y -= 300;

            SpriteManager.Camera.FarClipPlane = 30000f;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            FlatRedBallServices.Update(gameTime);

            ScreenManager.Activity();

            this.runner.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            FlatRedBallServices.Draw();

            this.runner.Draw();

            this.spriteBatch.Begin();
            for (int i = 0; i < this.reporter.Statuses.Count; i++)
            {
                spriteBatch.DrawString(this.font, this.reporter.Statuses[i], new Vector2(0, i * 25), Color.White);
            }
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
