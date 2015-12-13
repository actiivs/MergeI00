using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace MergeI00
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Managed Dll
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var dllName = new AssemblyName(args.Name).Name + ".dll";
                var execAsm = Assembly.GetExecutingAssembly();

                var resourceName = execAsm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(dllName));
                if (resourceName == null) return null;
                using (var stream = execAsm.GetManifestResourceStream(resourceName))
                {
                    var assbebmlyBytes = new byte[stream.Length];
                    stream.Read(assbebmlyBytes, 0, assbebmlyBytes.Length);
                    return Assembly.Load(assbebmlyBytes);
                }
            };

            base.OnStartup(e);
        }
    }
}
