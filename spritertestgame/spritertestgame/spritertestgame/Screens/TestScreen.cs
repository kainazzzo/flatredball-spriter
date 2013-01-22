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
using Microsoft.Xna.Framework;
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
	        const string filename = @"C:\FlatRedBallProjects\flatredball-spriter\spriterfiles\monsterexample\Example.scml";
            var sos =
            SpriterObjectSave.FromFile(filename);

            var oldDir = FileManager.RelativeDirectory;
	        FileManager.RelativeDirectory =
	            FileManager.GetDirectory(
	                filename);
            _so = sos.ToRuntime();
	        _so.ScaleX = .5f;
	        _so.ScaleY = .75f;
            FileManager.RelativeDirectory = oldDir;

            _so.AddToManagers(null);

            var rect = new AxisAlignedRectangle {X = 0, Y = 0, ScaleX = 1, ScaleY = 1, Color = Color.Yellow};

	        ShapeManager.AddAxisAlignedRectangle(rect);

    //        var sos2 =
    //SpriterObjectSave.FromFile(
    //    @"C:\FlatRedBallProjects\flatredball-spriter\spriterfiles\monsterexample\Example.SCML");

    //        oldDir = FileManager.RelativeDirectory;
    //        FileManager.RelativeDirectory = @"C:\FlatRedBallProjects\flatredball-spriter\spriterfiles\monsterexample\";

    //        _so2 = sos2.ToRuntime();
    //        _so2.AddToManagers(null);

            FileManager.RelativeDirectory = oldDir;

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
            StringBuilder sb = new StringBuilder();
		    for (int index = 0; index < _so.ObjectList.Count; index++)
		    {
		        var positionedObject = _so.ObjectList[index];
		        sb.AppendFormat("Object Name: [{0}]. RotationZ: [{1}]. RelativePosition: [{2}].",
		                        positionedObject.Name, positionedObject.RelativeRotationZ,
                                positionedObject.RelativePosition);
		        var sprite = positionedObject as Sprite;
                if (sprite != null)
                {
                    sb.AppendFormat(" ScaleX: [{0}]. ScaleY: [{1}].", sprite.ScaleX,
                                    sprite.ScaleY);
                }
		        sb.Append("\r\n");
		    }

		    FlatRedBall.Debugging.Debugger.Write(sb.ToString());
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
