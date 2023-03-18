using BlackSugar.Model;
using BlackSugar.WinApi;

namespace BlackSugar.Repository
{
    public class StorageItemServer : StorageItemBase ,IStorageItem
    {
        public StorageItemServer(string path) : base(path) { }

        public IEnumerable<IFileData> GetDatas() => NetShare.GetShareData(FullName);

        public IEnumerable<IFileData> SortDatas(IEnumerable<IFileData> fileDatas)
            => fileDatas.OrderBy(f => f.IsFile).NaturallyThenBy(f => f.Name);

        public IStorageItem? UpperLayer() => null;

        public override IFileData? ToFileData() => FileUtil.Create(FullName, true);       
    }
}
