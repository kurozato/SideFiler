using BlackSugar.Extension;
using BlackSugar.Model;
using BlackSugar.Service.Model;
using BlackSugar.Wpf;
using SideFiler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlackSugar.Views
{
    public interface IMainViewModel
    {
        //Service.Model.FileResultModel? SideItem { get; set; }
        //List<Service.Model.FileResultModel> SideItemsMirror { get; }
        //ObservableCollection<Service.Model.FileResultModel> SideItems { get; set; }

        //IEnumerable<UIFileData?>? FileItems { get; set; }
        ObservableCollection<UIFileData?> FileItems { get; set; }
        List<UIFileResultModel> SideItemsMirror { get; }
        ObservableCollection<UIFileResultModel> SideItems { get; set; }
        IEnumerable<object>? SelectedFiles { get; set; }
        UIFileData? SelectedFile { get; set; }
  
        UIFileResultModel? SideItem { get; set; }
        int SideIndex { get; set; }   
        string? FullPath { get; set; }
        long MaxID { get; set; }
        long? ID { get; set; }
        string? MainFilter { get; set; }
        string? SideFilter { get; set; }

        DelegateCommand CloseCommand { get; }
        DelegateCommand SelectMainCommand { get; }
        DelegateCommand AddCommand { get; }
        DelegateCommand<UIFileResultModel?> TabCloseCommand { get; }
        DelegateCommand ReloadCommand { get; }
        DelegateCommand UpFolderCommand { get; }
        DelegateCommand OpenExplorerCommand { get; }
        DelegateCommand PathEnterCommand { get; }
        DelegateCommand<IntPtr> RenameCommand { get; }
        DelegateCommand CreateFolderCommand { get; }
        DelegateCommand CopyCommand { get; }
        DelegateCommand CutCommand { get; }
        DelegateCommand<IntPtr> PasteCommand { get; }
        DelegateCommand SaveFileMenuCommand { get; }
        DelegateCommand OpenFileMenuCommand { get; }
        DelegateCommand OpenNewTabCommand { get; }
        DelegateCommand MainFilterCommand { get; }
        DelegateCommand MainFilterReleaseCommand { get; }
        DelegateCommand SideFilterCommand { get; }
        DelegateCommand SideFilterReleaseCommand { get; }

    }

    public class MainViewModel : BindableBase, IMainViewModel
    {
        //private IEnumerable<UIFileData?>? fileItems;

        //public IEnumerable<UIFileData?>? FileItems
        //{
        //    get => fileItems;
        //    set => SetProperty(ref fileItems, value);
        //}

        public ObservableCollection<UIFileData?> FileItems { get; set; }

        //public Service.Model.FileResultModel? SideItem { get; set; }
        //public List<Service.Model.FileResultModel> SideItemsMirror { get; }
        //public ObservableCollection<Service.Model.FileResultModel> SideItems { get; set; }

        public List<UIFileResultModel> SideItemsMirror { get; }
        public ObservableCollection<UIFileResultModel> SideItems { get; set; }

        public IEnumerable<object>? SelectedFiles { get; set; }

        public UIFileData? SelectedFile { get; set; }

        public UIFileResultModel? SideItem { get; set; }

        private int sideIndex;

        public int SideIndex
        {
            get => sideIndex;
            set
            {
                if (SetProperty(ref sideIndex, value))
                    ChangeProperty();
            }
        }

        private string? fullPath;

        public string? FullPath
        {
            get => fullPath;
            set => SetProperty(ref fullPath, value);
        }

        private  void ChangeProperty()
        {
            if (SideIndex < 0) return;
            var model = SideItems[SideIndex];
            SideItem = model;
            FullPath = model?.File?.FullName;

            UIHelper.Refill(FileItems, model?.Results);
        }

        public long? ID { get; set; }

        private string? mainFilter;
        public string? MainFilter
        {
            get => mainFilter;
            set => SetProperty(ref mainFilter, value);
        }

        private string? sideFilter;
        public string? SideFilter
        {
            get => sideFilter;
            set => SetProperty(ref sideFilter, value);
        }

        //public DelegateCommand Command { get; }
        //public Action? Action { get; set; }

        public long MaxID { get; set; }

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand SelectMainCommand { get; }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<UIFileResultModel?> TabCloseCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand UpFolderCommand { get; }
        public DelegateCommand OpenExplorerCommand { get; }
        public DelegateCommand PathEnterCommand { get; }
        public DelegateCommand<IntPtr> RenameCommand { get; }
        public DelegateCommand CreateFolderCommand { get; }
        public DelegateCommand CopyCommand { get; }
        public DelegateCommand CutCommand { get; }
        public DelegateCommand<IntPtr> PasteCommand { get; }
        public DelegateCommand SaveFileMenuCommand { get; }
        public DelegateCommand OpenFileMenuCommand { get; }
        public DelegateCommand OpenNewTabCommand { get; }
        public DelegateCommand MainFilterCommand { get; }
        public DelegateCommand MainFilterReleaseCommand { get; }
        public DelegateCommand SideFilterCommand { get; }
        public DelegateCommand SideFilterReleaseCommand { get; }

        public Action? SelectMainAction { get; set; }
        public Action? AddAction { get; set; }
        public Action<UIFileResultModel?>? TabCloseAction { get; set; }
        public Action? ReloadAction { get; set; }
        public Action? UpFolderAction { get; set; }
        public Action? OpenExplorerAction { get; set; }
        public Action? PathEnterAction { get; set; }
        public Action<IntPtr>? RenameAction { get; set; }
        public Action? CreateFolderAction { get; set; }
        public Action<Effect>? CopyCutAction { get; set; }
        public Action<IntPtr>? PasteAction { get; set; }
        public Action? SaveFileMenuAction { get; set; }
        public Action? OpenFileMenuAction { get; set; }
        public Action? OpenNewTabAction { get; set; }
        public Action? MainFilterAction { get; set; }
        public Action? MainFilterReleaseAction { get; set; }
        public Action? SideFilterAction { get; set; }
        public Action? SideFilterReleaseAction { get; set; }


        public MainViewModel()
        {
            sideIndex = -1;
            SideItems = new ObservableCollection<UIFileResultModel>();
            SideItemsMirror = new List<UIFileResultModel>();
            FileItems = new ObservableCollection<UIFileData?>();

            CloseCommand = new DelegateCommand(() => Application.Current.Shutdown());
           
            SelectMainCommand = new DelegateCommand(() => SelectMainAction?.Invoke());
            AddCommand = new DelegateCommand(() => AddAction?.Invoke());
            OpenNewTabCommand = new DelegateCommand(() => OpenNewTabAction?.Invoke());
            TabCloseCommand = new DelegateCommand<UIFileResultModel?>((item) => TabCloseAction?.Invoke(item));
            ReloadCommand = new DelegateCommand(() => ReloadAction?.Invoke(), () => SideItem?.File != null);
            UpFolderCommand = new DelegateCommand(() => UpFolderAction?.Invoke());
            OpenExplorerCommand = new DelegateCommand(() => OpenExplorerAction?.Invoke());
            PathEnterCommand = new DelegateCommand(() => PathEnterAction?.Invoke());

            RenameCommand = new DelegateCommand<IntPtr>((handle) => RenameAction?.Invoke(handle), (handle) => SelectedFile != null && SelectedFile.ExAttributes.HasFlag(ExFileAttributes.None));
            CreateFolderCommand = new DelegateCommand(() => CreateFolderAction?.Invoke(), () => SideItem?.File != null && ! SideItem.File.ExAttributes.HasFlag(ExFileAttributes.Server));
            CopyCommand = new DelegateCommand(() => CopyCutAction?.Invoke(Effect.Copy),()=> SelectedFiles != null && SelectedFiles.Any());
            CutCommand = new DelegateCommand(() => CopyCutAction?.Invoke(Effect.Move), () => SelectedFiles != null && SelectedFiles.Any());
            PasteCommand = new DelegateCommand<IntPtr>((handle) => PasteAction?.Invoke(handle), (handle) => SideItem?.File != null);

            SaveFileMenuCommand = new DelegateCommand(() => SaveFileMenuAction?.Invoke());
            OpenFileMenuCommand = new DelegateCommand(() => OpenFileMenuAction?.Invoke());

            MainFilterCommand = new DelegateCommand(() => MainFilterAction?.Invoke());
            MainFilterReleaseCommand = new DelegateCommand(() => MainFilterReleaseAction?.Invoke(), () => MainFilter?.Length > 0);
            SideFilterCommand = new DelegateCommand(() => SideFilterAction?.Invoke());
            SideFilterReleaseCommand = new DelegateCommand(() => SideFilterReleaseAction?.Invoke(), () => SideFilter?.Length > 0);
        }
    }
}
