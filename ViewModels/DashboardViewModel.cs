using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    using Database;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Traker.Events;
    using Traker.Models;
    using Traker.States;

    public class DashboardViewModel : Screen, IHandle<RefreshDatabase>
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        #region Private View Variables
        private ObservableCollection<string> _clientNames;
        private ObservableCollection<string> _clientEmails;
        private ObservableCollection<string> _clientPhones;
        private ObservableCollection<string> _jobDescripions;
        private ObservableCollection<string> _jobStatus;
        private ObservableCollection<string> _jobPrice;
        private ObservableCollection<string> _paid; // paid by client?
        private string _moneyReceived; // money received value shown total
        private string _moneyToRecieve; // money outstanding value shown total
        private string _moneyOverdue; // money overdue value shown total
        private string _moneyPrice; // money price = the final amount for a job (not invoiced)
        private string _newJobsCount;
        private string _inProgressJobsCount; 
        private string _completedJobsCount; 
        private string _invoicedJobsCount;
        ObservableCollection<DashboardModel> _dashboardData; // listo of data shown on the data grid
        public DashboardModel _selectedDataRow; // selected data row
        #endregion

        #region Data Variables
        List<ClientsModel> _clients;
        List<JobsModel> _jobs;
        List<InvoicesModel> _invoices;
        #endregion

        #region Private Field Variables
        private List<decimal> _receviedMoney; // money received
        private List<decimal> _outstandingdMoney; // mnoney to receive yet
        private List<decimal> _overdueMoney; // mnoney overdue (not paid + past due date)
        private List<decimal> _priceMoney; // mnoney overdue (not paid + past due date)
        private List<int> _newJobs; // new jobs
        private List<int> _inProgressJobs; // in progress jobs
        private List<int> _completedJobs; // completed jobs
        private List<int> _invoicedJobs; // completed jobs
        private JobDetailsViewModel _jobDetailsViewModel;
        private AddClientViewModel _addClientViewModel;
        private AddJobViewModel _addJobViewModel;
        #endregion

        public DashboardViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState)
        {
            _events = events;


            _windowManager = windowManager;

            _clientNames = new ObservableCollection<string>(); // client names
            _clientEmails = new ObservableCollection<string>(); // client emails
            _clientPhones = new ObservableCollection<string>(); // client phones
            _jobDescripions = new ObservableCollection<string>(); // job descriptions
            _jobStatus = new ObservableCollection<string>(); // job status
            _jobPrice = new ObservableCollection<string>(); // job price
            _paid = new ObservableCollection<string>(); // job price

            _moneyReceived = "0";
            _moneyToRecieve ="0";
            _moneyOverdue ="0";
            _moneyPrice = "0";

            _receviedMoney = new List<decimal>();
            _outstandingdMoney = new List<decimal>();
            _overdueMoney = new List<decimal>();
            _priceMoney = new List<decimal>();
            _newJobs = new List<int>();
            _inProgressJobs = new List<int>();
            _completedJobs = new List<int>();
            _invoicedJobs = new List<int>();

            //_database = new Database(State); // database
            _clients = new List<ClientsModel>();
            _jobs = new List<JobsModel>();
            _invoices = new List<InvoicesModel>();
            _dashboardData = new ObservableCollection<DashboardModel>();
            _selectedDataRow = new DashboardModel();

            State = appState;

            _jobDetailsViewModel = new JobDetailsViewModel(_events);
            _addClientViewModel = new AddClientViewModel(_events, State);
            _addJobViewModel = new AddJobViewModel(_events, State);


            _events.SubscribeOnPublishedThread(this);
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                //await _database.SetUpDatabase();
                await Database.SetUpDatabase();

                _clients = Database.FetchClientsTable(); // clients
                _jobs = Database.FetchJobsTable(); // jobs
                _invoices = Database.FetchInvoiceTable(); // invoices

                

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

        #region Private Functions
        private Task SetupDashboardData()
        {

            _dashboardData.Clear();
            int index = 0;
            foreach (var job in _jobs)
            {
                var client = _clients.First(c => c.ClientId == job.ClientId);

                DashboardModel dashboardEntry = new DashboardModel
                {
                    ClientName = client.FullName,
                    JobDescription = job.Description,
                    Price = job.FinalPrice.ToString("C"),
                    JobStatus = job.Status.ToString(),
                    DueDate = job.DueDate,

                    ClientId = client.ClientId,
                    ClientEmail = client.Email,
                    ClientPhone = client.PhoneNumber,
                    JobId = job.JobId,
                    InvoiceStatus = _invoices.Where(i => i.JobId == job.JobId).Select(i => i.Status).FirstOrDefault() ?? "Not Invoiced"
                    //Invoices = _invoices.Where(invoice => invoice.JobId == _jobs[index].JobId).ToList()
                };
                _dashboardData.Add(dashboardEntry);
                index++;
            }











            //for (int i = 0; i < _jobs.Count; i++)
            //{
            //    var job = _jobs[i];
            //    var client = _clients.First(c => c.ClientId == job.ClientId);

            //    DashboardModel dashboardEntry = new DashboardModel
            //    {
            //        ClientName = _clients[i].FullName,
            //        JobDescription = _jobs.Where(j => j.ClientId == _clients[i].ClientId).Select(j => j.Description).FirstOrDefault()!.ToString()!,
            //        Price = _jobs.Where(j => j.ClientId == _clients[i].ClientId).Select(j => j.FinalPrice).FirstOrDefault().ToString()!,
            //        Status = _jobs.Where(j => j.ClientId == _clients[i].ClientId).Select(j => j.Status).FirstOrDefault()!.ToString()!,
            //        DueDate = _jobs.Where(j => j.ClientId == _clients[i].ClientId).Select(j => j.DueDate).FirstOrDefault()!,
            //        Paid = _invoices.Where(inv => inv.JobId == _jobs[i].JobId).Select(inv => inv.IsPaid).FirstOrDefault().ToString()!,

            //        ClientId = _clients[i].ClientId,
            //        ClientEmail = _clients[i].Email,
            //        ClientPhone = _clients[i].PhoneNumber,
            //        Jobs = _jobs.Where(j => j.JobId == job.JobId).ToList(),
            //        Invoices = _invoices.Where(invoice => invoice.JobId == _jobs[i].JobId).ToList()
            //    };
            //    _dashboardData.Add(dashboardEntry);
            //}

            _receviedMoney.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _receviedMoney.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(_invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.FinalPrice, inv.Status })
                    .Where(x => x.Status == "Paid")
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyReceived = _receviedMoney.Sum().ToString("C"); // received


            _outstandingdMoney.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _outstandingdMoney.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(_invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.FinalPrice, inv.Status, inv.IssueDate, inv.DueDate })
                    .Where(x => x.Status == "Sent" && x.IssueDate < DateTime.Now.Date && x.DueDate > DateTime.Now.Date)
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyToReceive = _outstandingdMoney.Sum().ToString("C"); // outstanding

            _overdueMoney.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _overdueMoney.AddRange(_clients
                    .Where(client => client.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, client => client.ClientId, job => job.ClientId, (client, job) => job)
                    .Join(_invoices, job => job.JobId, invoice => invoice.JobId, (job, invoice) => new { job.FinalPrice, invoice.Status, invoice.DueDate })
                    .Where(x => x.Status == "Overdue" && x.DueDate > DateTime.Now.Date)
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyOverdue = _overdueMoney.Sum().ToString("C"); // overdue

            _priceMoney.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _priceMoney.AddRange(_jobs
                    .Where(j => j.ClientId == _dashboardData[i].ClientId)
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyPrice = _priceMoney.Sum().ToString("C"); // gross

            _newJobs.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _newJobs.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Where(j => j.Status == "New")
                    .Select(j => j.ClientId)
                    .ToList());
            }

            NewJobsCount = _newJobs.Count().ToString(); // new jobs count

            _inProgressJobs.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _inProgressJobs.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Where(j => j.Status == "InProgress")
                    .Select(j => j.ClientId)
                    .ToList());
            }

            InProgressJobsCount = _inProgressJobs.Count().ToString(); // in progress jobs count

            _completedJobs.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _completedJobs.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Where(j => j.Status == "Completed")
                    .Select(j => j.ClientId)
                    .ToList());
            }

            CompletedJobsCount = _completedJobs.Count().ToString(); // completed jobs count


            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _invoicedJobs.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Where(j => j.Status == "Invoiced")
                    .Select(j => j.ClientId)
                    .ToList());
            }

            InvoicedJobsCount = _invoicedJobs.Count().ToString(); // invoiced jobs count

            return Task.CompletedTask;
        }
        #endregion


        public async Task OpenJobDetails(DashboardModel selectedJob)
        {
            // if add menu open do nothing
            if (State.IsWindowOpen == true)
            {
                return;
            }

            Debug.WriteLine("OPEN MENU" + " | " + SelectedDataRow.ClientName + " - " + selectedJob.ClientName);
            // Debug.WriteLine(selectedJob.ClientName);
            //Debug.WriteLine(selectedJob.ClientName + " " + selectedJob.Jobs.Select(j => j.Description).FirstOrDefault()!.ToString());
            
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
            if (SelectedDataRow.ClientId == selectedJob.ClientId)
            {
                _jobDetailsViewModel = new JobDetailsViewModel(_events);
                _jobDetailsViewModel.selectedRow = selectedJob;
                await _windowManager.ShowPopupAsync(_jobDetailsViewModel, null, SettingsForDialog(300, 250));
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

        public async Task AddClient()
        {
            Debug.WriteLine("ADDING client..");
            
            if (State.IsWindowOpen == false)
            {
                // if is not free then report it with a message box
                // move this to a private function later
                if (_jobDetailsViewModel != null )
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
                await _windowManager.ShowWindowAsync(_addClientViewModel, null, SettingsForDialog(600, 500));
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
            Debug.WriteLine("ADDING job..");

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
                await _windowManager.ShowWindowAsync(_addJobViewModel, null, SettingsForDialog(600, 500));
                State.IsWindowOpen = true; // flag as open accross the project
            }

            // the state State.IsAddRowEntryOpen will be false again from addrowentryVM when user closes the window
            else if (State.IsWindowOpen == true)
            {
                return; // do nothing
            }
        }


        private dynamic SettingsForDialog(int height, int width)
        {
            dynamic settings = new ExpandoObject();
            settings.Title = string.Empty;
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.WindowStyle = WindowStyle.None;
            settings.AllowsTransparency = true;
            settings.Background = Brushes.Transparent;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.MinHeight = height;
            settings.MinWidth = width;
            return settings;
        }

        public Task HandleAsync(RefreshDatabase message, CancellationToken cancellationToken)
        {
            _clients.Clear();
            _jobs.Clear();
            _invoices.Clear();

            _clients = Database.FetchClientsTable(); // clients
            _jobs = Database.FetchJobsTable(); // jobs
            _invoices = Database.FetchInvoiceTable(); // invoices

            SetupDashboardData();

            return Task.CompletedTask;
        }

        //public async Task HandleAsync(CloseWindow message, CancellationToken cancellationToken)
        //{
        //    // add if else checks and try/catch
        //    if (message != null)
        //    {
        //        if (message.WindowName == "AddRowEntry")
        //        {
        //            if (_addRowEntryViewModel != null)
        //            {
        //                try
        //                {
        //                    await _addRowEntryViewModel.TryCloseAsync();
        //                    _addRowEntryViewModel = null;
        //                }
        //                catch (Exception ex)
        //                {
        //                    var real = ex.InnerException ?? ex;
        //                    Debug.WriteLine(real.Message);
        //                    Debug.WriteLine(real.StackTrace);
        //                }

        //            }
        //        }
        //    }
        //}



        #region Public View Variables
        public ObservableCollection<string> ClientNames
        {
            get { return _clientNames; }
            set
            {
                _clientNames = value;
                NotifyOfPropertyChange(() => ClientNames);
            }
        }

        public ObservableCollection<string> ClientEmails
        {
            get { return _clientEmails; }
            set
            {
                _clientEmails = value;
                NotifyOfPropertyChange(() => ClientEmails);
            }
        }

        public ObservableCollection<string> ClientPhones
        {
            get { return _clientPhones; }
            set
            {
                _clientPhones = value;
                NotifyOfPropertyChange(() => ClientPhones);
            }
        }
        public ObservableCollection<string> JobDescriptions
        {
            get { return _jobDescripions; }
            set
            {
                _jobDescripions = value;
                NotifyOfPropertyChange(() => JobDescriptions);
            }
        }

        public ObservableCollection<string> JobStatus
        {
            get { return _jobStatus; }
            set
            {
                _jobStatus = value;
                NotifyOfPropertyChange(() => JobStatus);
            }
        }

        public ObservableCollection<string> JobPrice
        {
            get { return _jobPrice; }
            set
            {
                _jobPrice = value;
                NotifyOfPropertyChange(() => JobPrice);
            }
        }

        public ObservableCollection<string> Paid
        {
            get { return _paid; }
            set
            {
                _paid = value;
                NotifyOfPropertyChange(() => Paid);
            }
        }

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

        public String MoneyPrice
        {
            get { return _moneyPrice; }
            set
            {
                _moneyPrice = value;
                NotifyOfPropertyChange(() => MoneyPrice);
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

        public String InProgressJobsCount
        {
            get { return _inProgressJobsCount; }
            set
            {
                _inProgressJobsCount = value;
                NotifyOfPropertyChange(() => InProgressJobsCount);
            }
        }

        public String CompletedJobsCount
        {
            get { return _completedJobsCount; }
            set
            {
                _completedJobsCount = value;
                NotifyOfPropertyChange(() => CompletedJobsCount);
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

        public DashboardModel SelectedDataRow
        {
            get { return _selectedDataRow; }
            set
            {
                _selectedDataRow = value;
                NotifyOfPropertyChange(() => SelectedDataRow);
            }
        }
        #endregion
    }
}
