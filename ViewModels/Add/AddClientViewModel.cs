using Caliburn.Micro;
using Traker.States;

namespace Traker.ViewModels.Add
{
    using Database;
    using System.Globalization;
    using System.Windows;
    using Traker.Data;
    using Traker.Events;
    using Traker.Services;

    public class AddClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        private string _clientType;
        private string _clientName;
        private string _jobTitle;
        private string _jobDescription;
        private string _price;
        private string _dueDate;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public AddClientViewModel(IEventAggregator events, AppState appState, DataService dataService)
        {
            _events = events;
            _dataService = dataService;

            _clientType = string.Empty;
            _clientName = string.Empty;
            _jobTitle = string.Empty;
            _jobDescription = string.Empty;
            _price = string.Empty;
            _dueDate = string.Empty;

            State = appState;
        }

        #region Caliburn Functions
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

            await Database.AddRow(ClientName, ClientType, JobTitle, JobDescription, amount, dueDate);

            // refresh database
            await _dataService.RefreshDatabase();

            // create folders
            await FileStore.CreateFolder(_dataService.Clients.OrderByDescending(c => c.ClientId).First().ClientId, ClientName);

            // refresh database
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
            await TryCloseAsync();
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

        public string JobTitle
        {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                NotifyOfPropertyChange(() => JobTitle);
            }
        }

        public string JobDescription
        {
            get { return _jobDescription; }
            set
            {
                _jobDescription = value;
                NotifyOfPropertyChange(() => JobDescription);
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
            }
        }
        #endregion
    }
}