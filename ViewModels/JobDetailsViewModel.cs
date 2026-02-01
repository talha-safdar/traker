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
    public class JobDetailsViewModel : Screen
    {
        public DashboardModel selectedRow;

        public JobDetailsViewModel()
        {

        }

        public Task setStatus(string status)
        {
            if (status == "New")
            {
                Debug.WriteLine("Status set to New");
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

            return Task.CompletedTask;
        }
    }
}
