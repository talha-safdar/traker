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
        #endregion

        public DashboardViewModel(IEventAggregator events)
        {
            _events = events;
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            _database = new Database(); // database
            await _database.SetUpDatabase();

            _clientNames = new ObservableCollection<string>(); // cclient names

            await LoadData();

            //return base.OnInitializedAsync(cancellationToken);
        }
        #endregion

        #region Private Functions
        private Task LoadData()
        {
            ClientNames.Clear();
            ClientNames = new ObservableCollection<string>(_database.LoadData());

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
