using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlackSugar.Extension;
using BlackSugar.Repository;
using BlackSugar.Service;
using BlackSugar.Wpf;
using BlackSugar.Service.Model;

namespace BlackSugar.Views
{
    public interface IUIInitializer
    {
        void Initialize();
        void InitializeMain(ResourceDictionary resource);
        void InitializeSub(ResourceDictionary resource);
    }

    public class UIInitializer : IUIInitializer
    {
        IJsonAdpter _adpter;
        UISettingsModel UISettingsModel;

        public UIInitializer(IJsonAdpter adpter)
        {
            _adpter = adpter ?? throw new ArgumentNullException(nameof(adpter));
        }

        public void Initialize()
        {
            UISettingsModel = _adpter.Get<UISettingsModel>(_adpter.ConvertFullPath(Literal.File_Json_UISettings, true), false) ?? UISettingsModel.Default;
            var themeHelper = new UIThemeHelper(UISettingsModel);
            //set folder icon
            FileIcon.SetCacheSource(FileIcon.KEY_FOLDER, FileIcon.GetFolderSource(themeHelper.FolderIcon));
        }

        public void InitializeMain(ResourceDictionary resource)
        {
            var themeHelper = new UIThemeHelper(UISettingsModel);

            resource.Clear();

            //Material Design
            resource.MergedDictionaries.Add(themeHelper.GetMaterialDesignTheme());

            resource.MergedDictionaries.AddRangeSource(
                //Material Design
                "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml",
                //Material Design Theme: MahApps
                "pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Fonts.xaml",
                "pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Flyout.xaml",
                //MahApps
                "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml",
                "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml",
                themeHelper.GetMahAppsMetroThemeUri()
                ); ;
        }

        public void InitializeSub(ResourceDictionary resource)
        {
            //get windows theme
            var theme = new WindowsColor.Preferences().GetTheme();

            resource.MergedDictionaries.AddRangeSource(
                //"pack://application:,,,/ModernWpf;component/ThemeResources/Dark.xaml",
                //"pack://application:,,,/ModernWpf;component/ThemeResources/Light.xaml",
                "pack://application:,,,../Resource/" + theme + "Adjust.xaml",
                "pack://application:,,,/ModernWpf;component/ThemeResources/" + theme + ".xaml",
                "pack://application:,,,/ModernWpf;component/ControlsResources.xaml"
            );
        }
    }
}
