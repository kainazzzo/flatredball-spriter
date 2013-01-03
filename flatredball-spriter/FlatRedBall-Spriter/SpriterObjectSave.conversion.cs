using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.IO;

namespace FlatRedBall_Spriter
{
    public partial class SpriterObjectSave
    {
        public SpriterObject ToRuntime()
        {
            var spriterObject = new SpriterObject();

            // TODO: Add conversion from save to runtime object

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
