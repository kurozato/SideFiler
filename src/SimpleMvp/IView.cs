using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.SimpleMvp
{
    public interface IView<TViewModel> where TViewModel : class
    {
        TViewModel? ViewModel { get; }

        object? DataContext { get; set; }

        bool? ShowDialog();

        void Show();

        dynamic Entitry { get; }
    }
}
