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
        public FileTransfer(bool delAfterUpload = false)
        {
            client = new FtpClient(Settings.Default.SwitchIp, Settings.Default.SwitchUser, Settings.Default.SwitchPassword, Settings.Default.SwitchPort);
            this.delAfterUpload = delAfterUpload;
        }

        public void Upload(Mod[] mList)
        {
            client.AutoConnect();
            foreach(Mod m in mList)
            {
                Console.WriteLine(m.Name);
                foreach (FileInfo mContent in m.Contents())
                {
                    string remotePath = Settings.Default.SkyrimNXDirectory + "/" + mContent.Name;
                    if (!client.FileExists(remotePath))
                    {
                        client.UploadFile(mContent.FullName, remotePath);
                    }
                }
                if(delAfterUpload)
                {
                    Directory.Delete(m.Path, true);
                }
            }
            client.Disconnect();
        }
    }
}
