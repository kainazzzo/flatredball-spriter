using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall.Math.Geometry;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Input;
using FlatRedBall.IO;
using FlatRedBall.Instructions;
using FlatRedBall.Math.Splines;
using FlatRedBall.Utilities;
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
using FlatRedBall_Spriter;

namespace spritertestgame.Screens
{
	public partial class TestScreen : Screen
	{
		// Generated Fields
		#if DEBUG
		static bool HasBeenLoadedWithGlobalContentManager = false;
		#endif
		private static FlatRedBall_Spriter.SpriterObject anthead;
		
		private FlatRedBall_Spriter.SpriterObject antHeadObj;

		public TestScreen()
			: base("TestScreen")
		{
		}

        public override void Initialize(bool addToManagers)
        {
			// Generated Initialize
			LoadStaticContent(ContentManagerName);
			antHeadObj = anthead.Clone();
			
			
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
			anthead.Destroy();
			anthead = null;
			
			if (antHeadObj != null)
			{
				antHeadObj.Destroy();
			}

			base.Destroy();

			CustomDestroy();

		}

		// Generated Methods
		public virtual void PostInitialize ()
		{
			bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
			FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
			FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
		}
		public virtual void AddToManagersBottomUp ()
		{
			anthead.AddToManagers(mLayer);
		}
		public virtual void ConvertToManuallyUpdated ()
		{
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
			bool registerUnload = false;
			if (!FlatRedBallServices.IsLoaded<FlatRedBall_Spriter.SpriterObject>(@"content/screens/testscreen/ant/anthead.scml", contentManagerName))
			{
			}
			anthead = FlatRedBall_Spriter.SpriterObjectSave.FromFile("content/screens/testscreen/ant/anthead.scml").ToRuntime();
			CustomLoadStaticContent(contentManagerName);
		}
		[System.Obsolete("Use GetFile instead")]
		public static object GetStaticMember (string memberName)
		{
			switch(memberName)
			{
				case  "anthead":
					return anthead;
			}
			return null;
		}
		public static object GetFile (string memberName)
		{
			switch(memberName)
			{
				case  "anthead":
					return anthead;
			}
			return null;
		}
		object GetMember (string memberName)
		{
			switch(memberName)
			{
				case  "anthead":
					return anthead;
			}
			return null;
		}


	}
}
