using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Graphics.Model;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;
using FlatRedBallExtensions;
using Microsoft.Xna.Framework;
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
	    private PositionedObject _spo1 = new ScaledPositionedObject
	    {
	        ScaleX = .5f,
	        ScaleY = .5f,
            Position = Vector3.Zero
	    };

	    private PositionedObject _spo2 = new ScaledPositionedObject
	    {
	        ScaleX = .5f,
	        ScaleY = .5f,
            Position = new Vector3(10, 0, 0)
	    };

	    private Sprite _sprite = new ScaledSprite
	    {
            Width = 32f,
            Height = 32f,
            Position = new Vector3(10, 0, 0),
            Texture = square
	    };

		void CustomInitialize()
		{
		    Camera.Main.BackgroundColor = Color.DarkSlateGray;
            Camera.Main.OrthogonalHeight = 240f;
            Camera.Main.OrthogonalWidth = 320f;
            
            SpriteManager.AddPositionedObject(_spo1);
            SpriteManager.AddPositionedObject(_spo2);
            SpriteManager.AddSprite(_sprite);

            _spo2.AttachTo(_spo1, true);
            _sprite.AttachTo(_spo2, true);

            //_sprite.RelativeRotationZVelocity = 10f;
            //_spo1.RotationZVelocity = 10f;

            SpriteInstance.Visible = false;
		    
		}

		void CustomActivity(bool firstTimeCalled)
		{
		    var x = 0;
		    if (x == 0)
		    {
		        x = 1;
		    }
		    else
		    {
		        x = 0;
		    }
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
