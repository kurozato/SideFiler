using BlackSugar.Model;
using BlackSugar.Service;
using BlackSugar.Service.Model;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;

namespace BlackSugar.Presenters
{
    public class InputNamePresenter : PresenterBase<InputNameViewModel>
    {
        ILogger _logger;
        ISideFilerService _service;
        IExConfiguration _config;

        public InputNamePresenter(ISideFilerService service, ILogger logger, IExConfiguration config)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        //
        //for Other Call
        //
        public void CreateFolderResult(UIFileResultModel uiModel)
        {
            try
            {
                ViewModel.Description = ResourceService.Current.GetResource("FolderName");
                ViewModel.Title = ResourceService.Current.GetResource("CreateFolder");
                ViewModel.Name = "Untitled";
                ViewModel.IsExtentionVisible = false;

                var view = Router.To(this);
                var owner = Router.To<IMainViewModel>();
                UIHelper.SetOwner(view, owner);
                if(view?.ShowDialog() == true)
                {
                    if (ViewModel.Name == null || ViewModel.Name.Trim().Length == 0)
                        return;

                    var file = _service.GetFileData(uiModel?.File?.FullName);
                    var model = new FileResultModel()
                    {
                        File = file,
                    };

                    _service.CreateFolder(model, ViewModel.Name);
                }

                view = null;
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

        //
        //for Other Call
        //
        public void RenameFileResult(IFileData file, UIFileResultModel uiModel, IntPtr handle)
        {
            try
            {
                ViewModel.Description = ResourceService.Current.GetResource("NewName");
                ViewModel.Title = ResourceService.Current.GetResource("RenameFile");
                if(file.IsFile)
                {
                    ViewModel.Name = Path.GetFileNameWithoutExtension(file.Name);
                    ViewModel.Extention = Path.GetExtension(file.Name);
                    ViewModel.IsExtentionVisible = true;
                }
                else
                {
                    ViewModel.Name = file.Name;
                    ViewModel.IsExtentionVisible = false;
                }
                var view = Router.To(this);
                var owner = Router.To<IMainViewModel>();
                UIHelper.SetOwner(view, owner);
                if (view?.ShowDialog() == true)
                {
                    var newName = ViewModel.Name;
                    if(ViewModel.IsExtentionVisible)
                        newName += ViewModel.Extention;

                    if (file.Name == newName || newName == null || newName.Trim().Length == 0)
                        return;

                    var uiFile = _service.GetFileData(uiModel?.File?.FullName);
                    var model = new FileResultModel()
                    {
                        File = uiFile,
                    };

                    _service.Rename(file, newName, model, handle);

                }
                view = null;
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


        //
        //for Other Call
        //
        public void OpenFileMenuResult()
        {
            try
            {
                var view = new OpenFileDialog()
                {
                    Filter = "SFJSON Files(*.sfjson)|*.sfjson|All Files(*.*)|*.*",
                    Title = ResourceService.Current.GetResource("SelectOpenFile"),
                    RestoreDirectory = true
                };

                if (view.ShowDialog() == true)
                    _service.Execute(_config.ExecutionPath, view.FileName);

                view = null;

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        //
        //for Other Call
        //
        public void SaveFileMenuResult(IEnumerable<UIFileResultModel> items)
        {
            try
            {
                var view = new SaveFileDialog()
                {
                    FileName = "untitled.sfjson",
                    Filter = "SFJSON Files(*.sfjson)|*.sfjson|All Files(*.*)|*.*",
                    Title = ResourceService.Current.GetResource("SelectSaveFile"),
                    RestoreDirectory = true
                };

                if (view.ShowDialog() == true)
                {
                    var content = items.Select(model => new { name = model.Label, path = model.File?.FullName }).ToList();
                    _service.SaveJsonFile(content, view.FileName);
                }

                view = null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }
    }
}
