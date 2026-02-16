using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;
    using System.Diagnostics;
    using System.Globalization;
    using Traker.Events;

    public class CreateInvoiceViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private string _billingName;
        private string _billingAddress;
        private string _billingCity;
        private string _billingPostcode;
        private string _billingCountry;
        private string _dueDate;
        private decimal _subtotal;
        private string _vatValue;
        private decimal _totalAmount;
        #endregion

        #region Public Variables
        public List<string> InvoiceStatusEdit { get; set; } = new List<string> { "0%", "5%", "20%" };

        /*
         * create a new model containing all the important information to pre-fill
         * the invoice form could be using DashboardModel since it's under row selection
         */
        public DashboardModel SelectedRow; // data passed by DashboardVM
        public List<ClientsModel> Clients;
        public List<JobsModel> Jobs;
        public List<InvoicesModel> Invoices;
        #endregion

        public CreateInvoiceViewModel(IEventAggregator events)
        {
            _events = events;

            SelectedRow = new DashboardModel(); // to be improved to add useful properties

            _billingName = string.Empty;
            _billingAddress = string.Empty;
            _billingCity = string.Empty;
            _billingPostcode = string.Empty;
            _billingCountry = string.Empty;
            _dueDate = string.Empty;
            _subtotal = 0.0m;
            _vatValue = string.Empty;
            _totalAmount = 0.0m;

        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            BillingName = SelectedRow.ClientName;

            return base.OnInitializedAsync(cancellationToken);
        }

        public Task CreateInvoice()
        {
            BillingName = "Obama";
            BillingAddress = "32 Downing Street";
            BillingCity = "London";
            BillingPostcode = "SW1A 2AA";
            BillingCountry = "United Kingdom";
            DueDate = "23/10/2026";
            Subtotal = 1000.00m;
            VatValue = "20%";
            TotalAmount = 1200.00m;


            /*
             * InvoiceNumber
             * IssueDate
             * Status
             */

            //public static Task CreateInvoice(int jobId, decimal subtotal, decimal taxAmount, decimal totalAmount, DateTime dueDate, string billingName, string billingAddress, string billingCity, string billingPostcode, string billingCountry)


            var dueDate = DateTime.MinValue;
            //decimal amount = 0;

            if (DueDate != String.Empty)
            {
                dueDate = DateTime.ParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            //if (VatValue != String.Empty)
            //{
            //    amount = decimal.Parse(VatValue, CultureInfo.InvariantCulture);
            //}


            int result = int.Parse(VatValue.TrimEnd('%'));

            Database.CreateInvoice(SelectedRow.JobId, Subtotal, result, TotalAmount, dueDate, BillingName, BillingAddress, BillingCity, BillingPostcode, BillingCountry);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());

            return Task.CompletedTask;
        }

        #region Public View Variables
        public string BillingName
        {
            get { return _billingName; }
            set
            {
                _billingName = value;
                NotifyOfPropertyChange(() => BillingName);
            }
        }

        public string BillingAddress
        {
            get { return _billingAddress; }
            set
            {
                _billingAddress = value;
                NotifyOfPropertyChange(() => BillingAddress);
            }
        }

        public string BillingCity
        {
            get { return _billingCity; }
            set
            {
                _billingCity = value;
                NotifyOfPropertyChange(() => BillingCity);
            }
        }

        public string BillingPostcode
        {
            get { return _billingPostcode; }
            set
            {
                _billingPostcode = value;
                NotifyOfPropertyChange(() => BillingPostcode);
            }
        }

        public string BillingCountry
        {
            get { return _billingCountry; }
            set
            {
                _billingCountry = value;
                NotifyOfPropertyChange(() => BillingCountry);
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
            }
        }

        public decimal Subtotal
        {
            get { return _subtotal; }
            set
            {
                _subtotal = value;
                NotifyOfPropertyChange(() => Subtotal);
            }
        }

        public string VatValue
        {
            get { return _vatValue; }
            set
            {
                _vatValue = value;
                NotifyOfPropertyChange(() => VatValue);

                // update VAT calculation
            }
        }

        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                _totalAmount = value;
                NotifyOfPropertyChange(() => TotalAmount);
            }
        }
        #endregion
    }
}
