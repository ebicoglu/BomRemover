using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRemover
{
    internal class Program
    {
        // Change the following parameters for your requirement

        private const string StartDirectory = @"D:\Github\volo";
        private static readonly List<string> FilePatterns = new List<string>()
        {
            "*.js",
            "*.json",
            "*.md",
            "*.xml",
            "*.xsd",
            "*.js",
            "*.css",
            "*.scss",
            "*.less",
            "*.tpl",
            "*.props"
        };


        private static async Task Main()
        {
            long affectedFilesCount = 0;

            foreach (var filePattern in FilePatterns)
            {
                affectedFilesCount += await RemoveBomAsync(filePattern, StartDirectory);
            }

            Console.WriteLine(Environment.NewLine +
                              new string('-', 25) +
                              Environment.NewLine +
                              "Modified files count: " + affectedFilesCount);

            Console.ReadKey();
        }

        private static async Task<long> RemoveBomAsync(string filePattern, string directory, long affectedFilesCount = 0)
        {
            foreach (var filename in Directory.GetFiles(directory, filePattern, SearchOption.AllDirectories))
            {
                var fileContent = await File.ReadAllBytesAsync(filename);
                if (HasBom(fileContent))
                {
                    await File.WriteAllBytesAsync(filename, fileContent.Skip(3).ToArray());
                    Console.WriteLine("+ BOM REMOVED > " + filename);
                    affectedFilesCount++;
                }
                else
                {
                    Console.WriteLine("- NO BOM > " + filename);
                }
            }

            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                return await RemoveBomAsync(filePattern, subDirectory, affectedFilesCount);
            }

            return affectedFilesCount;
        }

        private static bool HasBom(IReadOnlyList<byte> bytes)
        {
            return bytes.Count > 2 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
        }
    }
}
