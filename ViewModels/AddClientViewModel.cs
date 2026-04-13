using Caliburn.Micro;
using Traker.States;

namespace Traker.ViewModels
{
    using Database;
    using System.Globalization;
    using System.Windows;
    using Traker.Events;
    using Traker.Services;

    public class AddClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
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

        public AddClientViewModel(IEventAggregator events, AppState appState)
        {
            _events = events;

            _clientType = String.Empty;
            _clientName = String.Empty;
            _jobTitle = String.Empty;
            _jobDescription = String.Empty;
            _price = String.Empty;
            _dueDate = String.Empty;

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

        public Task AddRow()
        {
            // add checks
            // remove white spaces 
            // price textbox only number
            // money format use commas and dots
            // empty boxes on press add

            var dueDate = DateTime.MinValue;
            decimal amount = 0;

            // due date conversion
            if (DueDate != String.Empty)
            {
                dueDate = DateTime.ParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
                );
            }

            // money conversion
            if (Price != String.Empty)
            {
                amount = decimal.Parse(
                    Price,
                    CultureInfo.InvariantCulture
                );
            }

            Database.AddRow(ClientName, ClientType, JobTitle, JobDescription, amount, dueDate);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());

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