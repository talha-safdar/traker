using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    using Database;
    using System.Diagnostics;
    using System.Net.NetworkInformation;
    using System.Threading;
    using System.Windows.Controls;
    using Traker.Events;
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
        private readonly DataService _dataService;
        private readonly IWindowManager _windowManager;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState, DataService dataService)
        {
            _events = events;
            _windowManager = windowManager;
            State = appState;
            _dataService = dataService;

            _events.SubscribeOnPublishedThread(this);
        }

        //protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        //{
        //    await Database.SetUpDatabase();
        //    await _dataService.FetchDatabase();

        //    // check if user table is empty
        //    // if so, it means it's a fresh start
        //    // else ignore
        //    if (_dataService.Jobs?.Any() == false)
        //    {
        //        Debug.WriteLine("No jobs found in database. Assuming fresh start.");
        //        // open the setup window
        //        SetupViewModel setupViewModel = new SetupViewModel(_events, _windowManager, State, _dataService);
        //        await _windowManager.ShowWindowAsync(setupViewModel, null, CustomWindow.SettingsForDialog(800, 1000));

        //    }
        //    else
        //    {
        //        DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, State, _dataService);
        //        await ActivateItemAsync(dashboardViewModel, cancellationToken);
        //    }
        //}

        protected async override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            await Database.SetUpDatabase();
            await _dataService.FetchDatabase();

            // check if user table is empty
            // if so, it means it's a fresh start
            // else ignore
            if (_dataService.User?.Any() == false)
            {
                // open the setup window
                await Task.Delay(5000);
                SetupViewModel setupViewModel = new SetupViewModel(_events, _windowManager, State, _dataService);
                await _windowManager.ShowWindowAsync(setupViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            else
            {
                DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, State, _dataService);
                await ActivateItemAsync(dashboardViewModel);
            }
        }

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            
        }

        #region Event Handlers
        public Task HandleAsync(ShellVM message, CancellationToken cancellationToken)
        {
            if (message != null)
            {
                if (message.Command == Names.SetupCompleted)
                {
                    DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, State, _dataService);
                    ActivateItemAsync(dashboardViewModel, cancellationToken);
                }
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}
