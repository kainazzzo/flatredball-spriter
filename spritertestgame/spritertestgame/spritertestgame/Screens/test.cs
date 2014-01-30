using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FlatRedBall;
using FlatRedBall.Debugging;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Graphics.Model;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;
using FlatRedBallExtensions;
using FlatRedBall_Spriter;
using Microsoft.Xna.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.Localization;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
#endif

namespace spritertestgame.Screens
{
	public partial class test
	{
        //private PositionedObject _spo1 = new ScaledPositionedObject
        //{
        //    ScaleX = .5f,
        //    ScaleY = .5f,
        //    Position = Vector3.Zero
        //};

        //private PositionedObject _spo2 = new ScaledPositionedObject
        //{
        //    ScaleX = .5f,
        //    ScaleY = .5f,
        //    Position = new Vector3(10, 0, 0)
        //};

        //private Sprite _sprite = new ScaledSprite
        //{
        //    Width = 32f,
        //    Height = 32f,
        //    Position = new Vector3(10, 0, 0),
        //    Texture = square
        //};

	    void CustomInitialize()
		{
		    Camera.Main.BackgroundColor = Color.CornflowerBlue;
	        Camera.Main.Orthogonal = false;
            Camera.Main.UsePixelCoordinates();
	        Camera.Main.FarClipPlane = 10000f;
	        Camera.Main.NearClipPlane = -10000f;

	        //_square.Texture = FlatRedBallServices.Load<Texture2D>("content/entities/spriterentity/square.png");
	        //_squareParent.Texture = _square.Texture;


	        //SpriteManager.AddSprite(_square);
	        //SpriteManager.AddSprite(_squareParent);

	        //Camera.Main.Orthogonal = false;
	        //_square.RotationYVelocity = 1.0f;

	        
            
		}

		void CustomActivity(bool firstTimeCalled)
		{
            
		}

		void CustomDestroy()
		{
		    
		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
