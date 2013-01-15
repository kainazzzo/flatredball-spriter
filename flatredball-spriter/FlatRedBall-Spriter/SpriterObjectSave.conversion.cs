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
        public SpriterObject ToRuntime()
        {
            var spriterObject = new SpriterObject(FlatRedBallServices.GlobalContentManager, false);

            IDictionary<string, string> filenames = new Dictionary<string, string>();
            IDictionary<int, Sprite> persistentSprites = new Dictionary<int, Sprite>();
            IDictionary<int, PositionedObject> persistentBones = new Dictionary<int, PositionedObject>();
            IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            IDictionary<int, PositionedObject> boneRefDic = new Dictionary<int, PositionedObject>();
            IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary = new Dictionary<KeyFrameValues, int>();

            foreach (var folder in this.Folder)
            {
                foreach (var file in folder.File)
                {
                    string folderFileId = string.Format("{0}_{1}", folder.Id, file.Id);
                    filenames[folderFileId] = file.Name;
                    textures[folderFileId] = LoadTexture(file);
                }
            }


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

        private static void CreateRuntimeObjectsForSpriterBoneRef(Key key, IDictionary<int, PositionedObject> persistentBones,
                                                                  SpriterObject spriterObject,
                                                                  SpriterDataEntityAnimation animation, KeyFrame keyFrame,
                                                                  IDictionary<int, PositionedObject> boneRefDic, IDictionary<KeyFrameValues, int> boneRefParentDic)
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
                var timelineKey = timeline.Key.Single(k => k.Id == boneRef.Key);

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
                                                             KeyFrame keyFrame, IDictionary<KeyFrameValues, int> spriteRefParentDic)
        {
            foreach (var objectRef in key.ObjectRef)
            {
                Sprite sprite;
                PositionedObject pivot;

                var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                var timelineKey = timeline.Key.Single(k => k.Id == objectRef.Key);
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


                // TODO: tie the sprite to object_ref id?
                

                

                var values = GetKeyFrameValues(timelineKey, file, textures, folderFileId, objectRef.ZIndex);
                // TODO: Z-index
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
                                                         timelineKey.Object.PivotY, zIndex)
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
            string oldRelativeDirectory = FileManager.RelativeDirectory;
            FileManager.RelativeDirectory = FileManager.GetDirectory(filename);

            var sos = FileManager.XmlDeserialize<SpriterObjectSave>(filename);
            FileManager.RelativeDirectory = oldRelativeDirectory;

            return sos;
        }
    }
}
