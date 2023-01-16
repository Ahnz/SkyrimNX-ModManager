using SkyrimNX_ModManager.Models;
using System;
using System.IO;

namespace SkyrimNX_ModManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Initializing and checking Paths 

            DirectoryInfo[] modList = new DirectoryInfo(Properties.Settings.Default.ModsDirectory).GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (DirectoryInfo mod in modList)
            {
                string lastOutputDirectory;
                Mod currentMod = new Mod(mod.Name, mod.FullName);
                Console.WriteLine(currentMod.Name);
                lastOutputDirectory = currentMod.Transform(Operation.Convert, currentMod.Path);
                lastOutputDirectory = currentMod.Transform(Operation.Unpack, lastOutputDirectory);
                lastOutputDirectory = currentMod.Transform(Operation.Merge, lastOutputDirectory);
                Directory.Move(lastOutputDirectory, $"{Properties.Settings.Default.ConvertedModsDirectory}/{currentMod.Name}");
                currentMod.CleanUpDirectory();
            }

            Console.ReadLine();
        }
    }
}
