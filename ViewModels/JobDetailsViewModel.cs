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

        public DashboardModel selectedRow; // data passed by DashboardVM

        public JobDetailsViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public Task OpenFolder()
        {
            Debug.WriteLine("Open folder");

            FileStore.LocateFolder(selectedRow.ClientId, selectedRow.ClientName);

            return Task.CompletedTask;
        }

        public async Task SetStatus(string status)
        {
            await Database.SetStatus(status, selectedRow.ClientId, selectedRow.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
        }

        public async Task DeleteRow()
        {
            await Database.DeleteRow(selectedRow.ClientId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
        }
    }
}
