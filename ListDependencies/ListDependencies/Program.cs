using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ListDependencies
{
    internal class Program
    {
        public static string[] GetDependencyFileNames(string assemblyPath)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            var references = assembly.GetReferencedAssemblies();
            return references.OrderBy(r => r.Name).Select(r => r.Name + ".dll").ToArray();
        }

        public static void OutputDependencies(string path, string fileName, int depth)
        {
            depth++;
            var filePath = Path.Combine(path, fileName);
            var deps = GetDependencyFileNames(filePath);
            foreach (var dep in deps)
            {
                var depFile = path + dep;
                if (File.Exists(depFile))
                {
                    Console.WriteLine(Pad(depth) + dep);
                    OutputDependencies(path, dep, depth);
                }
                else
                    Console.WriteLine("~       " + Pad(depth - 1) + dep);
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1)
                {
                    var assemblyFilePath = args[0];

                    var assemblyPath = Path.GetDirectoryName(assemblyFilePath) + "\\";
                    var assemblyFile = Path.GetFileName(assemblyFilePath);
                    Console.WriteLine(assemblyFile);
                    Program.OutputDependencies(assemblyPath, assemblyFile, 0);
                }
                else
                    Console.WriteLine("Invalid number of arguments. Please specify file path:" + Environment.NewLine + "listdependencies.exe \"C:\\Foo\\bar.dll\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static string Pad(int amount)
        { return amount > 0 ? String.Empty.PadRight(amount * 8, ' ') : String.Empty; }
    }
}