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
    using System.Windows;
    using System.Windows.Input;
    using Traker.Events.DashboardVM;
    using Traker.Models.Database;
    using Traker.States;

    public class BusinessInfoViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
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

        private BusinessModel _businessModel;
        private UserModel _userModel;

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field variables
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        #endregion

        public BusinessInfoViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _individualView = false;
            _companyView = false;
            _businessName = string.Empty;
            _country = string.Empty;
            _city = string.Empty;
            _address = string.Empty;
            _postcode = string.Empty;
            _vatNumber = string.Empty;
            _registrationNumber = string.Empty;
            _businessNameOpacity = 0.0;

            _businessModel = new BusinessModel();
            _userModel = new UserModel();
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // submit button
                EnableSubmitBtn = false;
                OpacitySubmitBtn = _halfOpacity;

                _businessModel = await Database.FetchBusiness();
                _userModel = await Database.FetchUser();

                if (_businessModel.BusinessType == Names.Individual)
                {
                    BusinessName = _userModel.FullName;
                    BusinessNameOpacity = 0.5;
                    CompanyView = false;
                    IndividualView = true;
                }
                else if (_businessModel.BusinessType == Names.Company)
                {
                    BusinessName = _businessModel.BusinessName;
                    BusinessNameOpacity = 1.0;
                    IndividualView = false;
                    CompanyView = true;
                }

                Address = _businessModel.Address;
                City = _businessModel.City;
                Postcode = _businessModel.Postcode;
                Country = _businessModel.Country;
                VatNumber = _businessModel.VatNumber;
                RegistrationNumber = _businessModel.RegistrationNumber;
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Initialise Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BankInfoViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            await base.OnInitializedAsync(cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    await Exit();
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BusinessInfoViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task ConfirmBusinessInfoChanges()
        {
            try
            {
                await TryCloseAsync();
                _state.WindowFormOpen = false;
                await Task.Run(async() => 
                {
                    await Database.EditBusiness(_userModel.UserId, BusinessName.Trim(), Country.Trim(), City.Trim(), Address.Trim(), Postcode.Trim(), VatNumber.Trim(), RegistrationNumber.Trim());
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Confirm Changes";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BusinessInfoViewModel: ConfirmBusinessInfoChanges() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                if (_businessModel.Address != Address ||
                    _businessModel.City != City ||
                    _businessModel.Postcode != Postcode ||
                    _businessModel.Country != Country ||
                    _businessModel.VatNumber != VatNumber ||
                    _businessModel.RegistrationNumber != RegistrationNumber
                    )
                {
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 0;
                        _state.messageBoxVM.HeadMessage = "Discard changes?";
                        _state.messageBoxVM.Message = Names.DiscardEsc;
                        _state.messageBoxVM.ButtonStyle = Names.NoYes;
                        await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }

                    // if clicked yes
                    if (_state.messageBoxVM.Output == true)
                    {
                        await TryCloseAsync();
                        _state.WindowFormOpen = false;
                    }
                }
                else
                {
                    await TryCloseAsync();
                    _state.WindowFormOpen = false;
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BusinessInfoViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        private Task CanSubmit()
        {
            try
            {
                if (_businessModel.BusinessType == Names.Individual)
                {
                    if (BusinessName != _userModel.FullName ||
                    Address != _businessModel.Address ||
                    City != _businessModel.City ||
                    Postcode != _businessModel.Postcode ||
                    Country != _businessModel.Country ||
                    VatNumber != _businessModel.VatNumber ||
                    RegistrationNumber != _businessModel.RegistrationNumber)
                    {
                        EnableSubmitBtn = true;
                        OpacitySubmitBtn = _fullOpacity;
                    }
                    else
                    {
                        EnableSubmitBtn = false;
                        OpacitySubmitBtn = _halfOpacity;
                    }
                }
                else if (_businessModel.BusinessType == Names.Company)
                {
                    if (BusinessName != _businessModel.BusinessName ||
                    Address != _businessModel.Address ||
                    City != _businessModel.City ||
                    Postcode != _businessModel.Postcode ||
                    Country != _businessModel.Country ||
                    VatNumber != _businessModel.VatNumber ||
                    RegistrationNumber != _businessModel.RegistrationNumber)
                    {
                        EnableSubmitBtn = true;
                        OpacitySubmitBtn = _fullOpacity;
                    }
                    else
                    {
                        EnableSubmitBtn = false;
                        OpacitySubmitBtn = _halfOpacity;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Edit User";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: CanSubmit() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
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
                CanSubmit();
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                NotifyOfPropertyChange(() => Country);
                CanSubmit();
            }
        }

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyOfPropertyChange(() => City);
                CanSubmit();
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyOfPropertyChange(() => Address);
                CanSubmit();
            }
        }

        public string Postcode
        {
            get { return _postcode; }
            set
            {
                _postcode = value;
                NotifyOfPropertyChange(() => Postcode);
                CanSubmit();
            }
        }

        public string VatNumber
        {
            get { return _vatNumber; }
            set
            {
                _vatNumber = value;
                NotifyOfPropertyChange(() => VatNumber);
                CanSubmit();
            }
        }

        public string RegistrationNumber
        {
            get { return _registrationNumber; }
            set
            {
                _registrationNumber = value;
                NotifyOfPropertyChange(() => RegistrationNumber);
                CanSubmit();
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

        public bool EnableSubmitBtn
        {
            get { return _enableSubmitBtn; }
            set
            {
                _enableSubmitBtn = value;
                NotifyOfPropertyChange(() => EnableSubmitBtn);
            }
        }

        public double OpacitySubmitBtn
        {
            get { return _opacitySubmitBtn; }
            set
            {
                _opacitySubmitBtn = value;
                NotifyOfPropertyChange(() => OpacitySubmitBtn);
            }
        }
        #endregion
    }
}