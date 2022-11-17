using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.SimpleMvp
{
    /// <summary>
    /// 
    /// </summary>
    public class Router
    {
        private static IDependencyResolver? resolver;
        /// <summary>
        /// 
        /// </summary>
        public static IDependencyResolver? Resolver => resolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyResolver"></param>
        public static void Configure(IDependencyResolver dependencyResolver)
        {
            resolver = dependencyResolver;
        }

        private static IPresenter<TViewModel>? GetPresenter<TViewModel>()
           where TViewModel : class
        {
            var presenter = resolver?.Resolve<IPresenter<TViewModel>>();
            if (presenter?.ViewModel == null)
            {
                var vm = resolver?.Resolve<TViewModel>();
                presenter?.Set(vm);
                presenter?.Initialize();
            }
            return presenter;
        }

        /// <summary>
        /// get view(initialized by presenter)
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        public static IView<TViewModel> To<TViewModel>() 
            where TViewModel : class
        {
            var presenter = GetPresenter<TViewModel>();
            var view = resolver?.Resolve<IView<TViewModel>>();
            view.DataContext = presenter?.ViewModel;
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="resultName"></param>
        /// <param name="argument"></param>
        public static void NavigateTo<TViewModel>(string resultName, params object[] argument)
            where TViewModel : class
        {
            var presenter = GetPresenter<TViewModel>();
            if (!resultName.EndsWith("Result"))
                resultName += "Result";

            var arg = argument ?? new object[] { null };

            var result = presenter?.GetType().GetMethod(resultName);
            result?.Invoke(presenter, arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="resultName"></param>
        public static void NavigateTo<TView>(string resultName)
            where TView : class => NavigateTo<TView>(resultName, Array.Empty<object>());
    }
}
