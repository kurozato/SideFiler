using BlackSugar.Extension;
using BlackSugar.Model;
using BlackSugar.Repository;
using BlackSugar.Service.Model;
using BlackSugar.WinApi;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text.Json.Nodes;

namespace BlackSugar.Service
{
    public interface ISideFilerService
    {
        IFileData? GetFileData(string? path);

        bool Open(FileResultModel model);

        bool Up(FileResultModel model);

        bool Startup(FileResultModel model);

        void OpenExplorer(IFileData file, bool select);

        void SaveJsonFile(object content, string fileName);

        IEnumerable<FileResultModel>? GetData(string fileName);

        IEnumerable<IFileData?>? GetBookmarksFileData(string fileName);

        IEnumerable<BookmarkModel?> GetBookmarksData(string fileName);

        IEnumerable<ContextMenuModel?> GetContextMenusData(string fileName);

        void CopyOrMove(FileResultModel model, IntPtr handle, string[] data, Effect effect);

        void Delete(IEnumerable<string> data, IntPtr handle);

        void CreateFolder(FileResultModel model, string name);

        void Rename(IFileData file, string name, FileResultModel model, IntPtr handle);

        void Execute(string application, string arguments);

        void DoContextMenu(ContextMenuModel menu, string[] items, string? workingDirectory = null);

        void CheckDirectory(string path);

        void OpenTrash();

        void OpenCmd(IFileData? file);

        Task PushHistoryAsync(IFileData? file, string dbfile);

        Task InitilizeHistoryAsync(string dbfile);
    }

    public class SideFilerService : ISideFilerService
    {
        IStorageItemFactory _factory;
        IDbCommander _commander;
        IJsonAdpter _adpter;
        IFileOperator _operator;

        public SideFilerService(IStorageItemFactory factory, IDbCommander commander, IJsonAdpter adpter, IFileOperator @operator)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _commander = commander ?? throw new ArgumentNullException(nameof(commander));
            _adpter = adpter ?? throw new ArgumentNullException(nameof(adpter));
            _operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        }

        public IFileData? GetFileData(string? path)
        {
            var file = _factory.CreateInstance(path).ToFileData();
            if(file == null)
                throw FileDataNotFoundException.Create(path);

            return file;
        }


        public void Execute(string application, string? arguments)
            => _operator.Execute(application, arguments);

        public void OpenTrash() 
            => _operator.OpenTrash();

        public void OpenCmd(IFileData? file) 
            => _operator.OpenCmd(file);

        public void Delete(IEnumerable<string> data, IntPtr handle)
            => _operator.Delete(data.ToList(), handle);

        public void CreateFolder(FileResultModel model, string name)
            => _operator.CreateFolder(Path.Combine(model.File.FullName, name));

        public void Rename(IFileData file, string name, FileResultModel model, IntPtr handle)
            => _operator.Rename(file.FullName, Path.Combine(model.File.FullName, name), handle);

        public void CopyOrMove(FileResultModel model, IntPtr handle, string[] data, Effect effect)
        {
            if (effect == Effect.Move)
                _operator.Move(data.ToList(), model.File.FullName, handle);
            if (effect == Effect.Copy)
                _operator.Copy(data.ToList(), model.File.FullName, handle);
        }

        public bool Startup(FileResultModel model)
        {
            var startupItem = _factory.CreateInstance(null);
            model.Results = startupItem.SortDatas(startupItem.GetDatas());

            return true;
        }

        public bool Open(FileResultModel model)
        {
            var result = false;
            var file = model?.File;

            if (file == null) return false;

            if (file.IsFile)
                result = _operator.ExecuteOrMove(ref file);

            if (file.IsDirectory)
            {
                var item = _factory.CreateInstance(file.FullName);
                model.Results = item.SortDatas(item.GetDatas());
                model.File = file;
                result = true;
            }

            return result;
        }

