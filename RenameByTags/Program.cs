using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameByTagsMP3
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Хотите также переименовать файлы в подкаталогах?\nY/N");
            string userAnswer = System.Console.ReadLine();

            IEnumerable<string> directories = null;

            if (userAnswer.ToLower() == "y" || userAnswer.ToLower() == "yes")
            {
                directories = Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).Concat(new List<string>() { Directory.GetCurrentDirectory() });
            }
            else
            {
                if (userAnswer.ToLower() == "n" || userAnswer.ToLower() == "no")
                {
                    directories = new List<string>() { Directory.GetCurrentDirectory() };
                }
                else
                {
                    System.Console.WriteLine("Введено неверное значение.");
                    return;
                }
            }

            foreach (var dir in directories)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Blue;
                System.Console.WriteLine(dir);
                System.Console.ForegroundColor = System.ConsoleColor.White;

                IEnumerable<string> files = Directory.EnumerateFiles(dir).Where(x => Path.GetExtension(x) == ".mp3");

                foreach (var file in files)
                {
                    string artist, title, name, path;

                    TagLib.File f = TagLib.File.Create(file);

                    artist = f.Tag.FirstPerformer;
                    title = f.Tag.Title;

                    f.Dispose();

                    if (artist == null || title == null)
                        continue;

                    name = $"{artist} - {title}.mp3";
                    path = $"{dir}\\{name}";
                    System.Console.WriteLine(path);
                    try
                    {
                        File.Move(file, path);
                    }
                    catch (System.Exception)
                    {
                        System.Console.ForegroundColor = System.ConsoleColor.Red;
                        System.Console.WriteLine("Переименуйте файл вручную.");
                        System.Console.ForegroundColor = System.ConsoleColor.White;
                        System.Console.ReadLine();
                    }
                }
                System.Console.WriteLine();
            }

            System.Console.ReadLine();
        }
    }
}
