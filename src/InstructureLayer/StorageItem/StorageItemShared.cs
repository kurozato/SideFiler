using BlackSugar.Model;
using BlackSugar.WinApi;

namespace BlackSugar.Repository
{
    public class StorageItemShared : StorageItemBase, IStorageItem
    {
        public StorageItemShared(string path) : base(path) { }
        public IEnumerable<IFileData> GetDatas() 
            => DirectoryUtil.EnumerateFileSystemEntriesData(FullName)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));

        public IEnumerable<IFileData> SortDatas(IEnumerable<IFileData> fileDatas)
            => fileDatas.OrderBy(f => f.IsFile).NaturallyThenBy(f => f.Name);

        public IStorageItem? UpperLayer()
        {
            string? path;
            if (IsRoot)
            {
                path = FullName[..(FullName[2..].IndexOf('\\') + 3)];
                return new StorageItemServer(path); ;
            }
            path = Path.GetDirectoryName(FullName);
            return new StorageItemShared(path!);
        }
    }
}
