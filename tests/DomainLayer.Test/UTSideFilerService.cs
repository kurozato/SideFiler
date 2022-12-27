using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlackSugar.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSugar.Repository;
using Moq;
using BlackSugar.Service.Model;
using BlackSugar.Model;
using System.Runtime.CompilerServices;

namespace BlackSugar.Service.Tests
{
    [TestClass()]
    public class UTSideFilerService
    {
        SideFilerService _sideFilerService;
        Mock<IStorageItemFactory> _factoryMock;
        Mock<IDbCommander> _commanderMock;
        Mock<IJsonAdpter> _adpterMock;
        Mock<IFileOperator> _operatorMock;
        Mock<IStorageItem> _itemMock;

        [TestInitialize]
        public void Before()
        {
            _factoryMock = new Mock<IStorageItemFactory>();
            _commanderMock = new Mock<IDbCommander>();
            _adpterMock = new Mock<IJsonAdpter>();
            _itemMock = new Mock<IStorageItem>();
            _operatorMock = new Mock<IFileOperator>();

            _sideFilerService = new SideFilerService(_factoryMock.Object, _commanderMock.Object, _adpterMock.Object, _operatorMock.Object);
        }

        [TestMethod()]
        public void StartupTest()
        {
            _factoryMock.Setup(m => m.CreateInstance(It.IsAny<string>())).Returns(_itemMock.Object);

            var model = new FileResultModel();
            _sideFilerService.Startup(model);

            _factoryMock.Verify(m => m.CreateInstance(null), Times.Once);
            _itemMock.Verify(m => m.GetDatas(), Times.Once);
            _itemMock.Verify(m => m.SortDatas(It.IsAny<IEnumerable<IFileData>>()), Times.Once);
        }

        [TestMethod()]
        public void OpenTestForNull()
        {
            var exp = _sideFilerService.Open(null);
            exp.Is(false);
        }

        [TestMethod()]
        public void OpenTestForFile()
        {
            _operatorMock.Setup(m => m.ExecuteOrMove(ref It.Ref<IFileData>.IsAny)).Returns(false);

            var exp = _sideFilerService.Open(
                new FileResultModel() { 
                    File = new FakeFileData() { IsFile = true, IsDirectory = false } });

           exp.Is(false);

            _factoryMock.Verify(m => m.CreateInstance(It.IsAny<string>()), Times.Never);
            _operatorMock.Verify(m => m.ExecuteOrMove(ref It.Ref<IFileData>.IsAny), Times.Once);
        }


        [TestMethod()]
        public void OpenTestForFolder()
        {
            _factoryMock.Setup(m => m.CreateInstance(It.IsAny<string>())).Returns(_itemMock.Object);

            var exp = _sideFilerService.Open(
                new FileResultModel() {
                    File = new FakeFileData() { IsFile = false, IsDirectory = true }
                });

            Assert.IsTrue(exp);

            _factoryMock.Verify(m => m.CreateInstance(null), Times.Once);
            _itemMock.Verify(m => m.GetDatas(), Times.Once);
            _itemMock.Verify(m => m.SortDatas(It.IsAny<IEnumerable<IFileData>>()), Times.Once);
        }

        [TestMethod()]
        public void UpTestForNull()
        {
            var exp = _sideFilerService.Up(null);
            exp.Is(false);
        }

        [TestMethod()]
        public void UpTestForUpperItemIsNull()
        {
            _factoryMock.Setup(m => m.CreateInstance(It.IsAny<string>())).Returns(_itemMock.Object);
            _itemMock.Setup(m => m.UpperLayer()).Returns(null as IStorageItem);

            var exp = _sideFilerService.Up(new FileResultModel() {
                File = new FakeFileData() { IsFile = false, IsDirectory = true }
            });

            exp.Is(false);

            _factoryMock.Verify(m => m.CreateInstance(It.IsAny<string>()), Times.Once);
            _itemMock.Verify(m => m.UpperLayer(), Times.Once);
        }

        [TestMethod()]
        public void UpTestForNomal()
        {
            var upperItemMock = new Mock<IStorageItem>();
            _factoryMock.Setup(m => m.CreateInstance(It.IsAny<string>())).Returns(_itemMock.Object);
            _itemMock.Setup(m => m.UpperLayer()).Returns(upperItemMock.Object);

            var exp = _sideFilerService.Up(new FileResultModel() {
                File = new FakeFileData() { IsFile = false, IsDirectory = true }
            });

            exp.Is(true);

            _factoryMock.Verify(m => m.CreateInstance(It.IsAny<string>()), Times.Once);
            _itemMock.Verify(m => m.UpperLayer(), Times.Once);

            upperItemMock.Verify(m => m.ToFileData(), Times.Once);
            upperItemMock.Verify(m => m.GetDatas(), Times.Once);
            upperItemMock.Verify(m => m.SortDatas(It.IsAny<IEnumerable<IFileData>>()), Times.Once);
        }
    }
}