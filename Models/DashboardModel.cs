using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class DashboardModel
    {
        public int ClientId { get; set; } = 0;
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public List<JobsModel> Jobs { get; set; } = new List<JobsModel>(); // list of jobs of the client
        public List<InvoicesModel> Invoices { get; set; } = new List<InvoicesModel>(); // list of invoices of the client
    }
}