        public bool Up(FileResultModel model)
        {
            var file = model?.File;

            if (file == null) return false;

            var item = _factory.CreateInstance(file.FullName);
            var upperItem = item.UpperLayer();

            if (upperItem == null) return false;

            model.File = upperItem.ToFileData();
            model.Results = upperItem.SortDatas(upperItem.GetDatas());

            return true;

        }

        public void OpenExplorer(IFileData file, bool select)
        {
            if (file == null) return;

            _operator.OpenExplorer(file, select);
        }

        public void SaveJsonFile(object content, string fileName) 
            => _adpter.Save(content, fileName);

        public IEnumerable<FileResultModel>? GetData(string fileName)
        {
            return _adpter.Get<List<BookmarkModel>>(fileName, false)?
                     .Where(json => json?.Path != null)
                        .Select(json
                            => new FileResultModel()
                            {
                                File = _factory.CreateInstance(json.Path).ToFileData(),
                                Label = json.Name
                            });    
        }


        public IEnumerable<IFileData?>? GetBookmarksFileData(string fileName)
        {
            return _adpter.Get<List<BookmarkModel>>(fileName, false)?
                      .Where(json => json?.Path != null)
                      .Select(json => _factory.CreateInstance(json.Path).ToFileData());
        }

        public IEnumerable<BookmarkModel?> GetBookmarksData(string fileName)
        {
            return _adpter.Get<List<BookmarkModel>>(fileName, false) ?? Enumerable.Empty<BookmarkModel?>();
        }

        public IEnumerable<ContextMenuModel?> GetContextMenusData(string fileName)
        {
            return _adpter.Get<List<ContextMenuModel>>(fileName, false) ?? Enumerable.Empty<ContextMenuModel?>();
        }

        public void DoContextMenu(ContextMenuModel menu, string[] items, string? workingDirectory = null)
        {
            if(menu.Multiple.TryParse<Multiple>() == Multiple.Combine)
            {
                _operator.Execute(menu.App, menu.GetArguments(items), workingDirectory);                 
            }
            if(menu.Multiple.TryParse<Multiple>() == Multiple.Roop)
            {
                foreach(var item in items)
                    _operator.Execute(menu.App, menu.GetArguments(item), workingDirectory);
            }
        }

        public async Task<bool> OpenAsync(FileResultModel model, CancellationToken token)
        {
            var result = false;
            var file = model.File;

            if (file == null) return false;

            if (file.IsFile)
                result = _operator.ExecuteOrMove(ref file);

            if (file.IsDirectory)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var items = new List<IFileData>();
                        var item = _factory.CreateInstance(file.FullName);
                        foreach(var data in item.SortDatas(item.GetDatas()))
                        {
                            token.ThrowIfCancellationRequested();
                            items.Add(data);
                        }

                        model.Results = items;
                        result = true;
                    }
                    catch(OperationCanceledException ex)
                    {

                    }
                }, token);
               
                
            }

            return result;
        }

        public void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path);

            files.OrderByDescending(f => f).TakeLast(files.Length - 2).Invoke(f => File.Delete(f));
        }

        /***********************/
        public async Task PushHistoryAsync(IFileData? file, string dbfile)
        {
            if (file == null) return;

            string qry;
            string connect = _commander.ConnectionString(dbfile);
            qry = "";
            qry += "INSERT INTO History(name, path, date)VALUES(@Name, @Path, @Date); ";
            await _commander.ExecuteAsync(qry, new { Name = file.Name, Path = file.FullName, Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") }, connect);

        }

        public async Task InitilizeHistoryAsync(string dbfile)
        {
            string qry;
            string connect = _commander.ConnectionString(dbfile);
            qry = "";
            qry += "CREATE TABLE IF NOT EXISTS ";
            qry += "History( ";
            qry += " name TEXT NOT NULL,";
            qry += " path TEXT NOT NULL,";
            qry += " date TEXT NOT NULL";
            qry += ") ";

            await _commander.ExecuteAsync(qry, null, connect);
        }
    }
}