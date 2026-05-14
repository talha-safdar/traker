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
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
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
            return base.OnInitializedAsync(cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (_dataService.Business[0].Address != Address ||
                        _dataService.Business[0].City != City ||
                        _dataService.Business[0].Postcode != Postcode ||
                        _dataService.Business[0].Country != Country ||
                        _dataService.Business[0].VatNumber != VatNumber ||
                        _dataService.Business[0].RegistrationNumber != RegistrationNumber
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
                        }
                    }
                    else
                    {
                        await TryCloseAsync();
                    }
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
                await Task.Run(async() => 
                {
                    await Database.EditBusiness(_dataService.User[0].UserId, BusinessName.Trim(), Country.Trim(), City.Trim(), Address.Trim(), Postcode.Trim(), VatNumber.Trim(), RegistrationNumber.Trim());
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                    await TryCloseAsync();
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
                if (_dataService.Business[0].Address != Address ||
                    _dataService.Business[0].City != City ||
                    _dataService.Business[0].Postcode != Postcode ||
                    _dataService.Business[0].Country != Country ||
                    _dataService.Business[0].VatNumber != VatNumber ||
                    _dataService.Business[0].RegistrationNumber != RegistrationNumber
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
                    }
                }
                else
                {
                    await TryCloseAsync();
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