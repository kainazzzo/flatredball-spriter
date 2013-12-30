using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.IO;
using FlatRedBallExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatRedBall_Spriter
{
    public partial class SpriterObjectSave
    {
        [XmlIgnore]
        public ITextureLoader TextureLoader { get; set; }

        public string Directory { get; set; }
        public string FileName { get; set; }

        public SpriterObject ToRuntime()
        {
            var spriterObject = new SpriterObject(FlatRedBallServices.GlobalContentManager, false);

            IDictionary<string, string> filenames = new Dictionary<string, string>();
            IDictionary<int, ScaledSprite> persistentScaledSprites = new Dictionary<int, ScaledSprite>();
            IDictionary<int, ScaledPositionedObject> persistentBones = new Dictionary<int, ScaledPositionedObject>();
            IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            IDictionary<int, ScaledPositionedObject> boneRefDic = new Dictionary<int, ScaledPositionedObject>();
            IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary = new Dictionary<KeyFrameValues, int>();

            string oldDir = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory = this.Directory;
            foreach (var folder in this.Folder)
            {
                foreach (var file in folder.File)
                {
                    string folderFileId = string.Format("{0}_{1}", folder.Id, file.Id);
                    filenames[folderFileId] = file.Name;
                    textures[folderFileId] = LoadTexture(file);
                }
            }
            FileManager.RelativeDirectory = oldDir;


            foreach (var animation in Entity[0].Animation)
            {
                var mainline = animation.Mainline;
                var keyFrameList = new List<KeyFrame>();

                foreach (var key in mainline.Keys)
                {

                    var keyFrame = new KeyFrame { Time = key.Time / 1000.0f };
                    
                    // If it's a ScaledSprite (not a bone)
                    if (key.ObjectRef != null)
                    {
                        CreateRuntimeObjectsForSpriterObjectRef(key, persistentScaledSprites, spriterObject, animation, textures, keyFrame, keyFrameValuesParentDictionary);
                    }
                    
                    // If it's a bone (not a ScaledSprite)
                    if (key.BoneRef != null)
                    {
                        CreateRuntimeObjectsForSpriterBoneRef(key, persistentBones, spriterObject, animation, keyFrame, boneRefDic, keyFrameValuesParentDictionary);
                    }

                    keyFrameList.Add(keyFrame);
                }

                HandleUnreferencedTimelinekeys(animation, mainline, keyFrameList, persistentScaledSprites, spriterObject, textures, keyFrameValuesParentDictionary, persistentBones, boneRefDic);

                // find all the keyframevalues, and look up the bone id, then take that bone id and 
                // set the parent in the keyframevalues variable to the ScaledPositionedObject in the boneRefDic
                foreach (var pair in keyFrameList.SelectMany(keyFrame => keyFrame.Values))
                {
                    if (keyFrameValuesParentDictionary.ContainsKey(pair.Value))
                    {
                        int boneId = keyFrameValuesParentDictionary[pair.Value];
                        var parent = boneRefDic[boneId];
                        pair.Value.Parent = parent;
                    }
                    else if (pair.Key.GetType() != typeof(ScaledSprite))
                    {
                        pair.Value.Parent = spriterObject;
                    }
                }

                var SpriterObjectAnimation = new SpriterObjectAnimation(animation.Name,
                                                                        animation.Looping, animation.Length / 1000.0f,
                                                                        keyFrameList);
                spriterObject.Animations[animation.Name] = SpriterObjectAnimation;
            }
            return spriterObject;
        }

        private void HandleUnreferencedTimelinekeys(SpriterDataEntityAnimation animation, SpriterDataEntityAnimationMainline mainline,
                                         List<KeyFrame> keyFrameList, IDictionary<int, ScaledSprite> persistentScaledSprites, SpriterObject SpriterObject,
                                         IDictionary<string, Texture2D> textures, IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary,
                                         IDictionary<int, ScaledPositionedObject> persistentBones, IDictionary<int, ScaledPositionedObject> boneRefDic)
        {
            foreach (var timeline in animation.Timeline)
            {
                foreach (var timelineKey in timeline.Key)
                {
                    // if timeline key has an object, and no mainline keys for objects reference this key
                    if (timelineKey.Object != null &&
                        !mainline.Keys.Where(k => k.ObjectRef != null)
                                 .Any(k => k.ObjectRef.Any(r => r.Key == timelineKey.Id && r.Timeline == timeline.Id)) ||
                        timelineKey.Bone != null &&
                        !mainline.Keys.Where(k => k.BoneRef != null)
                                 .Any(k => k.BoneRef.Any(r => r.Key == timelineKey.Id && r.Timeline == timeline.Id)))
                    {
                        int index = keyFrameList.FindLastIndex(kf => Math.Abs(kf.Time - (timelineKey.Time/1000.0f)) < .0001f);
                        if (index > 0)
                        {
                            var keyFrame = new KeyFrame {Time = timelineKey.Time/1000.0f};
                            var mainlineKey = mainline.Keys.Single(k => k.Time == timelineKey.Time);
                            if (mainlineKey.ObjectRef != null && timelineKey.Object != null)
                            {
                                CreateRuntimeObjectsForSpriterObjectRef(mainlineKey, persistentScaledSprites,
                                                                        SpriterObject, animation, textures, keyFrame,
                                                                        keyFrameValuesParentDictionary, timelineKey);
                            }

                            if (mainlineKey.BoneRef != null && timelineKey.Bone != null)
                            {
                                CreateRuntimeObjectsForSpriterBoneRef(mainlineKey, persistentBones, SpriterObject,
                                                                      animation, keyFrame, boneRefDic,
                                                                      keyFrameValuesParentDictionary, timelineKey);
                            }
                            keyFrameList.Insert(index, keyFrame);
                        }
                    }
                }
            }
        }

        private static void CreateRuntimeObjectsForSpriterBoneRef(Key key, IDictionary<int, ScaledPositionedObject> persistentBones,
                                                                  SpriterObject SpriterObject,
                                                                  SpriterDataEntityAnimation animation, KeyFrame keyFrame,
                                                                  IDictionary<int, ScaledPositionedObject> boneRefDic, IDictionary<KeyFrameValues, int> boneRefParentDic,
                                                                  Key timelineKeyOverride = null)
        {
            IDictionary<int, KeyBone> bones = new Dictionary<int, KeyBone>();

            foreach (var boneRef in key.BoneRef)
            {
                ScaledPositionedObject bone;
                if (persistentBones.ContainsKey(boneRef.Id))
                {
                    bone = persistentBones[boneRef.Id];
                }
                else
                {
                    bone = new ScaledPositionedObject() {Name = "bone" + boneRef.Id};
                    bone.AttachTo(SpriterObject, true);

                    persistentBones[boneRef.Id] = bone;
                    SpriterObject.ObjectList.Add(bone);
                }

                var timeline = animation.Timeline.Single(t => t.Id == boneRef.Timeline);
                var timelineKey = timelineKeyOverride ?? timeline.Key.Single(k => k.Id == boneRef.Key);
                if (timelineKeyOverride == null && key.Time != timelineKey.Time)
                {
                    var nextTimelineKey = timeline.Key.FirstOrDefault(k => k.Id == boneRef.Key + 1);
                    if (nextTimelineKey != null)
                    {
                        timelineKey = InterpolateToNewTimelineKey(key.Time, timelineKey, nextTimelineKey);
                    }
                }

                var timelineKeyBone = new KeyBone(timelineKey.Bone);

                bones[boneRef.Id] = timelineKeyBone;

                keyFrame.Values[bone] = new KeyFrameValues
                    {
                        RelativePosition = new Vector3(timelineKeyBone.X, timelineKeyBone.Y, 0.0f),
                        RelativeRotation = new Vector3(0.0f, 0.0f, timelineKeyBone.Angle),
                        RelativeScaleX = timelineKeyBone.ScaleX,
                        RelativeScaleY = timelineKeyBone.ScaleY,
                        Spin = timelineKey.Spin
                    };

                boneRefDic[boneRef.Id] = bone;
                if (boneRef.Parent.HasValue)
                {
                    boneRefParentDic[keyFrame.Values[bone]] = boneRef.Parent.Value;
                }
            }
        }
        public static float GetPercentageIntoFrame(int milliSecondsIn, int currentKeyFrameTime, int nextKeyFrameTime)
        {

            float retVal = (milliSecondsIn - currentKeyFrameTime) / (float)(nextKeyFrameTime - currentKeyFrameTime);
            if (float.IsInfinity(retVal) || float.IsNaN(retVal))
            {
                return 0.0f;
            }
            else
            {
                return retVal;
            }
        }

        private static Key InterpolateToNewTimelineKey(int time, Key timelineKey, Key nextTimelineKey)
        {
            var percent = GetPercentageIntoFrame(time, timelineKey.Time, nextTimelineKey.Time);
            return new Key
            {
                Bone = timelineKey.Bone == null
                    ? null
                    : new KeyBone
                    {
                        Angle = MathHelper.Lerp(timelineKey.Bone.Angle, nextTimelineKey.Bone.Angle, percent),
                        ScaleX = MathHelper.Lerp(timelineKey.Bone.ScaleX, nextTimelineKey.Bone.ScaleX, percent),
                        ScaleY = MathHelper.Lerp(timelineKey.Bone.ScaleY, nextTimelineKey.Bone.ScaleY, percent),
                        X = MathHelper.Lerp(timelineKey.Bone.X, nextTimelineKey.Bone.X, percent),
                        Y = MathHelper.Lerp(timelineKey.Bone.Y, nextTimelineKey.Bone.Y, percent)
                    },
                Spin = timelineKey.Spin,
                Object = timelineKey.Object == null
                    ? null
                    : new KeyObject
                    {
                        Alpha = MathHelper.Lerp(timelineKey.Object.Alpha, nextTimelineKey.Object.Alpha, percent),
                        Angle = MathHelper.Lerp(timelineKey.Object.Alpha, nextTimelineKey.Object.Alpha, percent),
                        File = timelineKey.Object.File,
                        Folder = timelineKey.Object.Folder,
                        PivotX = !timelineKey.Object.PivotX.HasValue || !nextTimelineKey.Object.PivotX.HasValue ? 
                            (float?)null : MathHelper.Lerp(timelineKey.Object.PivotX.Value, nextTimelineKey.Object.PivotX.Value, percent),
                        PivotY = !timelineKey.Object.PivotY.HasValue || !nextTimelineKey.Object.PivotY.HasValue ?
                            (float?)null : MathHelper.Lerp(timelineKey.Object.PivotY.Value, nextTimelineKey.Object.PivotY.Value, percent),
                        ScaleX = MathHelper.Lerp(timelineKey.Object.ScaleX, nextTimelineKey.Object.ScaleX, percent),
                        ScaleY = MathHelper.Lerp(timelineKey.Object.ScaleY, nextTimelineKey.Object.ScaleY, percent),
                        X = MathHelper.Lerp(timelineKey.Object.X, nextTimelineKey.Object.X, percent),
                        Y = MathHelper.Lerp(timelineKey.Object.Y, nextTimelineKey.Object.Y, percent)
                    },
                Time = time
            };
        }


        private void CreateRuntimeObjectsForSpriterObjectRef(Key key, IDictionary<int, ScaledSprite> persistentScaledSprites, SpriterObject SpriterObject,
                                                             SpriterDataEntityAnimation animation, IDictionary<string, Texture2D> textures,
                                                             KeyFrame keyFrame, IDictionary<KeyFrameValues, int> SpriterefParentDic,
                                                             Key timelineKeyOverride = null)
        {
            foreach (var objectRef in key.ObjectRef)
            {
                ScaledSprite ScaledSprite;
                ScaledPositionedObject pivot;

                var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                Key timelineKey = timelineKeyOverride ?? timeline.Key.Single(k => k.Id == objectRef.Key);
                var folderFileId = string.Format("{0}_{1}", timelineKey.Object.Folder,
                                                 timelineKey.Object.File);
                var file =
                    this.Folder.First(f => f.Id == timelineKey.Object.Folder)
                        .File.First(f => f.Id == timelineKey.Object.File);


                if (persistentScaledSprites.ContainsKey(objectRef.Id))
                {
                    ScaledSprite = persistentScaledSprites[objectRef.Id];
                    pivot = (ScaledPositionedObject)ScaledSprite.Parent;
                }
                else
                {
                    var name = objectRef.Name ?? objectRef.Id.ToString(CultureInfo.InvariantCulture);
                    pivot = new ScaledPositionedObject { Name = string.Format("{0}_pivot", name) };

                    ScaledSprite = new ScaledSprite {Name = string.Format("{0}_sprite", name), Width = file.Width, Height = file.Height, ParentScaleChangesPosition = false};

                    ScaledSprite.AttachTo(pivot, true);
                    pivot.AttachTo(SpriterObject, true);

                    persistentScaledSprites[objectRef.Id] = ScaledSprite;
                    SpriterObject.ObjectList.Add(ScaledSprite);
                    SpriterObject.ObjectList.Add(pivot);
                }

                var values = GetKeyFrameValues(timelineKey, file, textures, folderFileId, objectRef);

                values.ScaledSprite.Parent = pivot;
                if (objectRef.Parent.HasValue)
                {
                    SpriterefParentDic[values.Pivot] = objectRef.Parent.Value;
                }
                else
                {
                    values.Pivot.Parent = SpriterObject;
                }

                keyFrame.Values[pivot] = values.Pivot;
                keyFrame.Values[ScaledSprite] = values.ScaledSprite;
            }
        }


        public Texture2D LoadTexture(SpriterDataFolderFile file)
        {
            return TextureLoader.FromFile(file.Name);
        }

        private static KeyFramePivotScaledSpriteValues GetKeyFrameValues(Key timelineKey, SpriterDataFolderFile file, IDictionary<string, Texture2D> textures, string folderFileId, KeyObjectRef objectRef)
        {
            var pivotValue = new KeyFrameValues
                {
                    RelativePosition = new Vector3(timelineKey.Object.X,
                                           timelineKey.Object.Y,
                                           0.0f),
                    RelativeRotation = new Vector3
                        {
                            Z = timelineKey.Object.Angle
                        },
                    Spin = timelineKey.Spin
                };
            int width = file.Width;
            int height = file.Height;

            var pivotY = 1.0f;
            var pivotX = 0.0f;

            if (timelineKey.Object.PivotX.HasValue)
            {
                pivotX = timelineKey.Object.PivotX.Value;
            }
            else if (file.PivotX.HasValue)
            {
                pivotX = file.PivotX.Value;
            }

            if (timelineKey.Object.PivotY.HasValue)
            {
                pivotY = timelineKey.Object.PivotY.Value;
            }
            else if (file.PivotY.HasValue)
            {
                pivotY = file.PivotY.Value;
            }

            var ScaledSpriteValue = new KeyFrameValues
                {
                    Texture = textures[folderFileId],
                    RelativeScaleX = timelineKey.Object.ScaleX,
                    RelativeScaleY = timelineKey.Object.ScaleY,
                    RelativePosition = GetSpriteRelativePosition(width, height, pivotX,
                                                         pivotY, objectRef.ZIndex),
                    Alpha = timelineKey.Object.Alpha
                };
            return new KeyFramePivotScaledSpriteValues { Pivot = pivotValue, ScaledSprite = ScaledSpriteValue };
        }

        public static Vector3 GetSpriteRelativePosition(float width, float height, float pivotX, float pivotY, int zIndex)
        {
            return new Vector3(-width * (pivotX - .5f), -height * (pivotY - .5f), zIndex * 0.0001f);
        }
        private class KeyFramePivotScaledSpriteValues
        {
            public KeyFrameValues Pivot { get; set; }
            public KeyFrameValues ScaledSprite { get; set; }
            public KeyFrameValues Bone { get; set; }
        }


        public static SpriterObjectSave FromFile(string filename)
        {
            filename = filename.Replace(@"\", "/");
            var sos = FileManager.XmlDeserialize<SpriterObjectSave>(filename);
            sos.FileName = FileManager.GetDirectory(filename) + filename.Substring(filename.LastIndexOf("/", StringComparison.Ordinal) + 1);
            sos.Directory = FileManager.GetDirectory(filename);

            sos.TextureLoader = new FlatRedBallTextureLoader();

            return sos;
        }
    }
}
