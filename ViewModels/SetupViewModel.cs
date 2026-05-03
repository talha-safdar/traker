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
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
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
        private ObservableCollection<bool> _toggleButtons;
        private ObservableCollection<Brush> _backgroundButtons; // active=#333333. inactive=#1A1A1A
        private ObservableCollection<Brush> _foregroundText; // active=#FFFFFF. inactive=#888888

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

        // next button
        private bool _enableNextBtn;
        private double _opacityBtn;
        #endregion

        #region Private Class Field Variables
        private string _activeButonColour = "#333333";
        private string _inactiveButonColour = "#1A1A1A";
        private string _activeTextColour = "#FFFFFF";
        private string _inactiveTextColour = "#888888";

        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
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
            ToggleButtons = new ObservableCollection<bool>() { true, true };
            BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)) };
            ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)) };

            // pre-set business values for if is indiviudal
            _vatNumber = string.Empty;
            _registrationNumber = string.Empty;

            //user next button
            EnableNextBtn = false;
            OpacityBtn = _halfOpacity;

            //// user info
            //_fullName = string.Empty;
            //_email = string.Empty;
            //_phone = string.Empty;
            //_businessType = string.Empty;
            //// debug
            //_fullName = "Adolf Hitler";
            //_email = "adolf@gmail.com";
            //_phone = "34345435";

            // business info
            //_businessName = string.Empty;
            //_address = string.Empty;
            //_city = string.Empty;
            //_postcode = string.Empty;
            //_country = string.Empty;
            //debug
            //_businessName = "Nigga Destroyer Ltd";
            //_address = "32 Woodlands Street";
            //_city = "Berlin";
            //_postcode = "M4kr00t";
            //_country = "Germany";

            //_vatNumber = "-1"; // default -1 for non vat registered
            //_registrationNumber = "-1"; // default -1 for non company
            //_isIndividual = true;
            //_businessNameOpacity = 1.0;

            //// bank
            //_bankName = "Nazi Bank Ltd";
            //_accountName = "Adolf Muhammad Hitler";
            //_accountNumber = "1232434";
            //_sortcode = "12-12-12";
            //_IBAN = "342423423";
            //_BIC = "2342423";

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

            // reset the next button
            EnableNextBtn = false;
            OpacityBtn = _halfOpacity;
        }

        /// <summary>
        /// 2/3 Business setup
        /// </summary>
        public async Task ConfirmBusinessSetup()
        {
            // use checks and try-catch

            await Database.CreateBusiness(_dataService.User[0].UserId, BusinessName, BusinessType, Country, City, Address, Postcode, VatNumber, RegistrationNumber);

            await UIHelper.SwitchBetweenViews(SetupWindows, 3); // 3=bank

            // reset the next button
            EnableNextBtn = false;
            OpacityBtn = _halfOpacity;
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
            if (BusinessType == Names.Individual)
            {
                ToggleButtons = new ObservableCollection<bool>() { false, true };
                BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)) };
                ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)) };
            }
            else if (BusinessType == Names.Company)
            {
                ToggleButtons = new ObservableCollection<bool>() { true, false };
                BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeButonColour)) };
                ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeTextColour)) };
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Private Functions
        private Task CanMoveFromUserWindow()
        {
            if (string.IsNullOrEmpty(FullName) == false && string.IsNullOrEmpty(Email) == false && string.IsNullOrEmpty(Phone) == false && string.IsNullOrEmpty(BusinessType) == false)
            {
                EnableNextBtn = true;
                OpacityBtn = _fullOpacity;
            }
            else
            {
                EnableNextBtn = false;
                OpacityBtn = _halfOpacity;
            }
            return Task.CompletedTask;
        }

        private Task CanMoveFromBusinessWindow()
        {
            if (BusinessType == Names.Individual)
            {
                if (string.IsNullOrEmpty(BusinessName) == false && string.IsNullOrEmpty(Country) == false && string.IsNullOrEmpty(City) == false && string.IsNullOrEmpty(Address) == false && string.IsNullOrEmpty(Postcode) == false)
                {
                    EnableNextBtn = true;
                    OpacityBtn = _fullOpacity;
                }
                else
                {
                    EnableNextBtn = false;
                    OpacityBtn = _halfOpacity;
                }
            }
            else if (BusinessType == Names.Company)
            {
                if (string.IsNullOrEmpty(BusinessName) == false && string.IsNullOrEmpty(Country) == false && string.IsNullOrEmpty(City) == false && string.IsNullOrEmpty(Address) == false && string.IsNullOrEmpty(Postcode) == false && string.IsNullOrEmpty(VatNumber) == false && string.IsNullOrEmpty(RegistrationNumber) == false)
                {
                    EnableNextBtn = true;
                    OpacityBtn = _fullOpacity;
                }
                else
                {
                    EnableNextBtn = false;
                    OpacityBtn = _halfOpacity;
                }
            }
            return Task.CompletedTask;
        }        

        private Task CanMoveFromBankWindow()
        {
            if (string.IsNullOrEmpty(BankName) == false && string.IsNullOrEmpty(AccountName) == false && string.IsNullOrEmpty(AccountNumber) == false && string.IsNullOrEmpty(Sortcode) == false && string.IsNullOrEmpty(IBAN) == false && string.IsNullOrEmpty(BIC) == false)
            {
                EnableNextBtn = true;
                OpacityBtn = _fullOpacity;
            }
            else
            {
                EnableNextBtn = false;
                OpacityBtn = _halfOpacity;
            }
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
                CanMoveFromUserWindow();
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                CanMoveFromUserWindow();
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyOfPropertyChange(() => Phone);
                CanMoveFromUserWindow();
            }
        }

        public string BusinessType
        {
            get { return _businessType; }
            set
            {
                _businessType = value;
                NotifyOfPropertyChange(() => BusinessType);
                CanMoveFromUserWindow();
            }
        }

        public ObservableCollection<bool> ToggleButtons
        {
            get { return _toggleButtons; }
            set
            {
                _toggleButtons = value;
                NotifyOfPropertyChange(() => ToggleButtons);
            }
        }

        public ObservableCollection<Brush> BackgroundButtons
        {
            get { return _backgroundButtons; }
            set
            {
                _backgroundButtons = value;
                NotifyOfPropertyChange(() => BackgroundButtons);
            }
        }

        public ObservableCollection<Brush> ForegroundText
        {
            get { return _foregroundText; }
            set
            {
                _foregroundText = value;
                NotifyOfPropertyChange(() => ForegroundText);
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
                CanMoveFromBusinessWindow();
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                NotifyOfPropertyChange(() => Country);
                CanMoveFromBusinessWindow();
            }
        }

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyOfPropertyChange(() => City);
                CanMoveFromBusinessWindow();
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyOfPropertyChange(() => Address);
                CanMoveFromBusinessWindow();
            }
        }

        public string Postcode
        {
            get { return _postcode; }
            set
            {
                _postcode = value;
                NotifyOfPropertyChange(() => Postcode);
                CanMoveFromBusinessWindow();
            }
        }

        public string VatNumber
        {
            get { return _vatNumber; }
            set
            {
                _vatNumber = value;
                NotifyOfPropertyChange(() => VatNumber);
                CanMoveFromBusinessWindow();
            }
        }

        public string RegistrationNumber
        {
            get { return _registrationNumber; }
            set
            {
                _registrationNumber = value;
                NotifyOfPropertyChange(() => RegistrationNumber);
                CanMoveFromBusinessWindow();
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
                CanMoveFromBankWindow();
            }
        }

        public string AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                NotifyOfPropertyChange(() => AccountName);
                CanMoveFromBankWindow();
            }
        }

        public string AccountNumber
        {
            get { return _accountNumber; }
            set
            {
                _accountNumber = value;
                NotifyOfPropertyChange(() => AccountNumber);
                CanMoveFromBankWindow();
            }
        }

        public string Sortcode
        {
            get { return _sortcode; }
            set
            {
                _sortcode = value;
                NotifyOfPropertyChange(() => Sortcode);
                CanMoveFromBankWindow();
            }
        }

        public string IBAN
        {
            get { return _IBAN; }
            set
            {
                _IBAN = value;
                NotifyOfPropertyChange(() => IBAN);
                CanMoveFromBankWindow();
            }
        }

        public string BIC
        {
            get { return _BIC; }
            set
            {
                _BIC = value;
                NotifyOfPropertyChange(() => BIC);
                CanMoveFromBankWindow();
            }
        }
        #endregion

        public bool EnableNextBtn
        {
            get { return _enableNextBtn; }
            set
            {
                _enableNextBtn = value;
                NotifyOfPropertyChange(() => EnableNextBtn);
            }
        }

        public double OpacityBtn
        {
            get { return _opacityBtn; }
            set
            {
                _opacityBtn = value;
                NotifyOfPropertyChange(() => OpacityBtn);
            }
        }
        #endregion
    }
}