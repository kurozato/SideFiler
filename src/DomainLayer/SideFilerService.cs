using BlackSugar.Model;
using BlackSugar.Repository;
using BlackSugar.Service.Model;
using BlackSugar.WinApi;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text.Json.Nodes;

namespace BlackSugar.Service
{
    public interface ISideFilerService
    {
        IFileData? GetFileData(string path);
        UISettingsModel? GetUISettings();

        bool Open(FileResultModel model);

        bool Up(FileResultModel model);

        void Startup(FileResultModel model);

        void RegistRec(FileResultModel model);

        void InitilizeRec();

        void OpenExplorer(IFileData file);

        void SaveJsonFile(object content, string fileName);

        IEnumerable<FileResultModel>? GetData(string fileName);

        void CopyOrMove(FileResultModel model, IntPtr handle, string[] data, Effect effect);

        void Delete(IEnumerable<IFileData> data, IntPtr handle);

        void CreateFolder(FileResultModel model, string name);

        void Rename(IFileData file, string name, FileResultModel model, IntPtr handle);

        void Execute(string application, string arguments);
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

        public IFileData? GetFileData(string path) 
            => _factory.CreateInstance(path).ToFileData();
        public UISettingsModel? GetUISettings()
            => _adpter.Get<UISettingsModel>(_adpter.ConvertFullPath(Literal.File_Json_UISettings, true), false);
        public void Execute(string application, string arguments)
            => _operator.Execute(application, arguments);

        public void Delete(IEnumerable<IFileData> data, IntPtr handle)
            => _operator.Delete(data.Select(f => f.FullName).ToList(), handle);

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

        public void Startup(FileResultModel model)
        {
            var startupItem = _factory.CreateInstance(null);
            model.Results = startupItem.SortDatas(startupItem.GetDatas());
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

        public void OpenExplorer(IFileData file)
        {
            if (file == null) return;

            _operator.OpenExplorer(file);
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

            //var data = _adpter.Get(fileName)?.AsArray();

            //return data?.Select(node => new { name = node?["name"]?.ToString(), path = node?["path"]?.ToString() })
            //        .Where(json => json?.path != null)
            //        .Select(json 
            //            => new FileResultModel(){
            //                File = _factory.CreateInstance(json.path).ToFileData(),
            //                Label = json.name
            //            });       
        }

        public async Task<bool> OpenAsynic(FileResultModel model, CancellationToken token)
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

        /***********************/
        public void RegistRec(FileResultModel model)
        {
            var file = model?.File;

            if (file == null) return;

            string qry;
            string connect = _commander.ConnectionString(Literal.File_DB_CloseRec);
            qry = "SELECT recNo FROM SFCloseRec WHERE path = '@Path; ";
            var tmp = _commander.Get<dynamic>(qry, new { Path = file.FullName }, connect)?.FirstOrDefault();
            int recNo = tmp != null ? tmp.recNo : 0;
            qry = "";
            qry += "DELETE FROM SFCloseRec WHERE path = @Path; ";
            qry += "INSERT INTO SFCloseRec(recNo, name, path)VALUES(1, @Name, @Path); ";
            qry += "UPDATE SFCloseRec SET recNo = recNo + 1 WHERE recNo < @RecNo Or @RecNo = 0; ";
            _commander.Execute(qry, new { Name = file.Name, Path = file.FullName, RecNo = recNo }, connect);

        }

        public void InitilizeRec()
        {
            string qry;
            string connect = _commander.ConnectionString(Literal.File_DB_CloseRec);
            qry = "";
            qry += "CREATE TABLE IF NOT EXISTS ";
            qry += "SFCloseRec( ";
            qry += " name TEXT NOT NULL,";
            qry += " path TEXT NOT NULL";
            qry += " recNo INTEGER NOT NULL";
            qry += ") ";

            _commander.Execute(qry, null, connect);
        }
    }
}