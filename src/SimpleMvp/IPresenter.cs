namespace BlackSugar.SimpleMvp
{
    /// <summary>
    /// this is for method "NavigateTo".
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public interface IPresenter<TViewModel> where TViewModel : class
    {
        TViewModel? ViewModel { get; }

        void Set(TViewModel? viewModel);

        void Initialize();
    }
}