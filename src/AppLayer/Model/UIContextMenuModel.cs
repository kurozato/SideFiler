using BlackSugar.Service.Model;
using BlackSugar.Views;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
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
        public UIContextMenuModel(ContextMenuModel model)
        {
            BaseModel = model;
        }

        public UIContextMenuModel(ContextMenuModel model, BitmapSource source)
        {
            BaseModel = model;
            Icon = source;
        }

        private bool visible = true;
        public bool IsVisible
        {
            get => visible;
            set => SetProperty(ref visible, value);
        }

        public string? Content => BaseModel.Content;
        public BitmapSource Icon { get; }
    }
}
