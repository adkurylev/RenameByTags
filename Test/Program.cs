using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static IEnumerable<string> GetSubdirectories(string currentDirectory)
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(currentDirectory);

            if (directories != null)
            {
                foreach (var dir in directories)
                {
                    directories = directories.Concat(GetSubdirectories(dir));
                }
            }

            return directories;
        }
        static void Main(string[] args)
        {   
            var dir = GetSubdirectories(Directory.GetCurrentDirectory());
            dir = dir.Concat(new List<string> { Directory.GetCurrentDirectory()});

            foreach(var d in dir)
            {
                Console.WriteLine(d);
            }

            Console.ReadLine();
        }
    }
}
