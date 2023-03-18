using BlackSugar.Model;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Views
{
    public class ReadingListViewModel : BindableBase
    {
        public ObservableCollection<UIBookmarkModel> ReadingLists { get; set; }

        private UIBookmarkModel? selected;
        public UIBookmarkModel? Selected
        {
            get => selected;
            set => SetProperty(ref selected, value);
        }

        private int index;
        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }

        private string? filter;
        public string? Filter
        {
            get => filter;
            set => SetProperty(ref filter, value);
        }


        public DelegateCommand FilterCommand { get; }
        public DelegateCommand FilterReleaseCommand { get; }
        public DelegateCommand SelectCommand { get; }
        public DelegateCommand DelectCommand { get; }

        public Action? FilterAction { get; set; }
        public Action? FilterReleaseAction { get; set; }

        public Action? SelectAction { get; set; }
        public Action? DelectAction { get; set; }

        public ReadingListViewModel()
        {
            ReadingLists = new ObservableCollection<UIBookmarkModel>();
            FilterCommand = new DelegateCommand(() => FilterAction?.Invoke());
            FilterReleaseCommand = new DelegateCommand(() => FilterReleaseAction?.Invoke());
            SelectCommand = new DelegateCommand(() => SelectAction?.Invoke());
            DelectCommand = new DelegateCommand(() => DelectAction?.Invoke());
        }

    }
}
