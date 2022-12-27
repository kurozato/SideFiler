using BlackSugar.Extension;
using BlackSugar.Service.Model;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BlackSugar.Views
{
    public class UIThemeHelper
    {
        private IBaseTheme materialDesignTheme;
        private PrimaryColor primaryColor;
        private SecondaryColor secondaryColor;
        private string metroTheme;
        private string adjust;

        public Bitmap FolderIcon { get; }

        private bool ExistsMetroColor(string color)
        {
            var colors = new string[] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };

            return colors.Any(c => c == color);

        }

        public UIThemeHelper(UISettingsModel uiSettings)
        {
            if (uiSettings.Theme == UITheme.Dark)
            {
                materialDesignTheme = Theme.Dark;
                metroTheme = "Dark.";
                FolderIcon = Properties.Resources.folder_dark;
                adjust = "Dark";
            }
            else
            {
                materialDesignTheme = Theme.Light;
                metroTheme = "Light.";
                FolderIcon = Properties.Resources.folder_light;
                adjust = "Light";
            }

            var metroColor = uiSettings.BaseColor.ToUpperCamel();
            if (!ExistsMetroColor(metroColor))
                metroColor = "Steel";
            metroTheme += metroColor;

            primaryColor = uiSettings.AccentColor.TryParse<PrimaryColor>();
            secondaryColor = uiSettings.TabColor.TryParse<SecondaryColor>();

        }

        public ResourceDictionary GetMaterialDesignTheme()
        {
            var primary = SwatchHelper.Lookup[(MaterialDesignColor)primaryColor];
            var secondary = SwatchHelper.Lookup[(MaterialDesignColor)secondaryColor];
            ITheme theme = Theme.Create(materialDesignTheme, primary, secondary);
            var dic = new ResourceDictionary();
            dic.SetTheme(theme);
            return dic;
        }

        public string GetMahAppsMetroThemeUri()
        {
            return "pack://application:,,,/MahApps.Metro;component/Styles/Themes/" + metroTheme + ".xaml";
        }

        public string GetCustomThemeUri()
        {
            return "pack://application:,,,../Resource/" + adjust + "Adjust.xaml";
        }

    }
}