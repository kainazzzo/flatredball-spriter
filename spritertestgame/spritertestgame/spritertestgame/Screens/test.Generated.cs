using BitmapFont = FlatRedBall.Graphics.BitmapFont;

using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;

#if XNA4 || WINDOWS_8
using Color = Microsoft.Xna.Framework.Color;
#elif FRB_MDX
using Color = System.Drawing.Color;
#else
using Color = Microsoft.Xna.Framework.Graphics.Color;
#endif

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Microsoft.Xna.Framework.Media;
#endif

// Generated Usings
using FlatRedBall;
using FlatRedBall.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace spritertestgame.Screens
{
	public partial class test : Screen
	{
		// Generated Fields
		#if DEBUG
		static bool HasBeenLoadedWithGlobalContentManager = false;
		#endif
		protected static Microsoft.Xna.Framework.Graphics.Texture2D square;
		
		private FlatRedBall.Sprite SpriteInstance;

		public test()
			: base("test")
		{
		}

        public override void Initialize(bool addToManagers)
        {
			// Generated Initialize
			LoadStaticContent(ContentManagerName);
			SpriteInstance = new FlatRedBall.Sprite();
			
			
			PostInitialize();
			base.Initialize(addToManagers);
			if (addToManagers)
			{
				AddToManagers();
			}

        }
        
// Generated AddToManagers
		public override void AddToManagers ()
		{
			base.AddToManagers();
			AddToManagersBottomUp();
			CustomInitialize();
		}


		public override void Activity(bool firstTimeCalled)
		{
			// Generated Activity
			if (!IsPaused)
			{
				
			}
			else
			{
			}
			base.Activity(firstTimeCalled);
			if (!IsActivityFinished)
			{
				CustomActivity(firstTimeCalled);
			}


				// After Custom Activity
				
            
		}

		public override void Destroy()
		{
			// Generated Destroy
			square = null;
			square3bonetest = null;
			
			if (SpriteInstance != null)
			{
				SpriteInstance.Detach(); SpriteManager.RemoveSprite(SpriteInstance);
			}

			base.Destroy();

			CustomDestroy();

		}

		// Generated Methods
		public virtual void PostInitialize ()
		{
			bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
			FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
			SpriteInstance.Texture = square;
			SpriteInstance.TextureScale = 1f;
			if (SpriteInstance.Parent == null)
			{
				SpriteInstance.X = 0f;
			}
			else
			{
				SpriteInstance.RelativeX = 0f;
			}
			if (SpriteInstance.Parent == null)
			{
				SpriteInstance.Y = 0f;
			}
			else
			{
				SpriteInstance.RelativeY = 0f;
			}
			FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
		}
		public virtual void AddToManagersBottomUp ()
		{
			CameraSetup.ResetCamera(SpriteManager.Camera);
			SpriteManager.AddSprite(SpriteInstance);
			SpriteInstance.Texture = square;
			SpriteInstance.TextureScale = 1f;
			if (SpriteInstance.Parent == null)
			{
				SpriteInstance.X = 0f;
			}
			else
			{
				SpriteInstance.RelativeX = 0f;
			}
			if (SpriteInstance.Parent == null)
			{
				SpriteInstance.Y = 0f;
			}
			else
			{
				SpriteInstance.RelativeY = 0f;
			}
		}
		public virtual void ConvertToManuallyUpdated ()
		{
			SpriteManager.ConvertToManuallyUpdated(SpriteInstance);
		}
		public static void LoadStaticContent (string contentManagerName)
		{
			if (string.IsNullOrEmpty(contentManagerName))
			{
				throw new ArgumentException("contentManagerName cannot be empty or null");
			}
			#if DEBUG
			if (contentManagerName == FlatRedBallServices.GlobalContentManager)
			{
				HasBeenLoadedWithGlobalContentManager = true;
			}
			else if (HasBeenLoadedWithGlobalContentManager)
			{
				throw new Exception("This type has been loaded with a Global content manager, then loaded with a non-global.  This can lead to a lot of bugs");
			}
			#endif
			if (!FlatRedBallServices.IsLoaded<Microsoft.Xna.Framework.Graphics.Texture2D>(@"content/screens/test/square.png", contentManagerName))
			{
			}
			square = FlatRedBallServices.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(@"content/screens/test/square.png", contentManagerName);
			if (square3bonetest == null)
			{
				{
					// We put the { and } to limit the scope of oldDelimiter
					char oldDelimiter = CsvFileManager.Delimiter;
					CsvFileManager.Delimiter = ',';
					List<square3bonetest> temporaryCsvObject = new List<square3bonetest>();
					CsvFileManager.CsvDeserializeList(typeof(square3bonetest), "content/screens/test/square3bonetest.scml", temporaryCsvObject);
					CsvFileManager.Delimiter = oldDelimiter;
					square3bonetest = temporaryCsvObject;
				}
			}
			CustomLoadStaticContent(contentManagerName);
		}
		[System.Obsolete("Use GetFile instead")]
		public static object GetStaticMember (string memberName)
		{
			switch(memberName)
			{
				case  "square":
					return square;
				case  "square3bonetest":
					return square3bonetest;
			}
			return null;
		}
		public static object GetFile (string memberName)
		{
			switch(memberName)
			{
				case  "square":
					return square;
				case  "square3bonetest":
					return square3bonetest;
			}
			return null;
		}
		object GetMember (string memberName)
		{
			switch(memberName)
			{
				case  "square":
					return square;
				case  "square3bonetest":
					return square3bonetest;
			}
			return null;
		}


	}
}
