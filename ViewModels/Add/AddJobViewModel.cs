using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.ViewModels.Add
{
    using Database;
    using System.Globalization;
    using Traker.Data;
    using Traker.Events;
    using Traker.Helper;
    using Traker.States;

    // try use inheritance or a design pattern to avoid repetition

    public class AddJobViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Public Variables
        public ObservableCollection<DashboardModel> dashboardData; // to obtain the current dashboard clients list
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        #region Private View Variables
        // dont need private as it only displays names with IDs
        private AddJobModel _selectedClient;
        private string _jobTitle;
        private string _price;
        private string _dueDate;

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field Variables
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        #endregion

        private ObservableCollection<AddJobModel> _addJob;

        public AddJobViewModel(IEventAggregator events, AppState appState)
        {
            _events = events;
            _addJob = new ObservableCollection<AddJobModel>();

            _selectedClient = new AddJobModel();
            _jobTitle = string.Empty;
            _price = string.Empty;
            _dueDate = string.Empty;

            State = appState;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            EnableSubmitBtn = false;
            OpacitySubmitBtn = _halfOpacity;

            foreach (var client in dashboardData.DistinctBy(x => x.ClientId))
            {
                AddJob.Add(new AddJobModel
                {
                    ClientId = client.ClientId,
                    CreatedDate = client.CreatedDate.ToString(),
                    BusinessName = client.ClientType == Names.Individual ? client.ClientName : client.CompanyName,
                });
            }

            SelectedClient = AddJob.First(); // pre-select the first item in the list

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }


        public async Task AddJobToClient()
        {
            var dueDate = DateOnly.MinValue;
            decimal amount = 0;

            if (DueDate != string.Empty)
            {
                dueDate = DateOnly.ParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
                );
            }

            if (Price != string.Empty)
            {
                amount = decimal.Parse(
                    Price,
                    CultureInfo.InvariantCulture
                );
            }

            // add job under the client's id
            int jobId = await Database.AddNewJobToClient(SelectedClient.ClientId, JobTitle, amount, dueDate);

            // add job folder
            await FileStore.CreateJobFolder(SelectedClient.ClientId, jobId, SelectedClient.BusinessName, JobTitle);

            // refresh database
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
            await TryCloseAsync();
        }

        public async Task Exit()
        {
            State.IsWindowOpen = false;
            await TryCloseAsync();
        }

        #region View Variables
        public AddJobModel SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                NotifyOfPropertyChange(() => SelectedClient);
            }
        }
        #endregion

        #region Private Functions
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
            if (string.IsNullOrEmpty(SelectedClient.BusinessName) == false && string.IsNullOrEmpty(JobTitle) == false && string.IsNullOrEmpty(Price) == false && ValidateDate() == true)
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
        public ObservableCollection<AddJobModel> AddJob
        {
            get { return _addJob; }
            set
            {
                _addJob = value;
                NotifyOfPropertyChange(() => AddJob);
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
