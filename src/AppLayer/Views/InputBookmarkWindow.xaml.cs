using BlackSugar.SimpleMvp;
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
    /// InputSubWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InputBookmarkWindow : Window, IView<InputBookmarkViewModel>
    {
        public InputBookmarkViewModel? ViewModel => DataContext as InputBookmarkViewModel;

        public dynamic Entitry => this;

        public InputBookmarkWindow()
        {
            InitializeComponent();

            btnAddBookmark.Click += (s, e) => { DialogResult = true; };
        }
    }
}
