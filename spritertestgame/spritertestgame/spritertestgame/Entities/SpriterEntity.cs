using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Debugging;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;
using Microsoft.Xna.Framework;
using BitmapFont = FlatRedBall.Graphics.BitmapFont;
using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using Point = FlatRedBall.Math.Geometry.Point;
#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;


#endif

namespace spritertestgame.Entities
{
	public partial class SpriterEntity
	{
		private void CustomInitialize()
		{
		    TimeManager.TimeFactor = .1;
            SpriterInstance.RenderBones = false;
            SpriterInstance.StartAnimation("crouch_down");
		    //SpriterInstance.Looping = true;
		    SpriterInstance.AnimationFinished += (animation) =>
		    {
		        if (animation.Name == "crouch_down")
		        {
		            SpriterInstance.StartAnimation("stand_up");
		        }
                else if (animation.Name == "stand_up")
                {
                    SpriterInstance.StartAnimation("jump_start");
                }
                else if (animation.Name == "jump_start")
                {
                    SpriterInstance.StartAnimation("jump_loop");
                }
		    };
		}

	    private bool first = true;
		private void CustomActivity()
		{
		    if (first)
		    {
		        first = false;
                
		        var box = new Polygon
		        {
		            X = 0,
		            Y = 0,
		            Points = new[]
		            {
		                new Point(0, 0),
		                new Point(0, 64),
		                new Point(64, 64),
		                new Point(64, 0),
                        new Point(0, 0)
		            }
		        };
		        
		        ShapeManager.AddPolygon(box);
                box.AttachTo(SpriterInstance.ObjectList.ElementAt(15), false);
		        this.RotationY = MathHelper.ToRadians(45);
		    }
		    //Debugger.Write(MathHelper.ToDegrees(SpriterInstance.ObjectList[26].RotationZ));
            Debugger.Write(string.Format("Seconds into animation: {0}", SpriterInstance.SecondsIn));
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
