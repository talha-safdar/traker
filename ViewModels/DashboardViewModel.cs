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
    using System.Windows;
    using Traker.Models;

    public class DashboardViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
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
        private string _newJobsCount;
        private string _inProgressJobsCount; 
        private string _completedJobsCount; 
        private string _invoicedJobsCount;
        ObservableCollection<DashboardModel> _dashboardData; // listo of data shown on the data grid
        public DashboardModel _selectedDataRow; // selected data row
        #endregion

        #region Data Variables
        Database _database;
        List<ClientModel> _clients;
        List<JobModel> _jobs;
        List<InvoiceModel> _invoices;
        #endregion

        #region Private Field Variables
        private List<decimal> _receviedMoney; // money received
        private List<decimal> _outstandingdMoney; // mnoney to receive yet
        private List<decimal> _overdueMoney; // mnoney overdue (not paid + past due date)
        private List<int> _newJobs; // new jobs
        private List<int> _inProgressJobs; // in progress jobs
        private List<int> _completedJobs; // completed jobs
        private List<int> _invoicedJobs; // completed jobs
        #endregion

        public DashboardViewModel(IEventAggregator events)
        {
            _events = events;

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

            _receviedMoney = new List<decimal>();
            _outstandingdMoney = new List<decimal>();
            _overdueMoney = new List<decimal>();
            _newJobs = new List<int>();
            _inProgressJobs = new List<int>();
            _completedJobs = new List<int>();
            _invoicedJobs = new List<int>();

            _database = new Database(); // database
            _clients = new List<ClientModel>();
            _jobs = new List<JobModel>();
            _invoices = new List<InvoiceModel>();
            _dashboardData = new ObservableCollection<DashboardModel>();
            _selectedDataRow = new DashboardModel();
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {               
                await _database.SetUpDatabase();

                _clients = _database.FetchClientsTable(); // clients
                _jobs = _database.FetchJobsTable(); // jobs
                _invoices = _database.FetchInvoiceTable(); // invoices

                

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
        #endregion

        #region Private Functions
        private Task SetupDashboardData()
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                DashboardModel dashboardEntry = new DashboardModel
                {
                    ClientId = _clients[i].ClientId,
                    ClientName = _clients[i].FullName,
                    ClientEmail = _clients[i].Email,
                    ClientPhone = _clients[i].PhoneNumber,
                    Jobs = _jobs.Where(job => job.ClientId == _clients[i].ClientId).ToList(),
                    Invoices = _invoices.Where(invoice => invoice.JobId == _jobs[i].JobId).ToList()
                };
                _dashboardData.Add(dashboardEntry);
            }

            //ClientNames.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    ClientNames.Add(_dashboardData[i].ClientName);
            //}

            //ClientEmails.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    ClientEmails.Add(_dashboardData[i].ClientEmail);
            //}

            //ClientPhones.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    ClientPhones.Add(_dashboardData[i].ClientPhone);
            //}

            //JobDescriptions.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    JobDescriptions.Add(_dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.Description).ToList().FirstOrDefault()!);
            //}

            //JobStatus.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    JobStatus.Add(_dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.Status).ToList().FirstOrDefault()!);
            //}

            //JobPrice.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    JobPrice.Add(_dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.FinalPrice.ToString("C")).ToList().FirstOrDefault()!);
            //}

            //Paid.Clear();
            //for (int i = 0; i < _dashboardData.Count; i++)
            //{
            //    var paid = _dashboardData[i].Invoices.Where(inv => inv.JobId == _dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.JobId).FirstOrDefault()).Select(inv => inv.IsPaid).FirstOrDefault();
            //    if (paid == true)
            //    {
            //        Paid.Add("Yes");
            //    }
            //    else
            //    {
            //        Paid.Add("No");
            //    }
            //}



            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _receviedMoney.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(_invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.FinalPrice, inv.IsPaid })
                    .Where(x => x.IsPaid == true)
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyReceived = _receviedMoney.Sum().ToString("C"); // received





            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _outstandingdMoney.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(_invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.FinalPrice, inv.IsPaid })
                    .Where(x => x.IsPaid == false)
                    .Select(x => x.FinalPrice)
                    .ToList());
            }
            MoneyToReceive = _outstandingdMoney.Sum().ToString("C"); // outstanding





            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _overdueMoney.AddRange(_clients
                    .Where(client => client.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, client => client.ClientId, job => job.ClientId, (client, job) => job)
                    .Join(_invoices, job => job.JobId, invoice => invoice.JobId, (job, invoice) => new { job.FinalPrice, invoice.IsPaid, invoice.DueDate })
                    .Where(x => x.IsPaid == false && x.DueDate < DateTime.Now)
                    .Select(x => x.FinalPrice)
                    .ToList());
            }

            MoneyOverdue = _overdueMoney.Sum().ToString("C"); // overdue


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



        public void Selection()
        {
            Debug.WriteLine("HOLA");
        }


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
            get { return new ObservableCollection<DashboardModel>(_dashboardData); }
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
                Debug.WriteLine(_selectedDataRow.ClientName + " " + _selectedDataRow.ClientId);
                NotifyOfPropertyChange(() => SelectedDataRow);
            }
        }
        #endregion
    }
}
