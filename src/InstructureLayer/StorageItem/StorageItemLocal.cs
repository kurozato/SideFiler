using BlackSugar.Model;
using BlackSugar.WinApi;

namespace BlackSugar.Repository
{
    public class StorageItemLocal : StorageItemBase, IStorageItem
    {
        public StorageItemLocal(string path) : base(path) { }
        public IEnumerable<IFileData> GetDatas()
            => DirectoryUtil.EnumerateFileSystemEntriesData(FullName)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));

        public IEnumerable<IFileData> SortDatas(IEnumerable<IFileData> fileDatas)
            => fileDatas.OrderBy(f => f.IsFile).NaturallyThenBy(f => f.Name);

        public IStorageItem? UpperLayer()
        {
            if (IsRoot) return null;

            var path = Path.GetDirectoryName(FullName);
            return new StorageItemLocal(path!);
        }
    }
}
