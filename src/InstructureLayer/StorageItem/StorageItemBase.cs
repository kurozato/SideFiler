using BlackSugar.Model;
using BlackSugar.WinApi;

namespace BlackSugar.Repository
{
    public class StorageItemBase 
    {
        protected string FullName { get; }

        public StorageItemBase(string path)
        {
            FullName = path ?? throw new ArgumentNullException(nameof(path));
        }

        public bool IsNetworkRoot 
            => System.Text.RegularExpressions.Regex.IsMatch(FullName, @"^(\\\\[^\\]+([^\\]|\\))$");

        public bool IsUnc => new Uri(FullName).IsUnc;

        public bool IsRoot => Path.GetPathRoot(FullName)?.TrimEnd('\\') == FullName.TrimEnd('\\');

        public virtual IFileData? ToFileData() => FileUtil.Create(FullName);  
    }
}
