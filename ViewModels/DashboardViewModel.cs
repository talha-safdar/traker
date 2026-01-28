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
        #endregion

        #region Data Variables
        Database _database;
        List<Clients> _clients;
        List<Jobs> _jobs;
        List<Invoices> _invoices;
        #endregion

        public DashboardViewModel(IEventAggregator events)
        {
            _events = events;
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                _database = new Database(); // database
                await _database.SetUpDatabase();

                _clients = _database.FetchClientsTable(); // clients
                _jobs = _database.FetchJobsTable(); // jobs
                _invoices = new List<Invoices>(); // invoices


                _clientNames = new ObservableCollection<string>(); // cclient names

                await LoadData();

                //return base.OnInitializedAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while fetching 'Clients' table. Please try again.\n\n{ex.Message}",
                    "Fetch Clients Tables",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                Debug.WriteLine($"Fetch Clients error: {ex.Message}");
                System.Environment.Exit(1); // kill process
            }
        }
        #endregion

        #region Private Functions
        private Task LoadData()
        {
            ClientNames.Clear();
            ClientNames = new ObservableCollection<string>(_database.FetchClientNames());

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
        #endregion
    }
}
