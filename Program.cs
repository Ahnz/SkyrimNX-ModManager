using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimNX_ModManager
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo[] modList = new DirectoryInfo(Properties.Settings.Default.ModsDirectory).GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (DirectoryInfo mod in modList)
            {
                Console.WriteLine(mod.Name);
                ConvertMod(mod);
            }
            Console.ReadLine();
        }
        static void ConvertMod(DirectoryInfo mod)
        {
            string modName = mod.Name;
            string modPath = mod.FullName;
            string modCompletePath = $"{Properties.Settings.Default.ConvertedModsDirectory}/{modName}";
            DirectoryInfo modUnpackedPath = new DirectoryInfo($@"{Properties.Settings.Default.ModsDirectory}/{modName}_Unpacked");

            Directory.CreateDirectory(modCompletePath);
            RunTask(@"Toolkit\UNPACK_MOD.BAT", $"\"{modPath}\""); //Unpack
            // TODO: Convert
            RunTask(@"Toolkit\Utilities\bsarch.exe", $"pack \"{modUnpackedPath.FullName}\" \"{modCompletePath}/{modName}.bsa\" -sse -af 0x00000003 -ff 0x00000000"); //Merge BSA

            // Combine
            FileInfo[] modFiles = modUnpackedPath.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in modFiles)
            {
                file.MoveTo($@"{modCompletePath}/{file.Name}");
            }
            modUnpackedPath.Delete(true);
        }

        static void RunTask(string process, string args)
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
