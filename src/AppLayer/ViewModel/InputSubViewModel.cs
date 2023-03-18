using BlackSugar.Service.Model;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Views
{
    public class InputBookmarkViewModel : BindableBase
    {
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

        public bool Bookmark { get; set; }

        public BookmarkModel GetEditBookmarkModel() => new() { Name = Name, Path = Path };

        public DelegateCommand AddBookmarkCommand { get; }
        public DelegateCommand AddReadingListCommand { get; }

        public InputBookmarkViewModel()
        {
            AddBookmarkCommand = new DelegateCommand(() => { Bookmark = true; });
            AddReadingListCommand = new DelegateCommand(() => { Bookmark = false; });
        }
    }
}
