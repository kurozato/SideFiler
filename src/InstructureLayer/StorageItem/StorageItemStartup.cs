using BlackSugar.Model;
using BlackSugar.WinApi;
using System.Linq;

namespace BlackSugar.Repository
{
    public class StorageItemStartup : IStorageItem
    {
        //TODO:'string.Empty' is not good
        //public StorageItemStartup() : base(string.Empty) { }
        public IEnumerable<IFileData> GetDatas()
        {
            return Enumerable.Empty<IFileData>()
                .Concat(DirectoryUtil.EnumerateDriveData())
                .Concat(DirectoryUtil.EnumerateSpecsialFolderData());
        }

        public IEnumerable<IFileData> SortDatas(IEnumerable<IFileData> fileDatas)
            => fileDatas.OrderByDescending(f => f.ExAttributes).NaturallyThenBy(f => f.Name);

        public IStorageItem? UpperLayer() => null;

        public IFileData? ToFileData() => null;
    }
}
