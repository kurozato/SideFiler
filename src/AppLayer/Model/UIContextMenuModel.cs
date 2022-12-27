using BlackSugar.Service.Model;
using BlackSugar.Views;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlackSugar.Model
{
    public class UIContextMenuModel: BindableBase
    {
        public ContextMenuModel BaseModel { get; }

        public UIContextMenuModel()
        {
            Targets = new ObservableCollection<UIComboBoxModel>() {
                new UIComboBoxModel("file", "file"),
                new UIComboBoxModel("deirectory","deirectory"),
                new UIComboBoxModel("both","both")
            };

            Multiples = new ObservableCollection<UIComboBoxModel>() {
                new UIComboBoxModel("roop", "roop"),
                new UIComboBoxModel("combine","combine")
            };
        }

        public UIContextMenuModel(ContextMenuModel model) : this(model, null) 
        { 

        }

        public UIContextMenuModel(ContextMenuModel model, BitmapSource? source) : this()
        {
            BaseModel = model;
            Icon = source;

            SetEditContextMenu(BaseModel);
        }

        public BitmapSource? Icon { get; }

        private bool visible = true;
        public bool IsVisible
        {
            get => visible;
            set => SetProperty(ref visible, value);
        }


        private string? iconPath;
        public string? IconPath
        {
            get => iconPath;
            set => SetProperty(ref iconPath, value);
        }

        private string? content;
        public string? Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
        private string? app;
        public string? App
        {
            get => app;
            set => SetProperty(ref app, value);
        }
        private string? commandline;
        public string? Commandline
        {
            get => commandline;
            set => SetProperty(ref commandline, value);
        }
        private string? delimiter;
        public string? Delimiter
        {
            get => delimiter;
            set => SetProperty(ref delimiter, value);
        }

        public ObservableCollection<UIComboBoxModel> Targets { get; set; }

        private UIComboBoxModel? target;
        public UIComboBoxModel? Target
        {
            get => target;
            set => SetProperty(ref target, value);
        }
        public ObservableCollection<UIComboBoxModel> Multiples { get; set; }

        private UIComboBoxModel? multiple;
        public UIComboBoxModel? Multiple
        {
            get => multiple;
            set => SetProperty(ref multiple, value);
        }


        public void SetEditContextMenu(ContextMenuModel value)
        {
            IconPath = value.IconPath;
            Content = value.Content;
            App = value.App;
            Commandline = value.Commandline;
            Delimiter = value.Delimiter;

            Target = Targets.FirstOrDefault(comb => comb.Value == value.Target);
            Multiple = Multiples.FirstOrDefault(comb => comb.Value == value.Multiple);
        }

        public ContextMenuModel GetEditContextMenu()
           => new ContextMenuModel()
           {
               IconPath = IconPath,
               Content = Content,
               App = App,
               Commandline = Commandline,
               Delimiter = Delimiter,
               Target = Target?.Value,
               Multiple = Multiple?.Value
           };



        public static IEnumerable<UIContextMenuModel> Convert(IEnumerable<ContextMenuModel>? models, Func<string, string> funcGetFullPath)
        {
            if (models != null)
            {
                foreach (var model in models)
                {
                    BitmapSource? source = null;
                    if ((model.IconPath != null))
                    {
                        source = FileIcon.GetBitmapSource(funcGetFullPath(model.IconPath));
                        source.Freeze();
                    }

                    yield return new UIContextMenuModel(model, source);
                }
            }
        }
    }
}
