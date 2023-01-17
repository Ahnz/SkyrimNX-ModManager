using SkyrimNX_ModManager.Controllers;
using SkyrimNX_ModManager.Models;
using SkyrimNX_ModManager.Properties;
using System;
using System.IO;

namespace SkyrimNX_ModManager
{
    class Program
    {
        static ModConverter converter = new ModConverter();
        static void Main(string[] args)
        {
            // TODO: Initializing and checking Paths

            int choice;
            Console.WriteLine("Please select an Option:");
            Console.WriteLine("(1) Convert Mods");
            Console.WriteLine("(2) Upload Mods");
            Console.WriteLine("(3) Change Load Order");
            Console.Write("\nEnter choice: ");
            Int32.TryParse(Console.ReadLine(), out choice);
            Console.Clear();

            switch (choice)
            {
                case 1:
                    PrepareLocalMods();
                    break;
                case 2:

                    UploadMods();
                    break;
                default:
                    break;
            }
        }

        static void PrepareLocalMods()
        {
            Mod[] baseModList = converter.ReturnModList(Settings.Default.ModsDirectory);

            // Prepare Mods
            foreach (Mod currentMod in baseModList)
            {
                Console.WriteLine("Converting " + currentMod.Name);

                currentMod.Path = converter.Transform(Operation.Convert, currentMod);
                currentMod.Path = converter.Transform(Operation.Unpack, currentMod);
                currentMod.Path = converter.Transform(Operation.Merge, currentMod);
                Directory.Move(currentMod.Path, $"{Settings.Default.ConvertedModsDirectory}/{currentMod.Name}");
                converter.CleanUpDirectory();
            }
        }
        static void UploadMods()
        {            
            Mod[] convertedModList = converter.ReturnModList(Settings.Default.ConvertedModsDirectory);

            FileTransfer filetransfer = new FileTransfer(true);
            filetransfer.Upload(convertedModList);           
        }
    }
}
