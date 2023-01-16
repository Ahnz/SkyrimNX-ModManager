using System;
using System.Diagnostics;
using System.IO;

namespace SkyrimNX_ModManager.Models
{
    enum Operation
    {
        Convert,
        Unpack,
        Merge
    }

    class Mod
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string PathConverted { get; private set; }
        public string PathUnpacked { get; private set; }
        public string PathMerged { get; private set; }
        public Mod(string Name, string Path)
        {
            this.Name = Name;
            this.Path = Path;
        }

        public string Transform(Operation operation, string path)
        {
            Console.WriteLine(operation);
            string outputPath = null;
            switch (operation)
            {
                case Operation.Convert:
                    PathConverted = path + "_ConvertedAndPacked";
                    outputPath = PathConverted;
                    break;

                case Operation.Unpack:
                    PathUnpacked = path + "_Unpacked";
                    outputPath = PathUnpacked;
                    break;

                case Operation.Merge:
                    bool containsEsp = false;
                    PathMerged = path + "_Merged";
                    outputPath = PathMerged;

                    Directory.CreateDirectory(PathMerged);
                    FileInfo[] modFiles = new DirectoryInfo(path).GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo modFile in modFiles)
                    {
                        if (modFile.Extension.Equals(".esp"))
                        { containsEsp = true; }
                        modFile.MoveTo($@"{PathMerged}/{modFile.Name}");
                    }

                    if(!containsEsp)
                    {
                        File.Copy("Dummy/emptyESL.esp", $"{PathMerged}/{Name}.esp");
                    }

                    RunTask(@"Toolkit\Utilities\bsarch.exe", $"pack \"{path}\" \"{PathMerged}/{Name}.bsa\" -sse -af 0x00000003 -ff 0x00000000");
                    return outputPath;

            }
            RunTask($@"Toolkit\{operation}_MOD.BAT", $"\"{path}\"");
            return outputPath;
        }
        public void CleanUpDirectory()
        {
            if (Directory.Exists(PathConverted))
            {
                Directory.Delete(PathConverted, true);
            }
            if (Directory.Exists(PathUnpacked))
            {
                Directory.Delete(PathUnpacked, true);
            }
            if (Directory.Exists(PathMerged))
            {
                Directory.Delete(PathMerged, true);
            }

            string[] logFiles = Directory.GetFiles(Properties.Settings.Default.ModsDirectory, "*.log", SearchOption.TopDirectoryOnly);
            foreach (string logFile in logFiles)
            {
                File.Delete(logFile);
            }
        }
        private void RunTask(string process, string args)
        {
            Process processChild;
            ProcessStartInfo taskInfo;

            taskInfo = new ProcessStartInfo(process, args);

            taskInfo.CreateNoWindow = true;
            taskInfo.UseShellExecute = false;

            processChild = Process.Start(taskInfo);
            processChild.WaitForExit();
        }
    }
}
