using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBallExtensions;
using Microsoft.Xna.Framework;

namespace FlatRedBall_Spriter
{
    public sealed class SpriterObject : ScaledPositionedObject
    {
        #region Constructor
        public SpriterObject(string contentManagerName) :
            this(contentManagerName, true)
        {
        }


        public SpriterObject(string contentManagerName, bool addToManagers)
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
            ObjectList = new List<PositionedObject>();
            CollisionBoxes = new ShapeCollection();
        }
        #endregion

        public List<PositionedObject> ObjectList { get; private set; }
        public ShapeCollection CollisionBoxes { get; private set; } 

        public List<KeyFrame> KeyFrameList { get { return CurrentAnimation != null ? CurrentAnimation.KeyFrames : null; } }

        public Dictionary<string, SpriterObjectAnimation> Animations { get; private set; }

        public bool Animating { get; set; }
        public float SecondsIn { get; private set; }
        public int CurrentKeyFrameIndex { get; private set; }
        public int NextKeyFrameIndex { get { return CurrentKeyFrameIndex + 1; } }
        public int PreviousKeyFrameIndex { get { return CurrentKeyFrameIndex - 1; }}

        public bool RenderBones
        {
            get { return _renderBones; }
            set
            {
                _renderBones = value;
                ObjectList.OfType<SpriterBone>().ToList().ForEach(bone => bone.Visible = value);
            }
        }

        public bool RenderPoints
        {
            get { return _renderPoints; }
            set
            {
                _renderPoints = value;
                ObjectList.OfType<SpriterPoint>().ToList().ForEach(point => point.Visible = value);
            }
        }

