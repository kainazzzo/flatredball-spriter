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
            var spriterObject = new SpriterObject();

            IDictionary<string, string> filenames = new Dictionary<string, string>();
            IDictionary<int, Sprite> persistentSprites = new Dictionary<int, Sprite>();
            foreach (var folder in this.Folder)
            {
                foreach (var file in folder.File)
                {
                    string folderFileId = string.Format("{0}_{1}", folder.Id, file.Id);
                    filenames[folderFileId] = file.Name;
                }
            }


            var animation = Entity[0].Animation[0];
            var mainline = animation.Mainline;
            foreach (var key in mainline.Keys)
            {
                
                var keyFrame = new KeyFrame {Time = key.Time/1000.0};
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
                    }

                    // TODO: tie the sprite to object_ref id?
                    var timeline = animation.Timeline.Single(t => t.Id == objectRef.Timeline);
                    var timelineKey = timeline.Key.Single(k => k.Id == objectRef.Key);
                    string folderFileId = string.Format("{0}_{1}", timelineKey.Object.Folder, timelineKey.Object.File);
                    var filename =
                        filenames[folderFileId];

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
                            ScaleX = timelineKey.Object.ScaleX
                            // TODO: Figure out how to load the texture
                        };
                    // TODO: Z-index

                    keyFrame.Values[sprite] = values;
                    spriterObject.KeyFrameList.Add(keyFrame);
                    
                }
                
            }


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
