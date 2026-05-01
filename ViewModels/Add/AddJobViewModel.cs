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
    using Traker.Events;
    using Traker.States;

    // try use inheritance or a design pattern to avoid repetation

    public class AddJobViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Public Variables
        public ObservableCollection<DashboardModel> dashboardData; // to obtain the current dashboard clients list
        #endregion

        #region Public View Variables
        // dont need private as it only displays names with IDs
        // public BindableCollection<string> ClientsList { get; } = new BindableCollection<string>();
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        #region Private View Variables
        // dont need private as it only displays names with IDs
        private AddJobModel _selectedClient;
        private string _jobDescription;
        private string _jobTitle;
        private string _price;
        private string _dueDate;
        #endregion


        private ObservableCollection<AddJobModel> _addJob;

        public AddJobViewModel(IEventAggregator events, AppState appState)
        {
            _events = events;
            _addJob = new ObservableCollection<AddJobModel>();

            State = appState;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            foreach (var client in dashboardData.DistinctBy(x => x.ClientId))
            {
                AddJob.Add(new AddJobModel
                {
                    ClientId = client.ClientId,
                    CreatedDate = client.CreatedDate.ToString(),
                    FullName = client.ClientName,
                    JobDescription = client.JobDescription,
                    Price = client.Price
                });
            }
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

            await Database.AddNewJobToClient(SelectedClient.ClientId, JobTitle, amount, dueDate);
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