        public bool RenderCollisionBoxes
        {
            get { return _renderCollisionBoxes; }
            set
            {
                _renderCollisionBoxes = value;
                ObjectList.OfType<ScaledPolygon>().ToList().ForEach(polygon => polygon.Visible = value);
            }
        }

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
                if (!Reverse && NextKeyFrameIndex > KeyFrameList.Count - 1)
                {
                    if (Looping)
                    {
                        return FirstKeyFrameWithEndTime;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (Reverse && PreviousKeyFrameIndex < 0)
                {
                    if (Looping)
                    {
                        return KeyFrameList.Last();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (Reverse)
                    {
                        return KeyFrameList[PreviousKeyFrameIndex];
                    }
                    else
                    {
                        return KeyFrameList[NextKeyFrameIndex];
                    }
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
                else if (CurrentKeyFrameIndex < 0)
                {
                    if (Looping)
                    {
                        return FirstKeyFrameWithEndTime;
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
        private void ResetAnimation()
        {
            if (KeyFrameList == null || KeyFrameList.Count == 0)
            {
                CurrentAnimation = Animations.Values.FirstOrDefault();

            }

            SecondsIn = 0f;
            if (!Reverse)
            {
                CurrentKeyFrameIndex = 0;
            }
            else
            {
                CurrentKeyFrameIndex = -1;
            }

            UpdateCollisionBoxes();

            SetAllObjectValuesToCurrentFrame();
        }

        public void StartAnimation()
        {
            ResetAnimation();
            Animating = true;
        }

        public void StartAnimation(string animationName)
        {
            SpriterObjectAnimation animation;
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
                if (Reverse)
                {
                    SecondsIn -= secondDifference;
                }
                else
                {
                    SecondsIn += secondDifference;
                }

                
                if (!Reverse && NextKeyFrame != null && SecondsIn >= NextKeyFrame.Time)
                {
                    ++CurrentKeyFrameIndex;
                    UpdateCollisionBoxes();
                }
                else if (Reverse && NextKeyFrame != null && SecondsIn <= NextKeyFrame.Time)
                {
                    --CurrentKeyFrameIndex;
                    UpdateCollisionBoxes();
                }

                // Interpolate between the current keyframe and next keyframe values based on time difference
                if (((!Reverse && SecondsIn < AnimationTotalTime) || (Reverse && SecondsIn > 0f)) && NextKeyFrame != null)
                {
                    float percentage = GetPercentageIntoFrame(SecondsIn, CurrentKeyFrame.Time, NextKeyFrame.Time);
                    foreach (var currentPair in CurrentKeyFrame.Values)
                    {
                        SetInterpolatedValues(currentPair, percentage);
                    }

                    if (Math.Abs(NextKeyFrame.Time - CurrentKeyFrame.Time) < .000001f)
                    {
                        foreach (var keyFrameValues in NextKeyFrame.Values)
                        {
                            SetInterpolatedValues(keyFrameValues, percentage);
                        }
                        ++CurrentKeyFrameIndex;
                        UpdateCollisionBoxes();
                    }
                }
                else
                {
                    if ((!Reverse && SecondsIn >= AnimationTotalTime) || (Reverse && SecondsIn < 0))
                    {
                        if (!Looping)
                        {
                            Animating = false;
                            OnAnimationFinished(CurrentAnimation);
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

                UpdateAllObjectDependencies();
            }
        }

        private void UpdateCollisionBoxes()
        {
            CollisionBoxes.Clear();
            CollisionBoxes.Polygons.AddRange(CurrentKeyFrame.Values.Keys.OfType<ScaledPolygon>());
        }

        private void UpdateAllObjectDependencies()
        {
            foreach (var obj in ObjectList)
            {
                obj.UpdateDependencies(TimeManager.CurrentTime);
            }
        }

        private void SetInterpolatedValues(KeyValuePair<PositionedObject, KeyFrameValues> currentPair, float percentage)
        {
            var currentValues = currentPair.Value;
            var nextValues = NextKeyFrame == null
                ? currentValues
                : NextKeyFrame.Values.ContainsKey(currentPair.Key)
                    ? NextKeyFrame.Values[currentPair.Key]
                    : currentValues;

            //var nextValues = NextKeyFrame.Values[currentPair.Key];
            var currentObject = currentPair.Key;

            if (currentValues.Parent == null)
            {
                currentObject.AttachTo(this, true);
            }
            else if (currentObject.Parent != currentValues.Parent)
            {
                currentObject.AttachTo(currentValues.Parent, true);
            }

            // Position
            // In a single dimension, if the spriterobject is on x = 5, and the position to move a subobject to is x=8, then it should actually move to x=13, because we add the spriterobject's position to the subobject's interpolated value, since while positions are absolute, they are "absolute" relative to the container
            currentObject.RelativePosition = Vector3.Lerp(currentValues.RelativePosition, nextValues.RelativePosition,
                percentage);

            if (FlipHorizontal)
            {
                currentObject.RelativeZ *= -1;
            }


            if (float.IsNaN(currentObject.RelativePosition.X) ||
                float.IsNaN(currentObject.RelativePosition.Y)
                || float.IsNaN(currentObject.RelativePosition.Z))
            {
                throw new Exception(string.Format("Float.IsNaN true! Object name {0} RelativePosition: {1}",
                    currentObject.Name, currentObject.RelativePosition));
            }


            // Angle
            int spin = currentValues.Spin;
            float angleA = currentValues.RelativeRotation.Z;
            float angleB = nextValues.RelativeRotation.Z;

            if (Reverse)
            {
                spin *= -1;
            }

            if (spin == 1 && angleB - angleA < 0)
            {
                angleB += 360f;
            }
            else if (spin == -1 && angleB - angleA > 0)
            {
                angleB -= 360f;
            }

            currentObject.RelativeRotationZ =
                MathHelper.ToRadians(MathHelper.Lerp(angleA,
                    angleB, percentage));



            // Sprite specific stuff
            var sprite = currentObject as ScaledSprite;
            if (sprite != null)
            {
                sprite.Texture = currentValues.Texture;
                sprite.Alpha = MathHelper.Lerp(currentValues.Alpha, nextValues.Alpha, percentage);
            }

            var spo = currentObject as IRelativeScalable;
            if (spo != null)
            {
                spo.RelativeScaleX = MathHelper.Lerp(currentValues.RelativeScaleX, nextValues.RelativeScaleX, percentage);
                spo.RelativeScaleY = MathHelper.Lerp(currentValues.RelativeScaleY, nextValues.RelativeScaleY, percentage);
            }
        }

        private void RestartAnimationWithWrapping()
        {
            float start;
            if (!Reverse)
            {
                start = SecondsIn - AnimationTotalTime;
            }
            else
            {
                start = AnimationTotalTime - Math.Abs(SecondsIn);
            }
            StartAnimation();
            if (!Reverse)
            {
                SecondsIn = start > 0f ? start : 0f;
            }
            else
            {
                SecondsIn = start < AnimationTotalTime ? start : AnimationTotalTime;
            }
            // This is a hack
            TimeManager.CurrentTime += .0000001;
            TimedActivity(0, 0, 0);
            TimeManager.CurrentTime -= .0000001;
        }

        private void SetAllObjectValuesToCurrentFrame()
        {
            foreach (var pair in CurrentKeyFrame.Values)
            {
                SetInterpolatedValues(pair, 0);
            }
            UpdateAllObjectDependencies();
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
            foreach (var positionedObject in ObjectList)
            {
                SpriteManager.RemovePositionedObject(positionedObject);
            }
        }

        // Generated Fields
#if DEBUG
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once RedundantDefaultFieldInitializer
        static bool HasBeenLoadedWithGlobalContentManager = false;
#endif

        // This is made global so that static lazy-loaded content can access it.
        public static string ContentManagerName
        {
            get;
            set;
        }

        private static readonly object LockObject = new object();
        private static readonly List<string> RegisteredUnloads = new List<string>();
        private static readonly List<string> LoadedContentManagers = new List<string>();

        private SpriterObjectAnimation _currentAnimation;
        private bool _renderCollisionBoxes;
        private bool _renderBones;
        private bool _renderPoints;
        private bool _flipHorizontal;
        private bool _reverse;

        public int Index { get; set; }
        public bool Used { get; set; }

        public Layer LayerProvidedByContainer { get; set; }

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

            if (ObjectList != null)
            {
                foreach (var currentObject in ObjectList)
                {
                    var sprite = currentObject as ScaledSprite;
                    if (sprite != null)
                    {
                        SpriteManager.AddSprite(sprite);
                    }
                    else
                    {
                        var scaledPolygon = currentObject as Polygon;
                        if (scaledPolygon != null)
                        {
                            var visible = scaledPolygon.Visible;
                            ShapeManager.AddPolygon(scaledPolygon);
                            scaledPolygon.Visible = visible;
                        }
                        else
                        {
                            SpriteManager.AddPositionedObject(currentObject);
                        }
                    }
                }
            }

            SpriteManager.AddPositionedObject(this);
            AddToManagersBottomUp(layerToAddTo);
        }

        public void PostInitialize()
        {
            bool oldShapeManagerSuppressAdd = ShapeManager.SuppressAddingOnVisibilityTrue;
            ShapeManager.SuppressAddingOnVisibilityTrue = true;
            ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }

        private void AddToManagersBottomUp(Layer layerToAddTo)
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
// ReSharper disable once ConvertToConstant.Local
            bool registerUnload = false;
            if (LoadedContentManagers.Contains(contentManagerName) == false)
            {
                LoadedContentManagers.Add(contentManagerName);
                lock (LockObject)
                {
                    if (!RegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("SpriterObjectTestEntityStaticUnload", UnloadStaticContent);
                        RegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (registerUnload && ContentManagerName != FlatRedBallServices.GlobalContentManager)
            // ReSharper disable HeuristicUnreachableCode
            {
                lock (LockObject)
                {
                    if (!RegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("SpriterObjectTestEntityStaticUnload", UnloadStaticContent);
                        RegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            // ReSharper restore HeuristicUnreachableCode
        }
        public static void UnloadStaticContent()
        {
            if (LoadedContentManagers.Count != 0)
            {
                LoadedContentManagers.RemoveAt(0);
                RegisteredUnloads.RemoveAt(0);
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
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                if (CurrentAnimation != null)
                {
                    CurrentAnimation.Looping = true;
                }
            }
        }

        public SpriterObject Clone()
        {
            var so = new SpriterObject(FlatRedBallServices.GlobalContentManager, false)
            {
                Animations = new Dictionary<string, SpriterObjectAnimation>()
            };

            var allObjects =
                Animations.Select(a => a.Value)
                    .SelectMany(v => v.KeyFrames)
                    .SelectMany(kf => kf.Values)
                    .Select(kfv => kfv.Key)
                    .GroupBy(k => k.Name)
                    .Select(g => g.First().Clone<PositionedObject>())
                    .ToList();



            foreach (var animationPair in Animations)
            {
                var keyframes = new List<KeyFrame>();
                animationPair.Value.KeyFrames.ForEach(kf =>
                    {
                        var keyFrame = new KeyFrame
                            {
                                Time = kf.Time,
                                Values = new Dictionary<PositionedObject, KeyFrameValues>(kf.Values.Count)
                            };

                        foreach (var kfPair in kf.Values)
                        {
                            var parent = kfPair.Value.Parent == null || kfPair.Value.Parent.Name == null
                                ? null
                                : allObjects.First(k => k.Name == kfPair.Value.Parent.Name);

                            var kfv = new KeyFrameValues
                            {
                                Alpha = kfPair.Value.Alpha,
                                Parent = parent,
                                RelativePosition = kfPair.Value.RelativePosition,
                                RelativeRotation = kfPair.Value.RelativeRotation,
                                RelativeScaleX = kfPair.Value.RelativeScaleX,
                                RelativeScaleY = kfPair.Value.RelativeScaleY,
                                Spin = kfPair.Value.Spin,
                                Texture = kfPair.Value.Texture
                            };

                            keyFrame.Values[allObjects.First(k => k.Name == kfPair.Key.Name)] = kfv;
                        }
                        keyframes.Add(keyFrame);
                    });
                so.Animations[animationPair.Key] = new SpriterObjectAnimation(animationPair.Value.Name,
                                                                              animationPair.Value.Looping,
                                                                              animationPair.Value.TotalTime,
                                                                              keyframes);
            }
            so.ObjectList = so.Animations.Select(a => a.Value)
                    .SelectMany(v => v.KeyFrames)
                    .SelectMany(kf => kf.Values)
                    .Select(kfv => kfv.Key)
                    .GroupBy(k => k.Name)
                    .Select(g => g.First())
                    .ToList();

            return so;
        }

        public void AddToManagers()
        {
            AddToManagers(null);
        }

        public event Action<SpriterObjectAnimation> AnimationFinished;

        private void OnAnimationFinished(SpriterObjectAnimation animation)
        {
            Action<SpriterObjectAnimation> handler = AnimationFinished;
            if (handler != null) handler(animation);
        }

        public bool FlipHorizontal
        {
            get { return _flipHorizontal; }
            set
            {
                _flipHorizontal = value;
                RelativeRotationY = value ? MathHelper.ToRadians(180f) : 0f;
            }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set
            {
                if (_reverse != value)
                {
                    // If changing to reverse, the current keyframe actually has to switch to the next one, because it's going backwards
                    if (value)
                    {
                        ++CurrentKeyFrameIndex;
                    }
                    // Otherwise if changing to normal, then switch the current keyframe back to the one in the past
                    else
                    {
                        --CurrentKeyFrameIndex;
                    }
                }
                _reverse = value;
            }
        }
    }
}
