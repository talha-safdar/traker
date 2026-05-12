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

namespace Traker.ViewModels.Edit
{
    using Database;
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

        #region Private Class Field Variables
        private JobsListViewModel _jobsListViewModel;
        #endregion

        #region Public State Variable
        public DataService Data { get; } // data from the database
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public DashboardModel SelectedJob; // data passed by DashboardVM

        public EditClientViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            Data = dataService;
            State = state;
            _jobsListViewModel = new JobsListViewModel(_events, _windowManager, State, Data);
            SelectedJob = new DashboardModel();
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            // pre-fill form with available data
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

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            // ESC button
            if (e.Key == Key.Escape)
            {
                await TryCloseAsync();
            }
        }

        public async Task ConfirmEditClient()
        {
            await Database.EditClient(SelectedJob.ClientId, ClientType.Trim(), ClientName.Trim(), ClientEmail.Trim(), CompanyName.Trim(), PhoneNumber.Trim(), BillingAddress.Trim(), City.Trim(), Postcode.Trim(), Country.Trim(), IsActive);

            // check if name changes for folder naming purpose
            if ((SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName) != ClientName)
            {
                await FileStore.UpdateClientFolderName(SelectedJob.ClientId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName.Trim() : SelectedJob.CompanyName.Trim(), SelectedJob.ClientType == Names.Individual ? ClientName.Trim() : CompanyName.Trim());
            }

            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
            await TryCloseAsync();
        }

        public async Task DeleteClient()
        {
            if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
            {
                State.messageBoxVM.Symbol = 0;
                State.messageBoxVM.HeadMessage = "Delete Client";
                State.messageBoxVM.Message = Names.DeleteClientConfirmation;
                State.messageBoxVM.ButtonStyle = Names.NoYes;
                await _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
            }

            // if clicked yes
            if (State.messageBoxVM.Output == true)
            {
                // delete client folder
                await FileStore.DeleteClientFolder(SelectedJob.ClientId, SelectedJob.ClientName.Trim());

                // delete client database row
                await Database.DeleteClient(SelectedJob.ClientId);
                await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                await TryCloseAsync();
            }
        }

        public async Task Exit()
        {
            await TryCloseAsync();
        }

        public async Task OpenJobsList()
        {
            _jobsListViewModel.SelectedJob = SelectedJob;
            await _windowManager.ShowDialogAsync(_jobsListViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
        }
        #endregion

        #region Private Functions
        private Task ToggleClientType()
        {
            if (ClientType == Names.Individual)
            {
                CompanyName = string.Empty;
            }
            else if (ClientType == Names.Company)
            {
                //CompanyName = ClientName;
                //ClientName = string.Empty;
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
        #endregion
    }
}