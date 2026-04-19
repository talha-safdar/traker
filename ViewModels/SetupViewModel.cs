using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Services;
using Traker.States;

namespace Traker.ViewModels
{
    using Database;

    public class SetupViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly AppState _appState;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        // user info
        private string _fullName;
        private string _email;
        private string _phone;
        private string _companyType;

        // business info
        private string _businessName;
        private string _country;
        private string _city;
        private string _address;
        private string _postcode;
        #endregion

        public SetupViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState, DataService dataService)
        {
            _events = events;
            _windowManager = windowManager;
            _appState = appState;
            _dataService = dataService;
        }

        #region Public View Functions
        public async Task ConfirmUserSetup()
        {
            await Database.Createuser(FullName, Email, Phone);

            // refresh database

            // check if company or not then deal with busienss name
        }

        public Task SetBusinessType(string businessType)
        {
            CompanyType = businessType;
            return Task.CompletedTask;
        }
        #endregion

        #region Public View Variables
        #region user info
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyOfPropertyChange(() => Phone);
            }
        }

        public string CompanyType
        {
            get { return _companyType; }
            set
            {
                _companyType = value;
                NotifyOfPropertyChange(() => CompanyType);
            }
        }
        #endregion

        #region business info
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
        #endregion
        #endregion
    }
}