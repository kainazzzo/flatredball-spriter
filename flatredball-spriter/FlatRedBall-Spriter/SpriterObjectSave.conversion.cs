﻿using System;
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
            IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

            foreach (var folder in this.Folder)
            {
                foreach (var file in folder.File)
                {
                    string folderFileId = string.Format("{0}_{1}", folder.Id, file.Id);
                    filenames[folderFileId] = file.Name;
                    textures[folderFileId] = LoadTexture(file);
                }
            }


            var animation = Entity[0].Animation[0];
            var mainline = animation.Mainline;
            foreach (var key in mainline.Keys)
            {
                
                var keyFrame = new KeyFrame {Time = key.Time/1000.0f};
                foreach (var objectRef in key.ObjectRef)
                {
                    Sprite sprite;
                    PositionedObject pivot;
                    if (persistentSprites.ContainsKey(objectRef.Id))
                    {
                        sprite = persistentSprites[objectRef.Id];
                        pivot = sprite.Parent;
                    }
                    else
                    {
                        pivot = new PositionedObject {Name = "pivot"};

                        sprite = new Sprite {Name = "sprite", PixelSize = .5f};

                        sprite.AttachTo(pivot, true);
                        pivot.AttachTo(spriterObject, true);

                        persistentSprites[objectRef.Id] = sprite;
                        spriterObject.ObjectList.Add(sprite);
                        spriterObject.ObjectList.Add(pivot);
                    }

                    spriterObject.Looping = animation.Looping;
                    spriterObject.AnimationTotalTime = animation.Length / 1000.0f;

                    // TODO: tie the sprite to object_ref id?
                    var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                    var timelineKey = timeline.Key.Single(k => k.Id == objectRef.Key);
                    var folderFileId = string.Format("{0}_{1}", timelineKey.Object.Folder, timelineKey.Object.File);

                    var file =
                        this.Folder.First(f => f.Id == timelineKey.Object.Folder)
                            .File.First(f => f.Id == timelineKey.Object.File);

                    var values = GetKeyFrameValues(timelineKey, file, textures, folderFileId);
                    // TODO: Z-index

                    keyFrame.Values[pivot] = values.Pivot;
                    keyFrame.Values[sprite] = values.Sprite;
                    spriterObject.KeyFrameList.Add(keyFrame);
                }
                
            }

            return spriterObject;
        }

        public virtual Texture2D LoadTexture(SpriterDataFolderFile file)
        {
            return FlatRedBallServices.Load<Texture2D>(file.Name);
        }

        private static KeyFramePivotSpriteValues GetKeyFrameValues(Key timelineKey, SpriterDataFolderFile file,
            IDictionary<string, Texture2D> textures, string folderFileId)
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

            var spriteValue = new KeyFrameValues
                {
                    Texture = textures[folderFileId],
                    ScaleX = (file.Width / 2.0f * timelineKey.Object.ScaleX),
                    ScaleY = (file.Height / 2.0f * timelineKey.Object.ScaleY),
                    Position = GetSpriteRelativePosition(file.Width, file.Height, timelineKey.Object.PivotX,
                        timelineKey.Object.PivotY)
                };
            return new KeyFramePivotSpriteValues { Pivot = pivotValue, Sprite = spriteValue };
        }

        public static Vector3 GetSpriteRelativePosition(float width, float height, float pivotX, float pivotY)
        {
            return new Vector3(-width * (pivotX - .5f), -height * (pivotY - .5f), 0f);
        }
        private class KeyFramePivotSpriteValues
        {
            public KeyFrameValues Pivot { get; set; }
            public KeyFrameValues Sprite { get; set; }
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
