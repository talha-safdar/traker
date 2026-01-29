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
        private string _moneyToRecieve; // money to be reveived yet
        #endregion

        #region Data Variables
        Database _database;
        List<ClientsModel> _clients;
        List<JobsModel> _jobs;
        List<InvoicesModel> _invoices;
        List<DashboardModel> _dashboardData;
        #endregion

        #region Private Field Variables
        private List<decimal> _paidJobs;
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

            _paidJobs = new List<decimal>();

            _database = new Database(); // database
            _clients = new List<ClientsModel>();
            _jobs = new List<JobsModel>();
            _invoices = new List<InvoicesModel>();
            _dashboardData = new List<DashboardModel>();
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
                    ClientName = _clients[i].Name,
                    ClientEmail = _clients[i].Email,
                    ClientPhone = _clients[i].Phone,
                    Jobs = _jobs.Where(job => job.ClientId == _clients[i].ClientId).ToList(),
                    Invoices = _invoices.Where(invoice => invoice.JobId == _jobs[i].JobId).ToList()
                };
                _dashboardData.Add(dashboardEntry);
            }

            ClientNames.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                ClientNames.Add(_dashboardData[i].ClientName);
            }

            ClientEmails.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                ClientEmails.Add(_dashboardData[i].ClientEmail);
            }

            ClientPhones.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                ClientPhones.Add(_dashboardData[i].ClientPhone);
            }

            JobDescriptions.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                JobDescriptions.Add(_dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.Description).ToList().FirstOrDefault()!);
            }

            JobStatus.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                JobStatus.Add(_dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.Status).ToList().FirstOrDefault()!);
            }

            JobPrice.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                JobPrice.Add(_dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.Price.ToString("C")).ToList().FirstOrDefault()!);
            }

            Paid.Clear();
            for (int i = 0; i < _dashboardData.Count; i++)
            {
                var paid = _dashboardData[i].Invoices.Where(inv => inv.JobId == _dashboardData[i].Jobs.Where(j => j.ClientId == _dashboardData[i].ClientId).Select(j => j.JobId).FirstOrDefault()).Select(inv => inv.IsPaid).FirstOrDefault();
                if (paid == true)
                {
                    Paid.Add("Yes");
                }
                else
                {
                    Paid.Add("No");
                }
            }

            _paidJobs.Clear();

            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _paidJobs.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(_invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.Price, inv.IsPaid })
                    .Where(x => x.IsPaid == true)
                    .Select(x => x.Price)
                    .ToList());
            }

            MoneyReceived = _paidJobs.Sum().ToString("C");

            _paidJobs.Clear();

            for (int i = 0; i < _dashboardData.Count; i++)
            {
                _paidJobs.AddRange(_clients
                    .Where(c => c.ClientId == _dashboardData[i].ClientId)
                    .Join(_jobs, c => c.ClientId, j => j.ClientId, (c, j) => j)
                    .Join(_invoices, j => j.JobId, inv => inv.JobId, (j, inv) => new { j.Price, inv.IsPaid })
                    .Where(x => x.IsPaid == false)
                    .Select(x => x.Price)
                    .ToList());
            }

            MoneyToReceive = _paidJobs.Sum().ToString("C");


            return Task.CompletedTask;
        }
        #endregion

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
        #endregion
    }
}
