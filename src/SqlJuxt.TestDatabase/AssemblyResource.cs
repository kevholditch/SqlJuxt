using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SqlJuxt.TestDatabase
{
    public sealed class AssemblyResource
    {
        private AssemblyResource(Assembly assembly, string resourceName)
        {
            Assembly = assembly;
            ResourceName = resourceName;
        }

        private Assembly Assembly { get; set; }

        private string ResourceName { get; set; }

        public static AssemblyResource InThisAssembly(string resourceName)
        {
            return InAssembly(typeof(AssemblyResource).Assembly, resourceName);
        }

        public static AssemblyResource InAssembly(Assembly assembly, string named)
        {
            var resourceNames = assembly.GetManifestResourceNames().ToArray();
            var resourceName =
                resourceNames.FirstOrDefault(name => name.EndsWith(named, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentException(
                    string.Format("No assembly resource can be found that matches the name {0}.", named), "named");
            }

            return new AssemblyResource(assembly, resourceName);
        }

        public FileInfo SaveToDisk(DirectoryInfo directory)
        {
            var filePath = Path.Combine(Path.GetFullPath(directory.FullName), ResourceName);
            return SaveToDisk(filePath);
        }

        public FileInfo SaveToDisk(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var writer = new FileStream(filePath, FileMode.CreateNew))
            {
                using (var resourceStream = Assembly.GetManifestResourceStream(ResourceName))
                {
                    if (resourceStream == null) return new FileInfo(filePath);
                    resourceStream.Seek(0, SeekOrigin.Begin);
                    resourceStream.CopyTo(writer);
                }
            }

            return new FileInfo(filePath);
        }

        public Stream OpenStream()
        {
            return Assembly.GetManifestResourceStream(ResourceName);
        }

        public string GetText()
        {
            var stream = Assembly.GetManifestResourceStream(ResourceName);

            if (stream == null)
                throw new InvalidOperationException("Unable to load stream from resource " + ResourceName);

            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                return NormalizeLineEndings(text);
            }
        }

        private static string NormalizeLineEndings(string text)
        {
            return Regex.Replace(text, @"\r\n|\n\r|\n|\r", Environment.NewLine);
        }
    }
}