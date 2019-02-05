using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace GoPRO_MP4_Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("GoPROMP4Renamer <source directory>");
                return;
            }

            var sourceDir = new DirectoryInfo(args[0]);

            if (sourceDir.Exists == false)
            {
                Console.WriteLine($"GoPROMP4Renamer <source> does not exists - [{sourceDir.FullName}]");
                return;
            }

            foreach (FileInfo fileInfo in sourceDir.GetFiles("*.MP4", SearchOption.TopDirectoryOnly))
            {
                if (false == string.Equals(fileInfo.Extension, ".MP4", StringComparison.CurrentCultureIgnoreCase)) continue;

                ShellObject shellObject = ShellObject.FromParsingName(fileInfo.FullName);

                if (shellObject == null)
                {
                    Console.WriteLine($"GoPROMP4Renamer - ParsingName for [{fileInfo.Name}] to ShellObject failed.");
                    continue;
                }

                DateTime? dateEncoded = shellObject.Properties.GetProperty(SystemProperties.System.Media.DateEncoded)?.ValueAsObject as DateTime?;

                if (dateEncoded == null)
                {
                    Console.WriteLine($"GoPROMP4Renamer - System.Media.DateEncoded for [{fileInfo.Name}] returned null.");
                    continue;
                }

                DateTime mediaCreated = dateEncoded.Value;
                FileInfo newFileInfo = new FileInfo(Path.Combine(sourceDir.FullName, $"Img{mediaCreated:yyyyMMdd}_{mediaCreated:HHmmss}{fileInfo.Extension}"));

                if (newFileInfo.Exists)
                {
                    Console.WriteLine($"GoPROMP4Renamer - Cannot rename [{fileInfo.Name}] => [{newFileInfo.Name}] - File already exists.");
                    continue;
                }

                try
                {
                    Console.WriteLine($"GoPROMP4Renamer - Rename file [{fileInfo.Name}] => [{newFileInfo.Name}]");
                    fileInfo.MoveTo(newFileInfo.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"GoPROMP4Renamer - Cannot rename [{fileInfo.Name}] => [{newFileInfo.Name}]");
                    Console.WriteLine($"- Type:{e.GetType()} : {e.Message}{Environment.NewLine}");
                }
            }
        }
    }
}
