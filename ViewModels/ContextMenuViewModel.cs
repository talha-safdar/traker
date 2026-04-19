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
    using Traker.Models.Database;
    using Traker.Services;

    public class ContextMenuViewModel : Screen
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

        public ContextMenuViewModel(IEventAggregator events, IWindowManager windowManager, DataService data)
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
        }

        public Task OpenFolder()
        {
            Debug.WriteLine("Open folder");

            // get list of jobs under the client ID
            List<JobsModel> jobDetails = new List<JobsModel>();
            jobDetails = Data.Jobs.Where(j => j.ClientId == SelectedJob.ClientId).ToList();

            FileStore.LocateFolder(SelectedJob.ClientId, SelectedJob.ClientName, jobDetails);

            return Task.CompletedTask;
        }

        public Task CreateInvoice()
        {
            _createInvoice = new CreateInvoiceViewModel(_events, Data);
            _createInvoice.SelectedJob = SelectedJob;
            _windowManager.ShowWindowAsync(_createInvoice, null, CustomWindow.SettingsForDialog(800, 500));
            return Task.CompletedTask;
        }

        public async Task EditClient()
        {
            await _events.PublishOnUIThreadAsync(new CallFromDashboard() { FunctionName = "EditClient" });
        }

        public async Task DeleteRow()
        {
            await Database.DeleteJob(SelectedJob.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
        }
    }
}