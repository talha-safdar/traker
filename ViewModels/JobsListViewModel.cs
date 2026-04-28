using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Helper;
using Traker.Models;
using Traker.Services;
using Traker.States;
using Traker.ViewModels.Edit;

namespace Traker.ViewModels
{
    public class JobsListViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        #region Private View Variables
        private ObservableCollection<DashboardModel> _jobsList;
        private string _clientName;
        #endregion

        #region Public State Variable
        public DataService Data { get; } // data from the database
        #endregion

        #region Private Class Field Variables
        private EditJobViewModel _editJobViewModel;
        #endregion

        public DashboardModel SelectedJob; // data passed by EdiClientVM

        public JobsListViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService)
        {
            _events = events;
            _windowManager = windowManager;
            Data = dataService;
            _jobsList = new ObservableCollection<DashboardModel>();
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            var jobsForClient = Data.Jobs.Where(j => j.ClientId == SelectedJob.ClientId).ToList();
            _clientName = SelectedJob.ClientName;

            foreach (var job in jobsForClient)
            {
                DashboardModel currentJob = new DashboardModel
                {
                    ClientId = job.ClientId,
                    TypeIcon = (SelectedJob.TypeIcon == "Individual") ? "/Resources/Media/Images/Icons/Lucide/user-round.svg" : "/Resources/Media/Images/Icons/Lucide/building.svg",
                    ClientName = SelectedJob.ClientName,
                    ClientEmail = SelectedJob.ClientEmail,
                    ClientPhone = SelectedJob.ClientPhone,
                    CompanyName = SelectedJob.CompanyName,
                    BillingAddress = SelectedJob.BillingAddress,
                    City = SelectedJob.City,
                    Postcode = SelectedJob.Postcode,
                    Country = SelectedJob.Country,
                    CreatedDate = SelectedJob.CreatedDate,
                    IsActive = SelectedJob.IsActive,

                    JobId = job.JobId,
                    JobTitle = job.Title,
                    JobDescription = job.Description,
                    Price = job.FinalPrice.ToString("C"),
                    AmountReceived = job.AmountReceived.ToString("C"),
                    JobStatus = job.Status.ToString(),
                    StartDate = job.StartDate,
                    DueDate = job.DueDate,

                    HasInvoice = Data.Invoices.Any(i => i.JobId == job.JobId && i.IsDeleted == false),
                    InvoiceStatus = Data.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.Status).FirstOrDefault() ?? "Not invoiced"
                };
                _jobsList.Add(currentJob);
            }

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View FUnctions
        public async Task EditJob(DashboardModel jobSelected)
        {
            _editJobViewModel = new EditJobViewModel(_events);
            _editJobViewModel.SelectedJob = jobSelected; // pass selected row to EditJobViewModel
            await _windowManager.ShowWindowAsync(_editJobViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
        }

        public void SelectJob(DashboardModel selectedJob)
        {
            if (selectedJob == null)
            {
                // error display
                return;
            }

            SelectedJob = selectedJob;
        }

        public async Task Exit()
        {
            await TryCloseAsync();
        }
        #endregion

        #region Public View Variables 
        public ObservableCollection<DashboardModel> JobsList
        {
            get { return _jobsList; }
            set
            {
                _jobsList = value;
                NotifyOfPropertyChange(() => JobsList);
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
        #endregion
    }
}
