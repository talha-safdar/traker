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
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Models.Database;
    using Traker.Services;

    public class JobContextMenuViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        public DashboardModel SelectedJob; // data passed by DashboardVM
        public DataService Data { get; } // data from the database

        #region Private Field Vairables
        private CreateInvoiceViewModel _createInvoice;
        #endregion

        public JobContextMenuViewModel(IEventAggregator events, IWindowManager windowManager, DataService data)
        {
            _events = events;
            _windowManager = windowManager;
            Data = data;

            _createInvoice = new CreateInvoiceViewModel(_events, Data);
        }

        public async Task SetStatus(string status)
        {
            await Database.SetStatus(status, SelectedJob.ClientId, SelectedJob.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
            await Data.RefreshDatabase();
            await TryCloseAsync();
        }

        public async Task OpenFolder()
        {
            // get list of jobs under the client ID
            List<JobsModel> jobDetails = new List<JobsModel>();
            jobDetails = Data.Jobs.Where(j => j.ClientId == SelectedJob.ClientId).ToList();

            await FileStore.LocateFolder(SelectedJob.ClientId, SelectedJob.ClientName, jobDetails);
            await TryCloseAsync();;
        }

        public async Task CreateInvoice()
        {
            _createInvoice = new CreateInvoiceViewModel(_events, Data);
            _createInvoice.SelectedJob = SelectedJob;
            _windowManager.ShowWindowAsync(_createInvoice, null, CustomWindow.SettingsForDialog(800, 500, false));
            await TryCloseAsync();
        }

        public async Task EditClient()
        {
            await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = "EditClient" });
            await TryCloseAsync();
        }

        public async Task DeleteRow()
        {
            await Database.DeleteJob(SelectedJob.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
            await TryCloseAsync();
        }
    }
}