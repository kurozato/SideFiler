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
        IUIInitializer _initializer;

        public InputNamePresenter(ISideFilerService service, ILogger logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //
        //for Other Call
        //
        public void CreateFolderResult(UIFileResultModel uiModel)
        {
            try
            {
                ViewModel.Description = "Folder Name:";
                ViewModel.Title = "Create Folder";
                ViewModel.Name = "Untitled";

                var view = Router.To<InputNameViewModel>();
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
                ViewModel.Description = "New Name:";
                ViewModel.Title = "Rename Item";
                ViewModel.Name = file.Name;

                var view = Router.To<InputNameViewModel>();
                var owner = Router.To<IMainViewModel>();
                UIHelper.SetOwner(view, owner);
                if (view?.ShowDialog() == true)
                {
                    if (file.Name == ViewModel.Name || ViewModel.Name == null || ViewModel.Name.Trim().Length == 0)
                        return;

                    var uiFile = _service.GetFileData(uiModel?.File?.FullName);
                    var model = new FileResultModel()
                    {
                        File = uiFile,
                    };

                    _service.Rename(file, ViewModel.Name, model, handle);

                }
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
        public void OpenFileMenuResult()
        {
            try
            {
                var view = new OpenFileDialog()
                {
                    Filter = "SFJSON Files(*.sfjson)|*.sfjson|All Files(*.*)|*.*",
                    Title = "Select Open File.",
                    RestoreDirectory = true
                };

                if (view.ShowDialog() == true)
                    _service.Execute(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SideFiler.exe"), view.FileName);

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
                    Title = "Select Save File.",
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
