using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Views
{
    public class InputNameViewModel : BindableBase
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string extention;
        public string Extention
        {
            get => extention;
            set => SetProperty(ref extention, value);
        }

        private string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private bool visible = true;
        public bool IsExtentionVisible
        {
            get => visible;
            set => SetProperty(ref visible, value);
        }
    }
}
