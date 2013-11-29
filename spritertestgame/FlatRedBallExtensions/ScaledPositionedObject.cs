using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall;
using FlatRedBall.Gui;
using Microsoft.Xna.Framework;

namespace FlatRedBallExtensions
{
    public class ScaledPositionedObject : PositionedObject
    {
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }

        public override void UpdateDependencies(double currentTime)
        {
// ReSharper disable once CompareOfFloatsByEqualityOperator
            if (currentTime == LastDependencyUpdate) return;


            // All parents up the chain need to be updated first
            if (Parent != null)
            {
                Parent.UpdateDependencies(currentTime);
            }

            var scaledParent = Parent as ScaledPositionedObject;
            if (scaledParent == null) return;

            Position = new Vector3(RelativePosition.X*scaledParent.ScaleX,
                RelativePosition.Y*scaledParent.ScaleY,
                RelativePosition.Z*scaledParent.ScaleZ);
            Position += Parent.Position;
        }
    }
}
