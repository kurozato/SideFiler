using System;
using System.ComponentModel;
using System.Reflection;

namespace BlackSugar.SimpleMvp
{
    /// <summary>
    /// this is for WinFroms.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public abstract class PresenterBase<TViewModel> : IPresenter<TViewModel>
          where TViewModel : class
    {
        protected TViewModel? _viewModel;

        protected bool initialize = false;

        public TViewModel? ViewModel => _viewModel;

        public virtual void Initialize()
        {
            if (initialize)
            {
                if(ViewModel != null)
                {
                    SetActionResultAuto();
                    SetActionResultManual();
                }
               
                InitializeView();
                initialize = false;
            }
        }

        protected virtual void InitializeView()
        {

        }

        ///// <summary>
        ///// XXXAction(Public Property) += XXXResult(Public Method)
        ///// If you remove XXXAction(Public Property) from "Action" and XXXResult(Public Method) from "Result", the same value
        ///// </summary>
        //protected void SetActionResult(bool nonThrowException = false)
        //{
        //    if (ViewModel == null) return;
           
        //    foreach (var action in ViewModel.GetType().GetProperties().Where(p => p.Name.EndsWith("Action")))
        //    {
        //        var mResult = action.Name.Replace("Action", "Result");
        //        var result = this.GetType().GetMethod(mResult, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
        //        if (result != null)
        //        {
        //            var dResult = Delegate.CreateDelegate(action.PropertyType, this, result);
        //            var setHandler = action.GetSetMethod();
        //            setHandler?.Invoke(ViewModel, new object[] { dResult });
        //        }
        //        else
        //        {
        //            if (nonThrowException == false) throw new NotImplementedException(action.Name);
        //        }
        //    }
        //}

        protected void SetActionResultAuto(bool nonThrowException = false)
        {
            foreach (var result in this.GetType().GetMethods().Where(m => m.GetCustomAttribute<ActionAutoLinkAttribute>() != null))
            {
                var target = result.Name.Replace("Result", "Action");
                SetActionResultLink(target, result, nonThrowException);
            }
        }

        protected void SetActionResultManual(bool nonThrowException = false)
        {
            foreach (var result in this.GetType().GetMethods().Where(m => m.GetCustomAttribute<ActionManualLinkAttribute>() != null))
            {
                var target = result.GetCustomAttribute<ActionManualLinkAttribute>()?.Name;

                if (target?.EndsWith("Action", StringComparison.Ordinal) == false)
                    target += "Action";

                SetActionResultLink(target, result, nonThrowException);

            }
        }

        private void SetActionResultLink(string target, MethodInfo result, bool nonThrowException = false)
        {
            var action = ViewModel.GetType().GetProperty(target, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            if (action != null)
            {
                var dResult = Delegate.CreateDelegate(action.PropertyType, this, result);
                var setHandler = action.GetSetMethod();
                setHandler?.Invoke(ViewModel, new object[] { dResult });
            }
            else
            {
                if (nonThrowException == false) throw new NotImplementedException(target);
            }
        }

        public virtual void Set(TViewModel? viewModel)
        {
            _viewModel = viewModel;
            initialize = true;
        }

        

    }
}
