using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Controls;
using FlatRedBall.Glue.Elements;
using FlatRedBall.Glue.Plugins.ExportedInterfaces;
using FlatRedBall.Glue.Plugins.Interfaces;
using FlatRedBall.Glue.SaveClasses;
using FlatRedBall.Glue.Plugins;


namespace SpriterPlugin
{
    public class SpriterPlugin : INewFile
    {
        [Import("GlueProjectSave")]
        public GlueProjectSave GlueProjectSave
        {
            get;
            set;
        }

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

        public string FriendlyName
        {
            get { return "Spriter Integration"; }
        }

        public bool ShutDown(PluginShutDownReason reason)
        {
            // Do anything your plugin needs to do to shut down
            // or don't shut down and return false

            return true;
        }

        public void StartUp()
        {
            // Do anything your plugin needs to do when it first starts up

        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }


        public void AddNewFileOptions(NewFileWindow newFileWindow)
        {
        }

        public bool CreateNewFile(AssetTypeInfo assetTypeInfo, object extraData, string directory, string name, out string resultingName)
        {
        }

        public void ReactToNewFile(ReferencedFileSave newFile)
        {
            string absoluteFileName = ProjectManager.MakeAbsolute(newFile.Name, true);
            Console.WriteLine(absoluteFileName);
        }
    }
}
