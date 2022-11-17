using BlackSugar.Model;

namespace BlackSugar.Repository
{
    public interface IStorageItem
    {
        IEnumerable<IFileData> GetDatas();

        IEnumerable<IFileData> SortDatas(IEnumerable<IFileData>  fileDatas);

        IStorageItem? UpperLayer();

        IFileData? ToFileData();
    }
}