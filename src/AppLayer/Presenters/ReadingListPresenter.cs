using BlackSugar.Model;
using BlackSugar.Service;
using BlackSugar.SimpleMvp;
using BlackSugar.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSugar.Presenters
{
    public class ReadingListPresenter : PresenterBase<ReadingListViewModel>
    {
        ILogger _logger;
        ISideFilerService _service;
        IExConfiguration _config;

        public ReadingListPresenter(ISideFilerService service, ILogger logger, IExConfiguration config)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected override void InitializeView()
        {
            ViewModel.ReadingLists.Clear();
            UIHelper.Refill(ViewModel.ReadingLists, getReadingList());
        }

        private IEnumerable<UIBookmarkModel> getReadingList() 
            => _service.GetReadingListData(_config.GetFullPath(Literal.File_DB_CloseRec, false))
                    .Select(b => new UIBookmarkModel(b));

        [ActionAutoLink]
        public void FilterResult()
        {
            try
            {

                if (ViewModel?.Filter == null || ViewModel?.Filter?.Length == 0)
                    return;

                ViewModel.ReadingLists.Clear();
                var filtering = getReadingList()?.Where(f => f?.Path?.ToUpper().IndexOf(ViewModel?.Filter?.ToUpper()) >= 0);
                UIHelper.Refill(ViewModel.ReadingLists, filtering);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void FilterReleaseResult()
        {
            try
            {
                ViewModel.Filter = null;
                ViewModel.ReadingLists.Clear();
                UIHelper.Refill(ViewModel.ReadingLists, getReadingList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }

        [ActionAutoLink]
        public void SelectResult()
        {
            try
            {
                var selected = ViewModel?.Selected;

                if (selected == null) return;

                Router.NavigateTo<IMainViewModel>("SelectBookmark", selected);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UIHelper.ShowErrorMessage(ex);
            }
        }
    }
}
