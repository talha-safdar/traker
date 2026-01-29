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
        public List<JobModel> Jobs { get; set; } = new List<JobModel>(); // list of jobs of the client
        public List<InvoiceModel> Invoices { get; set; } = new List<InvoiceModel>(); // list of invoices of the client
    }
}
