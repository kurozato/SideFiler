using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Repository
{
    public interface IStorageItemFactory
    {
        IStorageItem CreateInstance(string? path);

        StorageItemBase BaseInstance(string path);
    }

    public class StorageItemFactory : IStorageItemFactory
    {
        public StorageItemBase BaseInstance(string path) => new StorageItemBase(path);

        public IStorageItem CreateInstance(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return new StorageItemStartup();

            var item = new StorageItemBase(path);
            if (item.IsNetworkRoot)
                return new StorageItemServer(path);

            if (item.IsUnc)
                return new StorageItemShared(path);

            return new StorageItemLocal(path);
            
        }
    }
}
