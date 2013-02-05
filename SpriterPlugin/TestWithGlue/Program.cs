using System;
using System.Reflection;
using System.Windows.Forms;
using FlatRedBall.Glue.Plugins;
using SpriterPlugin;

namespace TestWithGlue
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PluginManagerBase.AddGlobalOnInitialize.Add(Assembly.GetAssembly(typeof(Glue.Form1)));
            PluginManagerBase.AddGlobalOnInitialize.Add(Assembly.GetAssembly(typeof(MyPlugin)));
            PluginManager.HandleExceptions = false;
            Application.Run(new Glue.Form1());
        }
    }
}
