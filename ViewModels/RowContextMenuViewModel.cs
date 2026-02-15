using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;
    using Traker.Data;
    using Traker.Events;
    using Traker.Helper;

    public class RowContextMenuViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        public DashboardModel SelectedRow; // data passed by DashboardVM
        public List<ClientsModel> Clients;
        public List<JobsModel> Jobs;
        public List<InvoicesModel> Invoices;

        #region Private Field Vairables
        private CreateInvoiceViewModel _createInvoice;
        #endregion
        public RowContextMenuViewModel(IEventAggregator events, IWindowManager windowManager)
        {
            _events = events;
            _windowManager = windowManager;

            _createInvoice = new CreateInvoiceViewModel();
        }

        public Task CreateInvoice()
        {
            _createInvoice = new CreateInvoiceViewModel();
            _createInvoice.SelectedRow = SelectedRow;
            _windowManager.ShowWindowAsync(_createInvoice, null, CustomWindow.SettingsForDialog(800, 500));

            return Task.CompletedTask;
        }

        public Task OpenFolder()
        {
            Debug.WriteLine("Open folder");

            // get list of jobs under the client ID
            List<JobsModel> jobDetails = new List<JobsModel>();
            jobDetails = Jobs.Where(j => j.ClientId == SelectedRow.ClientId).ToList();

            FileStore.LocateFolder(SelectedRow.ClientId, SelectedRow.ClientName, jobDetails);

            return Task.CompletedTask;
        }

        public async Task SetStatus(string status)
        {
            await Database.SetStatus(status, SelectedRow.ClientId, SelectedRow.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
        }

        public async Task DeleteRow()
        {
            await Database.DeleteRow(SelectedRow.ClientId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
        }
    }
}
