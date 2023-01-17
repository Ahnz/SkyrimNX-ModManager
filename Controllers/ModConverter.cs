using SkyrimNX_ModManager.Models;
using SkyrimNX_ModManager.Properties;
using System;
using System.Diagnostics;
using System.IO;

namespace SkyrimNX_ModManager.Controllers
{
    enum Operation
    {
        Convert,
        Unpack,
        Merge
    }
    class ModConverter
    {
        private string[] prefixList = { "_ConvertedAndPacked", "_Unpacked", "_Merged" };
        public string Transform(Operation operation, Mod mod)
        {
            string dest = mod.Path + prefixList[Convert.ToInt32(operation)]; // default underlying type for an enum is int

            switch (operation)
            {
                case Operation.Convert:
                    RunTask($@"Toolkit\{operation}_MOD.BAT", $"\"{mod.Path}\"");
                    break;

                case Operation.Unpack:
                    RunTask($@"Toolkit\{operation}_MOD.BAT", $"\"{mod.Path}\"");
                    break;

                case Operation.Merge:
                    MergeFiles(dest, mod);
                    break;
            }
            return dest;
        }
        public Mod[] ReturnModList(string dirPath)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(dirPath).GetDirectories("*", SearchOption.TopDirectoryOnly);
            Mod[] mList = new Mod[dirs.Length];
            for (int i = 0; i < dirs.Length; i++)
            {
                mList[i] = new Mod(dirs[i].Name, dirs[i].FullName);
            }
            return mList;
        }
        public void CleanUpDirectory()
        {
            string src = Settings.Default.ModsDirectory;

            foreach (string prefix in prefixList)
            {
                foreach (string path in Directory.GetDirectories(src, $"*{prefix}"))
                {
                    Directory.Delete(path, true);
                }
            }

            string[] logFiles = Directory.GetFiles(src, "*.log", SearchOption.TopDirectoryOnly);
            foreach (string logFile in logFiles)
            {
                File.Delete(logFile);
            }
        }
        private void MergeFiles(string dest, Mod mod)
        {
            bool containsEsp = false;

            Directory.CreateDirectory(dest);
            foreach (FileInfo modFile in mod.Contents(SearchOption.TopDirectoryOnly))
            {
                if (modFile.Extension.Equals(".esp"))
                { containsEsp = true; }
                modFile.MoveTo($@"{dest}/{modFile.Name}");
            }

            if (!containsEsp)
            {
                File.Copy("Dummy/emptyESL.esp", $"{dest}/{mod.Name}.esp");
            }

            RunTask(@"Toolkit\Utilities\bsarch.exe", $"pack \"{mod.Path}\" \"{dest}/{mod.Name}.bsa\" -sse -af 0x00000003 -ff 0x00000000");
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

