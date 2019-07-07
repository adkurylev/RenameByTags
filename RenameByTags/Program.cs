using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameByTags
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).Where(x => Path.GetExtension(x) == ".mp3");

            foreach (var file in files)
            {
                string artist, title, name;

                TagLib.File f = TagLib.File.Create(file);

                artist = f.Tag.FirstPerformer;
                title = f.Tag.Title;

                f.Dispose();

                if (artist == null || title == null)
                    continue;

                name = $"{artist} - {title}.mp3";

                File.Move(file, $"{name}");
            }

        }
    }
}
