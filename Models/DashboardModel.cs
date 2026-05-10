using Caliburn.Micro;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models.Database;

namespace Traker.Models
{
    /// <summary>
    /// Represents the data model for a client dashboard, including client details, job information, and invoice status.
    /// </summary>
    /// <remarks><para> The <see cref="DashboardModel"/> class is designed to aggregate and expose information
    /// relevant to a client dashboard view. It provides properties for client identification, contact details, job
    /// descriptions, pricing, due dates, and invoice statuses. </para> <para> This model supports data binding
    /// scenarios and notifies property changes to facilitate UI updates. It also includes collections of related jobs
    /// and invoices for the client. </para></remarks>
    public class DashboardModel : PropertyChangedBase
    {
        // client
        public int ClientId { get; set; } = 0;
        public string ClientType { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string InvoiceStatus { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // jobs
        public int JobId { get; set; } = 0;
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
        public string JobStatus { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = new DateOnly();
        public DateOnly DueDate { get; set; } = new DateOnly();
        public decimal AmountReceived { get; set; } = 0.0m;
        public List<JobsModel> Jobs { get; set; } = new List<JobsModel>(); // list of jobs of the client
        public DateOnly CreatedDate { get; set; } = new DateOnly();


        // invoice
        public bool HasInvoice { get; set; } = false;
        public DateOnly InvoiceDueDate { get; set; } = new DateOnly();
        public DateOnly PaidDate { get; set; } = new DateOnly();
        public List<InvoicesModel> Invoices { get; set; } = new List<InvoicesModel>(); // list of invoices of the client

        // UI
        public string TypeIcon { get; set; } = string.Empty;
    }
}