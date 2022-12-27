using BlackSugar.SimpleMvp;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlackSugar.Views
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class Settings : MetroWindow, IView<SettingsViewModel>
    {
        public SettingsViewModel? ViewModel => DataContext as SettingsViewModel;

        public dynamic Entitry => this;

        public Settings(IUIInitializer uiInitializer)
        {

            uiInitializer.InitializeMain(this.Resources);

            InitializeComponent();
        }
    }
}
