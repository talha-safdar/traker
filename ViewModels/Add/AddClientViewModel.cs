using Caliburn.Micro;
using Traker.States;

namespace Traker.ViewModels.Add
{
    using Database;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using Traker.Data;
    using Traker.Events;
    using Traker.Helper;
    using Traker.Services;

    public class AddClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        private string _clientType;
        private string _businessName; // generalised for both client or company name
        private string _jobTitle;
        private string _price;
        private string _dueDate;
        private string _businessNameText; // individual=client name, company=company name

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        #region Private Class Field Variables
        private string _clientNameTxt = "Client Name";
        private string _companyNameTxt = "Company Name";
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        #endregion

        public AddClientViewModel(IEventAggregator events, AppState appState, DataService dataService)
        {
            _events = events;
            _dataService = dataService;

            _clientType = string.Empty;
            _businessName = string.Empty;
            _jobTitle = string.Empty;
            _price = string.Empty;
            _dueDate = string.Empty;
            _businessNameText = string.Empty;

            State = appState;
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                ClientType = Names.Individual; // default start with individual
                BusinessNameText = _clientNameTxt;

                // submit button
                EnableSubmitBtn = false;
                OpacitySubmitBtn = _halfOpacity;
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while initializing Add Client page. Please try again.\n\n{ex.Message}",
                    "Add Client",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                //Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: OnInitializedAsync() FAIL");
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
        public async Task Exit()
        {
            try
            {
                State.IsWindowOpen = false;
                await TryCloseAsync();
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: Exit() OK");
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while exiting app. Please try again.\n\n{ex.Message}",
                    "Exit App",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: Exit() FAIL");
            }
        }

        public async Task AddClient()
        {
            // add checks
            // remove white spaces 
            // price textbox only number
            // money format use commas and dots
            // empty boxes on press add

            var dueDate = DateOnly.MinValue;
            decimal amount = 0;

            // due date conversion
            if (DueDate != string.Empty)
            {
                dueDate = DateOnly.ParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
                );
            }

            // money conversion
            if (Price != string.Empty)
            {
                amount = decimal.Parse(
                    Price,
                    CultureInfo.InvariantCulture
                );
            }

            /*
             * 0 = clientId
             * 1 = jobId
             */
            List<int> clientJobIds = new List<int>();

            if (ClientType == Names.Individual)
            {
                clientJobIds = await Database.AddIndividualClient(BusinessName, ClientType, JobTitle, amount, dueDate);
            }
            else if (ClientType == Names.Company)
            {
                clientJobIds = await Database.AddCompanyClient(BusinessName, ClientType, JobTitle, amount, dueDate);
            }

            // refresh database
            await _dataService.RefreshDatabase();

            // create Client folder in file store and get its name
            string clientFolderName = await FileStore.CreateClientFolder(_dataService.Clients.OrderByDescending(c => c.ClientId).First().ClientId, BusinessName);

            // create Job folder in file store and get its name
            string jobFolderName = await FileStore.CreateJobFolder(clientJobIds[0], clientJobIds[1], BusinessName, JobTitle);

            // update client and job databases with their fodler names
            await Database.SetClientFolderName(clientJobIds[0], clientFolderName);
            await Database.SetJobFolderName(clientJobIds[1], jobFolderName);

            // refresh database
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
            await TryCloseAsync();
        }
        #endregion

        #region Private Functions
        private Task ToggleClientType()
        {
            if (ClientType == Names.Individual)
            {
                BusinessNameText = _clientNameTxt;
            }
            else if (ClientType == Names.Company)
            {
                BusinessNameText = _companyNameTxt;
            }
            return Task.CompletedTask;
        }

        private bool ValidateDate()
        {
            return DateTime.TryParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate)
                && parsedDate.Date >= DateTime.Today;
        }

        private Task CanSubmit()
        {
            if (string.IsNullOrEmpty(ClientType) == false && string.IsNullOrEmpty(BusinessName) == false && string.IsNullOrEmpty(JobTitle) == false && string.IsNullOrEmpty(Price) == false && ValidateDate() == true)
            {
                
                EnableSubmitBtn = true;
                OpacitySubmitBtn = _fullOpacity;
            }
            else
            {
                EnableSubmitBtn = false;
                OpacitySubmitBtn = _halfOpacity;
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
                CanSubmit();
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

        public string JobTitle
        {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                NotifyOfPropertyChange(() => JobTitle);
                CanSubmit();
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
                CanSubmit();
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
                CanSubmit();
            }
        }

        public string BusinessNameText
        {
            get { return _businessNameText; }
            set
            {
                _businessNameText = value;
                NotifyOfPropertyChange(() => BusinessNameText);
                CanSubmit();
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