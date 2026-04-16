using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Events;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;

    public class EditJobViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        public DashboardModel SelectedRow; // data passed by DashboardVM

        public EditJobViewModel(IEventAggregator events)
        {
            _events = events;
            SelectedRow = new DashboardModel();
        }

        #region Public View Functions
        public void ConfirmJobClient()
        {
            // for type 

            // Database.EditClient(SelectedRow.ClientId, ClientType, ClientName, ClientEmail, CompanyName, PhoneNumber, BillingAddress, City, Postcode, Country, IsActive);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());
        }

        public async Task Exit()
        {
            await TryCloseAsync();
        }
        #endregion
    }
}