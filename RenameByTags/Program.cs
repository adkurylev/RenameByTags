using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace RenameByTagsMP3
{
    class FileMP3
    {
        public string name, path, artist;

        public FileMP3(TagLib.File file, string directory)
        {
            artist = file.Tag.FirstPerformer;
            name = $"{file.Tag.FirstPerformer} - {file.Tag.Title}.mp3";
            path = directory;
        }

        public override string ToString()
        {
            return path + "\\" + name;
        }
    }

    class Program
    {
        static bool GetUserAnswer(string messageToUser)
        {
            string res;

            do
            {
                Console.WriteLine(messageToUser);
                res = Console.ReadLine().ToLower();
            }
            while ((res != "y") && (res != "yes") && (res != "n") && (res != "no"));

            return ((res == "y") || (res == "yes"));
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
            //названием альбома, вставить картинку в тэги.
            //Группировка в папки по артистам.

            bool userAnswer = GetUserAnswer("Хотите также переименовать файлы в подкаталогах?\ny/n");

            IEnumerable<string> directories = new List<string>() { Directory.GetCurrentDirectory() };

            //Если пользователь хочет переименовать файлы в подкаталогах и вводит "yes", переменной directories 
            //присваивается коллекция, состоящая из текущего каталога и его подкаталогов,
            //с помощью метода Concat добавляется текущий каталог.
            if (userAnswer)
            {
                directories = GetSubdirectories(Directory.GetCurrentDirectory()).Concat(directories);
            }

            userAnswer = GetUserAnswer("Хотите сгруппировать файлы в папки по артистам?\ny/n");

            List<FileMP3> fileList = new List<FileMP3>();
            string[] wrongSymbols = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|", "+" };

            foreach (var dir in directories)
            {
                //Вывод в консоль название каталога
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(dir);
                Console.ForegroundColor = ConsoleColor.White;

                //Получение файлов с расширением .mp3
                IEnumerable<string> files = Directory.EnumerateFiles(dir).Where(x => Path.GetExtension(x) == ".mp3");

                bool groupFlag = userAnswer & (dir == Directory.GetCurrentDirectory());

                foreach (var oldFile in files)
                {
                    TagLib.File f = TagLib.File.Create(oldFile);
                    FileMP3 file = new FileMP3(f, dir);
                    f.Dispose();

                    //Если название и артист не указаны, ничего не делаем с файлом
                    if (file.name == " - .mp3")
                    {
                        continue;
                    }

                    bool correctNameFlag = false;

                    foreach(var i in wrongSymbols)
                    {
                        correctNameFlag = file.name.Contains(i) || correctNameFlag;
                    }

                    Console.WriteLine(file);

                    if (correctNameFlag)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Переименуйте верхний файл вручную.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                        continue;
                    }

                    try
                    {
                        File.Move(oldFile, file.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    if (groupFlag)
                    {
                        fileList.Add(file);
                    }
                }

                IEnumerable<IGrouping<string, FileMP3>> group = null;
                if (userAnswer)
                {
                    group = fileList.GroupBy(x => x.artist);
                }
                
                foreach(var g in group)
                {
                    Directory.CreateDirectory(g.Key);
                    foreach(var el in g)
                    {
                        File.Move(el.ToString(), el.path + "\\" + el.artist + "\\" + el.name);
                    }
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
