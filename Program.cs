using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BomRemover
{
    internal class Program
    {
        private static void Main()
        {
            const string startDirectory = @"d:\Github\abp";
            long affectedFilesCount = 0;

            var filePatterns = new List<string>()
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

            foreach (var filePattern in filePatterns)
            {
                affectedFilesCount += RemoveBom(filePattern, startDirectory);

            }

            Console.WriteLine(Environment.NewLine +
                              new string('-', 25) +
                              Environment.NewLine +
                              "Modified files count: " + affectedFilesCount);

            Console.ReadKey();
        }

        private static long RemoveBom(string filePattern, string directory, long affectedFilesCount = 0)
        {
            foreach (var filename in Directory.GetFiles(directory, filePattern, SearchOption.AllDirectories))
            {
                var bytes = System.IO.File.ReadAllBytes(filename);
                if (bytes.Length > 2 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                {
                    System.IO.File.WriteAllBytes(filename, bytes.Skip(3).ToArray());
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
                return RemoveBom(filePattern, subDirectory, affectedFilesCount);
            }

            return affectedFilesCount;
        }
    }
}
