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

            SpriterInstance.StartAnimation();
		}

		private void CustomActivity()
		{
		    //Debugger.Write(MathHelper.ToDegrees(SpriterInstance.ObjectList[26].RotationZ));
            Debugger.Write(string.Format("Seconds into animation: {0}", SpriterInstance.SecondsIn));
		    if (SpriterInstance.SecondsIn > 0f && SpriterInstance.SecondsIn < .01f)
		    {
		        var x = 0;
		    }
		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
