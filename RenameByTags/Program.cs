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

            //Если пользователь хочет переименовать файлы в подкаталогах и вводит "yes", переменной directories присваивается коллекция, состоящая из текущего каталога и его подкаталогов,
            //с помощью метода Concat добавляется текущий каталог.
            //Иначе, переменная directories получает значение только текущего каталога.
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
                //Вывод в консоль название каталога
                System.Console.ForegroundColor = System.ConsoleColor.Blue;
                System.Console.WriteLine(dir);
                System.Console.ForegroundColor = System.ConsoleColor.White;

                //Получение файлов с расширением .mp3
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
                    //Если метаданные файла содержат запрещенные символы, на консоль выводится соответствующее сообщение.
                    try
                    {
                        File.Move(file, path);
                    }
                    catch (System.Exception)
                    {
                        System.Console.ForegroundColor = System.ConsoleColor.Red;
                        System.Console.Write("Переименуйте верхний файл вручную.");
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
