using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;

namespace XNuvem.Environment.Extensions
{
    public static class ExtensionLoader
    {
        private const string BinFolder = "~/bin";

        private const string ModulesFolder = "~/Modules";

        private const string ProbingFolder = "~/App_Data/addons";

        public static void Load()
        {
            // Get bin modules directories
            var binModules = new DirectoryInfo(HostingEnvironment.MapPath(ModulesFolder))
                .GetDirectories()
                .Select(d => Path.Combine(d.FullName, "bin"));
            foreach (var binPath in binModules) CopyFilesToProbingFolder(binPath);

            var filesToLoad = new DirectoryInfo(HostingEnvironment.MapPath(ProbingFolder))
                .GetFiles("*.dll")
                .Select(f => f.FullName);
            foreach (var fileName in filesToLoad)
            {
                var assembly = TryLoadExtension(fileName);
                if (assembly != null) BuildManager.AddReferencedAssembly(assembly);
            }
        }

        public static Assembly TryLoadExtension(string fileName)
        {
            try
            {
                var asssemblyName = AssemblyName.GetAssemblyName(fileName);
                return Assembly.Load(asssemblyName);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                return null;
            }
        }

        internal static void CopyFilesToProbingFolder(string path)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files) CopyFileIntoProbing(file);
#if DEBUG
            // if in debug mode, copy the debug diagnostics files and configurations
            var debugFiles = dir.GetFiles("*.pdb").Union(dir.GetFiles("*.xml"));
            foreach (var file in debugFiles) CopyFileIntoProbing(file);
#endif
        }

        internal static void CopyFileIntoProbing(FileInfo file)
        {
            var probingFileName = Path.Combine(HostingEnvironment.MapPath(ProbingFolder), file.Name);
            var shoudCopy = !FileExistsOnBinFolder(file.Name)
                            && (!File.Exists(probingFileName) ||
                                file.LastWriteTimeUtc > File.GetLastWriteTimeUtc(probingFileName));
            if (shoudCopy) File.Copy(file.FullName, probingFileName, true);
        }

        internal static bool FileExistsOnBinFolder(string fileName)
        {
            var binFileName = Path.Combine(HostingEnvironment.MapPath(BinFolder), fileName);
            return File.Exists(binFileName);
        }
    }
}