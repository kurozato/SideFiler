using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignColors;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using BlackSugar.Wpf;
using BlackSugar.SimpleMvp;
using BlackSugar.Presenters;
using BlackSugar.Repository;
using BlackSugar.Service;
using BlackSugar.Views;
using NLog.Config;
using NLog.Targets;
using Windows.UI.ViewManagement;
using BlackSugar.Service.Model;
using BlackSugar.Model;
using BlackSugar.WinApi;

namespace SideFiler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeLogger();

            var resolver = new DependencyResolver();
            resolver.Set(services =>
            {
                //repository
                services.AddSingleton<IStorageItemFactory, StorageItemFactory>();
                services.AddSingleton<IDbCommander, DbCommander>();
                services.AddSingleton<IJsonAdpter, JsonAdpter>();
                services.AddSingleton<IFileOperator, FileOperator>();

                //service
                services.AddSingleton<ILogger>(LogManager.GetCurrentClassLogger());
                services.AddSingleton<ISideFilerService, SideFilerService>();

                services.AddSingleton<IClipboardHelper, ClipboardHelper>();

                //validator

                //presenter
                services.AddSingleton<IPresenter<IMainViewModel>, MainPresenter>();
                services.AddSingleton<IPresenter<InputNameViewModel>, InputNamePresenter>();

                //viewModel
                services.AddSingleton<IMainViewModel, MainViewModel>();
                services.AddSingleton<InputNameViewModel>();

                //view
                services.AddSingleton<IView<IMainViewModel>, MainWindow>();
                services.AddTransient<IView<InputNameViewModel>, InputNameWindow>();

            });

            Router.Configure(resolver);

            SetUITheme(UISettingsModel.Default);

            var view = Router.To<IMainViewModel>();
            if(e.Args.Length == 0)
                view?.ViewModel?.AddCommand?.Execute(view?.ViewModel);
            else
                view?.ViewModel?.ExpandCommand?.Execute(e.Args[0]);

            view?.Show();
            
        }

        private void InitializeLogger()
        {
            var file = new FileTarget("file")
            {
                Encoding = System.Text.Encoding.UTF8,
                Layout = "${longdate} [${threadid:padding=8}] [${uppercase:${level:padding=-5}}] ${callsite}() ${message} ${exception:format=tostring}",
                FileName = "${basedir}/logs/${date:format=yyyyMMdd}.log",
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                ArchiveFileName = "${basedir}/logs/archives/archive.{#}.log",
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 7
            };

            var conf = new LoggingConfiguration();
            conf.AddTarget(file);
            conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, file));

            LogManager.Configuration = conf;
        }

        private void SetUITheme(UISettingsModel uiSettings)
        { 
            var themeHelper = new UIThemeHelper(uiSettings);

            this.Resources.Clear();

            //Material Design
            this.Resources.MergedDictionaries.Add(themeHelper.GetMaterialDesignTheme());

            this.Resources.MergedDictionaries.AddRangeSource(
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

            //set folder icon
            AssociatedIcon.SetCacheSource(AssociatedIcon.KEY_FOLDER, AssociatedIcon.GetFolderSource(themeHelper.FolderIcon));

            themeHelper = null;
        }
    }
}
