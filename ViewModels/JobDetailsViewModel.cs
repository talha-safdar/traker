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
    using Traker.Events;

    public class JobDetailsViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        public DashboardModel selectedRow;

        public JobDetailsViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public async Task setStatus(string status)
        {
            if (status == "New")
            {
                Debug.WriteLine("Status set to New");
                await Database.SetStatus(status, selectedRow.ClientId, selectedRow.JobId);

                await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
            }
            else if (status == "Active")
            {
                Debug.WriteLine("Status set to Active");

                var a = selectedRow.ClientId;
                //var b = selectedRow.Jobs.Where(j => j.Cl)
            }
            else if (status == "Done")
            {
                Debug.WriteLine("Status set to Done");
            }
        }
    }
}
