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
using flatredball_spriter_test;
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

	    private SpriterObject _so;
	    private SpriterObject so;
	    private ScaledPolygon polygon;

	    void CustomInitialize()
		{
		    Camera.Main.BackgroundColor = Color.CornflowerBlue;
		    

            #region xml
            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <entity id=""0"" name=""entity_000"">
        <obj_info name=""box_000"" type=""box"" w=""32"" h=""32"" pivot_x=""0"" pivot_y=""0""/>
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
                <key id=""1"" time=""500"">
                    <object_ref id=""0"" timeline=""0"" key=""1"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" obj=""0"" name=""box_000"" object_type=""box"">
                <key id=""0"" spin=""-1"">
                    <object x=""10"" y=""-10"" pivot_x=""0"" pivot_y=""1"" angle=""45"" scale_x=""0.5"" scale_y=""0.5""/>
                </key>
                <key id=""1"" time=""500"">
                    <object pivot_x=""0"" pivot_y=""1"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";
            #endregion


            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);
            so = sos.ToRuntime();
            so.AddToManagers();
            so.RenderCollisionBoxes = true;

            so.StartAnimation();
	        polygon = (ScaledPolygon)so.ObjectList[0];

		}

		void CustomActivity(bool firstTimeCalled)
		{
		    Camera.Main.Z += InputManager.Mouse.ScrollWheel*-10;
            //squareInstance.Position.X = GuiManager.Cursor.WorldXAt(0);
            //squareInstance.Position.Y = GuiManager.Cursor.WorldYAt(0);
            Debugger.Write(polygon.Visible);
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
