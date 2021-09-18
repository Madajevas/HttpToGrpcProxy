using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace HttpToGrpcProxy.Powershell
{
    public class ModuleInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static readonly string s_modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);

            return Assembly.LoadFrom(Path.Combine(s_modulePath, $"{assemblyName.Name}.dll"));
        }
    }
}
