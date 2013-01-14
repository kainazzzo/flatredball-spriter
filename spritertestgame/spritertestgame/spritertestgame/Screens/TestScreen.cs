using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.IO;
using FlatRedBall.Input;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Graphics.Model;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;
using FlatRedBall_Spriter;
using Microsoft.Xna.Framework.Graphics;
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
	public partial class TestScreen
	{
	    private SpriterObject _so;
	    private SpriterObject _so2;

	    void CustomInitialize()
		{

            var sos =
    SpriterObjectSave.FromFile(
        @"c:\flatredballprojects\flatredball-spriter\spriterfiles\simpleballanimation\ballmove.scml");

            var oldDir = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory =
                FileManager.GetDirectory(
                    @"c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png");
            _so = sos.ToRuntime();
            FileManager.RelativeDirectory = oldDir;

            _so.AddToManagers(null);

    //        var sos2 =
    //SpriterObjectSave.FromFile(
    //    @"C:\FlatRedBallProjects\flatredball-spriter\spriterfiles\monsterexample\Example.SCML");

    //        oldDir = FileManager.RelativeDirectory;
    //        FileManager.RelativeDirectory = @"C:\FlatRedBallProjects\flatredball-spriter\spriterfiles\monsterexample\";

    //        _so2 = sos2.ToRuntime();
    //        _so2.AddToManagers(null);

    //        FileManager.RelativeDirectory = oldDir;

            SpriteManager.Camera.UsePixelCoordinates();
	        SpriteManager.Camera.Y += 125;
            FlatRedBallServices.GraphicsOptions.TextureFilter = TextureFilter.Point;


		}

		void CustomActivity(bool firstTimeCalled)
		{
            if (firstTimeCalled)
            {
                _so.StartAnimation();
                //_so2.StartAnimation("Idle");
            }
            FlatRedBall.Debugging.Debugger.Write(string.Format("RelativePosition: {0}", _so.ObjectList[1].RelativePosition));
            

		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
