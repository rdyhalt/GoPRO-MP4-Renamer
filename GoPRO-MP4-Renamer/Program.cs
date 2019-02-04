using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPRO_MP4_Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("GoPROMP4Renamer <source>");
                return;
            }

            var sourceDir = new DirectoryInfo(args[0]);

            if (sourceDir.Exists == false)
            {
                Console.WriteLine($"GoPROMP4Renamer <source> does not exists - {sourceDir.FullName}");
                return;
            }

            foreach (FileInfo fileInfo in sourceDir.GetFiles("*.mp4"))
            {
                DateTime mediaCreated = fileInfo.CreationTime;
                string newFileName = Path.Combine(sourceDir.FullName, $"Img{mediaCreated:yyyymmdd}_{mediaCreated:HHmmss}{fileInfo.Extension}");




                
            }
        }
    }
}
