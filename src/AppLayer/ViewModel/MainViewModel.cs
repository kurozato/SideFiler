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
        ObservableCollection<UIBookmarkModel?> Bookmarks { get; set; }
        ObservableCollection<UIContextMenuModel> ContextMenus { get; set; }
        ObservableCollection<UIFileData>? FileItems { get; set; }
        ObservableCollection<UIFileResultModel> SideItems { get; set; }
        IEnumerable<object>? SelectedFiles { get; set; }
        UIFileData? SelectedFile { get; set; }
        int SelectedIndex { get; set; }

        UIFileResultModel? SideItem { get; set; }
        int SideIndex { get; set; }
        string? FullPath { get; set; }
        long MaxID { get; set; }
        long? ID { get; set; }
        string? MainFilter { get; set; }
        string? SideFilter { get; set; }
        IntPtr Handle { get; set; }

        DelegateCommand CloseCommand { get; }
        DelegateCommand SelectMainCommand { get; }
        DelegateCommand AddCommand { get; }
        DelegateCommand<UIFileResultModel?> TabCloseCommand { get; }
        DelegateCommand ReloadCommand { get; }
        DelegateCommand UpFolderCommand { get; }
        DelegateCommand OpenExplorerCommand { get; }
        DelegateCommand PathEnterCommand { get; }
        DelegateCommand DeleteCommand { get; }
        DelegateCommand RenameCommand { get; }
        DelegateCommand CreateFolderCommand { get; }
        DelegateCommand CopyCommand { get; }
        DelegateCommand CutCommand { get; }
        DelegateCommand PasteCommand { get; }
        DelegateCommand SaveFileMenuCommand { get; }
        DelegateCommand OpenFileMenuCommand { get; }
        DelegateCommand OpenNewTabCommand { get; }
        DelegateCommand MainFilterCommand { get; }
        DelegateCommand MainFilterReleaseCommand { get; }
        DelegateCommand SideFilterCommand { get; }
        DelegateCommand SideFilterReleaseCommand { get; }
        DelegateCommand<string> ExpandCommand { get; }
        DelegateCommand<string[]> DropFileCommand { get; }
        DelegateCommand<UIContextMenuModel> ContextMenuCommand { get; }
        DelegateCommand AdjustMenuCommand { get; }
        DelegateCommand SettingMenuCommand { get; }
        DelegateCommand RecentlyCloseFolderCommand { get; }
        DelegateCommand AddBookmarkCommand { get; }
        DelegateCommand OpenTrashMenuCommand { get; }
        DelegateCommand OpenCmdMenuCommand { get; }
        DelegateCommand OpenDownloadMenuCommand { get; }
    }

    public class MainViewModel : BindableBase, IMainViewModel
    {
        private void AfterChangeSideIndex()
        {
            if (SideIndex < 0) return;
            var model = SideItems[SideIndex];
            SideItem = model;
            FullPath = model?.File?.FullName;
            FileItems = model?.ToObservableCollection();
            MainFilter = null;
            SelectedIndex = model.Index;
        }

        private void BeforeChangeSideIndex()
        {
            if (0 <= SideIndex && SideIndex < SideItems.Count)
                SideItems[sideIndex].Index = SelectedIndex;
        }

        private int sideIndex;
        public int SideIndex
        {
            get => sideIndex;
            set
            {
                BeforeChangeSideIndex();
                if (SetProperty(ref sideIndex, value))
                    AfterChangeSideIndex();
            }
        }

        public UIFileResultModel? SideItem { get; set; }

        private string? fullPath;
        public string? FullPath
        {
            get => fullPath;
            set => SetProperty(ref fullPath, value);
        }

        private ObservableCollection<UIFileData>? fileItems;
        public ObservableCollection<UIFileData>? FileItems
        {
            get => fileItems;
            set => SetProperty(ref fileItems, value);
        }

        public IntPtr Handle { get; set; }

        public ObservableCollection<UIFileResultModel> SideItems { get; set; }

        public IEnumerable<object>? SelectedFiles { get; set; }

        public UIFileData? SelectedFile { get; set; }


        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value);
        }

        public long? ID { get; set; }
        public long MaxID { get; set; }

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

        public ObservableCollection<UIBookmarkModel?> Bookmarks { get; set; }

        public ObservableCollection<UIContextMenuModel> ContextMenus { get; set; }

        //public DelegateCommand Command { get; }
        //public Action? Action { get; set; }

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand SelectMainCommand { get; }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<UIFileResultModel?> TabCloseCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand UpFolderCommand { get; }
        public DelegateCommand OpenExplorerCommand { get; }
        public DelegateCommand PathEnterCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RenameCommand { get; }
        public DelegateCommand CreateFolderCommand { get; }
        public DelegateCommand CopyCommand { get; }
        public DelegateCommand CutCommand { get; }
        public DelegateCommand PasteCommand { get; }
        public DelegateCommand SaveFileMenuCommand { get; }
        public DelegateCommand OpenFileMenuCommand { get; }
        public DelegateCommand OpenNewTabCommand { get; }
        public DelegateCommand MainFilterCommand { get; }
        public DelegateCommand MainFilterReleaseCommand { get; }
        public DelegateCommand SideFilterCommand { get; }
        public DelegateCommand SideFilterReleaseCommand { get; }
        public DelegateCommand<string> ExpandCommand { get; }
        public DelegateCommand<UIBookmarkModel> SelectBookmarkCommand { get; }
        public DelegateCommand<string[]> DropFileCommand { get; }
        public DelegateCommand<UIContextMenuModel> ContextMenuCommand { get; }
        public DelegateCommand AdjustMenuCommand { get; }
        public DelegateCommand SettingMenuCommand { get; }
        public DelegateCommand RecentlyCloseFolderCommand { get; }
        public DelegateCommand AddBookmarkCommand { get; }
        public DelegateCommand OpenTrashMenuCommand { get; }
        public DelegateCommand OpenCmdMenuCommand { get; }
        public DelegateCommand OpenDownloadMenuCommand { get; }

        public Action<UIFileResultModel?>? TabCloseAction { get; set; }
      
        public Action? OpenExplorerAction { get; set; }
        public Action? RenameAction { get; set; }
        public Action? CreateFolderAction { get; set; }
        public Action<Effect>? CopyCutAction { get; set; }
    
        public Action? SaveFileMenuAction { get; set; }
        public Action? OpenFileMenuAction { get; set; }   
        public Action? MainFilterAction { get; set; }
        public Action? MainFilterReleaseAction { get; set; }
        public Action? SideFilterAction { get; set; }
        public Action? SideFilterReleaseAction { get; set; }
        public Action<UIContextMenuModel>? ContextMenuAction { get; set; }
        public Action? SettingMenuAction { get; set; }
        public Action? RecentlyCloseFolderAction { get; set; }
        public Action? AddBookmarkAction { get; set; }

        public Func<Task>? AddAction { get; set; }
        public Func<Task>? ReloadAction { get; set; }
        public Func<Task>? UpFolderAction { get; set; }
        public Func<Task>? PathEnterAction { get; set; }
        public Func<Task>? OpenNewTabAction { get; set; }
        public Func<Task>? SelectMainAction { get; set; }
        public Func<string, Task>? ExpandAction { get; set; }
        public Func<UIBookmarkModel, Task>? SelectBookmarkAction { get; set; }
        public Func<Task>? PasteAction { get; set; }
        public Func<Task>? DeleteAction { get; set; }
        public Func<string[], Task>? DropFileAction { get; set; }

        public Action? AdjustMenuAction { get; set; }
        public Action? OpenTrashMenuAction { get; set; }
        public Action? OpenCmdMenuAction { get; set; }
        public Action? OpenDownloadMenuAction { get; set; }

        public MainViewModel()
        {
            sideIndex = -1;
            SideItems = new ObservableCollection<UIFileResultModel>();

            FileItems = new ObservableCollection<UIFileData>();
            Bookmarks = new ObservableCollection<UIBookmarkModel?>();
            ContextMenus = new ObservableCollection<UIContextMenuModel>();

            CloseCommand = new DelegateCommand(() => Application.Current.Shutdown());
          
            TabCloseCommand = new DelegateCommand<UIFileResultModel?>((item) => TabCloseAction?.Invoke(item));
            OpenExplorerCommand = new DelegateCommand(() => OpenExplorerAction?.Invoke(), () => SideItem?.File != null);
         
            RenameCommand = new DelegateCommand(() => RenameAction?.Invoke(), () => SelectedFile != null && !SelectedFile.ExAttributes.HasFlag(ExFileAttributes.SpecsialFolder));
            CreateFolderCommand = new DelegateCommand(() => CreateFolderAction?.Invoke(), () => SideItem?.File != null && ! SideItem.File.ExAttributes.HasFlag(ExFileAttributes.Server));
            CopyCommand = new DelegateCommand(() => CopyCutAction?.Invoke(Effect.Copy),()=> SelectedFiles != null && SelectedFiles.Any());
            CutCommand = new DelegateCommand(() => CopyCutAction?.Invoke(Effect.Move), () => SelectedFiles != null && SelectedFiles.Any());
            PasteCommand = new DelegateCommand(async () => await PasteAction?.Invoke(), () => SideItem?.File != null);
            DeleteCommand = new DelegateCommand(async () => await DeleteAction?.Invoke());

            SaveFileMenuCommand = new DelegateCommand(() => SaveFileMenuAction?.Invoke());
            OpenFileMenuCommand = new DelegateCommand(() => OpenFileMenuAction?.Invoke());

            MainFilterCommand = new DelegateCommand(() => MainFilterAction?.Invoke());
            MainFilterReleaseCommand = new DelegateCommand(() => MainFilterReleaseAction?.Invoke(), () => MainFilter?.Length > 0);
            SideFilterCommand = new DelegateCommand(() => SideFilterAction?.Invoke());
            SideFilterReleaseCommand = new DelegateCommand(() => SideFilterReleaseAction?.Invoke(), () => SideFilter?.Length > 0);

            
            AddCommand = new DelegateCommand(async () => await AddAction?.Invoke());
            ReloadCommand = new DelegateCommand(async () => await ReloadAction?.Invoke(), () => SideItem?.File != null);
            UpFolderCommand = new DelegateCommand(async () => await UpFolderAction?.Invoke());
            PathEnterCommand = new DelegateCommand(async () => await PathEnterAction?.Invoke());
            OpenNewTabCommand = new DelegateCommand(async () => await OpenNewTabAction?.Invoke());
            SelectMainCommand = new DelegateCommand(async () => await SelectMainAction?.Invoke());
            ExpandCommand = new DelegateCommand<string>(async (path) => await ExpandAction?.Invoke(path));
            SelectBookmarkCommand = new DelegateCommand<UIBookmarkModel>(async (bookmark) => await SelectBookmarkAction?.Invoke(bookmark));

            DropFileCommand = new DelegateCommand<string[]>(async (data) => await DropFileAction?.Invoke(data));
            ContextMenuCommand = new DelegateCommand<UIContextMenuModel>((menu) => ContextMenuAction?.Invoke(menu));            
            AdjustMenuCommand = new DelegateCommand(() => AdjustMenuAction?.Invoke());
            SettingMenuCommand = new DelegateCommand(()=> SettingMenuAction?.Invoke());
            RecentlyCloseFolderCommand = new DelegateCommand(() => RecentlyCloseFolderAction?.Invoke());
            AddBookmarkCommand = new DelegateCommand(() => AddBookmarkAction?.Invoke(), () => SideItem?.File != null);

            OpenTrashMenuCommand = new DelegateCommand(()=> OpenTrashMenuAction?.Invoke());
            OpenCmdMenuCommand = new DelegateCommand(() => OpenCmdMenuAction?.Invoke());
            OpenDownloadMenuCommand = new DelegateCommand(() => OpenDownloadMenuAction?.Invoke());
        }
    }
}
