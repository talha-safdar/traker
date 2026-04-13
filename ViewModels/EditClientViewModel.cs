using Caliburn.Micro;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Events;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;

    class EditClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private string _clientName;
        private string _clientType;
        private string _companyName;
        private string _clientEmail;
        private string _phoneNumber;
        private string _billingAddress;
        private string _city;
        private string _postcode;
        private string _country;
        private string _createdDate;
        private bool _isActive;
        #endregion

        public DashboardModel SelectedRow; // data passed by DashboardVM

        public EditClientViewModel(IEventAggregator events)
        {
            _events = events;
            SelectedRow = new DashboardModel();
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            // pre-fill form with available data
            ClientType = SelectedRow.ClientType;
            ClientName = SelectedRow.ClientName;
            ClientEmail = SelectedRow.ClientEmail;
            CompanyName = SelectedRow.CompanyName;
            PhoneNumber = SelectedRow.ClientPhone;
            BillingAddress = SelectedRow.BillingAddress;
            City = SelectedRow.City;
            Postcode = SelectedRow.City;
            Country = SelectedRow.Country;
            CreatedDate = SelectedRow.CreatedDate.ToString();
            IsActive = SelectedRow.IsActive;

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public void ConfirmEditClient()
        {
            // for type 

            Database.EditClient(SelectedRow.ClientId, ClientType, ClientName, ClientEmail, CompanyName, PhoneNumber, BillingAddress, City, Postcode, Country, IsActive);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());
        }

        public async Task Exit()
        {
            await TryCloseAsync();
        }
        #endregion

        #region Public View Variables
        public string ClientType
        {
            get { return _clientType; }
            set
            {
                _clientType = value;
                NotifyOfPropertyChange(() => ClientType);
            }
        }

        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                NotifyOfPropertyChange(() => ClientName);
            }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                _companyName = value;
                NotifyOfPropertyChange(() => CompanyName);
            }
        }

        public string ClientEmail
        {
            get { return _clientEmail; }
            set
            {
                _clientEmail = value;
                NotifyOfPropertyChange(() => ClientEmail);
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                NotifyOfPropertyChange(() => PhoneNumber);
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

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyOfPropertyChange(() => City);
            }
        }

        public string Postcode
        {
            get { return _postcode; }
            set
            {
                _postcode = value;
                NotifyOfPropertyChange(() => Postcode);
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                NotifyOfPropertyChange(() => Country);
            }
        }

        public string CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                NotifyOfPropertyChange(() => CreatedDate);
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                NotifyOfPropertyChange(() => IsActive);
            }
        }
        #endregion
    }
}
