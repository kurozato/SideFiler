using BlackSugar.Extension;
using BlackSugar.Model;
using BlackSugar.Service;
using BlackSugar.Service.Model;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Presenters
{
    public class SubPresenter : PresenterBase<SubViewModel>
    {
        ILogger _logger;
        ISideFilerService _service;
        IExConfiguration _config;

        public SubPresenter(ILogger logger, ISideFilerService service, IExConfiguration config)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected override void InitializeView()
        {

        }

        private IView<TViewModel> GetView<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var view = Router.Resolver.Resolve<IView<TViewModel>>();
            view.DataContext = viewModel;
            return view;
        }
         

        //
        //for Other Call
        //
        public void CreateFolderResult(UIFileResultModel uiModel)
        {
            try
            {

                ViewModel.InputName = new InputNameViewModel
                {
                    Description = ResourceService.Current.GetResource("FolderName"),
                    Title = ResourceService.Current.GetResource("CreateFolder"),
                    Name = "Untitled",
                    IsExtentionVisible = false
                };

                var view = GetView(ViewModel.InputName);
                var owner = Router.To<IMainViewModel>();
                UIHelper.SetOwner(view, owner);
                if (view?.ShowDialog() == true)
                {
                    if (ViewModel.InputName.Name == null || ViewModel.InputName.Name.Trim().Length == 0)
                        return;

                    var file = _service.GetFileData(uiModel?.File?.FullName);
                    var model = new FileResultModel()
                    {
                        File = file,
                    };

                    _service.CreateFolder(model, ViewModel.InputName.Name);
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
                ViewModel.InputName = new InputNameViewModel
                {
                    Description = ResourceService.Current.GetResource("NewName"),
                    Title = ResourceService.Current.GetResource("RenameFile"),
                    Name = file.IsFile ? Path.GetFileNameWithoutExtension(file.Name) : file.Name,
                    Extention = Path.GetExtension(file.Name),
                    IsExtentionVisible = file.IsFile
                };

                var view = GetView(ViewModel.InputName);
                var owner = Router.To<IMainViewModel>();
                UIHelper.SetOwner(view, owner);
                if (view?.ShowDialog() == true)
                {
                    var newName = ViewModel.InputName.Name;
                    if (ViewModel.InputName.IsExtentionVisible)
                        newName += ViewModel.InputName.Extention;

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

        //
        //for Other Call
        //
        public void AddBookmarkResult(IFileData file)
        {
            try
            {
                ViewModel.InputBookmark = new InputBookmarkViewModel
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                var bookmark = ViewModel.InputBookmark.GetEditBookmarkModel();
                var bookmarks = _service.GetBookmarksData(_config.GetFullPath(Literal.File_Json_Bookmarks)).ToList();
                var exists = bookmarks.Any(m => m.Path == bookmark.Path);

                var view = GetView(ViewModel.InputBookmark);
                var owner = Router.To<IMainViewModel>();
                UIHelper.SetOwner(view, owner);
                if (view?.ShowDialog() == true)
                {
                    if (ViewModel.InputBookmark.Bookmark)
                    {

                        bookmark = ViewModel.InputBookmark.GetEditBookmarkModel();
                        bookmarks.Where(m => m.Path == bookmark.Path).Invoke(m => m.Name = bookmark.Name);
                        if (!exists)
                            bookmarks.Add(bookmark);

                        _service.SaveJsonFile(bookmarks, _config.GetFullPath(Literal.File_Json_Bookmarks));
                    }
                    else
                    {
                        _service.RegistReadingList(
                            ViewModel.InputBookmark.GetEditBookmarkModel(),
                            _config.GetFullPath(Literal.File_DB_CloseRec, false));
                    }
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


    }
}
