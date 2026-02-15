using Caliburn.Micro;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class DashboardModel : PropertyChangedBase
    {
        #region Private View Variables
        // editable
        private string _clientName = string.Empty;
        private string _jobDescription = string.Empty;
        private string _price = string.Empty;
        private string _jobStatus = string.Empty;
        private DateTime _dueDate = new DateTime();
        private string _invoiceStatus = string.Empty;

        // useful for data searching
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public int ClientId { get; set; } = 0;
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

        public DateTime DueDate
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
