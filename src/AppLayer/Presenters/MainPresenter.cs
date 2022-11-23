﻿using BlackSugar.Service;
using BlackSugar.Service.Model;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;
using BlackSugar.Model;
using NLog;
using SideFiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices;
using System.Windows;
using BlackSugar.Extension;
using System.Windows.Controls.Primitives;

namespace BlackSugar.Presenters
{
    public class MainPresenter : PresenterBase<IMainViewModel>
    {
        ILogger _logger;
        ISideFilerService _service;
        IClipboardHelper _clipboard;

        public MainPresenter(ISideFilerService service, ILogger logger, IClipboardHelper clipboard)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
        }

        //public async Task ChangeSideIndexAsync(int index)
        //{
        //    ViewModel.SideIndex = index;

        //    if (ViewModel.SideIndex < 0) return;
        //    var item = ViewModel.SideItems[ViewModel.SideIndex];

        //    ViewModel.FullPath = item?.File?.FullName;
        //    ViewModel.ID = item?.ID;

        //    var results = await Task.Run(() => item?.Results?.ToList());

        //    if (item?.ID == ViewModel.ID)
        //        UIHelper.Refill(ViewModel.FileItems, results);
        //        //ViewModel.FileItems = results;
        //}

        private async Task updateSideAsync(FileResultModel model)
        {
            var point = ViewModel.SideIndex;
            ViewModel.ID = model?.ID;

            ViewModel.FileItems?.Clear();

            var item = new UIFileResultModel(model);
            await item.SetResultsToEntityAsync(model.Results);

            ViewModel.SideItems[point] = item;
            ViewModel.SideItemsMirror[point] = item;
             
            if (item?.ID == ViewModel.ID)
                ViewModel.SideIndex = point;
                //await ChangeSideIndexAsync(point);
        }

        private async Task addSideAsync(FileResultModel model)
        {

            ViewModel.FileItems = UIFileResultModel.EmptyResult;
            ViewModel.FileItems.Clear();

            //to entity
            var item = new UIFileResultModel(model);
            await item.SetResultsToEntityAsync(model.Results);

            ViewModel.SideItems.Add(item);
            ViewModel.SideItemsMirror.Add(item);

            //await ChangeSideIndexAsync(ViewModel.SideItems.Count - 1);
            ViewModel.SideIndex = ViewModel.SideItems.Count - 1;
        }

        [ActionAutoLink]
        public async Task AddResult()
        {
            try
            {
                ViewModel.MaxID += 1;
                var model = new FileResultModel
                {
                    ID = ViewModel.MaxID,
                    Label = "New Tab",
                };
                _service.Startup(model);

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
                var model = new FileResultModel
                {
                    File = file,
                    Label = file?.Name
                };

                if (_service.Open(model))
                {
                    ViewModel.MaxID += 1;
                    model.ID = ViewModel.MaxID;

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
        public void TabCloseResult(UIFileResultModel? item)
        {
            try
            {
                var index = item == null ? ViewModel.SideIndex : ViewModel.SideItems.IndexOf(item);
                
                ViewModel.SideItems.RemoveAt(index);
                ViewModel.SideItemsMirror.RemoveAt(index);

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
                var file = _service.GetFileData(selected?.FullName);
                var model = new FileResultModel()
                {
                    Label = file?.Name,
                    File = file,
                    ID = ViewModel?.SideItem?.ID
                };

                if (_service.Open(model))
                    await updateSideAsync(model);
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
                var file = _service.GetFileData(item?.File?.FullName);
                var model = new FileResultModel()
                {
                    Label = file?.Name,
                    File = file,
                    ID = item?.ID
                };

                _service.Open(model);
                await updateSideAsync(model);
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
                var file = _service.GetFileData(item?.File?.FullName);
                 var model = new FileResultModel()
                {
                    File = file,
                    ID = item?.ID
                };
                if (_service.Up(model))
                {
                    model.Label = model?.File?.Name;
                    await updateSideAsync(model);
                }
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
                var item = ViewModel?.SideItem;
                var file = _service.GetFileData(item?.File?.FullName);
                _service.OpenExplorer(file);
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
                var file = _service.GetFileData(ViewModel?.FullPath);
                if(file == null)
                    throw new System.IO.DirectoryNotFoundException("path:'" + ViewModel?.FullPath + "' is not found.");


                var model = new FileResultModel()
                {
                    Label = file.Name,
                    File = file,
                    ID = ViewModel?.SideItem?.ID
                };

                if (_service.Open(model))
                    await updateSideAsync(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

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
        public void PasteResult(IntPtr handle)
        {
            try
            {
                var data = _clipboard.GetFiles();

                if (data == null) return;

                var item = ViewModel?.SideItem;
                var file = _service.GetFileData(item?.File?.FullName);
                var model = new FileResultModel()
                {
                    File = file,
                    ID = item?.ID
                };
                _service.CopyOrMove(model, handle, data, _clipboard.GetDropEffect());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void RenameResult(IntPtr handle)
        {
            try
            {
                if (ViewModel?.SelectedFile == null) return;

                var file = _service.GetFileData(ViewModel?.SelectedFile?.FullName);

                Router.NavigateTo<InputNameViewModel>("RenameFile", file, ViewModel?.SideItem, handle);

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
                //ViewModel.FileItems = ViewModel?.SideItem?.Results?.ToList();
                //UIHelper.Refill(ViewModel.FileItems, ViewModel?.SideItem?.Results);
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
            
                var filtering = ViewModel?.SideItemsMirror?.Where(r => r?.File?.FullName?.ToUpper()?.IndexOf(ViewModel?.SideFilter?.ToUpper()) >= 0);

                UIHelper.Refill(ViewModel?.SideItems, filtering);
                //ViewModel.SideItems.Clear();
                //foreach (var item in filtering)
                //    ViewModel?.SideItems?.Add(item);

                if (ViewModel?.SideItems?.Count > 0)
                    ViewModel.SideIndex = 0;

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

                UIHelper.Refill(ViewModel?.SideItems, ViewModel?.SideItemsMirror);
                //ViewModel.SideItems.Clear();
                //foreach (var item in ViewModel.SideItemsMirror)
                //    ViewModel.SideItems.Add(item);

                if (ViewModel.SideItems.Count > 0)
                    ViewModel.SideIndex = 0;
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
    }
}
