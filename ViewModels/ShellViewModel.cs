using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    using Database;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        public ShellViewModel(IEventAggregator events, IWindowManager windowManager)
        {
            _events = events;
            _windowManager = windowManager;
        }

        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            Database.SetUpDatabase();
            DashboardViewModel dashboardViewModel = new DashboardViewModel(_events);
            await ActivateItemAsync(dashboardViewModel, cancellationToken);
        }
    }
}
