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

    public class JobDetailsViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        public DashboardModel SelectedRow; // data passed by DashboardVM
        public List<ClientsModel> Clients;
        public List<JobsModel> Jobs;
        public List<InvoicesModel> Invoices;

        public JobDetailsViewModel(IEventAggregator events)
        {
            _events = events;
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
