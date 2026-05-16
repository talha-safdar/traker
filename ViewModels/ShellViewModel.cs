using Caliburn.Micro;

namespace Traker.ViewModels
{
    using Database;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Traker.Events.ShellVM;
    using Traker.Helper;
    using Traker.Services;
    using Traker.States;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive,
    #region Interfaces
        IHandle<ShellVM>
    #endregion
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _events.SubscribeOnPublishedThread(this);
        }

        protected async override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Database.SetUpDatabase();
                await _dataService.FetchDatabase();

                // check if user table is empty
                // if so, it means it's a fresh start
                // else ignore
                if (_dataService.User?.Any() == false)
                {
                    // open the setup window
                    await Task.Delay(2000);
                    SetupViewModel setupViewModel = new SetupViewModel(_events, _windowManager, _dataService, _state);
                    await _windowManager.ShowWindowAsync(setupViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
                else
                {
                    DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, _dataService, _state);
                    await ActivateItemAsync(dashboardViewModel);
                }
            }
            catch (Exception ex)
            {
                // not already open?
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Shell OnViewReady";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _state.messageBoxVM.Action = Names.Close;
                    await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: OnViewReady() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            if (_state.UserContextMenuViewModel != null)
            {
                await _state.UserContextMenuViewModel.TryCloseAsync(false);
                _state.UserContextMenuViewModel = null;
            }
            if (IoC.Get<FilterJobsViewModel>().IsActive == true)
            {
                await IoC.Get<FilterJobsViewModel>().TryCloseAsync(false);
            }
            if (IoC.Get<SortJobsViewModel>().IsActive == true)
            {
                await IoC.Get<SortJobsViewModel>().TryCloseAsync(false);
            }
        }

        public Task Exit()
        {
            Application.Current.Shutdown();
            return Task.CompletedTask;
        }

        public Task Minimise()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            return Task.CompletedTask;
        }

        #region Event Handlers
        public Task HandleAsync(ShellVM message, CancellationToken cancellationToken)
        {
            if (message != null)
            {
                if (message.Command == Names.SetupCompleted)
                {
                    DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, _dataService, _state);
                    ActivateItemAsync(dashboardViewModel, cancellationToken);
                }
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}