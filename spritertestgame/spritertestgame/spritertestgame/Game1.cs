using System;
using System.Collections.Generic;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.IO;
using FlatRedBall.Utilities;
using FlatRedBall_Spriter;
using Microsoft.Xna.Framework;
#if !FRB_MDX
using System.Linq;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endif
using FlatRedBall.Screens;
using Scurvy.Test;

namespace spritertestgame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
			
			BackStack<string> bs = new BackStack<string>();
			bs.Current = string.Empty;
			
			#if WINDOWS_PHONE
			// Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            graphics.IsFullScreen = true;
			
			#endif
        }

        protected override void Initialize()
        {
            Renderer.UseRenderTargets = false;
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
			GlobalContent.Initialize();

            SpriteManager.Camera.BackgroundColor = Color.Black;

            IsMouseVisible = true;



            this.runner = new TestRunner<Game1>(this.Services);
            this.reporter = new XnaTestReporter();
            this.runner.Reporter(this.reporter);

			FlatRedBall.Screens.ScreenManager.Start(typeof(spritertestgame.Screens.TestScreen));

            base.Initialize();
        }

        protected SpriterObject So { get; set; }

        protected TestStatusReporter reporter { get; set; }

        protected TestRunner<Game1> runner { get; set; }


        protected override void Update(GameTime gameTime)
        {
            FlatRedBallServices.Update(gameTime);

            FlatRedBall.Screens.ScreenManager.Activity();

            this.runner.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            FlatRedBallServices.Draw();

            this.runner.Draw();

            base.Draw(gameTime);
        }
    }
}
