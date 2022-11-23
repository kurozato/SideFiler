using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlackSugar.Service;
using BlackSugar.Presenters;
using NLog;
using BlackSugar.SimpleMvp;
using Microsoft.Extensions.DependencyInjection;
using BlackSugar.Views;
using SideFiler;
using BlackSugar.Model;
using BlackSugar.Service.Model;
using MahApps.Metro.Controls;
using BlackSugar.WinApi;
using BlackSugar.Extension;
using System.Linq;

namespace BlackSugar.Presenters.Tests
{
    [TestClass]
    public class UTPresenters
    {
        MainPresenter mainPresenter;
        InputNamePresenter inputNamePresenter;

        Mock<ISideFilerService> _serviceMock;
        Mock<ILogger> _loggerMock;
        Mock<IView<IMainViewModel>> _mainViewMock;
        Mock<IView<InputNameViewModel>> _inputViewMock;
        Mock<IClipboardHelper> _clipboardMock;

        [TestInitialize]
        public void Init()
        {
            //mock
            _serviceMock = new Mock<ISideFilerService>();
            _loggerMock = new Mock<ILogger>();
            _inputViewMock = new Mock<IView<InputNameViewModel>>();
            _mainViewMock = new Mock<IView<IMainViewModel>>();
            _clipboardMock = new Mock<IClipboardHelper>();

            var resolver = new DependencyResolver();
            resolver.Set(services =>
            {

                services.AddSingleton(p => _loggerMock.Object);
                services.AddSingleton(p => _serviceMock.Object);
                services.AddSingleton(p => _clipboardMock.Object);

                //presenter
                services.AddSingleton<IPresenter<IMainViewModel>, MainPresenter>();
                services.AddSingleton<IPresenter<InputNameViewModel>, InputNamePresenter>();

                //viewModel
                services.AddSingleton<IMainViewModel, MainViewModel>();
                services.AddSingleton<InputNameViewModel>();

                //view
                services.AddSingleton(p => _mainViewMock.Object);
                services.AddTransient(p => _inputViewMock.Object);

            });

            Router.Configure(resolver);

            AssociatedIcon.SetCacheSource(AssociatedIcon.KEY_FOLDER, null);

            Router.To<IMainViewModel>();
            Router.To<InputNameViewModel>();

            mainPresenter = Router.Resolver.Resolve<IPresenter<IMainViewModel>>() as MainPresenter;
            inputNamePresenter = Router.Resolver.Resolve<IPresenter<InputNameViewModel>>() as InputNamePresenter;
        }

        private FileResultModel GetTestModelData(int count, FileAttributes attributes)
        {
            var model = new FileResultModel();
            var impl = new List<IFileData>();
            for (var i = 0; i < count; i++)
                impl.Add(new FakeFileData() { Attributes = attributes });

            model.Results = impl;

            return model;
        }

        [TestMethod]
        public async Task AddResultTest()
        {
            var vm = mainPresenter.ViewModel;


            vm.SideItems.Count.Is(0);
            vm.SideItemsMirror.Count.Is(0);

            await mainPresenter.AddResult();

            vm.SideItems.Count.Is(1);
            vm.SideItemsMirror.Count.Is(1);
        }

        [TestMethod]
        public async Task OpenNewTabResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { FullName = @"C:\TEST☆DAZE" };

            vm.SelectedFile = new UIFileData(file);

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);
            _serviceMock
              .Setup(s => s.Open(It.IsAny<FileResultModel>()))
              .Callback<FileResultModel>(f => f.Results = GetTestModelData(10, FileAttributes.Directory).Results)
              .Returns(true);


            vm.SelectedFile = new UIFileData(file);

            vm.SideItems.Count.Is(0);
            vm.SideItemsMirror.Count.Is(0);

            await mainPresenter.OpenNewTabResult();

