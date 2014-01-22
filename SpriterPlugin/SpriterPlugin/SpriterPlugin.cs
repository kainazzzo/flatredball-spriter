using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EditorObjects.Parsing;
using FlatRedBall.Glue.Elements;
using FlatRedBall.Glue.Plugins.ExportedInterfaces;
using FlatRedBall.Glue.Plugins.Interfaces;
using FlatRedBall.Glue.Plugins;
using FlatRedBall.IO;
using FlatRedBall.Math.Geometry;
using FlatRedBall_Spriter;
// add the following using statements:
using FlatRedBall.Glue.VSHelpers;
using System.Reflection;


namespace SpriterPlugin
{
    [Export(typeof(PluginBase))]
    public class SpriterPlugin : PluginBase
    {
        // Add the following at class scope:
        CodeBuildItemAdder _itemAdder;

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
            AvailableAssetTypes.Self.AddAssetType(new AssetTypeInfo
            {
                FriendlyName = "SpriterObjectCollection (.scml)",
                CanBeObject = true,
                QualifiedRuntimeTypeName = new PlatformSpecificType{ Platform = "All", QualifiedType = "FlatRedBall_Spriter.SpriterObjectCollection"},
                QualifiedSaveTypeName = "FlatRedBall_Spriter.SpriterObjectSave",
                Extension = "scml",
                AddToManagersMethod = new List<string>{ "this.AddToManagers()" },
                LayeredAddToManagersMethod = new List<string>{ "this.AddToManagers(mLayer)" },
                CustomLoadMethod = "{THIS} = SpriterObjectSave.FromFile(\"{FILE_NAME}\").ToSpriterObjectCollection();",
                DestroyMethod = "this.Destroy()",
                ShouldAttach = true,
                MustBeAddedToContentPipeline = false,
                CanBeCloned = true,
                HasCursorIsOn = false,
                HasVisibleProperty = false,
                CanIgnorePausing = false,
                
            });

            AvailableAssetTypes.Self.AddAssetType(new AssetTypeInfo
            {
                FriendlyName = "SpriterObject",
                CanBeObject = true,
                QualifiedRuntimeTypeName = new PlatformSpecificType { Platform = "All", QualifiedType = "FlatRedBall_Spriter.SpriterObject" },
                QualifiedSaveTypeName = "FlatRedBall_Spriter.SpriterObjectSave",
                AddToManagersMethod = new List<string> { "this.AddToManagers()" },
                LayeredAddToManagersMethod = new List<string> { "this.AddToManagers(mLayer)" },
                CustomLoadMethod = "{THIS} = SpriterObjectSave.FromFile(\"{FILE_NAME}\").ToRunTime();",
                DestroyMethod = "this.Destroy()",
                ShouldAttach = true,
                MustBeAddedToContentPipeline = false,
                CanBeCloned = true,
                HasCursorIsOn = false,
                HasVisibleProperty = false,
                CanIgnorePausing = false,

            });

            _itemAdder = new CodeBuildItemAdder();


            _itemAdder.Add("SpriterPlugin.FlatRedBallTextureLoader.cs");
            _itemAdder.Add("SpriterPlugin.ITextureLoader.cs");
            _itemAdder.Add("SpriterPlugin.KeyFrame.cs");
            _itemAdder.Add("SpriterPlugin.KeyFrameValues.cs");
            _itemAdder.Add("SpriterPlugin.SpriterObject.cs");
            _itemAdder.Add("SpriterPlugin.SpriterObjectAnimation.cs");
            _itemAdder.Add("SpriterPlugin.SpriterObjectSave.conversion.cs");
            _itemAdder.Add("SpriterPlugin.SpriterObjectSave.serialization.cs");
            _itemAdder.Add("SpriterPlugin.IRelativeScalable.cs");
            _itemAdder.Add("SpriterPlugin.ScaledPositionedObject.cs");
            _itemAdder.Add("SpriterPlugin.ScaledPositionedObjectExtensions.cs");
            _itemAdder.Add("SpriterPlugin.ScaledSprite.cs");
            _itemAdder.Add("SpriterPlugin.SpriterPoint.cs");
            _itemAdder.Add("SpriterPlugin.ScaledPolygon.cs");
            _itemAdder.Add("SpriterPlugin.SpriterBone.cs");
            _itemAdder.Add("SpriterPlugin.SpriterObjectCollection.cs");

            _itemAdder.OutputFolderInProject = "SpriterPlugin";

            // Add the following code to your StartUp method:
            ReactToLoadedGlux += HandleGluxLoad;

            TryAddContainedObjects += OnTryAddContainedObjects;
        }

        private bool OnTryAddContainedObjects(string absoluteFileName, List<string> objects)
        {
            if (!string.IsNullOrEmpty(absoluteFileName) &&
                FileManager.GetExtension(absoluteFileName).ToLowerInvariant() == "scml")
            {
                var oldDir = FileManager.RelativeDirectory;
                FileManager.RelativeDirectory = FileManager.GetDirectory(absoluteFileName);
                var sos = SpriterObjectSave.FromFile(absoluteFileName);
                FileManager.RelativeDirectory = oldDir;
                foreach (var entity in sos.Entity)
                {
                    objects.Add(string.Format("{0} (SpriterObject)", entity.Name));
                }
                return true;
            }
            return false;
        }

        private void HandleGluxLoad()
        {
            // Implement the HandleGluxLoad method as follows:
            Assembly assembly = Assembly.GetExecutingAssembly();
            _itemAdder.PerformAddAndSave(assembly);
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
