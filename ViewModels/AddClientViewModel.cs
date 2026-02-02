using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;
using Traker.States;

namespace Traker.ViewModels
{
    using Database;
    using System.Globalization;
    using Traker.Events;

    public class AddClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private string _clientName;
        private string _jobDescription;
        private string _price;
        private string _dueDate;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public AddClientViewModel(IEventAggregator events, AppState appState)
        {
            _events = events;

            _clientName = String.Empty;
            _jobDescription = String.Empty;
            _price = String.Empty;
            _dueDate = String.Empty;

            State = appState;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }


        public async Task Exit()
        {
            // argument true only for dialogs
            // empty for the rest

            // debug
            //DateTime dueDate = DateTime.ParseExact(
            //"12/12/2026",
            //"dd/MM/yyyy",
            //CultureInfo.InvariantCulture
            //);

            //decimal amount = decimal.Parse(
            //    "12342",
            //    CultureInfo.InvariantCulture
            //);

            //await Database.AddRow("Luca", "Rapina", amount, dueDate);

            //await _events.PublishOnUIThreadAsync(new RefreshDatabase());

            State.IsAddRowEntryOpen = false;
            await TryCloseAsync();
        }

        public Task AddRow()
        {
            // add checks
            // remove white spaces 
            // price textbox only number
            // money format use commas and dots
            // empty boxes on press add

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

            Database.AddRow(ClientName, JobDescription, amount, dueDate);

            _events.PublishOnUIThreadAsync(new RefreshDatabase());

            return Task.CompletedTask;
        }

        #region Public View Variables
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                NotifyOfPropertyChange(() => ClientName);
            }
        }

        public string JobDescription
        {
            get { return _jobDescription; }
            set
            {
                _jobDescription = value;
                NotifyOfPropertyChange(() => JobDescription);
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }

        public string DueDate
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
