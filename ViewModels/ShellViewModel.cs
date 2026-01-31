using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    using Database;
    using System.Net.NetworkInformation;
    using System.Windows.Controls;
    using Traker.States;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState)
        {
            _events = events;
            _windowManager = windowManager;
            State = appState;
        }

        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel(_events, _windowManager, State);
            await ActivateItemAsync(dashboardViewModel, cancellationToken);
        }


        public async Task OnMouseDownEvent(Grid gridSource)
        {
            
        }
    }
}
