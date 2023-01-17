using FluentFTP;
using SkyrimNX_ModManager.Models;
using SkyrimNX_ModManager.Properties;
using System;
using System.IO;

namespace SkyrimNX_ModManager.Controllers
{
    public class FileTransfer
    {
        private FtpClient client;
        private bool delAfterUpload;
        private bool enableModAfterUpload;
        public FileTransfer(bool delAfterUpload = false, bool enableModAfterUpload = true)
        {
            client = new FtpClient(Settings.Default.SwitchIp, Settings.Default.SwitchUser, Settings.Default.SwitchPassword, Settings.Default.SwitchPort);
            this.delAfterUpload = delAfterUpload;
            this.enableModAfterUpload = enableModAfterUpload;
        }

        public void Upload(Mod m)
        {
            client.AutoConnect();
            Console.WriteLine(m.Name);
            foreach (FileInfo mContent in m.Contents())
            {
                string remotePath = Settings.Default.SkyrimNXDirectory + "/" + mContent.Name;
                if (!client.FileExists(remotePath))
                {
                    client.UploadFile(mContent.FullName, remotePath);
                }
            }
            if (delAfterUpload)
            {
                Directory.Delete(m.Path, true);
            }
            client.Disconnect();
        }
    }
}
