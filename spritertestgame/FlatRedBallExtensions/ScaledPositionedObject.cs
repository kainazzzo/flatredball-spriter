using System;
using FlatRedBall;
using Microsoft.Xna.Framework;

namespace FlatRedBallExtensions
{
    public class ScaledPositionedObject : PositionedObject
    {
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }

        public ScaledPositionedObject() : base()
        {
            ScaleX = ScaleY = ScaleZ = 1.0f;
        }

        public override void UpdateDependencies(double currentTime)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            lock (this)
            {
                if (mLastDependencyUpdate == currentTime)
                {
                    return;
                }
                else
                {
                    mLastDependencyUpdate = currentTime;
                }
            }

            if (Parent != null)
            {
                Parent.UpdateDependencies(currentTime);


                #region Apply dependency update

                if (ParentRotationChangesRotation)
                {
                    // Set the property RotationMatrix rather than the field mRotationMatrix
                    // so the individual rotation values get updated.
                    RotationMatrix = RelativeRotationMatrix * Parent.RotationMatrix;
                }
                else
                {
                    RotationMatrix = RelativeRotationMatrix;
                }

                if (!IgnoreParentPosition)
                {
                    var scaledParent = Parent as ScaledPositionedObject;
                    var scaleX = scaledParent == null ? 1.0f : scaledParent.ScaleX;
                    var scaleY = scaledParent == null ? 1.0f : scaledParent.ScaleY;
                    var scaleZ = scaledParent == null ? 1.0f : scaledParent.ScaleZ;

                    if (ParentRotationChangesPosition)
                    {
                        Position.X = mParent.Position.X +
                                     Parent.RotationMatrix.M11 * RelativePosition.X * scaleX +
                                     Parent.RotationMatrix.M21 * RelativePosition.Y * scaleY +
                                     Parent.RotationMatrix.M31 * RelativePosition.Z * scaleZ;

                        Position.Y = mParent.Position.Y +
                                     Parent.RotationMatrix.M12 * RelativePosition.X * scaleX +
                                     Parent.RotationMatrix.M22 * RelativePosition.Y * scaleY +
                                     Parent.RotationMatrix.M32 * RelativePosition.Z * scaleZ;

                        Position.Z = mParent.Position.Z +
                                     Parent.RotationMatrix.M13 * RelativePosition.X * scaleX +
                                     Parent.RotationMatrix.M23 * RelativePosition.Y * scaleY +
                                     Parent.RotationMatrix.M33 * RelativePosition.Z * scaleZ;
                    }
                    else
                    {
                        Position = new Vector3(RelativePosition.X * scaleX,
                            RelativePosition.Y * scaleY,
                            RelativePosition.Z * scaleZ) + Parent.Position;
                        Position = RelativePosition + Parent.Position;
                    }
                }
#if DEBUG
                if (float.IsNaN(Position.Z))
                {
                    string error = "The PositionedObject of type " + GetType() + " has a " +
                                   "NaN on its Z property.  Its name is \"" + Name + "\".  ";

                    if (Parent != null)
                    {
                        error += "Its parent is of type " + Parent.GetType() + " and its name is \"" + Parent.Name +
                                 "\".";
                    }
                    else
                    {
                        error += "This object does not have a parent";
                    }
                    throw new Exception(error);
                }
#endif
                #endregion
            }
        }
    }
}
