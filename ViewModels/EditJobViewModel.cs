using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Events;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;
    using System.Globalization;

    public class EditJobViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private string _clientName;
        private DateOnly _createdDate;
        private string _jobTitle;
        private string _jobDescription;
        private string _status;
        private string _price;
        private string _amountReceived;
        private DateOnly _startDate;
        private DateOnly _dueDate;
        private string _reaminingAmount;
        #endregion

        public DashboardModel SelectedJob; // data passed by DashboardVM

        public EditJobViewModel(IEventAggregator events)
        {
            _events = events;
            SelectedJob = new DashboardModel();
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            ClientName = SelectedJob.ClientName;
            CreatedDate = SelectedJob.CreatedDate;
            JobTitle = SelectedJob.JobTitle;
            JobDescription = SelectedJob.JobDescription;
            Status = SelectedJob.JobStatus;
            Price = SelectedJob.Price;
            AmountReceived = SelectedJob.AmountReceived;
            StartDate = SelectedJob.StartDate;
            DueDate = SelectedJob.DueDate;

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #region Public View Functions
        public void ConfirmJobChanges()
        {
            // price
            decimal.TryParse(Price,
            NumberStyles.Currency,
            CultureInfo.CurrentCulture,
            out var priceFormatted);
            
            // amount received
            decimal.TryParse(AmountReceived,
            NumberStyles.Currency,
            CultureInfo.CurrentCulture,
            out var AmountReceivedFormatted);


            Database.EditJob(SelectedJob.JobId, JobTitle, JobDescription, Status, priceFormatted.ToString(), AmountReceivedFormatted.ToString(), StartDate, DueDate);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());
        }

        public Task CancelJobChanges()
        {
            // for type 

            // Database.EditClient(SelectedRow.ClientId, ClientType, ClientName, ClientEmail, CompanyName, PhoneNumber, BillingAddress, City, Postcode, Country, IsActive);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());

            return Task.CompletedTask;
        }

        public async Task DeleteJob()
        {
            await Database.DeleteJob(SelectedJob.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
        }

        public async Task Exit()
        {
            await TryCloseAsync();
        }
        #endregion

        #region Public View Variables
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                NotifyOfPropertyChange(() => ClientName);
            }
        }

        public DateOnly CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                NotifyOfPropertyChange(() => CreatedDate);
            }
        }

        public string JobTitle
        {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                NotifyOfPropertyChange(() => JobTitle);
            }
        }

        public string JobDescription
        {
            get { return _jobDescription; }
            set
            {
                _jobDescription = value;
                NotifyOfPropertyChange(() => JobDescription);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                if (AmountReceived != null)
                {
                    ReaminingAmount = (decimal.Parse(Price, NumberStyles.Currency, CultureInfo.CurrentCulture) - decimal.Parse(AmountReceived, NumberStyles.Currency, CultureInfo.CurrentCulture)).ToString("c");
                }
                NotifyOfPropertyChange(() => Price);
            }
        }

        public string AmountReceived
        {
            get { return _amountReceived; }
            set
            {
                _amountReceived = value;
               ReaminingAmount = (decimal.Parse(Price, NumberStyles.Currency, CultureInfo.CurrentCulture) - decimal.Parse(AmountReceived, NumberStyles.Currency, CultureInfo.CurrentCulture)).ToString("c");
                NotifyOfPropertyChange(() => AmountReceived);
            }
        }

        public DateOnly StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        public DateOnly DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
            }
        }

        public string ReaminingAmount
        {
            get { return _reaminingAmount; }
            set
            {
                _reaminingAmount = value;
                NotifyOfPropertyChange(() => ReaminingAmount);
            }
        }
        #endregion
    }
}