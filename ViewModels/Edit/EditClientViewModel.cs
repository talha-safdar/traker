using Caliburn.Micro;
using Traker.Models;

namespace Traker.ViewModels.Edit
{
    using Database;
    using Microsoft.VisualBasic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using Traker.Data;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Services;
    using Traker.States;

    class EditClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state; // state binding variable accessible from other viewmodels
        #endregion

        #region Public Variables
        public DashboardModel SelectedJob;
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

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field Variables
        private JobsListViewModel _jobsListViewModel;
        #endregion

        public EditClientViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            SelectedJob = new DashboardModel();

            _clientName = string.Empty;
            _clientType = string.Empty;
            _companyName = string.Empty;
            _clientEmail = string.Empty;
            _phoneNumber = string.Empty;
            _billingAddress = string.Empty;
            _city = string.Empty;
            _postcode = string.Empty;
            _country = string.Empty;
            _createdDate = string.Empty;

            _jobsListViewModel = new JobsListViewModel(_events, _windowManager, _dataService, _state);
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // submit button
                EnableSubmitBtn = false;
                //OpacitySubmitBtn = _halfOpacity;

                ClientType = SelectedJob.ClientType;
                ClientName = SelectedJob.ClientName;
                ClientEmail = SelectedJob.ClientEmail;
                CompanyName = SelectedJob.CompanyName;
                PhoneNumber = SelectedJob.ClientPhone;
                BillingAddress = SelectedJob.Address;
                City = SelectedJob.City;
                Postcode = SelectedJob.Postcode;
                Country = SelectedJob.Country;
                CreatedDate = SelectedJob.CreatedDate.ToString();
                IsActive = SelectedJob.IsActive;
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
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (SelectedJob.ClientType != ClientType ||
                        SelectedJob.CompanyName != CompanyName ||
                        SelectedJob.ClientName != ClientName ||
                        SelectedJob.ClientEmail != ClientEmail ||
                        SelectedJob.ClientPhone != PhoneNumber ||
                        SelectedJob.Address != BillingAddress ||
                        SelectedJob.City != City ||
                        SelectedJob.Postcode != Postcode ||
                        SelectedJob.Country != Country)
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
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task ConfirmEditClient()
        {
            try
            {
                if (SelectedJob.ClientType != ClientType ||
                SelectedJob.CompanyName != CompanyName ||
                SelectedJob.ClientName != ClientName ||
                SelectedJob.ClientEmail != ClientEmail ||
                SelectedJob.ClientPhone != PhoneNumber ||
                SelectedJob.Address != BillingAddress ||
                SelectedJob.City != City ||
                SelectedJob.Postcode != Postcode ||
                SelectedJob.Country != Country)
                {
                    // check if name changes for folder naming purpose
                    if ((SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName) != ClientName)
                    {
                        // if it returns true then the folder was successfully renamed
                        if (await FileStore.UpdateClientFolderName(SelectedJob.ClientId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName.Trim() : SelectedJob.CompanyName.Trim(), SelectedJob.ClientType == Names.Individual ? ClientName.Trim() : CompanyName.Trim()) == true)
                        {
                            await Task.Run(async () =>
                            {
                                // only update database if file folder change name was successful
                                await Database.EditClient(SelectedJob.ClientId, ClientType.Trim(), ClientName.Trim(), ClientEmail.Trim(), CompanyName.Trim(), PhoneNumber.Trim(), BillingAddress.Trim(), City.Trim(), Postcode.Trim(), Country.Trim(), IsActive);
                                await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                            });
                            await TryCloseAsync();
                        }
                    }
                    else // folder name not changed
                    {
                        await Task.Run(async () =>
                        {
                            // only update database if file folder change name was successful
                            await Database.EditClient(SelectedJob.ClientId, ClientType.Trim(), ClientName.Trim(), ClientEmail.Trim(), CompanyName.Trim(), PhoneNumber.Trim(), BillingAddress.Trim(), City.Trim(), Postcode.Trim(), Country.Trim(), IsActive);
                            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                        });
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
                    _state.messageBoxVM.HeadMessage = "Confirm Edit";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: ConfirmEditClient() FAIL\n\t{ex.Message}");
            }
        }

        public async Task DeleteClient()
        {
            try
            {
                await Task.Run(async () => {
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 0;
                        _state.messageBoxVM.HeadMessage = "Delete Client";
                        _state.messageBoxVM.Message = Names.DeleteClientConfirmation;
                        _state.messageBoxVM.ButtonStyle = Names.NoYes;
                        await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }

                    // if clicked yes
                    if (_state.messageBoxVM.Output == true)
                    {
                        // delete client folder
                        await FileStore.DeleteClientFolder(SelectedJob.ClientId, SelectedJob.ClientName.Trim());

                        // delete client database row
                        await Database.DeleteClient(SelectedJob.ClientId);
                        await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                        await TryCloseAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Confirm Edit";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: DeleteClient() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                if (SelectedJob.ClientType != ClientType ||
                    SelectedJob.CompanyName != CompanyName ||
                    SelectedJob.ClientName != ClientName ||
                    SelectedJob.ClientEmail != ClientEmail ||
                    SelectedJob.ClientPhone != PhoneNumber ||
                    SelectedJob.Address != BillingAddress ||
                    SelectedJob.City != City ||
                    SelectedJob.Postcode != Postcode ||
                    SelectedJob.Country != Country)
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
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OpenJobsList()
        {
            try
            {
                _jobsListViewModel.SelectedJob = SelectedJob;
                await _windowManager.ShowDialogAsync(_jobsListViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Open Jobs List";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditClientViewModel: OpenJobsList() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        private Task ToggleClientType()
        {
            if (ClientType == Names.Individual)
            {
                CompanyName = string.Empty;
            }
            return Task.CompletedTask;
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
                ToggleClientType();
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