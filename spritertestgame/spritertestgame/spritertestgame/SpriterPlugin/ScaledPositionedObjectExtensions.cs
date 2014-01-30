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
                    var scaledParent = scaledPositionedObject.Parent as IRelativeScalable;
                    var thisAsIRelativeScalable = scaledPositionedObject as IRelativeScalable;
                    var thisAsSprite = scaledPositionedObject as ScaledSprite;
                    
                    var thisRelativeScaleX = thisAsIRelativeScalable == null
                        ? 1.0f
                        : thisAsIRelativeScalable.RelativeScaleX;

                    var thisRelativeScaleY = thisAsIRelativeScalable == null ? 
                        1.0f
                        : thisAsIRelativeScalable.RelativeScaleY;

                    var thisRelativeScaleZ = thisAsIRelativeScalable == null ? 
                        1.0f : thisAsIRelativeScalable.RelativeScaleZ;

                    var parentScaleX = scaledParent == null ? 1.0f : scaledParent.ScaleX;
                    var parentScaleY = scaledParent == null ? 1.0f : scaledParent.ScaleY;
                    var parentScaleZ = scaledParent == null ? 1.0f : scaledParent.ScaleZ;

                    if (thisAsSprite != null)
                    {
                        thisAsSprite.Height = thisAsSprite.TextureHeight*parentScaleY;
                        thisAsSprite.Width = thisAsSprite.TextureWidth*parentScaleX;
                        
                    }
                    else if (thisAsIRelativeScalable != null)
                    {
                        thisAsIRelativeScalable.ScaleX = parentScaleX * thisRelativeScaleX;
                        thisAsIRelativeScalable.ScaleY = parentScaleY * thisRelativeScaleY;
                        thisAsIRelativeScalable.ScaleZ = parentScaleZ * thisRelativeScaleZ;
                    }

                    //if (thisAsIRelativeScalable != null && !thisAsIRelativeScalable.ParentScaleChangesPosition)
                    //{
                    //    parentScaleX = parentScaleY = parentScaleZ = 1.0f;
                    //}

                    if (thisAsSprite != null)
                    {
                        parentScaleX *= thisRelativeScaleX;
                        parentScaleY *= thisRelativeScaleY;
                        parentScaleZ *= thisRelativeScaleZ;
                    }

                    if (scaledPositionedObject.ParentRotationChangesPosition)
                    {
                        scaledPositionedObject.Position.X = scaledPositionedObject.Parent.Position.X + scaledPositionedObject.Parent.RotationMatrix.M11 * scaledPositionedObject.RelativePosition.X * parentScaleX + scaledPositionedObject.Parent.RotationMatrix.M21 * scaledPositionedObject.RelativePosition.Y * parentScaleY + scaledPositionedObject.Parent.RotationMatrix.M31 * scaledPositionedObject.RelativePosition.Z * parentScaleZ;

                        scaledPositionedObject.Position.Y = scaledPositionedObject.Parent.Position.Y + scaledPositionedObject.Parent.RotationMatrix.M12 * scaledPositionedObject.RelativePosition.X * parentScaleX + scaledPositionedObject.Parent.RotationMatrix.M22 * scaledPositionedObject.RelativePosition.Y * parentScaleY + scaledPositionedObject.Parent.RotationMatrix.M32 * scaledPositionedObject.RelativePosition.Z * parentScaleZ;

                        scaledPositionedObject.Position.Z = scaledPositionedObject.Parent.Position.Z + scaledPositionedObject.Parent.RotationMatrix.M13 * scaledPositionedObject.RelativePosition.X * parentScaleX + scaledPositionedObject.Parent.RotationMatrix.M23 * scaledPositionedObject.RelativePosition.Y * parentScaleY + scaledPositionedObject.Parent.RotationMatrix.M33 * scaledPositionedObject.RelativePosition.Z * parentScaleZ;
                    }
                    else
                    {
                        scaledPositionedObject.Position = new Vector3(scaledPositionedObject.RelativePosition.X * parentScaleX, scaledPositionedObject.RelativePosition.Y * parentScaleY, scaledPositionedObject.RelativePosition.Z * parentScaleZ) + scaledPositionedObject.Parent.Position;
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