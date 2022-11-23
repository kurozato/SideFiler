using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSugar.Model;
using BlackSugar.WinApi;
using BlackSugar.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackSugar.Repository.Tests
{
    [TestClass]
    public class UTStorageItemFactory
    {
        [TestMethod]
        public void UTBaseInstance()
        {
            var instance = new StorageItemFactory().BaseInstance(string.Empty);
            Assert.IsInstanceOfType(instance, typeof(StorageItemBase));
            instance.IsInstanceOf<StorageItemBase>();
        }


        [TestMethod]
        public void UTStorageItemStartup()
        {
            var instance = new StorageItemFactory().CreateInstance(null);
            instance.IsInstanceOf<StorageItemStartup>();
            instance.UpperLayer().IsNull();
            instance.ToFileData().IsNull();
         }

        [TestMethod]
        public void UTStorageItemLocal()
        {
            var path = @"D:\";
            var instance = new StorageItemFactory().CreateInstance(path);

            instance.IsInstanceOf<StorageItemLocal>();
            instance.UpperLayer().IsNull();

            var file = instance.ToFileData();

            file.FullName.Is(path);
            file.IsDirectory.Is(true);
        }

        [TestMethod]
        public void UTStorageItemShared()
        {
            var path = @"\\DESKTOP-OSJJT9B\Share";
            var instance = new StorageItemFactory().CreateInstance(path);

            instance.IsInstanceOf<StorageItemShared>();
            instance.UpperLayer().IsInstanceOf<StorageItemServer>();

            var file = instance.ToFileData();

            file.FullName.Is(path);
            file.IsDirectory.Is(true);
        }

        [TestMethod]
        public void UTStorageItemServer()
        {
            var path = @"\\DESKTOP-OSJJT9B";
            var instance = new StorageItemFactory().CreateInstance(path);

            instance.IsInstanceOf<StorageItemServer>();
            instance.UpperLayer().IsNull();

            var file = instance.ToFileData();

            file.FullName.Is(path);
            file.IsDirectory.Is(true);

        }

    }
}
