﻿using BlackSugar.Service;
using BlackSugar.Service.Model;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;
using BlackSugar.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlackSugar.Extension;
using SideFiler;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Drawing;

namespace BlackSugar.Presenters
{
    public class MainPresenter : PresenterBase<IMainViewModel>
    {
        ILogger _logger;
        ISideFilerService _service;
        IClipboardHelper _clipboard;
        IExConfiguration _config;

        public MainPresenter(ISideFilerService service, ILogger logger, IClipboardHelper clipboard, IExConfiguration config)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected override void InitializeView()
        {
            try
            {
                buildBookmarks();
                buildContexts();

                _service.InitilizeRec(_config.GetFullPath(Literal.File_DB_CloseRec, false));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        private void buildBookmarks()
        {
            ViewModel.Bookmarks.Clear();

            var bookmarks = _service.GetBookmarksData(_config.GetFullPath(Literal.File_Json_Bookmarks));
            var source = FileIcon.GetFolderSource();
            foreach (var bookmark in bookmarks)
                ViewModel.Bookmarks.Add(new UIBookmarkModel(bookmark, source));
        }

        private void buildContexts()
        {
            var contexts = new List<ContextMenuModel>();
            contexts.Add(new ContextMenuModel()
            {
                Content = ResourceService.Current.GetResource("OpenNewTab"),
                Result = "OpenNewTab",
                Target = "directory"
            });
            var menus = _service.GetContextMenusData(_config.GetFullPath(Literal.Direcotry_ContextMenu, Literal.File_Json_ContextMenu));
            contexts.AddRange(menus);

            var uiContexts = UIContextMenuModel.Convert(contexts, iconPath => _config.GetFullPath(Literal.Direcotry_ContextMenu, iconPath, false));

            UIHelper.Refill(ViewModel.ContextMenus, uiContexts);
        }

        private async Task updateSideAsync(FileResultModel? model)
        {
            var point = ViewModel.SideIndex;
            ViewModel.ID = model?.ID;

            ViewModel.FileItems?.Clear();

            var item = new UIFileResultModel(model);
            await item.SetResultsToEntityAsync(model.Results);

            ViewModel.SideItems[point] = item;
             
            if (item?.ID == ViewModel.ID)
                ViewModel.SideIndex = point;
        }

        private async Task addSideAsync(FileResultModel? model)
        {

            ViewModel.FileItems = UIFileResultModel.EmptyResult;
            ViewModel.FileItems.Clear();

            //to entity
            var item = new UIFileResultModel(model);
            await item.SetResultsToEntityAsync(model.Results);

            ViewModel.SideItems.Add(item);

            ViewModel.SideIndex = ViewModel.SideItems.Count - 1;
        }

        public async Task ResultTemplateAsync(string? path, long? ID, Func<FileResultModel, bool> predicate)
        {
            try
            {
                var file = _service.GetFileData(path);
                var model = new FileResultModel(file, ID);

                if (predicate(model))
                {
                    model.Label = model.File?.Name;
                    await updateSideAsync(model);
                }

            }
            catch (FileDataNotFoundException fileEx)
            {
                UIHelper.ShowErrorMessageEx(fileEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task AddResult()
        {
            try
            {
                ViewModel.MaxID += 1;
                var model = new FileResultModel(null, ViewModel.MaxID);
             
                _service.Startup(model);
                model.Label = ResourceService.Current.GetResource("NewTab");

                await addSideAsync(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task OpenNewTabResult()
        {
            try
            {
                if (ViewModel?.SelectedFile == null) return;

                var selected = ViewModel?.SelectedFile;
                var file = _service.GetFileData(selected?.FullName);
                var model = new FileResultModel(file, null);

                if (_service.Open(model))
                {
                    ViewModel.MaxID += 1;
                    model.ID = ViewModel.MaxID;
                    model.Label = model.File?.Name;

                    await addSideAsync(model);
                }
            }
            catch (FileDataNotFoundException fileEx)
            {
                UIHelper.ShowErrorMessageEx(fileEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void TabCloseResult(UIFileResultModel? item)
        {
            try
            {
                var index = item == null ? ViewModel.SideIndex : ViewModel.SideItems.IndexOf(item);

                var deleted = ViewModel.SideItems[index];
                var file = _service.GetFileData(deleted?.File?.FullName);
                _service.RegistRec(file, _config.GetFullPath(Literal.File_DB_CloseRec, false));

                ViewModel.SideItems.RemoveAt(index);

                index -= ViewModel.SideItems.Count - 1 < index ? 1 : 0;
                ViewModel.SideIndex = index;

                if (ViewModel.SideItems.Count == 0)
                    Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }

        }

        [ActionAutoLink]
        public async Task SelectMainResult()
        {
            try
            {
                var selected = ViewModel?.SelectedFile;
                var item = ViewModel?.SideItem;

                await ResultTemplateAsync(selected?.FullName, item?.ID, _service.Open);                    
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task ReloadResult()
        {
            try
            {
                var item = ViewModel?.SideItem;

                await ResultTemplateAsync(item?.File?.FullName, item?.ID, _service.Open);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task UpFolderResult()
        {
            try
            {
                var item = ViewModel?.SideItem;

                await ResultTemplateAsync(item?.File?.FullName, item?.ID, _service.Up);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void OpenExplorerResult()
        {
            try
            {
                var selected = ViewModel?.SelectedFile;
                var item = ViewModel?.SideItem;

                var path = selected?.FullName ?? item?.File?.FullName;
                var file = _service.GetFileData(path);

                _service.OpenExplorer(file, selected != null);
            }
            catch (FileDataNotFoundException fileEx)
            {
                UIHelper.ShowErrorMessageEx(fileEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task PathEnterResult()
        {
            try
            {
                var item = ViewModel?.SideItem;

                await ResultTemplateAsync(ViewModel?.FullPath, item?.ID, _service.Open);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void DeleteResult()
        {
            try
            {
                var items = ViewModel?.SelectedFiles?.Cast<UIFileData>();
                if (items == null) return;

                _service.Delete(items.Select(f => f.FullName), ViewModel.Handle);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        //for ContextMenu(future)
        public void CopyResult() => CopyCutResult(Effect.Copy);
        public void CutResult() => CopyCutResult(Effect.Move);

        [ActionAutoLink]
        public void CopyCutResult(Effect effect)
        {
            try
            {
                var items = ViewModel?.SelectedFiles?.Cast<UIFileData>();

                if (items == null) return;

                _clipboard.SetFiles(items, effect);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void PasteResult()
        {
            try
            {
                var data = _clipboard.GetFiles();

                if (data == null) return;

                var item = ViewModel?.SideItem;
                var file = _service.GetFileData(item?.File?.FullName);
                var model = new FileResultModel(file, item?.ID);

                _service.CopyOrMove(model, ViewModel.Handle, data, _clipboard.GetDropEffect());
            }
            catch (FileDataNotFoundException fileEx)
            {
                UIHelper.ShowErrorMessageEx(fileEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void RenameResult()
        {
            try
            {
                if (ViewModel?.SelectedFile == null) return;

                var file = _service.GetFileData(ViewModel?.SelectedFile?.FullName);

                Router.NavigateTo<InputNameViewModel>("RenameFile", file, ViewModel?.SideItem, ViewModel.Handle);
                //await ReloadResult();
            }
            catch (FileDataNotFoundException fileEx)
            {
                UIHelper.ShowErrorMessageEx(fileEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void CreateFolderResult()
        {
            try
            {
                Router.NavigateTo<InputNameViewModel>("CreateFolder", ViewModel?.SideItem);
                //await ReloadResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void OpenFileMenuResult()
        {
            try
            {
                Router.NavigateTo<InputNameViewModel>("OpenFileMenu");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void SaveFileMenuResult()
        {
            try
            {
                Router.NavigateTo<InputNameViewModel>("SaveFileMenu", ViewModel?.SideItems?.ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void MainFilterResult()
        {
            try
            {
                if (ViewModel?.MainFilter == null || ViewModel?.MainFilter?.Length == 0)
                    return;
                //ViewModel.FileItems = ViewModel?.SideItem?.Results?.Select(file => ViewFileData.Create(file))?.ToList();

                ViewModel.FileItems = UIFileResultModel.EmptyResult;
                var filtering = ViewModel?.SideItem?.Results?.Where(f => f?.FullName?.ToUpper().IndexOf(ViewModel?.MainFilter?.ToUpper()) >= 0);
                //ViewModel.FileItems = filtering?.ToList();
                UIHelper.Refill(ViewModel.FileItems, filtering);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void MainFilterReleaseResult()
        {
            try
            {
                ViewModel.MainFilter = null;
                ViewModel.FileItems = UIFileResultModel.EmptyResult;

                ViewModel.FileItems = ViewModel?.SideItem?.Results;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void SideFilterResult()
        {
            try
            {
                if (ViewModel?.SideFilter == null || ViewModel?.SideFilter?.Length == 0)
                    return;
             
                var filtering = ViewModel?.SideItems?.Where(r => r?.File?.FullName?.ToUpper()?.IndexOf(ViewModel?.SideFilter?.ToUpper()) < 0);
                foreach (var item in filtering)
                    item.IsVisible = false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void SideFilterReleaseResult()
        {
            try
            {
                ViewModel.SideFilter = null;

                foreach (var item in ViewModel?.SideItems)
                    item.IsVisible = true;

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task ExpandResult(string path)
        {
            try
            {
                foreach (var model in _service.GetData(path) ?? Enumerable.Empty<FileResultModel>())
                {
                    if(_service.Open(model))
                        await addSideAsync(model);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public async Task SelectBookmarkResult(UIBookmarkModel bookmark)
        {
            try
            {
                var item = ViewModel?.SideItem;

                await ResultTemplateAsync(bookmark?.Path, item?.ID, _service.Open);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void DropFileResult(string[] data)
        {
            try
            {
                if (data == null) return;

                var item = ViewModel?.SideItem;
                var file = _service.GetFileData(item?.File?.FullName);
                var model = new FileResultModel(file, item?.ID);

                _service.CopyOrMove(model, ViewModel.Handle, data, Effect.Copy);
            }
            catch (FileDataNotFoundException fileEx)
            {
                UIHelper.ShowErrorMessageEx(fileEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void ContextMenuResult(UIContextMenuModel menu)
        {
            try
            {
                var selects = ViewModel.SelectedFiles.Cast<UIFileData>();

                if (menu.BaseModel.Result != null)
                    Router.NavigateTo<IMainViewModel>(menu.BaseModel.Result);
                else
                    _service.DoContextMenu(menu.BaseModel, selects.Select(f => f.FullName).ToArray());

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }

        }

        [ActionAutoLink]
        public void AdjustMenuResult()
        {
            try
            {
                var isFolder = ViewModel?.SelectedFile?.ExAttributes.HasFlag(ExFileAttributes.Folder);

                foreach (var menu in ViewModel?.ContextMenus)
                {
                    if (menu.BaseModel.Target.ToLower() == "directory")
                        menu.IsVisible = (isFolder == true);
                    else
                        menu.IsVisible = (isFolder != true);
                } 
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void SettingMenuResult()
        {
            try
            {
                Router.NavigateTo<SettingsViewModel>("ShowMenu");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void RecentlyCloseFolderResult()
        {
            try
            {
                var file = _config.GetFiles(Literal.Direcotry_Backups, "*.*").OrderBy(f => f).FirstOrDefault();
                if (file == null) return;

                _service.Execute(_config.ExecutionPath, file);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        public void ReBuildResult()
        {
            buildBookmarks();
            buildContexts();
        }

        public void ClosedResult()
        {
            try
            {
                var content = ViewModel.SideItems
                    .Where(model => model.File?.FullName != null)
                    .Select(model => new { name = model.Label, path = model.File?.FullName }).ToList();

                if (content.Count == 0) return;

                _service.CheckDirectory(_config.GetFullPath(Literal.Direcotry_Backups, false));                
                _service.SaveJsonFile(content, _config.GetFullPath(Literal.Direcotry_Backups, DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"), false));

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }          
        }

        //END
    }
}
