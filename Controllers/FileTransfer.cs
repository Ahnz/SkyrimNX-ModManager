using FluentFTP;
using SkyrimNX_ModManager.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimNX_ModManager.Controllers
{
    public class FileTransfer
    {
        private FtpClient client;
        public FileTransfer()
        {
            client = new FtpClient(Settings.Default.SwitchIp, Settings.Default.SwitchUser, Settings.Default.SwitchPassword, Settings.Default.SwitchPort);
        }

        public void Upload(string[] modList)
        {
            client.AutoConnect();
            foreach(string modPath in modList)
            {
                FileInfo[] modFiles = new DirectoryInfo(modPath).GetFiles();
                foreach (FileInfo mod in modFiles)
                {
                    Console.WriteLine(mod.Name);
                    string remotePath = Settings.Default.SkyrimNXDirectory + "/" + mod.Name;
                    if (!client.FileExists(remotePath))
                    {
                        client.UploadFile(mod.FullName, remotePath);
                    }
                }
            }
            client.Disconnect();
        }
    }
}
