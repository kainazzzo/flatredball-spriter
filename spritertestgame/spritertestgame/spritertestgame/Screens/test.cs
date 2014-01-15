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
        <obj_info name=""bone_000"" type=""bone"" w=""58.7963"" h=""10""/>
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <bone_ref id=""0"" timeline=""1"" key=""0""/>
                    <object_ref id=""0"" parent=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
                <key id=""1"" time=""500"">
                    <bone_ref id=""0"" timeline=""1"" key=""1""/>
                    <object_ref id=""0"" parent=""0"" timeline=""0"" key=""1"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""point_000"" object_type=""point"">
                <key id=""0"">
                    <object x=""63.235316"" y=""-90.941161"" angle=""41.552613""/>
                </key>
                <key id=""1"" time=""500"" spin=""-1"">
                    <object x=""77.351861"" y=""-91.791555"" angle=""221.552613""/>
                </key>
            </timeline>
            <timeline id=""1"" obj=""0"" name=""bone_000"" object_type=""bone"">
                <key id=""0"">
                    <bone x=""23"" y=""100"" angle=""318.447387""/>
                </key>
                <key id=""1"" time=""500"" spin=""-1"">
                    <bone x=""23"" y=""100"" angle=""0.830315""/>
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
            

            so.StartAnimation();
	        
		}

		void CustomActivity(bool firstTimeCalled)
		{
		    Camera.Main.Z += InputManager.Mouse.ScrollWheel*-10;
            

		    if (InputManager.Keyboard.KeyPushed(Keys.Space))
		    {
		        so.RenderPoints = !so.RenderPoints;
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
