using System;
using FlatRedBall;
using Microsoft.Xna.Framework;

namespace FlatRedBallExtensions
{
    public static class ScaledPositionedObjectExtensions
    {
        public static void UpdateDependenciesHelper(this PositionedObject scaledPositionedObject, double currentTime)
        {
            if (scaledPositionedObject.Parent != null)
            {
                scaledPositionedObject.Parent.UpdateDependencies(currentTime);

                #region Apply dependency update

                if (scaledPositionedObject.ParentRotationChangesRotation)
                {
                    // Set the property RotationMatrix rather than the field mRotationMatrix
                    // so the individual rotation values get updated.
                    scaledPositionedObject.RotationMatrix = scaledPositionedObject.RelativeRotationMatrix * scaledPositionedObject.Parent.RotationMatrix;
                }
                else
                {
                    scaledPositionedObject.RotationMatrix = scaledPositionedObject.RelativeRotationMatrix;
                }

                if (!scaledPositionedObject.IgnoreParentPosition)
                {
                    var scaledParent = scaledPositionedObject.Parent as ScaledPositionedObject;

                    var thisAsScaledPositionedObject = scaledPositionedObject as ScaledPositionedObject;
                    var thisAsScaledSprite = scaledPositionedObject as ScaledSprite;

                    var thisScaleX = thisAsScaledPositionedObject == null
                        ? (thisAsScaledSprite == null ? 1.0f : thisAsScaledSprite.ScaleY)
                        : thisAsScaledPositionedObject.ScaleY;

                    var thisScaleY = thisAsScaledPositionedObject == null
                        ? (thisAsScaledSprite == null ? 1.0f : thisAsScaledSprite.ScaleY)
                        : thisAsScaledPositionedObject.ScaleY;

                    var thisScaleZ = thisAsScaledPositionedObject == null ? 1.0f : thisAsScaledPositionedObject.ScaleZ;

                    var scaleX = scaledParent == null ? 1.0f : scaledParent.ScaleX * thisScaleX;
                    var scaleY = scaledParent == null ? 1.0f : scaledParent.ScaleY * thisScaleY;
                    var scaleZ = scaledParent == null ? 1.0f : scaledParent.ScaleZ * thisScaleZ;

                    if (scaledPositionedObject.ParentRotationChangesPosition)
                    {
                        scaledPositionedObject.Position.X = scaledPositionedObject.Parent.Position.X + scaledPositionedObject.Parent.RotationMatrix.M11 * scaledPositionedObject.RelativePosition.X * scaleX + scaledPositionedObject.Parent.RotationMatrix.M21 * scaledPositionedObject.RelativePosition.Y * scaleY + scaledPositionedObject.Parent.RotationMatrix.M31 * scaledPositionedObject.RelativePosition.Z * scaleZ;

                        scaledPositionedObject.Position.Y = scaledPositionedObject.Parent.Position.Y + scaledPositionedObject.Parent.RotationMatrix.M12 * scaledPositionedObject.RelativePosition.X * scaleX + scaledPositionedObject.Parent.RotationMatrix.M22 * scaledPositionedObject.RelativePosition.Y * scaleY + scaledPositionedObject.Parent.RotationMatrix.M32 * scaledPositionedObject.RelativePosition.Z * scaleZ;

                        scaledPositionedObject.Position.Z = scaledPositionedObject.Parent.Position.Z + scaledPositionedObject.Parent.RotationMatrix.M13 * scaledPositionedObject.RelativePosition.X * scaleX + scaledPositionedObject.Parent.RotationMatrix.M23 * scaledPositionedObject.RelativePosition.Y * scaleY + scaledPositionedObject.Parent.RotationMatrix.M33 * scaledPositionedObject.RelativePosition.Z * scaleZ;
                    }
                    else
                    {
                        scaledPositionedObject.Position = new Vector3(scaledPositionedObject.RelativePosition.X * scaleX, scaledPositionedObject.RelativePosition.Y * scaleY, scaledPositionedObject.RelativePosition.Z * scaleZ) + scaledPositionedObject.Parent.Position;
                        scaledPositionedObject.Position = scaledPositionedObject.RelativePosition + scaledPositionedObject.Parent.Position;
                    }
                }
#if DEBUG
                if (float.IsNaN(scaledPositionedObject.Position.Z))
                {
                    string error = "The PositionedObject of type " + scaledPositionedObject.GetType() + " has a " +
                                   "NaN on its Z property.  Its name is \"" + scaledPositionedObject.Name + "\".  ";

                    if (scaledPositionedObject.Parent != null)
                    {
                        error += "Its parent is of type " + scaledPositionedObject.Parent.GetType() + " and its name is \"" + scaledPositionedObject.Parent.Name +
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