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
    using System.Net;
    using System.Threading;
    using Traker.Events;
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
        private string _businessType;

        // business info
        private string _businessName;
        private string _address;
        private string _city;
        private string _postcode;
        private string _country;
        private string _vatNumber;
        private string _registrationNumber;
        private bool _isIndividual; // for UI
        private double _businessNameOpacity; // for UI

        // bank info
        private string _bankName;
        private string _accountName;
        private string _accountNumber;
        private string _sortcode;
        private string _IBAN;
        private string _BIC;

        // window management
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
            _businessType = string.Empty;
            // debug
            _fullName = "Adolf Hitler";
            _email = "adolf@gmail.com";
            _phone = "34345435";

            // business info
            _businessName = string.Empty;
            //_address = string.Empty;
            //_city = string.Empty;
            //_postcode = string.Empty;
            //_country = string.Empty;
            //debug
            _businessName = "Nigga Destroyer Ltd";
            _address = "32 Woodlands Street";
            _city = "Berlin";
            _postcode = "M4kr00t";
            _country = "Germany";

            _vatNumber = "-1"; // default -1 for non vat registered
            _registrationNumber = "-1"; // default -1 for non company
            _isIndividual = true;
            _businessNameOpacity = 1.0;

            // bank
            _bankName = "Nazi Bank Ltd";
            _accountName = "Adolf Muhammad Hitler";
            _accountNumber = "1232434";
            _sortcode = "12-12-12";
            _IBAN = "342423423";
            _BIC = "2342423";

        // window management
        SetupWindows = new ObservableCollection<bool>() { true, false, false, false }; // 0=user, (1=individual, 2=company), 3=bank

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        /// <summary>
        /// 1/3 User setup
        /// </summary>
        public async Task ConfirmUserSetup()
        {
            await Database.CreateUser(FullName, Email, Phone);

            // refresh database
            await _dataService.RefreshDatabase();

            // check if company or not then deal with busienss name
            // if user table is not empty
            // only exepcted one row
            if (_dataService.User?.Any() == true)
            {
                // individual
                if (BusinessType == Names.Individual)
                {
                    BusinessName = FullName;
                    IsIndividual = true; // set businessName to readonly
                    BusinessNameOpacity = 0.5; // set opacity to half
                    await UIHelper.SwitchBetweenViews(SetupWindows, 1); // 1=individual
                }
                else if (BusinessType == Names.Company)
                {
                    IsIndividual = false;
                    BusinessNameOpacity = 1.0; // set opacity to full
                    await UIHelper.SwitchBetweenViews(SetupWindows, 2); // 2=company
                }

            }
        }

        /// <summary>
        /// 2/3 Business setup
        /// </summary>
        public async Task ConfirmBusinessSetup()
        {
            // use checks and try-catch

            await Database.CreateBusiness(_dataService.User[0].UserId, BusinessName, BusinessType, Country, City, Address, Postcode, VatNumber, RegistrationNumber);

            await UIHelper.SwitchBetweenViews(SetupWindows, 3); // 3=bank
        }

        /// <summary>
        /// 3/3 Bank setup
        /// </summary>
        public async Task ConfirmBankSetup()
        {
            await Database.CreateBank(_dataService.User[0].UserId, BankName, AccountName, AccountNumber, Sortcode, IBAN, BIC);
            await TryCloseAsync();            
            await _dataService.RefreshDatabase(); // refresh the data service
            await _events.PublishOnUIThreadAsync(new ShellVM { Command = Names.SetupCompleted });
        }

        public Task SetBusinessType(string businessType)
        {
            BusinessType = businessType;
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

        public string BusinessType
        {
            get { return _businessType; }
            set
            {
                _businessType = value;
                NotifyOfPropertyChange(() => BusinessType);
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

        #region bank info
        public string BankName
        {
            get { return _bankName; }
            set
            {
                _bankName = value;
                NotifyOfPropertyChange(() => BankName);
            }
        }

        public string AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                NotifyOfPropertyChange(() => AccountName);
            }
        }

        public string AccountNumber
        {
            get { return _accountNumber; }
            set
            {
                _accountNumber = value;
                NotifyOfPropertyChange(() => AccountNumber);
            }
        }

        public string Sortcode
        {
            get { return _sortcode; }
            set
            {
                _sortcode = value;
                NotifyOfPropertyChange(() => Sortcode);
            }
        }

        public string IBAN
        {
            get { return _IBAN; }
            set
            {
                _IBAN = value;
                NotifyOfPropertyChange(() => IBAN);
            }
        }

        public string BIC
        {
            get { return _BIC; }
            set
            {
                _BIC = value;
                NotifyOfPropertyChange(() => BIC);
            }
        }
        #endregion
        #endregion
    }
}