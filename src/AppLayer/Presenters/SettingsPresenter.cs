using BlackSugar.Service;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;
using BlackSugar.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSugar.Service.Model;
using System.Collections.ObjectModel;

namespace BlackSugar.Presenters
{
    public class SettingsPresenter : PresenterBase<SettingsViewModel>
    {
        ILogger _logger;
        ISideFilerService _service;
        IExConfiguration _config;
        IUIInitializer _initializer;

        IView<SettingsViewModel>? View;

        public SettingsPresenter(ISideFilerService service, ILogger logger, IExConfiguration config, IUIInitializer initializer)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }

        protected override void InitializeView()
        {
        }


        private const string tagBookmark = "Bookmark";
        private const string tagContextMenu = "ContextMenu";

        private void SetSettingData()
        {
            var bookmarks = _service.GetBookmarksData(_config.GetFullPath(Literal.File_Json_Bookmarks));
            ViewModel.Bookmarks.Clear();
            foreach (var bookmark in bookmarks)
                ViewModel.Bookmarks.Add(new UIBookmarkModel(bookmark));

            var menus = _service.GetContextMenusData(_config.GetFullPath(Literal.Direcotry_ContextMenu, Literal.File_Json_ContextMenu));
            ViewModel.ContextMenus.Clear();
            foreach (var menu in menus)
                ViewModel.ContextMenus.Add(new UIContextMenuModel(menu));


            ViewModel.Language = ViewModel.Languages.SingleOrDefault(l => l.Value == _initializer.UISettingsModel.Language);
        
        }

        private void BuildComboBox()
        {
            ViewModel.Themes = new ObservableCollection<UIComboBoxModel>() {
                new UIComboBoxModel(ResourceService.Current.GetResource("Light"), "Light"),
                new UIComboBoxModel(ResourceService.Current.GetResource("Dark"),"Dark")
            };
            ViewModel.Theme = ViewModel.Themes.SingleOrDefault(l => l.Value == _initializer.UISettingsModel.ThemeName);
        }

        public void ShowMenuResult()
        {
            try
            {
                View = Router.To(this);
                BuildComboBox();
                SetSettingData();

                View.ShowDialog();
                View = null;
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void AddResult(string param)
        {
            try
            {
                switch (param)
                {
                    case tagBookmark:
                        ViewModel.Bookmarks.Add(new UIBookmarkModel());
                        ViewModel.BookmarksIndex = ViewModel.Bookmarks.Count - 1;
                        break;
                    case tagContextMenu:
                        ViewModel.ContextMenus.Add(new UIContextMenuModel());
                        ViewModel.ContextMenusIndex = ViewModel.ContextMenus.Count - 1;
                        break;

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void EditResult(string param)
        {
            try
            {
                var index = ViewModel.BookmarksIndex;
                ViewModel.Bookmarks.RemoveAt(index);
                ViewModel.Bookmarks.Insert(index, ViewModel.SelectedBookmark);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void CommitResult(string param)
        {
            try
            {
                object content;
                string file;
                switch (param)
                {
                    case tagBookmark:
                        content = ViewModel.Bookmarks.Select(u => u.GetEditBookmark());
                        file = _config.GetFullPath(Literal.File_Json_Bookmarks);
                        break;
                    case tagContextMenu:
                        content = ViewModel.ContextMenus.Select(u => u.GetEditContextMenu());
                        file = _config.GetFullPath(Literal.Direcotry_ContextMenu, Literal.File_Json_ContextMenu);
                        break;
                    default:

                        _initializer.ChangeTheme(ViewModel.Theme.Value, 
                            View.Entitry.Resources, 
                            Router.To<IMainViewModel>().Entitry.Resources);

                        _initializer.ChangeLanguage(ViewModel.Language.Value);
                        //BuildComboBox();

                        content = _initializer.UISettingsModel;
                        file = _config.GetFullPath(Literal.File_Json_UISettings);

                        break;
                }

                _service.SaveJsonFile(content, file);
                Router.NavigateTo<IMainViewModel>("ReBuild");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void VisitResult()
        {
            try
            {
                _service.Execute("https://github.com/kurozato/SideFiler", null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

    }
}
