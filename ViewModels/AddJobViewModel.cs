using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.ViewModels
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

        //protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        //{
        //    foreach (var client in dashboardData)
        //    {
        //        ClientsList.Add(client.ClientName);
        //    }
        //    return base.OnInitializedAsync(cancellationToken);
        //}

        protected override void OnViewLoaded(object view)
        {
            foreach (var client in dashboardData.DistinctBy(x => x.ClientId))
            {
                //ClientsList.Add(client.ClientName);
                AddJob.Add(new AddJobModel
                {
                    ClientId = client.ClientId,
                    FullName = client.ClientName,
                    JobDescription = client.JobDescription,
                    Price = client.Price
                });
            }

            //SelectedClient = AddJob.FirstOrDefault()!.FullName;
            //NotifyOfPropertyChange(() => AddJob);

            base.OnViewLoaded(view);
        }


        public Task AddJobToClient()
        {
            var dueDate = DateTime.MinValue;
            decimal amount = 0;

            if (DueDate != String.Empty)
            {
                dueDate = DateTime.ParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
                );
            }

            if (Price != String.Empty)
            {
                amount = decimal.Parse(
                    Price,
                    CultureInfo.InvariantCulture
                );
            }

            Database.AddNewJobToClient(SelectedClient.ClientId, JobDescription, amount, dueDate);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());

            return Task.CompletedTask;
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

        public String JobDescription
        {
            get { return _jobDescription; }
            set
            {
                _jobDescription = value;
                NotifyOfPropertyChange(() => JobDescription);
            }
        }

        public String Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }

        public String DueDate
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
