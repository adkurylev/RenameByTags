using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream f = new FileStream("../../../example.mp3", FileMode.Open);
            BinaryReader br = new BinaryReader(f);

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i + " " + (char)br.ReadByte());
            }

            br.Dispose();

            Console.ReadLine();
        }
    }
}
