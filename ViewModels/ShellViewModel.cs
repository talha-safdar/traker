using Caliburn.Micro;

namespace Traker.ViewModels
{
    using Database;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Traker.Data;
    using Traker.Events.ShellVM;
    using Traker.Helper;
    using Traker.Services;
    using Traker.States;
    using Traker.Views;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive,
    #region Interfaces
        IHandle<ShellVM>
    #endregion
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly DashboardViewModel _dashboardViewModel;
        #endregion

        #region Public View Variables
        public AppState State { get; set; }
        #endregion

        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state, DashboardViewModel dashboardViewModel)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            State = state;
            _dashboardViewModel = dashboardViewModel;

            _events.SubscribeOnPublishedThread(this);
        }

        #region Caliburn Functions
        protected async override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // check if all user data is present, if not open the setup window
                if (await Database.CheckUserDatabase() == false) // if false it means ds exists but check failed
                {
                    // No database found
                    State.SplashText = "Database is corrupted";

                    await Task.Delay(1000);

                    // Delete current db
                    State.SplashText = "Deleting current database...";

                    await Task.Delay(1000);

                    if (await FileStore.DeleteDatabase() == true)
                    {
                        State.SplashText = "Current Database Deleted";
                        Logger.LogActivity(Logger.INFO, "ShellViewModel: Deleted Corrupted Database");
                    }
                    else
                    {
                        State.SplashText = "Cannot access the database";
                        await Task.Delay(1000);
                        Environment.Exit(1); // close app
                        Logger.LogActivity(Logger.WARNING, "ShellViewModel: Failed to Delete Corrupted Database");
                    }

                    // Creating database
                    await Task.Delay(1000);
                    State.SplashText = "Creating a new database...";
                    await Task.Delay(1000);
                    await Database.SetUpDatabaseBG(); // it creates the database if it does not exist
                }

                State.SplashText = "Initialising database";
                await Task.Delay(1000);
                await _dataService.FetchDatabaseBG(); // it feteches database

                State.SplashText = "Populating dashboard";
                await Task.Delay(1000);
                await _dashboardViewModel.RefreshDatabase(); // Get data ready for dashboard

                if (_dataService.User?.Any() == true)
                {
                    await ActivateItemAsync(_dashboardViewModel);
                }

                await base.OnInitializedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Initialise";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
        }

        protected override async void OnViewReady(object view)
        {
            try
            {
                base.OnViewReady(view);

                // check if user table is empty
                // if so, it means it's a fresh start
                // else ignore
                if (_dataService.User?.Any() == false)
                {
                    // open the setup window
                    await Task.Delay(1000);
                    SetupViewModel setupViewModel = new SetupViewModel(_events, _windowManager, _dataService, State);
                    await _windowManager.ShowWindowAsync(setupViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
            }
            catch (Exception ex)
            {
                // not already open?
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Shell OnViewReady";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    State.messageBoxVM.Action = Names.Close;
                    await _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: OnViewReady() FAIL\n\t{ex.Message}");
            }
        }

        protected override void OnViewLoaded(object view)
        {
            try
            {
                base.OnViewLoaded(view);

                // Find the splash window by its type and close it
                var splash = Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w is SplashScreenView);
                splash?.Close(); // if not null close it
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "On View Loaded";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: OnViewLoaded() FAIL\n\t{ex.Message}");
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            try
            {
                // Re-assign the "New Boss" so Application.Current.MainWindow works everywhere
                Application.Current.MainWindow = Window.GetWindow(view as DependencyObject);
                base.OnViewAttached(view, context);
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "On View Attached";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: OnViewAttached() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            try
            {
                if (State.UserContextMenuViewModel != null)
                {
                    await State.UserContextMenuViewModel.TryCloseAsync(false);
                    State.UserContextMenuViewModel = null;
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
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Close Other Windows";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: OnMouseDownEvent() FAIL\n\t{ex.Message}");
            }
        }

        public Task Exit()
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Exit Form";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: Exit() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        public Task Minimise()
        {
            try
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Exit Form";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"ShellViewModel: Exit() FAIL\n\t{ex.Message}");
            }

            return Task.CompletedTask;
        }

        #region Event Handlers
        public Task HandleAsync(ShellVM message, CancellationToken cancellationToken)
        {
            if (message != null)
            {
                if (message.Command == Names.SetupCompleted)
                {
                    DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, _dataService, State);
                    ActivateItemAsync(dashboardViewModel, cancellationToken);
                }
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}