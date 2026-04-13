using Caliburn.Micro;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #region Private View Variables
        // editable and shown on the data grid
        private string _clientName = string.Empty;
        private string _jobTitle = string.Empty;
        private string _jobDescription = string.Empty;
        private string _price = string.Empty;
        private string _jobStatus = string.Empty;
        private DateOnly _dueDate = new DateOnly();
        private string _invoiceStatus = string.Empty;

        /*
         * to add information that could potentially pre-fill the invoice form
         */

        // to show or not to show dropdown menu box for invoice status
        public bool HasInvoice { get; set; } = false;

        // useful for data searching and storing extra data about the client
        public int ClientId { get; set; } = 0;
        public string ClientType { get; set; } = string.Empty;
        public string TypeIcon { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateOnly CreatedDate { get; set; } = new DateOnly();
        public bool IsActive { get; set; } = true;
        public int JobId { get; set; } = 0;
        public List<JobsModel> Jobs { get; set; } = new List<JobsModel>(); // list of jobs of the client
        public List<InvoicesModel> Invoices { get; set; } = new List<InvoicesModel>(); // list of invoices of the client
        #endregion

        #region Public View Variables
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                // update database here
                NotifyOfPropertyChange(() => ClientName);
            }
        }

        public string JobTitle
        {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                // update database here
                NotifyOfPropertyChange(() => JobTitle);
            }
        }

        public string JobDescription
        {
            get { return _jobDescription; }
            set
            {
                _jobDescription = value;
                // update database here
                NotifyOfPropertyChange(() => JobDescription);
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                // update database here
                NotifyOfPropertyChange(() => Price);
            }
        }

        public string JobStatus
        {
            get { return _jobStatus; }
            set
            {
                _jobStatus = value;
                // update database here
                NotifyOfPropertyChange(() => JobStatus);
            }
        }

        public DateOnly DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                // update database here
                NotifyOfPropertyChange(() => DueDate);
            }
        }

        public string InvoiceStatus
        {
            get { return _invoiceStatus; }
            set
            {
                _invoiceStatus = value;
                // update database here
                NotifyOfPropertyChange(() => InvoiceStatus);
            }
        }
        #endregion
    }
}