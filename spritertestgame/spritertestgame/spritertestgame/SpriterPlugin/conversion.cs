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

        public SpriterObjectCollection ToRuntime()
        {
            return new SpriterObjectCollection
            {
                SpriterEntities = Entity.ToDictionary(e => e.Name, CreateSpriterObjectFromEntity)
            };
        }

        private SpriterObject CreateSpriterObjectFromEntity(SpriterDataEntity entity)
        {
            var spriterObject = new SpriterObject(FlatRedBallServices.GlobalContentManager, false);

            IDictionary<string, string> filenames = new Dictionary<string, string>();
            IDictionary<int, ScaledSprite> persistentScaledSprites = new Dictionary<int, ScaledSprite>();
            IDictionary<int, SpriterBone> persistentBones = new Dictionary<int, SpriterBone>();
            IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            IDictionary<string, ObjectInfo> boxes = new Dictionary<string, ObjectInfo>();
            IDictionary<int, ScaledPositionedObject> boneRefDic = new Dictionary<int, ScaledPositionedObject>();
            IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary = new Dictionary<KeyFrameValues, int>();
            IDictionary<int, ScaledPolygon> persistentScaledPolygons = new Dictionary<int, ScaledPolygon>();
            IDictionary<int, SpriterPoint> persistentPoints = new Dictionary<int, SpriterPoint>();

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


            if (entity.ObjectInfos != null)
            {
                boxes =
                    entity.ObjectInfos.Where(oi => oi.Type == "box")
                        .ToDictionary(s => s.Name);
            }

            foreach (var animation in entity.Animation)
            {
                var mainline = animation.Mainline;
                var keyFrameList = new List<KeyFrame>();

                foreach (var key in mainline.Keys)
                {
                    var keyFrame = new KeyFrame {Time = key.Time/1000.0f};

                    // If it's a ScaledSprite (not a bone)
                    if (key.ObjectRef != null)
                    {
                        CreateRuntimeObjectsForSpriterObjectRef(key, persistentScaledSprites, spriterObject, animation, textures,
                            keyFrame, keyFrameValuesParentDictionary, boxes, persistentScaledPolygons, persistentPoints);
                    }

                    // If it's a bone (not a ScaledSprite)
                    if (key.BoneRef != null)
                    {
                        CreateRuntimeObjectsForSpriterBoneRef(key, persistentBones, spriterObject, animation, keyFrame,
                            boneRefDic, keyFrameValuesParentDictionary, entity);
                    }

                    keyFrameList.Add(keyFrame);
                }

                HandleUnreferencedTimelinekeys(animation, mainline, keyFrameList, persistentScaledSprites, spriterObject,
                    textures, keyFrameValuesParentDictionary, persistentBones, boneRefDic, boxes, persistentScaledPolygons,
                    persistentPoints, entity);

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
                    else if (pair.Key.GetType() != typeof (ScaledSprite) && pair.Key.GetType() != typeof (ScaledPolygon))
                    {
                        pair.Value.Parent = spriterObject;
                    }
                }

                var SpriterObjectAnimation = new SpriterObjectAnimation(animation.Name,
                    animation.Looping, animation.Length/1000.0f,
                    keyFrameList);
                spriterObject.Animations[animation.Name] = SpriterObjectAnimation;
            }
            return spriterObject;
        }

        private void HandleUnreferencedTimelinekeys(SpriterDataEntityAnimation animation, SpriterDataEntityAnimationMainline mainline, List<KeyFrame> keyFrameList, IDictionary<int, ScaledSprite> persistentScaledSprites, SpriterObject SpriterObject, IDictionary<string, Texture2D> textures, IDictionary<KeyFrameValues, int> keyFrameValuesParentDictionary, IDictionary<int, SpriterBone> persistentBones, IDictionary<int, ScaledPositionedObject> boneRefDic, IDictionary<string, ObjectInfo> boxes, IDictionary<int, ScaledPolygon> persistentScaledPolygons, IDictionary<int, SpriterPoint> persistentPoints, SpriterDataEntity entity)
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
                                                                        keyFrameValuesParentDictionary, boxes, persistentScaledPolygons, persistentPoints, timelineKeyOverride: timelineKey);
                            }


                            if (mainlineKey.BoneRef != null && timelineKey.Bone != null)
                            {
                                CreateRuntimeObjectsForSpriterBoneRef(mainlineKey, persistentBones, SpriterObject,
                                                                      animation, keyFrame, boneRefDic,
                                                                      keyFrameValuesParentDictionary, entity, timelineKey);
                            }
                            keyFrameList.Insert(index, keyFrame);
                        }
                    }
                }
            }
        }

        private static void CreateRuntimeObjectsForSpriterBoneRef(Key key, IDictionary<int, SpriterBone> persistentBones,
                                                                  SpriterObject SpriterObject,
                                                                  SpriterDataEntityAnimation animation, KeyFrame keyFrame,
                                                                  IDictionary<int, ScaledPositionedObject> boneRefDic, IDictionary<KeyFrameValues, int> boneRefParentDic, SpriterDataEntity entity,
                                                                  Key timelineKeyOverride = null)
        {
            IDictionary<int, KeyBone> bones = new Dictionary<int, KeyBone>();

            foreach (var boneRef in key.BoneRef)
            {
                SpriterBone bone;
                var timeline = animation.Timeline.Single(t => t.Id == boneRef.Timeline);

                if (persistentBones.ContainsKey(boneRef.Id))
                {
                    bone = persistentBones[boneRef.Id];
                }
                else
                {
                    var objectInfo = entity.ObjectInfos == null ? (ObjectInfo)null : entity.ObjectInfos.FirstOrDefault(o => o.Type == "bone" && o.Name == timeline.Name);
                    bone = new SpriterBone
                    {
                        Name = timeline.Name,
                        Length = objectInfo == null ? 200 : objectInfo.Width
                    };

                    bone.AttachTo(SpriterObject, true);

                    persistentBones[boneRef.Id] = bone;
                    SpriterObject.ObjectList.Add(bone);
                }

                var timelineKey = timelineKeyOverride ?? timeline.Key.Single(k => k.Id == boneRef.Key);
                if (timelineKeyOverride == null && key.Time != timelineKey.Time)
                {
                    var nextTimelineKey = timeline.Key.FirstOrDefault(k => k.Time > key.Time) ?? new Key(timeline.Key.First()) { Time = animation.Length };

                    timelineKey = InterpolateToNewTimelineKey(key, timelineKey, nextTimelineKey);
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

        private static Key InterpolateToNewTimelineKey(Key mainlineKey, Key timelineKey, Key nextTimelineKey)
        {
            
            var time = mainlineKey.Time;
            var percent = GetPercentageIntoFrame(time, timelineKey.Time, nextTimelineKey.Time);
            return new Key
            {
                Bone = timelineKey.Bone == null
                    ? null
                    : new KeyBone
                    {
                        Angle = LinearAngle(timelineKey.Bone.Angle, nextTimelineKey.Bone.Angle, timelineKey.Spin, percent),
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
                        Angle = LinearAngle(timelineKey.Object.Angle, nextTimelineKey.Object.Angle, timelineKey.Spin, percent),
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

        private static float LinearAngle(float angleA, float angleB, int spin, float percent)
        {
            if (spin == 0)
            {
                return angleA;
            }
            if (spin > 0)
            {
                if ((angleB - angleA) < 0)
                {
                    angleB += 360;
                }
            }
            else if (spin < 0)
            {
                if ((angleB - angleA) > 0)
                {
                    angleB -= 360;
                }
            }

            return MathHelper.Lerp(angleA, angleB, percent);
        }


        private void CreateRuntimeObjectsForSpriterObjectRef(Key key, IDictionary<int, ScaledSprite> persistentScaledSprites, SpriterObject SpriterObject, SpriterDataEntityAnimation animation, IDictionary<string, Texture2D> textures, KeyFrame keyFrame, IDictionary<KeyFrameValues, int> SpriterefParentDic, IDictionary<string, ObjectInfo> boxes, IDictionary<int, ScaledPolygon> persistentScaledPolygons, IDictionary<int, SpriterPoint> persistentPoints, Key timelineKeyOverride = null)
        {
            foreach (var objectRef in key.ObjectRef)
            {
                var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                Key timelineKey = timelineKeyOverride ?? timeline.Key.Single(k => k.Id == objectRef.Key);
                if (timelineKeyOverride == null && key.Time != timelineKey.Time)
                {
                    var nextTimelineKey = timeline.Key.FirstOrDefault(k => k.Time > key.Time) ??
                                          new Key(timeline.Key.First()) { Time = animation.Length };

                    timelineKey = InterpolateToNewTimelineKey(key, timelineKey, nextTimelineKey);
                }

                if (timeline.ObjectType == "box")
                {
                    ScaledPolygon scaledPolygon;
                    ScaledPositionedObject pivot;
                    var box = boxes[timeline.Name];

                    if (persistentScaledPolygons.ContainsKey(objectRef.Id))
                    {
                        scaledPolygon = persistentScaledPolygons[objectRef.Id];
                        pivot = (ScaledPositionedObject)scaledPolygon.Parent;
                    }
                    else
                    {
                        scaledPolygon = ScaledPolygon.CreateRectangle(timelineKey.Object.X, timelineKey.Object.Y, (int)box.Width, (int)box.Height);
                        scaledPolygon.ParentScaleChangesPosition = false;
                        scaledPolygon.Visible = false;

                        var name = objectRef.Name ?? objectRef.Id.ToString(CultureInfo.InvariantCulture);
                        pivot = new ScaledPositionedObject { Name = string.Format("{0}_pivot", name) };

                        scaledPolygon.Name = timeline.Name;

                        scaledPolygon.AttachTo(pivot, true);
                        pivot.AttachTo(SpriterObject, true);

                        persistentScaledPolygons[objectRef.Id] = scaledPolygon;
                        SpriterObject.ObjectList.Add(scaledPolygon);
                        SpriterObject.ObjectList.Add(pivot);
                    }

                    var values = GetKeyFrameValues(timelineKey, box, objectRef);

                    values.ScaledPolygon.Parent = pivot;
                    if (objectRef.Parent.HasValue)
                    {
                        SpriterefParentDic[values.Pivot] = objectRef.Parent.Value;
                    }
                    else
                    {
                        values.Pivot.Parent = SpriterObject;
                    }

                    keyFrame.Values[pivot] = values.Pivot;
                    keyFrame.Values[scaledPolygon] = values.ScaledPolygon;
                }
                else if (timeline.ObjectType == "point")
                {
                    SpriterPoint point;


                    if (persistentPoints.ContainsKey(objectRef.Id))
                    {
                        point = persistentPoints[objectRef.Id];
                    }
                    else
                    {
                        point = new SpriterPoint
                        {
                            X = timelineKey.Object.X,
                            Y = timelineKey.Object.Y,
                            Name = timeline.Name
                        };
                        SpriterObject.ObjectList.Add(point);
                        persistentPoints[objectRef.Id] = point;
                    }

                    KeyFrameValues values = GetKeyFrameValuesForPoint(timelineKey, objectRef);

                    if (objectRef.Parent.HasValue)
                    {
                        SpriterefParentDic[values] = objectRef.Parent.Value;
                    }
                    else
                    {
                        values.Parent = SpriterObject;
                    }

                    keyFrame.Values[point] = values;
                }
                else if (string.IsNullOrEmpty(timeline.ObjectType))
                {
                    var folderFileId = string.Format("{0}_{1}", timelineKey.Object.Folder,
                        timelineKey.Object.File);
                    var file =
                        this.Folder.First(f => f.Id == timelineKey.Object.Folder)
                            .File.First(f => f.Id == timelineKey.Object.File);

                    ScaledSprite scaledSprite;
                    ScaledPositionedObject pivot;
                    if (persistentScaledSprites.ContainsKey(objectRef.Id))
                    {
                        scaledSprite = persistentScaledSprites[objectRef.Id];
                        pivot = (ScaledPositionedObject) scaledSprite.Parent;
                    }
                    else
                    {
                        var name = objectRef.Name ?? objectRef.Id.ToString(CultureInfo.InvariantCulture);
                        pivot = new ScaledPositionedObject {Name = string.Format("{0}_pivot", name)};

                        scaledSprite = new ScaledSprite
                        {
                            Name = string.Format("{0}_sprite", name),
                            Width = file.Width,
                            Height = file.Height,
                            ParentScaleChangesPosition = false
                        };

                        scaledSprite.AttachTo(pivot, true);
                        pivot.AttachTo(SpriterObject, true);

                        persistentScaledSprites[objectRef.Id] = scaledSprite;
                        SpriterObject.ObjectList.Add(scaledSprite);
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
                    keyFrame.Values[scaledSprite] = values.ScaledSprite;
                }
            }
        }

        

        public Texture2D LoadTexture(SpriterDataFolderFile file)
        {
            return TextureLoader.FromFile(file.Name);
        }

        private KeyFrameValues GetKeyFrameValuesForPoint(Key timelineKey, KeyObjectRef objectRef)
        {
            return new KeyFrameValues
            {
                RelativePosition = new Vector3(timelineKey.Object.X, timelineKey.Object.Y, 0.0f),
                RelativeRotation = new Vector3
                {
                    Z = timelineKey.Object.Angle
                },
                Spin = timelineKey.Spin
            };
        }

        private KeyFramePivotScaledValues GetKeyFrameValues(Key timelineKey, ObjectInfo box, KeyObjectRef objectRef)
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
            var width = (int)box.Width;
            var height = (int)box.Height;

            float pivotX = timelineKey.Object.PivotX.HasValue ? timelineKey.Object.PivotX.Value : box.PivotX;

            float pivotY = timelineKey.Object.PivotY.HasValue ? timelineKey.Object.PivotY.Value : box.PivotY;

            var scaledPolygonValue = new KeyFrameValues
                {
                    RelativeScaleX = timelineKey.Object.ScaleX,
                    RelativeScaleY = timelineKey.Object.ScaleY,
                    RelativePosition = GetPivotedRelativePosition(width, height, pivotX,
                                                         pivotY, objectRef.ZIndex)
                };
            return new KeyFramePivotScaledValues { Pivot = pivotValue, ScaledPolygon = scaledPolygonValue };
        }

        private static KeyFramePivotScaledValues GetKeyFrameValues(Key timelineKey, SpriterDataFolderFile file, IDictionary<string, Texture2D> textures, string folderFileId, KeyObjectRef objectRef)
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
                    RelativePosition = GetPivotedRelativePosition(width, height, pivotX,
                                                         pivotY, objectRef.ZIndex),
                    Alpha = timelineKey.Object.Alpha
                };
            return new KeyFramePivotScaledValues { Pivot = pivotValue, ScaledSprite = ScaledSpriteValue };
        }

        public static Vector3 GetPivotedRelativePosition(float width, float height, float pivotX, float pivotY, int zIndex)
        {
            return new Vector3(-width * (pivotX - .5f), -height * (pivotY - .5f), zIndex * 0.0001f);
        }
        private class KeyFramePivotScaledValues
        {
            public KeyFrameValues Pivot { get; set; }
            public KeyFrameValues ScaledSprite { get; set; }
            public KeyFrameValues ScaledPolygon { get; set; }
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

    public partial class Key
    {
        public Key()
        {
        }

        public Key(Key that)
        {
            if (that.Bone != null)
            {
                Bone = new KeyBone
                {
                    Angle = that.Bone.Angle,
                    ScaleX = that.Bone.ScaleX,
                    ScaleY = that.Bone.ScaleY,
                    X = that.Bone.X,
                    Y = that.Bone.Y
                };
            }

            if (that.BoneRef != null)
            {
                BoneRef = new List<KeyBoneRef>();
                foreach (var keyBoneRef in that.BoneRef)
                {
                    BoneRef.Add(new KeyBoneRef
                    {
                        Id = keyBoneRef.Id,
                        Key = keyBoneRef.Key,
                        Parent = keyBoneRef.Parent,
                        Timeline = keyBoneRef.Timeline
                    });
                }
            }

            Id = that.Id;

            if (that.Object != null)
            {
                this.Object = new KeyObject
                {
                    Alpha = that.Object.Alpha,
                    Angle = that.Object.Angle,
                    File = that.Object.File,
                    Folder = that.Object.Folder,
                    PivotX = that.Object.PivotX,
                    PivotY = that.Object.PivotY,
                    ScaleX = that.Object.ScaleX,
                    ScaleY = that.Object.ScaleY,
                    X = that.Object.X,
                    Y = that.Object.Y
                };
            }

            if (that.ObjectRef != null)
            {
                ObjectRef = new List<KeyObjectRef>();
                foreach (var keyObjectRef in that.ObjectRef)
                {
                    ObjectRef.Add(new KeyObjectRef
                    {
                        Id = keyObjectRef.Id,
                        Key = keyObjectRef.Key,
                        Name = keyObjectRef.Name,
                        Parent = keyObjectRef.Parent,
                        Timeline = keyObjectRef.Timeline,
                        ZIndex = keyObjectRef.ZIndex
                    });
                }
            }

            Spin = that.Spin;
            Time = that.Time;
        }
    }
}
