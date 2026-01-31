using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class DashboardModel
    {
        public string ClientName { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = new DateTime();
        public string Paid { get; set; } = string.Empty;



        // useful for data searching
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public int ClientId { get; set; } = 0;
        public List<JobModel> Jobs { get; set; } = new List<JobModel>(); // list of jobs of the client
        public List<InvoiceModel> Invoices { get; set; } = new List<InvoiceModel>(); // list of invoices of the client
    }
}
