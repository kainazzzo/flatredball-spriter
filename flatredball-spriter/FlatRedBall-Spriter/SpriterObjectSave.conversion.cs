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
                    textures[folderFileId] = FlatRedBallServices.Load<Texture2D>(file.Name);
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
                    if (persistentSprites.ContainsKey(objectRef.Id))
                    {
                        sprite = persistentSprites[objectRef.Id];
                    }
                    else
                    {
                        sprite = new Sprite();
                        persistentSprites[objectRef.Id] = sprite;
                        spriterObject.ObjectList.Add(sprite);
                        sprite.AttachTo(spriterObject, true);
                    }

                    // TODO: tie the sprite to object_ref id?
                    var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                    var timelineKey = timeline.Key.Single(k => k.Id == objectRef.Key);
                    string folderFileId = string.Format("{0}_{1}", timelineKey.Object.Folder, timelineKey.Object.File);
                    var filename =
                        filenames[folderFileId];
                    var file =
                        this.Folder.First(f => f.Id == timelineKey.Object.Folder)
                            .File.First(f => f.Id == timelineKey.Object.File);

                    var values = new KeyFrameValues
                        {
                            Position = new Vector3
                                {
                                    X = timelineKey.Object.X,
                                    Y = timelineKey.Object.Y
                                },
                            Rotation = new Vector3
                                {
                                    Z = timelineKey.Object.Angle
                                },
                            ScaleX = (file.Width / 2.0f * timelineKey.Object.ScaleX),
                            ScaleY = (file.Height / 2.0f * timelineKey.Object.ScaleY),
                            Texture = textures[folderFileId]
                        };
                    // TODO: Z-index

                    keyFrame.Values[sprite] = values;
                    spriterObject.KeyFrameList.Add(keyFrame);
                    
                }
                
            }

            var last = spriterObject.KeyFrameList[spriterObject.KeyFrameList.Count - 1];
            spriterObject.KeyFrameList.Add(new KeyFrame
                {
                    Time = this.Entity[0].Animation[0].Length / 1000.0f,
                    Values = last.Values
                });
            return spriterObject;
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
