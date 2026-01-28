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
        #endregion

        #region Data Variables
        Database _database;
        List<ClientsModel> _clients;
        List<JobsModel> _jobs;
        List<InvoicesModel> _invoices;
        List<DashboardModel> _dashboardData;
        #endregion

        public DashboardViewModel(IEventAggregator events)
        {
            _events = events;

            _clientNames = new ObservableCollection<string>(); // client names
            _clientEmails = new ObservableCollection<string>(); // client emails

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
                    ClientId = _clients[i].ClientId.ToString(),
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
        #endregion
    }
}
