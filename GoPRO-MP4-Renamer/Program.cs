using System;
using System.IO;
using System.Linq;
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

            var fileInfos = sourceDir.EnumerateFiles("*.MP4", SearchOption.AllDirectories).Union(sourceDir.EnumerateFiles("*.JPG", SearchOption.AllDirectories));

            foreach (FileInfo fileInfo in fileInfos)
            {
                DateTime? dateEncoded = null;

                if (string.Equals(fileInfo.Extension, ".MP4", StringComparison.OrdinalIgnoreCase))
                {
                    ShellObject shellObject = ShellObject.FromParsingName(fileInfo.FullName);

                    if (shellObject == null)
                    {
                        Console.WriteLine($"GoPROMP4Renamer - ParsingName for [{fileInfo.Name}] to ShellObject failed.");
                        continue;
                    }

                    dateEncoded = shellObject.Properties.GetProperty(SystemProperties.System.Media.DateEncoded)?.ValueAsObject as DateTime?;

                    if (dateEncoded == null)
                    {
                        Console.WriteLine($"GoPROMP4Renamer - System.Media.DateEncoded for [{fileInfo.Name}] returned null.");
                        continue;
                    }
                }
                else if (string.Equals(fileInfo.Extension, ".JPG", StringComparison.OrdinalIgnoreCase))
                {
                    ShellObject shellObject = ShellObject.FromParsingName(fileInfo.FullName);

                    if (shellObject == null)
                    {
                        Console.WriteLine($"GoPROMP4Renamer - ParsingName for [{fileInfo.Name}] to ShellObject failed.");
                        continue;
                    }

                    dateEncoded = shellObject.Properties.GetProperty(SystemProperties.System.Photo.DateTaken)?.ValueAsObject as DateTime?;

                    if (dateEncoded == null)
                    {
                        Console.WriteLine($"GoPROMP4Renamer - System.Media.DateEncoded for [{fileInfo.Name}] returned null.");
                        continue;
                    }
                }

                if (dateEncoded == null) continue;
                if (fileInfo.DirectoryName == null) continue;

                DateTime mediaCreated = dateEncoded.Value;
                FileInfo newFileInfo = new FileInfo(Path.Combine(fileInfo.DirectoryName, $"Img{mediaCreated:yyyyMMdd}_{mediaCreated:HHmmss}{fileInfo.Extension}"));

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
