using BlackSugar.Model;
using BlackSugar.WinApi;

namespace BlackSugar.Repository
{
    public class StorageItemStartup : StorageItemBase, IStorageItem
    {
        //TODO:'string.Empty' is not good
        public StorageItemStartup() : base(string.Empty) { }
        public IEnumerable<IFileData> GetDatas()
        {
            return DirectoryUtil.EnumerateDriveData()
                .Concat(DirectoryUtil.EnumerateSpecsialFolderData());
        }

        public IEnumerable<IFileData> SortDatas(IEnumerable<IFileData> fileDatas)
            => fileDatas.OrderByDescending(f => f.IsDrive).NaturallyThenBy(f => f.Name);

        public IStorageItem? UpperLayer() => null;

        public override IFileData? ToFileData() => null;
    }
}