            vm.SideItems.Count.Is(1);
            vm.SideItemsMirror.Count.Is(1);

        }

        [TestMethod]
        public void TabCloseResultTest()
        {
            var item = new UIFileResultModel();

            var vm = mainPresenter.ViewModel;
            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItems.Add(item);
            vm.SideItems.Add(new UIFileResultModel());

            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(item);
            vm.SideItemsMirror.Add(new UIFileResultModel());

            mainPresenter.TabCloseResult(item);

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);
            vm.SideItems.Contains(item).Is(false);
            vm.SideItemsMirror.Contains(item).Is(false);
        }

        [TestMethod]
        public async Task SelectMainResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { FullName = @"C:\TEST☆DAZE" };

            vm.SelectedFile = new UIFileData(file);

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);
            _serviceMock
                .Setup(s => s.Open(It.IsAny<FileResultModel>()))
                .Callback<FileResultModel>(f => f.Results = GetTestModelData(10, FileAttributes.Directory).Results)
                .Returns(true);

            //wpf
            vm.SideItems.CollectionChanged += (s, e) => { vm.SideIndex = -1; };

            vm.SelectedFile = new UIFileData(file);

            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideIndex = 0;

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);

            await mainPresenter.SelectMainResult();

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);
            vm.FileItems.Count.Is(10);

        }

        [TestMethod()]
        public async Task ReloadResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { FullName = @"C:\TEST☆DAZE" };

            vm.SideItem = new UIFileResultModel() { File = new UIFileData(file) };

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);

            _serviceMock
                .Setup(s => s.Open(It.IsAny<FileResultModel>()))
                .Callback<FileResultModel>(f => f.Results = GetTestModelData(10, FileAttributes.Directory).Results)
                .Returns(true);


            //wpf
            vm.SideItems.CollectionChanged += (s, e) => { vm.SideIndex = -1; };

            vm.SelectedFile = new UIFileData(file);

            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideIndex = 0;

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);

            await mainPresenter.ReloadResult();

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);
            vm.FileItems.Count.Is(10);

        }

        [TestMethod()]
        public async Task UpFolderResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { FullName = @"C:\TEST☆DAZE" };

            vm.SideItem = new UIFileResultModel() { File = new UIFileData(file) };

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);
            _serviceMock
                .Setup(s => s.Up(It.IsAny<FileResultModel>()))
                .Callback<FileResultModel>(f => f.Results = GetTestModelData(10, FileAttributes.Directory).Results)
                .Returns(true);

            //wpf
            vm.SideItems.CollectionChanged += (s, e) => { vm.SideIndex = -1; };

            vm.SelectedFile = new UIFileData(file);

            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideIndex = 0;

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);

            await mainPresenter.UpFolderResult();

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);
            vm.FileItems.Count.Is(10);

        }

        [TestMethod()]
        public void OpenExplorerResultTest()
        {
            mainPresenter.OpenExplorerResult();

            _serviceMock.Verify(s => s.OpenExplorer(It.IsAny<IFileData>()), Times.Once);
        }

        [TestMethod()]
        public async Task PathEnterResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { FullName = @"C:\TEST☆DAZE" };

            vm.FullPath = file.FullName;

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);
            _serviceMock
                .Setup(s => s.Open(It.IsAny<FileResultModel>()))
                .Callback<FileResultModel>(f => f.Results = GetTestModelData(10, FileAttributes.Directory).Results)
                .Returns(true);

            //wpf
            vm.SideItems.CollectionChanged += (s, e) => { vm.SideIndex = -1; };

            vm.SelectedFile = new UIFileData(file);

            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItems.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideItemsMirror.Add(new UIFileResultModel());
            vm.SideIndex = 0;

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);

            await mainPresenter.PathEnterResult();

            vm.SideItems.Count.Is(2);
            vm.SideItemsMirror.Count.Is(2);
            vm.FileItems.Count.Is(10);
        }

        [TestMethod()]
        public void RenameResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { Name = "TEST☆DAZE", FullName = @"C:\TEST☆DAZE" };

            vm.SelectedFile = new UIFileData(file);

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);

            _inputViewMock
                .Setup(v => v.ShowDialog())
                .Callback(() => { inputNamePresenter.ViewModel.Name = "TEST"; })
                .Returns(true);

            mainPresenter.RenameResult(IntPtr.Zero);

            _serviceMock.Verify(s => s.Rename(file, "TEST", It.IsAny<FileResultModel>(), IntPtr.Zero), Times.Once);
        }

        [TestMethod()]
        public void CreateFolderResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { FullName = @"C:\TEST☆DAZE" };

            vm.SideItem = new UIFileResultModel() { File = new UIFileData(file) };

            _serviceMock.Setup(s => s.GetFileData(@"C:\TEST☆DAZE")).Returns(file);

            _inputViewMock
                .Setup(v => v.ShowDialog())
                .Callback(() => { inputNamePresenter.ViewModel.Name = "TEST"; })
                .Returns(true);

            mainPresenter.CreateFolderResult();

            _serviceMock.Verify(s => s.CreateFolder(It.IsAny<FileResultModel>(), "TEST"), Times.Once);

        }

        [TestMethod()]
        public void MainFilterResultTest()
        {
            var vm = mainPresenter.ViewModel;

            vm.MainFilter = "TEST";
            var model = GetTestModelData(4, FileAttributes.Directory);

            vm.SideItem = new UIFileResultModel() { Results = new System.Collections.ObjectModel.ObservableCollection<UIFileData>() };

            UIHelper.Refill(vm.SideItem.Results, model.Results.Select(f => new UIFileData(new FakeFileData() { FullName = @"D:\Work\Test" })), false);
            UIHelper.Refill(vm.SideItem.Results, model.Results.Select(f => new UIFileData(new FakeFileData() { FullName = @"D:\Work\1" })), false);

            mainPresenter.MainFilterResult();

            vm.FileItems.Count.Is(4);

        }

        [TestMethod()]
        public void MainFilterReleaseResultTest()
        {
            var vm = mainPresenter.ViewModel;

            vm.MainFilter = "TEST";
            var model = GetTestModelData(4, FileAttributes.Directory);

            vm.SideItem = new UIFileResultModel() { Results = new System.Collections.ObjectModel.ObservableCollection<UIFileData>() };

            UIHelper.Refill(vm.SideItem.Results, model.Results.Select(f => new UIFileData(new FakeFileData() { FullName = @"D:\Work\Test" })), false);
            UIHelper.Refill(vm.SideItem.Results, model.Results.Select(f => new UIFileData(new FakeFileData() { FullName = @"D:\Work\1" })), false);

            mainPresenter.MainFilterResult();

            vm.FileItems.Count.Is(4);

            mainPresenter.MainFilterReleaseResult();

            vm.FileItems.Count.Is(8);

        }

        [TestMethod()]
        public void SideFilterResultTest()
        {
            var vm = mainPresenter.ViewModel;

            vm.SideFilter = "TEST";
            var model = GetTestModelData(4, FileAttributes.Directory);

            foreach (var item in model.Results.Select(f => new UIFileResultModel() { File = new UIFileData(new FakeFileData() { FullName = @"D:\Work\Test" }) }))
            {
                vm.SideItems.Add(item);
                vm.SideItemsMirror.Add(item);
            }
            foreach (var item in model.Results.Select(f => new UIFileResultModel() { File = new UIFileData(new FakeFileData() { FullName = @"D:\Work\1" }) }))
            {
                vm.SideItems.Add(item);
                vm.SideItemsMirror.Add(item);
            }

            mainPresenter.SideFilterResult();

            vm.SideItems.Count.Is(4);

        }

        [TestMethod()]
        public void SideFilterReleaseResultTest()
        {
            var vm = mainPresenter.ViewModel;

            vm.SideFilter = "TEST";
            var model = GetTestModelData(4, FileAttributes.Directory);

            foreach (var item in model.Results.Select(f => new UIFileResultModel() { File = new UIFileData(new FakeFileData() { FullName = @"D:\Work\Test" }) }))
            {
                vm.SideItems.Add(item);
                vm.SideItemsMirror.Add(item);
            }
            foreach (var item in model.Results.Select(f => new UIFileResultModel() { File = new UIFileData(new FakeFileData() { FullName = @"D:\Work\1" }) }))
            {
                vm.SideItems.Add(item);
                vm.SideItemsMirror.Add(item);
            }

            mainPresenter.SideFilterResult();

            vm.SideItems.Count.Is(4);

            mainPresenter.SideFilterReleaseResult();

            vm.SideItems.Count.Is(8);

        }

        [TestMethod()]
        public void CopyCutResultTest()
        {
            var vm = mainPresenter.ViewModel;
            var file = new FakeFileData() { Name = "TEST☆DAZE", FullName = @"C:\TEST☆DAZE" };

            var model = GetTestModelData(4, FileAttributes.Normal);

            var items = new List<object>(model.Results.Select(f => new UIFileData(file)));
            vm.SelectedFiles = items;

            mainPresenter.CopyCutResult(Effect.Copy);

            _clipboardMock.Verify(c => c.SetFiles(items.Cast<UIFileData>(), Effect.Copy), Times.Once);
        }

        [TestMethod()]
        public void PasteResultTest()
        {
            var data = new string[] { "TEST1", "TEST2", "TEST3" };
            _clipboardMock.Setup(c => c.GetFiles()).Returns(data);
            _clipboardMock.Setup(c => c.GetDropEffect()).Returns(Effect.Copy);

            mainPresenter.PasteResult(IntPtr.Zero);

            _serviceMock.Verify(s => s.CopyOrMove(It.IsAny<FileResultModel>(), IntPtr.Zero, data, Effect.Copy), Times.Once);
        }

        [TestMethod()]
        public async Task ExpandResultTest()
        {
            var vm = mainPresenter.ViewModel;

            var model = GetTestModelData(4, FileAttributes.Directory);
          
            var path = "";
            _serviceMock.Setup(s => s.GetData(path)).Returns(model.Results.Select(f => new FileResultModel() { File = new FakeFileData() { FullName = @"D:\Work\Test" } }));
            _serviceMock.Setup(s => s.Open(It.IsAny<FileResultModel>())).Returns(true);

            await mainPresenter.ExpandResult(path);

            vm.SideItems.Count.Is(4);
            vm.SideItemsMirror.Count.Is(4);
        }
    }
}