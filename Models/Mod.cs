using System.IO;

namespace SkyrimNX_ModManager.Models
{

    class Mod
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public FileInfo[] Contents(SearchOption options = SearchOption.AllDirectories)
        {
            return new DirectoryInfo(Path).GetFiles("*", options);
        }   
        public Mod(string Name, string Path)
        {
            this.Name = Name;
            this.Path = Path;
        }
    }
}
