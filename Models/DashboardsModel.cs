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
        public string JobStatus { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = new DateTime();
        public string InvoiceStatus { get; set; } = String.Empty;
        public string Paid { get; set; } = string.Empty;



        // useful for data searching
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public int ClientId { get; set; } = 0;
        public int JobId { get; set; } = 0;
        public List<JobsModel> Jobs { get; set; } = new List<JobsModel>(); // list of jobs of the client
        public List<InvoicesModel> Invoices { get; set; } = new List<InvoicesModel>(); // list of invoices of the client
    }
}
