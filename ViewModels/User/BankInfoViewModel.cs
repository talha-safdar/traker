using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Traker.Events;
using Traker.Services;

namespace Traker.ViewModels.User
{
    using Database;

    public class BankInfoViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
        #endregion

        #region Private View Variables
        private string _accountName;
        private string _accountNumber;
        private string _sortcode;
        private string _IBAN;
        private string _BIC;
        #endregion

        public BankInfoViewModel(IEventAggregator events, DataService dataService)
        {
            _events = events;
            _dataService = dataService;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            AccountName = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).AccountName;
            AccountNumber = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).AccountNumber;
            Sortcode = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).SortCode;
            IBAN = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).IBAN;
            BIC = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).BIC;
            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task ConfirmBankInfoChanges()
        {
            await Database.EditBank(_dataService.User[0].UserId, AccountName, AccountNumber, Sortcode, IBAN, BIC);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
        }
        #endregion

        #region Public View Variables
        public string AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                NotifyOfPropertyChange(() => AccountName);
            }
        }

        public string AccountNumber
        {
            get { return _accountNumber; }
            set
            {
                _accountNumber = value;
                NotifyOfPropertyChange(() => AccountNumber);
            }
        }

        public string Sortcode
        {
            get { return _sortcode; }
            set
            {
                _sortcode = value;
                NotifyOfPropertyChange(() => Sortcode);
            }
        }

        public string IBAN
        {
            get { return _IBAN; }
            set
            {
                _IBAN = value;
                NotifyOfPropertyChange(() => IBAN);
            }
        }

        public string BIC
        {
            get { return _BIC; }
            set
            {
                _BIC = value;
                NotifyOfPropertyChange(() => BIC);
            }
        }
        #endregion
    }
}
