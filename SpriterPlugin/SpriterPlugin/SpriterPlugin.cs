using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EditorObjects.Parsing;
using FlatRedBall.Glue.Plugins.ExportedInterfaces;
using FlatRedBall.Glue.Plugins.Interfaces;
using FlatRedBall.Glue.Plugins;
using FlatRedBall.IO;
using FlatRedBall_Spriter;


namespace SpriterPlugin
{
    [Export(typeof(PluginBase))]
    public class SpriterPlugin : PluginBase
    {
        [Import("GlueCommands")]
        public IGlueCommands GlueCommands
        {
            get;
            set;
        }
		
		[Import("GlueState")]
		public IGlueState GlueState
		{
		    get;
		    set;
        }

        public override string FriendlyName
        {
            get { return "Spriter Integration"; }
        }

        public override bool ShutDown(PluginShutDownReason reason)
        {
            // Do anything your plugin needs to do to shut down
            // or don't shut down and return false

            return true;
        }

        public override void StartUp()
        {
            // Do anything your plugin needs to do when it first starts up
            GetFilesReferencedBy += GetFilesReferencedByFunc;
        }

        private void GetFilesReferencedByFunc(string absoluteFileName, TopLevelOrRecursive topLevelOrRecursive, List<string> files)
        {
            if (!string.IsNullOrEmpty(absoluteFileName) && 
                FileManager.GetExtension(absoluteFileName).ToLowerInvariant() == "scml")
            {
                var oldDir = FileManager.RelativeDirectory;
                FileManager.RelativeDirectory = FileManager.GetDirectory(absoluteFileName);
                var spriterObjectSave = SpriterObjectSave.FromFile(absoluteFileName);
                foreach (var file in spriterObjectSave.Folder.SelectMany(folder => folder.File))
                {
                    var filename = FileManager.MakeAbsolute(file.Name);
                    if (!files.Contains(filename))
                    {
                        files.Add(filename);
                    }
                }

                FileManager.RelativeDirectory = oldDir;
            }
        }

        public override Version Version
        {
            get { return new Version(1, 0); }
        }


    }
}
