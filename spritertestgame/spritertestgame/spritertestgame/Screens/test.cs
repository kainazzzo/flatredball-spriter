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

		void CustomInitialize()
		{
		    Camera.Main.BackgroundColor = Color.CornflowerBlue;
		    Camera.Main.Orthogonal = false;
		    
            Camera.Main.UsePixelCoordinates();
            Camera.Main.Z += 1000f;
		    Camera.Main.FarClipPlane = 1000000f;

            #region xml

		    var sos =
		        TestSerializationUtility.DeserializeFromXml<SpriterObjectSave>(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <folder id=""0"">
        <file id=""0"" name=""square.png"" width=""32"" height=""32"" pivot_x=""0"" pivot_y=""1""/>
    </folder>
    <entity id=""0"" name=""entity_000"">
        <obj_info name=""bone1"" type=""bone"" w=""200"" h=""10""/>
        <obj_info name=""bone2"" type=""bone"" w=""200"" h=""10""/>
        <obj_info name=""bone3"" type=""bone"" w=""200"" h=""10""/>
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <bone_ref id=""0"" timeline=""0"" key=""0""/>
                    <bone_ref id=""1"" parent=""0"" timeline=""1"" key=""0""/>
                    <bone_ref id=""2"" parent=""1"" timeline=""2"" key=""0""/>
                    <object_ref id=""0"" parent=""0"" timeline=""3"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" parent=""1"" timeline=""4"" key=""0"" z_index=""1""/>
                    <object_ref id=""2"" parent=""2"" timeline=""5"" key=""0"" z_index=""2""/>
                </key>
                <key id=""1"" time=""500"">
                    <bone_ref id=""0"" timeline=""0"" key=""1""/>
                    <bone_ref id=""1"" parent=""0"" timeline=""1"" key=""1""/>
                    <bone_ref id=""2"" parent=""1"" timeline=""2"" key=""1""/>
                    <object_ref id=""0"" parent=""0"" timeline=""3"" key=""1"" z_index=""0""/>
                    <object_ref id=""1"" parent=""1"" timeline=""4"" key=""1"" z_index=""1""/>
                    <object_ref id=""2"" parent=""2"" timeline=""5"" key=""1"" z_index=""2""/>
                </key>
            </mainline>
            <timeline id=""0"" obj=""0"" name=""bone1"" object_type=""bone"">
                <key id=""0"">
                    <bone x=""100"" angle=""0"" scale_x=""0.5""/>
                </key>
                <key id=""1"" time=""500"" spin=""-1"">
                    <bone x=""100"" angle=""90"" scale_x=""0.5""/>
                </key>
            </timeline>
            <timeline id=""1"" obj=""1"" name=""bone2"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""200"" angle=""0""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <bone x=""200"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""2"" obj=""2"" name=""bone3"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""200"" angle=""0"" scale_x=""2""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <bone x=""200"" angle=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""3"" name=""square1"">
                <key id=""0"">
                    <object folder=""0"" file=""0"" x=""-180"" y=""100"" scale_x=""2""/>
                </key>
                <key id=""1"" time=""500"">
                    <object folder=""0"" file=""0"" x=""-180"" angle=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""4"" name=""square2"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-380"" angle=""0"" scale_x=""2""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""-380"" angle=""0"" scale_x=""2""/>
                </key>
            </timeline>
            <timeline id=""5"" name=""square3"">
                <key id=""0"">
                    <object folder=""0"" file=""0"" x=""-290"" y=""-100""/>
                </key>
                <key id=""1"" time=""500"">
                    <object folder=""0"" file=""0"" x=""-290"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
");
#endregion

		    sos.TextureLoader = Mock.Create<ITextureLoader>();
            sos.TextureLoader.Arrange(l => l.FromFile(Arg.AnyString)).Returns(square);
		    sos.Directory = "C:\\";

            _so = sos.ToRuntime();

            _so.StartAnimation();
            _so.AddToManagers(null);

            //_sprite.RelativeRotationZVelocity = 10f;
            //_spo1.RotationZVelocity = 10f;

            SpriteInstance.Visible = false;
		    
		}

		void CustomActivity(bool firstTimeCalled)
		{
            Debugger.Write(string.Format("{0}\r\n{1}\r\n{2}",
                _so.ObjectList[0], _so.ObjectList[2], _so.ObjectList[4]));
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
