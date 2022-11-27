using BlackSugar.Service.Model;
using BlackSugar.Views;
using BlackSugar.WinApi;
using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BlackSugar.Model
{
    public class UIFileResultModel
    {
        public long? ID { get; set; }
        public string? Label { get; set; }
        public UIFileData? File { get; set; }
        public ObservableCollection<UIFileData>? Results { get; set; }
       
        public UIFileResultModel()
        {

        }

        public UIFileResultModel (FileResultModel model)
        {
            ID = model.ID;
            Label = model.Label;
            File = UIFileData.Create(model.File);
        }


        public void SetResultsToEntity(IEnumerable<IFileData>? results)
        {
            var impl = new List<UIFileData?>();

            string ext;
            BitmapSource source;

            //to Entity
            foreach (var file in results ?? Enumerable.Empty<IFileData>())
            {
                ext = Path.GetExtension(file.FullName);

                if (FileIcon.Contains(ext, file.Attributes))
                    source = FileIcon.GetCacheSource(ext, file.Attributes);
                else
                {
                    source = FileIcon.Create(file.FullName);
                    FileIcon.SetCacheSource(ext, source);
                }
                impl.Add(UIFileData.Create(file, source));
            }

            Results = new ObservableCollection<UIFileData>(impl);
        }

        public async Task SetResultsToEntityAsync(IEnumerable<IFileData>? results)
        {
            await Task.Run(() => SetResultsToEntity(results));
        }

        public static ObservableCollection<UIFileData?>  EmptyResult => new ObservableCollection<UIFileData?>(); 

    }
}
