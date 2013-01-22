using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.IO;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FlatRedBall_Spriter
{
    public sealed class SpriterObject : PositionedObject
    {
        public PositionedObjectList<PositionedObject> ObjectList { get; private set; }
        public List<KeyFrame> KeyFrameList { get { return CurrentAnimation != null ? CurrentAnimation.KeyFrames : null; } }

        public Dictionary<string, SpriterObjectAnimation> Animations { get; private set; }

        public bool Animating { get; private set; }
        public float SecondsIn { get; private set; }
        public int CurrentKeyFrameIndex { get; private set; }
        public int NextKeyFrameIndex { get { return CurrentKeyFrameIndex + 1; } }
        public SpriterObjectAnimation CurrentAnimation
        {
            get { return _currentAnimation; }
            private set
            {
                
                _currentAnimation = value;
                if (_currentAnimation == null || _currentAnimation.KeyFrames.Count == 0)
                {
                    FirstKeyFrameWithEndTime = null;
                }
                else
                {
                    FirstKeyFrameWithEndTime = new KeyFrame
                        {
                            Time = _currentAnimation.TotalTime,
                            Values = _currentAnimation.KeyFrames[0].Values
                        };
                }
            }
        }

        private KeyFrame FirstKeyFrameWithEndTime { get; set; }

        public KeyFrame NextKeyFrame
        {
            get
            {
                if (NextKeyFrameIndex > KeyFrameList.Count - 1)
                {
                    if (Looping)
                    {
                        return FirstKeyFrameWithEndTime;
                    }
                    else return null;
                }
                else
                {
                    return KeyFrameList[NextKeyFrameIndex];
                }
            }
        }

        public KeyFrame CurrentKeyFrame
        {
            get
            {
                if (CurrentKeyFrameIndex > KeyFrameList.Count - 1)
                {
                    if (Looping)
                    {
                        return KeyFrameList[KeyFrameList.Count - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return KeyFrameList[CurrentKeyFrameIndex];
                }
            }
        }
        public void ResetAnimation()
        {
            SecondsIn = 0f;
            CurrentKeyFrameIndex = 0;
            if (this.KeyFrameList == null || this.KeyFrameList.Count == 0)
            {
                CurrentAnimation = Animations.Values.FirstOrDefault();
            }

            SetAllObjectValuesToCurrentFrame();
        }

        public void StartAnimation()
        {
            ResetAnimation();
            Animating = true;
        }


        public void StartAnimation(string animationName)
        {
            SpriterObjectAnimation animation = null;
            if (Animations.TryGetValue(animationName, out animation))
            {
                CurrentAnimation = animation;
                StartAnimation();
            }
            else
            {
                throw new ArgumentException("Animation name does not exist.", animationName);
            }
        }

        public override void TimedActivity(float secondDifference, double secondDifferenceSquaredDividedByTwo, float secondsPassedLastFrame)
        {
            base.TimedActivity(secondDifference, secondDifferenceSquaredDividedByTwo, secondsPassedLastFrame);

            if (Animating)
            {
                SecondsIn += secondDifference;

                if (NextKeyFrame != null && SecondsIn >= NextKeyFrame.Time)
                {
                    ++CurrentKeyFrameIndex;
                }

                // Interpolate between the current keyframe and next keyframe values based on time difference
                if (SecondsIn < AnimationTotalTime && NextKeyFrame != null)
                {
                    float percentage = GetPercentageIntoFrame(SecondsIn, CurrentKeyFrame.Time, NextKeyFrame.Time);
                    foreach (var currentPair in this.CurrentKeyFrame.Values)
                    {
                        SetInterpolatedValues(currentPair, percentage);
                    }

                    if (Math.Abs(this.NextKeyFrame.Time - this.CurrentKeyFrame.Time) < .00001f)
                    {
                        foreach (var keyFrameValues in this.NextKeyFrame.Values)
                        {
                            SetInterpolatedValues(keyFrameValues, percentage);
                        }
                        ++CurrentKeyFrameIndex;
                    }
                }
                else
                {
                    if (SecondsIn >= this.AnimationTotalTime)
                    {
                        ClearAllTextures();

                        if (!Looping)
                        {
                            ResetAnimation();
                            Animating = false;
                        }
                        else
                        {
                            RestartAnimationWithWrapping();
                        }
                    }
                    else
                    {
                        SetAllObjectValuesToCurrentFrame();
                    }


                }


            }
        }

        private void SetInterpolatedValues(KeyValuePair<PositionedObject, KeyFrameValues> currentPair, float percentage)
        {
            var currentValues = currentPair.Value;
            if (NextKeyFrame.Values.ContainsKey(currentPair.Key))
            {
                var nextValues = NextKeyFrame.Values[currentPair.Key];
                var currentObject = currentPair.Key;

                if (currentObject.Parent != currentValues.Parent)
                {
                    currentObject.AttachTo(currentValues.Parent, false);
                }

                // Position
                currentObject.RelativePosition = Vector3.Lerp(currentValues.Position, nextValues.Position,
                                                              percentage);

                currentObject.RelativePosition.X *= this.ScaleX;
                currentObject.RelativePosition.Y *= this.ScaleY;

                if (float.IsNaN(currentObject.RelativePosition.X) ||
                    float.IsNaN(currentObject.RelativePosition.Y)
                    || float.IsNaN(currentObject.RelativePosition.Z))
                {
                    throw new Exception(string.Format("Float.IsNaN true! Object name {0} RelativePosition: {1}",
                                                      currentObject.Name, currentObject.RelativePosition));
                }


                // Angle
                int spin = currentValues.Spin;
                float angleA = currentValues.Rotation.Z;
                float angleB = nextValues.Rotation.Z;

                if (spin == 1 && angleB - angleA < 0)
                {
                    angleB += 360f;
                }
                else if (spin == -1 && angleB - angleA >= 0)
                {
                    angleB -= 360f;
                }

                currentObject.RelativeRotationZ =
                    MathHelper.ToRadians(MathHelper.Lerp(angleA,
                                                         angleB, percentage));

                

                // Sprite specific stuff
                var sprite = currentObject as Sprite;
                if (sprite != null)
                {
                    sprite.Texture = currentValues.Texture;

                    // Scale
                    sprite.ScaleX = MathHelper.Lerp(currentValues.ScaleX, nextValues.ScaleX, percentage);
                    sprite.ScaleY = MathHelper.Lerp(currentValues.ScaleY, nextValues.ScaleY, percentage);
                    sprite.ScaleX *= this.ScaleX;
                    sprite.ScaleY *= this.ScaleY;

                    sprite.Alpha = MathHelper.Lerp(currentValues.Alpha, nextValues.Alpha, percentage);
                }
            }
        }

        private void RestartAnimationWithWrapping()
        {
            var start = SecondsIn - AnimationTotalTime;
            StartAnimation();
            SecondsIn = start > 0f ? start : 0f;
        }

        private void SetAllObjectValuesToCurrentFrame()
        {
            foreach (var pair in CurrentKeyFrame.Values)
            {
                pair.Key.AttachTo(pair.Value.Parent, true);
                pair.Key.RelativePosition = pair.Value.Position;
                pair.Key.RelativePosition.X *= this.ScaleX;
                pair.Key.RelativePosition.Y *= this.ScaleY;
                pair.Key.RelativeRotationZ = MathHelper.ToRadians(pair.Value.Rotation.Z);

                var sprite = pair.Key as Sprite;
                if (sprite != null)
                {
                    sprite.Texture = pair.Value.Texture;
                    sprite.ScaleX = pair.Value.ScaleX;
                    sprite.ScaleY = pair.Value.ScaleY;
                    sprite.Alpha = pair.Value.Alpha;

                    sprite.ScaleX *= this.ScaleX;
                    sprite.ScaleY *= this.ScaleY;
                }
            }
        }

        private void ClearAllTextures()
        {
            // ReSharper disable ForCanBeConvertedToForeach
            for (int index = 0; index < ObjectList.Count; index++)
            // ReSharper restore ForCanBeConvertedToForeach
            {
                var positionedObject = ObjectList[index];
                if (positionedObject.GetType() == typeof(Sprite))
                {
                    ((Sprite)positionedObject).Texture = null;
                }
            }
        }

        public static float GetPercentageIntoFrame(float secondsIntoAnimation, float currentKeyFrameTime, float nextKeyFrameTime)
        {

            float retVal = (secondsIntoAnimation - currentKeyFrameTime) / (nextKeyFrameTime - currentKeyFrameTime);
            if (float.IsInfinity(retVal) || float.IsNaN(retVal))
            {
                return 0.0f;
            }
            else
            {
                return retVal;
            }
        }

        public void Destroy()
        {
            SpriteManager.RemovePositionedObject(this);
        }

        // Generated Fields
#if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
#endif

        // This is made global so that static lazy-loaded content can access it.
        public static string ContentManagerName
        {
            get;
            set;
        }

        static object mLockObject = new object();
        static List<string> mRegisteredUnloads = new List<string>();
        static List<string> LoadedContentManagers = new List<string>();
        private SpriterObjectAnimation _currentAnimation;

        public int Index { get; set; }
        public bool Used { get; set; }

        public Layer LayerProvidedByContainer { get; set; }

        public SpriterObject(string contentManagerName) :
            this(contentManagerName, true)
        {
        }


        public SpriterObject(string contentManagerName, bool addToManagers) :
            base()
        {
            LayerProvidedByContainer = null;
            Animating = false;
            SecondsIn = 0f;
            CurrentKeyFrameIndex = 0;
            ScaleX = 1.0f;
            ScaleY = 1.0f;
            Animations = new Dictionary<string, SpriterObjectAnimation>(1);

            ContentManagerName = contentManagerName;
            InitializeSpriterObject(addToManagers);
            ObjectList = new PositionedObjectList<PositionedObject>();
        }

        private void InitializeSpriterObject(bool addToManagers)
        {
            // Generated Initialize
            LoadStaticContent(ContentManagerName);

            PostInitialize();
            if (addToManagers)
            {
                AddToManagers(null);
            }


        }

        public void AddToManagers(Layer layerToAddTo)
        {
            LayerProvidedByContainer = layerToAddTo;
            SpriteManager.AddPositionedObject(this);

            foreach (var sprite in this.ObjectList.OfType<Sprite>().ToList())
            {
                SpriteManager.AddSprite(sprite);
                if (sprite.Parent != null && sprite.Parent.GetType() == typeof(PositionedObject))
                {
                    SpriteManager.AddPositionedObject(sprite.Parent);
                }
            }
            AddToManagersBottomUp(layerToAddTo);
        }

        public void PostInitialize()
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }

        public void AddToManagersBottomUp(Layer layerToAddTo)
        {
            // We move this back to the origin and unrotate it so that anything attached to it can just use its absolute position
            float oldRotationX = RotationX;
            float oldRotationY = RotationY;
            float oldRotationZ = RotationZ;

            float oldX = X;
            float oldY = Y;
            float oldZ = Z;

            X = 0;
            Y = 0;
            Z = 0;
            RotationX = 0;
            RotationY = 0;
            RotationZ = 0;
            X = oldX;
            Y = oldY;
            Z = oldZ;
            RotationX = oldRotationX;
            RotationY = oldRotationY;
            RotationZ = oldRotationZ;
        }

        public void ConvertToManuallyUpdated()
        {
            this.ForceUpdateDependenciesDeep();
            SpriteManager.ConvertToManuallyUpdated(this);
        }
        public static void LoadStaticContent(string contentManagerName)
        {
            if (string.IsNullOrEmpty(contentManagerName))
            {
                throw new ArgumentException("contentManagerName cannot be empty or null");
            }
            ContentManagerName = contentManagerName;
#if DEBUG
            if (contentManagerName == FlatRedBallServices.GlobalContentManager)
            {
                HasBeenLoadedWithGlobalContentManager = true;
            }
            else if (HasBeenLoadedWithGlobalContentManager)
            {
                throw new Exception("This type has been loaded with a Global content manager, then loaded with a non-global.  This can lead to a lot of bugs");
            }
#endif
            bool registerUnload = false;
            if (LoadedContentManagers.Contains(contentManagerName) == false)
            {
                LoadedContentManagers.Add(contentManagerName);
                lock (mLockObject)
                {
                    if (!mRegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("SpriterObjectTestEntityStaticUnload", UnloadStaticContent);
                        mRegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            if (registerUnload && ContentManagerName != FlatRedBallServices.GlobalContentManager)
            {
                lock (mLockObject)
                {
                    if (!mRegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("SpriterObjectTestEntityStaticUnload", UnloadStaticContent);
                        mRegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
        }
        public static void UnloadStaticContent()
        {
            if (LoadedContentManagers.Count != 0)
            {
                LoadedContentManagers.RemoveAt(0);
                mRegisteredUnloads.RemoveAt(0);
            }
            if (LoadedContentManagers.Count == 0)
            {
            }
        }

        public float AnimationTotalTime
        {
            get
            {
                return CurrentAnimation != null ? CurrentAnimation.TotalTime : 0.0f;
            }
        }

        public bool Looping
        {
            get { return CurrentAnimation != null && CurrentAnimation.Looping; }
        }

        public void SetToIgnorePausing()
        {
            InstructionManager.IgnorePausingFor(this);
        }

        public float ScaleX { get; set; }

        public float ScaleY { get; set; }
    }


    // Extra classes
    public static class SpriterObjectTestEntityExtensionMethods
    {
    }
}
