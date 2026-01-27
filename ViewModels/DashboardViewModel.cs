using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    public class DashboardViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private ObservableCollection<string> _clientNames;
        #endregion

        public DashboardViewModel(IEventAggregator events)
        {
            _events = events;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            _clientNames = new ObservableCollection<string>();

            ClientNames.Add("Client A");
            ClientNames.Add("Client B");
            ClientNames.Add("Client C");

            return base.OnInitializedAsync(cancellationToken);
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
        #endregion
    }
}
