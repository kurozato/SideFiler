using BlackSugar.SimpleMvp;
using BlackSugar.Views.Extension;
using BlackSugar.Wpf;
using ModernWpf;
using ModernWpf.Controls.Primitives;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;


namespace BlackSugar.Views
{
    /// <summary>
    /// InputNameWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InputNameWindow : Window, IView<InputNameViewModel>
    {

        public InputNameViewModel? ViewModel => DataContext as InputNameViewModel;

        public InputNameWindow()
        {
            InitializeComponent();

            //get windows theme
            var theme = new BlackSugar.WindowsColor.Preferences().GetTheme();

            this.Resources.MergedDictionaries.AddRangeSource(
            //"pack://application:,,,/ModernWpf;component/ThemeResources/Dark.xaml",
            //"pack://application:,,,/ModernWpf;component/ThemeResources/Light.xaml",
            "pack://application:,,,../Resource/" + theme + "Adjust.xaml",
            "pack://application:,,,/ModernWpf;component/ThemeResources/" + theme + ".xaml",
            "pack://application:,,,/ModernWpf;component/ControlsResources.xaml"
            );

            btnOK.Click += (s, e) => { DialogResult = true; };
            btnCancel.Click += (s, e) => { DialogResult = false; };
        }
    }
}
