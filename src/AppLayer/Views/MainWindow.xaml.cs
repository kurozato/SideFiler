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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlackSugar.Views.Extension;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;

namespace SideFiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IView<IMainViewModel>
    {
        public IMainViewModel? ViewModel => DataContext as IMainViewModel;

        public IntPtr Handle => new System.Windows.Interop.WindowInteropHelper(this).Handle;

        public MainWindow(IUIInitializer uiInitializer)
        {
            InitializeComponent();

            uiInitializer.InitializeMain(this.Resources);

            ListMain.ItemDoubleClick(() => UIHelper.Executor(ViewModel?.SelectMainCommand));
            ListMain.KeyBind(Key.Enter, () => UIHelper.Executor(ViewModel?.SelectMainCommand));
            ListMain.MouseBind(MouseButton.XButton1, () => UIHelper.Executor(ViewModel?.UpFolderCommand));
            ListMain.DragDropFile((data) => UIHelper.Executor(ViewModel?.DropFileCommand, new Tuple<string[], IntPtr>(data, Handle)));
 
            //ListMain.MouseBind(MouseButton.Middle, () => UIHelper.Executor(ViewModel?.OpenNewTabCommand));
            ListMain.BlankAreaClick(() => ListMain.UnselectAll());

            FullPath.SetFocusSelectAll();

        }

    }  
}
