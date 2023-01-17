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
            ModConverter converter = new ModConverter();
            DirectoryInfo[] baseModList = new DirectoryInfo(Settings.Default.ModsDirectory).GetDirectories("*", SearchOption.TopDirectoryOnly);

            // Prepare Mods
            foreach (DirectoryInfo modFile in baseModList)
            {
                Console.WriteLine(modFile.Name);

                Mod currentMod = new Mod(modFile.Name, modFile.FullName);
                currentMod.Path = converter.Transform(Operation.Convert, currentMod);
                currentMod.Path = converter.Transform(Operation.Unpack, currentMod);
                currentMod.Path = converter.Transform(Operation.Merge, currentMod);
                Directory.Move(currentMod.Path, $"{Settings.Default.ConvertedModsDirectory}/{currentMod.Name}");
                converter.CleanUpDirectory();
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
