using BlackSugar.Model;
using BlackSugar.Service.Model;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlackSugar.Views
{
    public class SettingsViewModel : BindableBase
    {

        /* General */

        public ObservableCollection<UIComboBoxModel> Languages { get; set; }
        public UIComboBoxModel? Language { get; set; }

        public ObservableCollection<UIComboBoxModel> Themes { get; set; }
        public UIComboBoxModel? Theme { get; set; } 

        /* Bookmarks */

        public ObservableCollection<UIBookmarkModel> Bookmarks { get; set; }

        private UIBookmarkModel? selectedBookmark;
        public UIBookmarkModel? SelectedBookmark {
            get => selectedBookmark;
            set => SetProperty(ref selectedBookmark, value);
        }

        private int bookmarksIndex;
        public int BookmarksIndex
        {
            get => bookmarksIndex;
            set => SetProperty(ref bookmarksIndex, value);
        }

        /*ContextMenus */

        public ObservableCollection<UIContextMenuModel> ContextMenus { get; set; }

        private UIContextMenuModel? selectedContextMenu;
        public UIContextMenuModel? SelectedContextMenu
        {
            get => selectedContextMenu;
            set => SetProperty(ref selectedContextMenu, value);
        }

        private int contextMenusIndex;
        public int ContextMenusIndex
        {
            get => contextMenusIndex;
            set => SetProperty(ref contextMenusIndex, value);
        }

        /*** Command ***/

        //public DelegateCommand Command { get; }
        //public Action? Action { get; set; }


        public DelegateCommand<string> AddCommand { get; }
        public DelegateCommand<string> EditCommand { get; }
        public DelegateCommand<string> CommitCommand { get; }
        public DelegateCommand VisitCommand { get; }

        public Action<string>? AddAction { get; set; }
        public Action<string>? EditAction { get; set; }
        public Action<string>? CommitAction { get; set; }
        public Action? VisitAction { get; set; }


        public SettingsViewModel()
        {
            Languages = new ObservableCollection<UIComboBoxModel>() { 
                new UIComboBoxModel("English", "en"), 
                new UIComboBoxModel("日本語","ja-JP")
            };

            Themes = new ObservableCollection<UIComboBoxModel>();

            Bookmarks = new ObservableCollection<UIBookmarkModel>();
            ContextMenus = new ObservableCollection<UIContextMenuModel>();

            AddCommand = new DelegateCommand<string>((tag) => AddAction?.Invoke(tag));
            EditCommand = new DelegateCommand<string>((tag) => EditAction?.Invoke(tag));
            CommitCommand = new DelegateCommand<string>((tag) => CommitAction?.Invoke(tag));
            VisitCommand = new DelegateCommand(() => VisitAction?.Invoke());
        }
    }
}
