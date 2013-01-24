using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatRedBall_Spriter
{
    public partial class SpriterObjectSave
    {
        public virtual ITextureLoader TextureLoader
        {
            get { return new FlatRedBallTextureLoader(); }
        }

        public string FileName { get; set; }

        public SpriterObject ToRuntime()
        {
            var spriterObject = new SpriterObject(FlatRedBallServices.GlobalContentManager, false);

            IDictionary<string, string> filenames = new Dictionary<string, string>();
            IDictionary<int, Sprite> persistentSprites = new Dictionary<int, Sprite>();
            IDictionary<int, PositionedObject> persistentBones = new Dictionary<int, PositionedObject>();
            IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            IDictionary<int, PositionedObject> boneRefDic = new Dictionary<int, PositionedObject>();
            IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary = new Dictionary<KeyFrameValues, int>();

            string oldDir = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory = FileManager.GetDirectory(this.FileName);
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
                    if (key.ObjectRef != null)
                    {
                        CreateRuntimeObjectsForSpriterObjectRef(key, persistentSprites, spriterObject, animation, textures, keyFrame, keyFrameValuesParentDictionary);
                    }
                    
                    if (key.BoneRef != null)
                    {
                        CreateRuntimeObjectsForSpriterBoneRef(key, persistentBones, spriterObject, animation, keyFrame, boneRefDic, keyFrameValuesParentDictionary);
                    }

                    keyFrameList.Add(keyFrame);
                }

                HandleUnreferencedTimelinekeys(animation, mainline, keyFrameList, persistentSprites, spriterObject, textures, keyFrameValuesParentDictionary, persistentBones, boneRefDic);

                // find all the keyframevalues, and look up the bone id, then take that bone id and 
                // set the parent in the keyframevalues variable to the positionedobject in the boneRefDic
                foreach (var pair in keyFrameList.SelectMany(keyFrame => keyFrame.Values))
                {
                    if (keyFrameValuesParentDictionary.ContainsKey(pair.Value))
                    {
                        int boneId = keyFrameValuesParentDictionary[pair.Value];
                        var parent = boneRefDic[boneId];
                        pair.Value.Parent = parent;
                    }
                    else if (pair.Key.GetType() != typeof(Sprite))
                    {
                        pair.Value.Parent = spriterObject;
                    }
                }

                var spriterObjectAnimation = new SpriterObjectAnimation(animation.Name,
                                                                        animation.Looping, animation.Length / 1000.0f,
                                                                        keyFrameList);
                spriterObject.Animations[animation.Name] = spriterObjectAnimation;
            }
            return spriterObject;
        }

        private void HandleUnreferencedTimelinekeys(SpriterDataEntityAnimation animation, SpriterDataEntityAnimationMainline mainline,
                                         List<KeyFrame> keyFrameList, IDictionary<int, Sprite> persistentSprites, SpriterObject spriterObject,
                                         IDictionary<string, Texture2D> textures, IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary,
                                         IDictionary<int, PositionedObject> persistentBones, IDictionary<int, PositionedObject> boneRefDic)
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
                                CreateRuntimeObjectsForSpriterObjectRef(mainlineKey, persistentSprites,
                                                                        spriterObject, animation, textures, keyFrame,
                                                                        keyFrameValuesParentDictionary, timelineKey);
                            }

                            if (mainlineKey.BoneRef != null && timelineKey.Bone != null)
                            {
                                CreateRuntimeObjectsForSpriterBoneRef(mainlineKey, persistentBones, spriterObject,
                                                                      animation, keyFrame, boneRefDic,
                                                                      keyFrameValuesParentDictionary, timelineKey);
                            }
                            keyFrameList.Insert(index, keyFrame);
                        }
                    }
                }
            }
        }

        private static void CreateRuntimeObjectsForSpriterBoneRef(Key key, IDictionary<int, PositionedObject> persistentBones,
                                                                  SpriterObject spriterObject,
                                                                  SpriterDataEntityAnimation animation, KeyFrame keyFrame,
                                                                  IDictionary<int, PositionedObject> boneRefDic, IDictionary<KeyFrameValues, int> boneRefParentDic,
                                                                  Key timelineKeyOverride = null)
        {
            foreach (var boneRef in key.BoneRef)
            {
                PositionedObject bone;
                if (persistentBones.ContainsKey(boneRef.Id))
                {
                    bone = persistentBones[boneRef.Id];
                }
                else
                {
                    bone = new PositionedObject() {Name = "bone" + boneRef.Id};
                    bone.AttachTo(spriterObject, true);

                    persistentBones[boneRef.Id] = bone;
                    spriterObject.ObjectList.Add(bone);
                }

                var timeline = animation.Timeline.Single(t => t.Id == boneRef.Timeline);
                var timelineKey = timelineKeyOverride ?? timeline.Key.Single(k => k.Id == boneRef.Key);

                keyFrame.Values[bone] = new KeyFrameValues
                    {
                        Position = new Vector3(timelineKey.Bone.X / timelineKey.Bone.ScaleX,
                            timelineKey.Bone.Y / timelineKey.Bone.ScaleY, 0.0f),
                        Rotation = new Vector3(0.0f, 0.0f, timelineKey.Bone.Angle),
                        Spin = timelineKey.Spin
                    };

                boneRefDic[boneRef.Id] = bone;
                if (boneRef.Parent.HasValue)
                {
                    boneRefParentDic[keyFrame.Values[bone]] = boneRef.Parent.Value;
                }
            }
        }

        

        private void CreateRuntimeObjectsForSpriterObjectRef(Key key, IDictionary<int, Sprite> persistentSprites, SpriterObject spriterObject,
                                                             SpriterDataEntityAnimation animation, IDictionary<string, Texture2D> textures,
                                                             KeyFrame keyFrame, IDictionary<KeyFrameValues, int> spriteRefParentDic,
                                                             Key timelineKeyOverride = null)
        {
            foreach (var objectRef in key.ObjectRef)
            {
                Sprite sprite;
                PositionedObject pivot;

                var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                Key timelineKey = timelineKeyOverride ?? timeline.Key.Single(k => k.Id == objectRef.Key);
                var folderFileId = string.Format("{0}_{1}", timelineKey.Object.Folder,
                                                 timelineKey.Object.File);
                var file =
                    this.Folder.First(f => f.Id == timelineKey.Object.Folder)
                        .File.First(f => f.Id == timelineKey.Object.File);


                if (persistentSprites.ContainsKey(objectRef.Id))
                {
                    sprite = persistentSprites[objectRef.Id];
                    pivot = sprite.Parent;
                }
                else
                {
                    pivot = new PositionedObject {Name = "pivot"};

                    sprite = new Sprite {Name = "sprite", ScaleX =  file.Width / 2.0f, ScaleY = file.Height / 2.0f};

                    sprite.AttachTo(pivot, true);
                    pivot.AttachTo(spriterObject, true);

                    persistentSprites[objectRef.Id] = sprite;
                    spriterObject.ObjectList.Add(sprite);
                    spriterObject.ObjectList.Add(pivot);
                }

                var values = GetKeyFrameValues(timelineKey, file, textures, folderFileId, objectRef.ZIndex);

                values.Sprite.Parent = pivot;
                if (objectRef.Parent.HasValue)
                {
                    spriteRefParentDic[values.Pivot] = objectRef.Parent.Value;
                    CalculateRelativeScaleValues(values, objectRef, animation, key);
                }
                else
                {
                    values.Pivot.Parent = spriterObject;
                }

                keyFrame.Values[pivot] = values.Pivot;
                keyFrame.Values[sprite] = values.Sprite;
            }
        }

        private void CalculateRelativeScaleValues(KeyFramePivotSpriteValues keyFrameValues, KeyObjectRef objectRef, SpriterDataEntityAnimation animation, Key currentKey)
        {
            if (objectRef.Parent.HasValue)
            {
                // Find the bone reference in the current key
                var boneRef = currentKey.BoneRef.SingleOrDefault(br => br.Id == objectRef.Parent.Value);
                while (boneRef != null)
                {
                    KeyBoneRef @ref = boneRef;
                    var bone = animation.Timeline.Where(t => t.Id == @ref.Timeline).SelectMany(t => t.Key).Single(k => k.Id == @ref.Key).Bone;
                    keyFrameValues.Sprite.ScaleX *= bone.ScaleX;
                    keyFrameValues.Sprite.ScaleY *= bone.ScaleY;
                    float x = keyFrameValues.Sprite.Position.X * bone.ScaleX;
                    float y = keyFrameValues.Sprite.Position.Y * bone.ScaleY;
                    keyFrameValues.Sprite.Position = new Vector3(x, y, 0.0f);

                    x = keyFrameValues.Pivot.Position.X*bone.ScaleX;
                    y = keyFrameValues.Pivot.Position.Y*bone.ScaleY;
                    keyFrameValues.Pivot.Position = new Vector3(x, y, 0.0f);

                    boneRef = !boneRef.Parent.HasValue ? null : currentKey.BoneRef.SingleOrDefault(br => br.Id == boneRef.Parent.Value);
                }
            }
        }

        public virtual Texture2D LoadTexture(SpriterDataFolderFile file)
        {
            return FlatRedBallServices.Load<Texture2D>(file.Name);
        }

        private static KeyFramePivotSpriteValues GetKeyFrameValues(Key timelineKey, SpriterDataFolderFile file, IDictionary<string, Texture2D> textures, string folderFileId, int zIndex)
        {
            var pivotValue = new KeyFrameValues
                {
                    Position = new Vector3(timelineKey.Object.X,
                                           timelineKey.Object.Y,
                                           0.0f),
                    Rotation = new Vector3
                        {
                            Z = timelineKey.Object.Angle
                        },
                    Spin = timelineKey.Spin
                };
            int width = file.Width;
            int height = file.Height;

            if ((width == 0 || height == 0) && textures[folderFileId] != null)
            {
                width = textures[folderFileId].Width;
                height = textures[folderFileId].Height;
            }

            var spriteValue = new KeyFrameValues
                {
                    Texture = textures[folderFileId],
                    ScaleX = (width / 2.0f * timelineKey.Object.ScaleX),
                    ScaleY = (height / 2.0f * timelineKey.Object.ScaleY),
                    Position = GetSpriteRelativePosition(width, height, timelineKey.Object.PivotX,
                                                         timelineKey.Object.PivotY, zIndex),
                    Alpha = timelineKey.Object.Alpha
                };
            return new KeyFramePivotSpriteValues { Pivot = pivotValue, Sprite = spriteValue };
        }

        public static Vector3 GetSpriteRelativePosition(float width, float height, float pivotX, float pivotY, int zIndex)
        {
            return new Vector3(-width * (pivotX - .5f), -height * (pivotY - .5f), zIndex * 0.0001f);
        }
        private class KeyFramePivotSpriteValues
        {
            public KeyFrameValues Pivot { get; set; }
            public KeyFrameValues Sprite { get; set; }
            public KeyFrameValues Bone { get; set; }
        }


        public static SpriterObjectSave FromFile(string filename)
        {
            filename = filename.Replace(@"\", "/");
            var sos = FileManager.XmlDeserialize<SpriterObjectSave>(filename);
            sos.FileName = FileManager.GetDirectory(filename) + filename.Substring(filename.LastIndexOf("/") + 1);

            return sos;
        }
    }
}
