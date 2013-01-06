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
	    private SpriterObject so;
	    private Text text;
	    private Text text2;
	    private Text text3;

	    void CustomInitialize()
		{

            var sos =
    SpriterObjectSave.FromFile(
        @"c:\flatredballprojects\flatredball-spriter\spriterfiles\simpleballanimation\longballanimation.scml");

            var oldDir = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory =
                FileManager.GetDirectory(
                    @"c:/flatredballprojects/flatredball-spriter/spriterfiles/simpleballanimation/ball.png");
            so = sos.ToRuntime();
            FileManager.RelativeDirectory = oldDir;

            so.X = 200f;
            so.Y = 200f;

            so.AddToManagers(null);
            SpriteManager.Camera.Position.Z += 1900;
            SpriteManager.Camera.Position.Y -= 300;

            SpriteManager.Camera.FarClipPlane = 30000f;

		    text = TextManager.AddText("test");
		    text2 = TextManager.AddText("test2");
	        text3 = TextManager.AddText("test3");
		}

		void CustomActivity(bool firstTimeCalled)
		{
            if (firstTimeCalled)
            {
                so.StartAnimation();
            }
		    text.DisplayText = so.ObjectList[0].RelativePosition.ToString();
		    text2.DisplayText = string.Format("{0}", so.SecondsIn);
		    text3.DisplayText = so.CurrentKeyFrameIndex.ToString(CultureInfo.InvariantCulture);
		    text.X = 100f;
		    text.Y = 100f;
		    text2.X = 100f;
		    text2.Y = 150f;
		    text3.X = 100f;
		    text3.Y = 200f;
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
