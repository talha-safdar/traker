using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Events;
using Traker.Helper;
using Traker.Services;

namespace Traker.ViewModels.User
{
    using Database;

    public class BusinessInfoViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        // show either individual or company
        private bool _individualView;
        private bool _companyView;

        private string _businessName;
        private string _country;
        private string _city;
        private string _address;
        private string _postcode;
        private string _vatNumber;
        private string _registrationNumber;
        private double _businessNameOpacity; // for UI
        #endregion

        public BusinessInfoViewModel(IEventAggregator events, DataService dataService)
        {
            _events = events;
            _dataService = dataService;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {

            if (_dataService.Business[0]?.BusinessType == Names.Individual)
            {
                BusinessName = _dataService.User[0].FullName;
                BusinessNameOpacity = 0.5;
                CompanyView = false;
                IndividualView = true;
            }
            else if (_dataService.Business[0]?.BusinessType == Names.Company)
            {
                BusinessName = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).BusinessName;
                BusinessNameOpacity = 1.0;
                IndividualView = false;
                CompanyView = true;
            }

            Address = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).Address;
            City = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).City;
            Postcode = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).Postcode;
            Country = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).Country;
            VatNumber = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).VatNumber;
            RegistrationNumber = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).RegistrationNumber;

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task ConfirmBusinessInfoChanges()
        {
            await Database.EditBusiness(_dataService.User[0].UserId, BusinessName, Country, City, Address, Postcode, VatNumber, RegistrationNumber);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
        }
        #endregion

        #region Public View Variables
        public bool IndividualView
        {
            get => _individualView;
            set
            {
                _individualView = value;
                NotifyOfPropertyChange(() => IndividualView);
            }
        }

        public bool CompanyView
        {
            get => _companyView;
            set
            {
                _companyView = value;
                NotifyOfPropertyChange(() => CompanyView);
            }
        }

        public string BusinessName
        {
            get { return _businessName; }
            set
            {
                _businessName = value;
                NotifyOfPropertyChange(() => BusinessName);
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

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyOfPropertyChange(() => City);
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyOfPropertyChange(() => Address);
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

        public string VatNumber
        {
            get { return _vatNumber; }
            set
            {
                _vatNumber = value;
                NotifyOfPropertyChange(() => VatNumber);
            }
        }

        public string RegistrationNumber
        {
            get { return _registrationNumber; }
            set
            {
                _registrationNumber = value;
                NotifyOfPropertyChange(() => RegistrationNumber);
            }
        }

        public double BusinessNameOpacity
        {
            get { return _businessNameOpacity; }
            set
            {
                _businessNameOpacity = value;
                NotifyOfPropertyChange(() => BusinessNameOpacity);
            }
        }
        #endregion
    }
}
