using SkyrimNX_ModManager.Controllers;
using SkyrimNX_ModManager.Models;
using SkyrimNX_ModManager.Properties;
using System;
using System.IO;

namespace SkyrimNX_ModManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Initializing and checking Paths
            PrepareLocalMods();
            UploadMods();
            //Sort LoadOrder 
            Console.ReadLine();
        }

        static void PrepareLocalMods()
        {
            DirectoryInfo[] baseModList = new DirectoryInfo(Settings.Default.ModsDirectory).GetDirectories("*", SearchOption.TopDirectoryOnly);

            // Prepare Mods
            foreach (DirectoryInfo mod in baseModList)
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
        }

        static void UploadMods()
        {            
            string[] convertedModList = Directory.GetDirectories(Settings.Default.ConvertedModsDirectory, "*", SearchOption.TopDirectoryOnly);

            FileTransfer filetransfer = new FileTransfer();
            filetransfer.Upload(convertedModList);           
        }
    }
}
