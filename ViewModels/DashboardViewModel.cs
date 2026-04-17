using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Traker.ViewModels
{
    using Database;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Traker.Events;
    using Traker.Helper;
    using Traker.Models;
    using Traker.Services;
    using Traker.States;

    public class DashboardViewModel : Screen, IHandle<RefreshDatabase>, IHandle<CallFromDashboard>
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        public DataService Data { get; } // data from the database
        #endregion

        #region Public View Variables
        public List<string> JobStatusEdit { get; set; } = new List<string> { "New", "Active", "Done" };
        public List<string> InvoiceStatusEdit { get; set; } = new List<string> { "Created", "Sent", "Paid", "Overdue" };
        #endregion

        #region Private View Variables
        private string _moneyReceived; // money received value shown total
        private string _moneyToRecieve; // money outstanding value shown total
        private string _moneyOverdue; // money overdue value shown total
        private string _grossAmount; // money price = the final amount for a job (not invoiced)
        private string _newJobsCount;
        private string _activeJobsCount;
        private string _doneJobsCount;
        private string _invoicedJobsCount;
        private ObservableCollection<DashboardModel> _dashboardData; // listo of data shown on the data grid
        public DashboardModel _selectedJob; // selected data row automatically filled on click
        #endregion

        #region Data Variables
        //List<ClientsModel> _clients;
        //List<JobsModel> _jobs;
        //List<InvoicesModel> _invoices;
        #endregion

        #region Private Field Variables
        private List<decimal> _receviedMoney; // money received
        private List<decimal> _outstandingdMoney; // mnoney to receive yet
        private List<decimal> _overdueMoney; // mnoney overdue (not paid + past due date)
        //private List<decimal> _priceMoney; // mnoney overdue (not paid + past due date)
        //private List<int> _newJobs; // new jobs
        //private List<int> _inProgressJobs; // in progress jobs
        //private List<int> _completedJobs; // completed jobs
        private List<int> _invoicedJobs; // completed jobs
        private ContextMenuViewModel _jobDetailsViewModel;
        private AddClientViewModel _addClientViewModel;
        private AddJobViewModel _addJobViewModel;
        private EditClientViewModel _editClientViewModel;
        private EditJobViewModel _editJobViewModel;
        #endregion

        public DashboardViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState, DataService dataService)
        {
            _events = events;
            _windowManager = windowManager;

            State = appState;
            Data = dataService;

            _moneyReceived = "0";
            _moneyToRecieve = "0";
            _moneyOverdue = "0";
            _grossAmount = "0";
            _newJobsCount = "0";
            _activeJobsCount = "0";
            _doneJobsCount = "0";
            _invoicedJobsCount = "0";

            _receviedMoney = new List<decimal>();
            _outstandingdMoney = new List<decimal>();
            _overdueMoney = new List<decimal>();
            //_priceMoney = new List<decimal>();
            //_newJobs = new List<int>();
            //_inProgressJobs = new List<int>();
            //_completedJobs = new List<int>();
            _invoicedJobs = new List<int>();

            //_clients = new List<ClientsModel>();
            //_jobs = new List<JobsModel>();
            //_invoices = new List<InvoicesModel>();
            _dashboardData = new ObservableCollection<DashboardModel>();
            _selectedJob = new DashboardModel();


            _jobDetailsViewModel = new ContextMenuViewModel(_events, _windowManager);
            _addClientViewModel = new AddClientViewModel(_events, State);
            _addJobViewModel = new AddJobViewModel(_events, State);
            _editClientViewModel = new EditClientViewModel(_events, _windowManager, Data);

            _events.SubscribeOnPublishedThread(this);
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Database.SetUpDatabase();
                await Data.FetchDatabase();
                await SetupDashboardData();

                //return base.OnInitializedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while while loading Dashboard. Please try again.\n\n{ex.Message}",
                    "Dashboard",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                System.Environment.Exit(1); // kill process
            }
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion

        #region Public View Functions
        public void SelectJob(DashboardModel selectedJob)
        {
            if (selectedJob == null)
            {
                // error display
                return;
            }

            SelectedJob = selectedJob;
        }

        public async Task OpenContextMenu(DashboardModel selectedJob)
        {
            // if add menu open do nothing
            if (State.IsWindowOpen == true)
            {
                return;
            }

            if (_jobDetailsViewModel != null)
            {
                await _jobDetailsViewModel.TryCloseAsync(false);
            }

            // if using client name then it must be required at all the time

            /*
             * there are two variables to get the data from the row
             * 1 - on the right click (datagrid approach)
             * 2 - on the left click (to pass row's current object 'selectedJob')
             * to open the popup the datagrid selection object must match with the right-clicked onwe
             * we use the clientName for matching for now, but might use unique id in the future.
             * and the latter must become required hence Id seems perfect.
             * In fact we just swapped to clientId :)
             */
            if (SelectedJob.ClientId == selectedJob.ClientId)
            {
                _jobDetailsViewModel = new ContextMenuViewModel(_events, _windowManager);
                _jobDetailsViewModel.SelectedRow = selectedJob; // pass row selected
                _jobDetailsViewModel.Clients = Data.Clients;
                _jobDetailsViewModel.Jobs = Data.Jobs;
                _jobDetailsViewModel.Invoices = Data.Invoices;
                await _windowManager.ShowPopupAsync(_jobDetailsViewModel, null, CustomWindow.SettingsForDialog(310, 335)); // vertical, horizontal
            }
        }

        public async Task EditJob(DashboardModel jobSelected)
        {
            _editJobViewModel = new EditJobViewModel(_events);
            _editJobViewModel.SelectedJob = jobSelected; // pass selected row to EditJobViewModel
            await _windowManager.ShowWindowAsync(_editJobViewModel, null, CustomWindow.SettingsForDialog(800, 1000));
        }

        public async Task EditClient()
        {
            _editClientViewModel = new EditClientViewModel(_events, _windowManager, Data);
            _editClientViewModel.SelectedRow = SelectedJob; // pass selected row to EditClientViewModel
            await _windowManager.ShowWindowAsync(_editClientViewModel, null, CustomWindow.SettingsForDialog(800, 1000));
        }

        public async Task AddClient()
        {
            Debug.WriteLine("ADDING client..");

            if (State.IsWindowOpen == false)
            {
                // if is not free then report it with a message box
                // move this to a private function later
                if (_jobDetailsViewModel != null)
                {
                    await _jobDetailsViewModel.TryCloseAsync(false);
                    _jobDetailsViewModel = null;
                }
                if (_addJobViewModel != null)
                {
                    await _addJobViewModel.TryCloseAsync(false);
                    _addJobViewModel = null;
                }

                _addClientViewModel = new AddClientViewModel(_events, State);
                await _windowManager.ShowWindowAsync(_addClientViewModel, null, CustomWindow.SettingsForDialog(600, 500));
                State.IsWindowOpen = true; // flag as open accross the project
            }

            // the state State.IsAddRowEntryOpen will be false again from addrowentryVM when user closes the window
            else if (State.IsWindowOpen == true)
            {
                return; // do nothing
            }
        }

        public async Task AddJob()
        {
            //Debug.WriteLine("ADDING job..");

            if (State.IsWindowOpen == false)
            {
                // if State.popup is not free then report it with a message box
                if (_jobDetailsViewModel != null)
                {
                    await _jobDetailsViewModel.TryCloseAsync(false);
                    _jobDetailsViewModel = null;
                }
                if (_addClientViewModel != null)
                {
                    await _addClientViewModel.TryCloseAsync(false);
                    _addClientViewModel = null;
                }

                _addJobViewModel = new AddJobViewModel(_events, State);
                _addJobViewModel.dashboardData = _dashboardData; // pass dashboard data to AddJob
                await _windowManager.ShowWindowAsync(_addJobViewModel, null, CustomWindow.SettingsForDialog(600, 500));
                State.IsWindowOpen = true; // flag as open accross the project
            }

            // the state State.IsAddRowEntryOpen will be false again from addrowentryVM when user closes the window
            else if (State.IsWindowOpen == true)
            {
                return; // do nothing
            }
        }

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            if (_jobDetailsViewModel != null)
            {
                await _jobDetailsViewModel.TryCloseAsync(false);
                _jobDetailsViewModel = null;
            }
        }
        #endregion

        #region Private Functions
        private Task SetupDashboardData()
        {
            // row data
            _dashboardData.Clear();
            int index = 0;
            foreach (var job in Data.Jobs)
            {
                var client = Data.Clients.First(c => c.ClientId == job.ClientId);

                DashboardModel dashboardEntry = new DashboardModel
                {
                    ClientId = client.ClientId,
                    ClientType = client.Type,
                    TypeIcon = (client.Type == "Individual") ? "/Resources/Media/Images/Icons/Lucide/user-round.svg" : "/Resources/Media/Images/Icons/Lucide/building.svg",
                    ClientName = client.FullName,
                    ClientEmail = client.Email,
                    ClientPhone = client.PhoneNumber,
                    CompanyName = client.CompanyName,
                    BillingAddress = client.BillingAddress,
                    City = client.City,
                    Postcode = client.Postcode,
                    Country = client.Country,
                    CreatedDate = client.CreatedDate,
                    IsActive = client.IsActive,

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
                _dashboardData.Add(dashboardEntry);
                index++;
            }

            // received money
            _receviedMoney.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _receviedMoney.AddRange(Data.Clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(Data.Jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(Data.Invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.FinalPrice, inv.Status })
                    .Where(x => x.Status == "Paid")
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyReceived = _receviedMoney.Sum().ToString("C"); // received

            // outstanding money
            _outstandingdMoney.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _outstandingdMoney.AddRange(Data.Clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(Data.Jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(Data.Invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.FinalPrice, inv.Status, inv.IssueDate, inv.DueDate })
                    .Where(x => x.Status == "Sent" && x.IssueDate < DateOnly.FromDateTime(DateTime.Now) && x.DueDate > DateOnly.FromDateTime(DateTime.Now))
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyToReceive = _outstandingdMoney.Sum().ToString("C"); // outstanding

            // overdue money
            //_overdueMoney.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    _overdueMoney.AddRange(Data.Clients
            //        .Where(client => client.ClientId == _dashboardData[i].ClientId)
            //        .Join(Data.Jobs, client => client.ClientId, job => job.ClientId, (client, job) => job)
            //        .Join(Data.Invoices, job => job.JobId, invoice => invoice.JobId, (job, invoice) => new { job.FinalPrice, invoice.Status, invoice.DueDate })
            //        .Where(x => x.Status == "Overdue" && x.DueDate < DateOnly.FromDateTime(DateTime.Now))
            //        .Select(x => x.FinalPrice)
            //        .ToList());
            //}
            //MoneyOverdue = _overdueMoney.Sum().ToString("C"); // overdue
            MoneyOverdue = Data.Jobs.Where(j => j.DueDate > DateOnly.FromDateTime(Convert.ToDateTime(DateTime.Now)) && j.Status == "Invoiced")
                                      .Sum(j => j.FinalPrice).ToString("C");  // overdue

            GrossAmount = Data.Jobs.Sum(gross => gross.FinalPrice).ToString("C"); // gross amount
            NewJobsCount = Data.Jobs.Where(j => j.Status == "New").Count().ToString(); // new jobs count
            ActiveJobsCount = Data.Jobs.Where(j => j.Status == "Active").Count().ToString(); // active jobs count
            DoneJobsCount = Data.Jobs.Where(j => j.Status == "Done").Count().ToString(); // done jobs count

            // invoiced jobs
            _invoicedJobs.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _invoicedJobs.AddRange(Data.Clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(Data.Jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Where(j => j.Status == "Invoiced")
                    .Select(j => j.ClientId)
                    .ToList());
            }
            InvoicedJobsCount = _invoicedJobs.Count().ToString(); // invoiced jobs count

            return Task.CompletedTask;
        }
        #endregion

        #region Event Handlers
        public async Task HandleAsync(CallFromDashboard message, CancellationToken cancellationToken)
        {
            if (message != null)
            {
                if (message.FunctionName == "EditClient")
                {
                    await EditClient();
                }
            }
        }

        public async Task HandleAsync(RefreshDatabase message, CancellationToken cancellationToken)
        {
            await Data.ClearDataVariables();
            await Data.FetchDatabase();
            await SetupDashboardData();
        }
        #endregion

        // TO CHECK
        public Task UpdateJobStatus(DashboardModel row)
        {
            // update DB here
            return Task.CompletedTask;
        }

        #region Public View Variables     
        public String MoneyReceived
        {
            get { return _moneyReceived; }
            set
            {
                _moneyReceived = value;
                NotifyOfPropertyChange(() => MoneyReceived);
            }
        }

        public String MoneyToReceive
        {
            get { return _moneyToRecieve; }
            set
            {
                _moneyToRecieve = value;
                NotifyOfPropertyChange(() => MoneyToReceive);
            }
        }

        public String MoneyOverdue
        {
            get { return _moneyOverdue; }
            set
            {
                _moneyOverdue = value;
                NotifyOfPropertyChange(() => MoneyOverdue);
            }
        }

        public String GrossAmount
        {
            get { return _grossAmount; }
            set
            {
                _grossAmount = value;
                NotifyOfPropertyChange(() => GrossAmount);
            }
        }

        public String NewJobsCount
        {
            get { return _newJobsCount; }
            set
            {
                _newJobsCount = value;
                NotifyOfPropertyChange(() => NewJobsCount);
            }
        }

        public String ActiveJobsCount
        {
            get { return _activeJobsCount; }
            set
            {
                _activeJobsCount = value;
                NotifyOfPropertyChange(() => ActiveJobsCount);
            }
        }

        public String DoneJobsCount
        {
            get { return _doneJobsCount; }
            set
            {
                _doneJobsCount = value;
                NotifyOfPropertyChange(() => DoneJobsCount);
            }
        }

        public String InvoicedJobsCount
        {
            get { return _invoicedJobsCount; }
            set
            {
                _invoicedJobsCount = value;
                NotifyOfPropertyChange(() => InvoicedJobsCount);
            }
        }

        public ObservableCollection<DashboardModel> DashboardData
        {
            get { return _dashboardData; }
            set
            {
                _dashboardData = value;
                NotifyOfPropertyChange(() => DashboardData);
            }
        }

        public DashboardModel SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                NotifyOfPropertyChange(() => SelectedJob);
            }
        }
        #endregion
    }
}
