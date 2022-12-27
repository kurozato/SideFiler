using BlackSugar.Properties;
using BlackSugar.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Views
{
    public class ResourceService : BindableBase
    {
        private static readonly ResourceService _current = new ResourceService();
        public static ResourceService Current
        {
            get { return _current; }
        }

        private Resources _resources = new Resources();

        public Resources Resources
        {
            get => _resources;
            set => SetProperty(ref _resources, value);
        }

        public string? GetResource(string key)
        {
            return _resources.GetType()?.GetProperty(key)?.GetValue(_resources) as string;
        }

        public string GetCurrentCulture() => CultureInfo.CurrentCulture.Name;

        public void ChangeCulture(string name)
        {
            Resources.Culture = CultureInfo.GetCultureInfo(name);
            Resources = new Resources();
        }
    }

}
