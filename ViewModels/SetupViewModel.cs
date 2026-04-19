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
    using System.Collections.ObjectModel;
    using System.Threading;
    using Traker.Helper;

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
        private bool _isIndividual;
        private double _businessNameOpacity;

        // window management
        //private bool _userWindow;
        //private bool _businessWindow;
        //private bool _bankWindow;
        private ObservableCollection<bool> _setupWindows;
        #endregion

        public SetupViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState, DataService dataService)
        {
            // caliburn
            _events = events;
            _windowManager = windowManager;
            _appState = appState;
            _dataService = dataService;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            // user info
            _fullName = string.Empty;
            _email = string.Empty;
            _phone = string.Empty;
            _companyType = string.Empty;
            // debug
            _fullName = "Adolf Hitler";
            _email = "adolf@gmail.com";
            _phone = "34345435";

            // business info
            _businessName = string.Empty;
            _country = string.Empty;
            _city = string.Empty;
            _address = string.Empty;
            _postcode = string.Empty;
            _isIndividual = true;
            _businessNameOpacity = 1.0;

            // window management
            //_userWindow = true;
            //_businessWindow = false;
            //_bankWindow = false;
            SetupWindows = new ObservableCollection<bool>() { true, false, false }; // 0=user, 1=business, 2=bank

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task ConfirmUserSetup()
        {
            await Database.Createuser(FullName, Email, Phone);

            // refresh database
            await _dataService.RefreshDatabase();

            // check if company or not then deal with busienss name
            // if user table is not empty
            // only exepcted one row
            if (_dataService.User?.Any() == true)
            {
                // individual
                if (CompanyType == Names.Individual)
                {
                    BusinessName = FullName;
                    IsIndividual = true; // set businessName to readonly
                    BusinessNameOpacity = 0.5; // set opacity to half
                }
                else if (CompanyType == Names.Company)
                {
                    IsIndividual = false;
                    BusinessNameOpacity = 1.0; // set opacity to full
                }

                await UIHelper.SwitchBetweenViews(SetupWindows, 1);
            }
        }

        public async Task ConfirmBusinessSetup()
        {
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

        public bool IsIndividual
        {
            get { return _isIndividual; }
            set
            {
                _isIndividual = value;
                NotifyOfPropertyChange(() => IsIndividual);
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

        public ObservableCollection<bool> SetupWindows
        {
            get { return _setupWindows; }
            set
            {
                _setupWindows = value;
                NotifyOfPropertyChange(() => SetupWindows);
            }
        }
        #endregion
        #endregion
    }
}