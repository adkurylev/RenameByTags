using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace RenameByTagsMP3
{
    class Program
    {
        static string GetUserAnswer(string messageToUser)
        {
            string res;

            do
            {
                Console.WriteLine(messageToUser);
                res = Console.ReadLine().ToLower();
            }
            while ((res != "y") && (res != "yes") && (res != "n") && (res != "no"));

            return res;
        }

        static IEnumerable<string> GetSubdirectories(string currentDirectory)
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(currentDirectory);

            if(directories != null)
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
            //TODO: если в тэгах указан альбом, а рядом с файлом лежит картинка с 
            //названием альбома, вставить картинку в тэги

            string userAnswer = GetUserAnswer("Хотите также переименовать файлы в подкаталогах?\ny/n");

            IEnumerable<string> directories = new List<string>() { Directory.GetCurrentDirectory() };

            //Если пользователь хочет переименовать файлы в подкаталогах и вводит "yes", переменной directories 
            //присваивается коллекция, состоящая из текущего каталога и его подкаталогов,
            //с помощью метода Concat добавляется текущий каталог.
            //Иначе, переменная directories получает значение только текущего каталога.
            if (userAnswer == "y" || userAnswer == "yes")
            {
                directories = GetSubdirectories(Directory.GetCurrentDirectory()).Concat(directories);
            }

            foreach (var dir in directories)
            {
                //Вывод в консоль название каталога
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(dir);
                Console.ForegroundColor = ConsoleColor.White;

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
                    Console.WriteLine(path);
                    //Если метаданные файла содержат запрещенные символы, на консоль выводится соответствующее сообщение.
                    try
                    {
                        File.Move(file, path);
                    }
                    catch (System.Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Переименуйте верхний файл вручную.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                    }
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
