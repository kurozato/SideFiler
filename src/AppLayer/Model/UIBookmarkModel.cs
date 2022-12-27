using BlackSugar.Service.Model;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BlackSugar.Model
{
    public class UIBookmarkModel : BindableBase
    {
        public UIBookmarkModel() { }

        public UIBookmarkModel(BookmarkModel model) :this(model, null)
        { 

        }

        public UIBookmarkModel(BookmarkModel model, BitmapSource source)
        {
            SetEditBookmark(model);
            Icon = source;
        }

    

        public BitmapSource? Icon { get; }


        private string? name;
        public string? Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        private string? path;
        public string? Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }



        public void SetEditBookmark(BookmarkModel value)
        {
            Name = value.Name;
            Path = value.Path;
        }

        public BookmarkModel GetEditBookmark()
            => new BookmarkModel()
            {
                Name = Name,
                Path = Path,
            };


    }
}
