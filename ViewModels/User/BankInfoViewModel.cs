using Caliburn.Micro;
using Traker.Services;

namespace Traker.ViewModels.User
{
    using Database;
    using System.Windows;
    using System.Windows.Input;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.States;

    public class BankInfoViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private string _bankName;
        private string _accountName;
        private string _accountNumber;
        private string _sortcode;
        private string _IBAN;
        private string _BIC;
        #endregion

        public BankInfoViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _bankName = string.Empty;
            _accountName = string.Empty;
            _accountNumber = string.Empty;
            _sortcode = string.Empty;
            _IBAN = string.Empty;
            _BIC = string.Empty;
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                BankName = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).BankName;
                AccountName = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).AccountName;
                AccountNumber = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).AccountNumber;
                Sortcode = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).SortCode;
                IBAN = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).IBAN;
                BIC = _dataService.Bank.First(b => b.UserId == _dataService.User[0].UserId).BIC;
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Initialise Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BankInfoViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (_dataService.Bank[0].BankName != BankName ||
                        _dataService.Bank[0].AccountName != AccountName ||
                        _dataService.Bank[0].AccountNumber != AccountNumber ||
                        _dataService.Bank[0].SortCode != Sortcode ||
                        _dataService.Bank[0].IBAN != IBAN ||
                        _dataService.Bank[0].BIC != BIC
                        )
                    {
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                        {
                            _state.messageBoxVM.Symbol = 0;
                            _state.messageBoxVM.HeadMessage = "Discard changes?";
                            _state.messageBoxVM.Message = Names.DiscardEsc;
                            _state.messageBoxVM.ButtonStyle = Names.NoYes;
                            await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }

                        // if clicked yes
                        if (_state.messageBoxVM.Output == true)
                        {
                            await TryCloseAsync();
                        }
                    }
                    else
                    {
                        await TryCloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BankInfoViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task ConfirmBankInfoChanges()
        {
            try
            {
                await Task.Run(async() =>
                {
                    await TryCloseAsync();
                    await Database.EditBank(_dataService.User[0].UserId, BankName.Trim(), AccountName.Trim(), AccountNumber.Trim(), Sortcode.Trim(), IBAN.Trim(), BIC.Trim());
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Confirm Changes";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BankInfoViewModel: ConfirmBankInfoChanges() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                if (_dataService.Bank[0].BankName != BankName ||
                    _dataService.Bank[0].AccountName != AccountName ||
                    _dataService.Bank[0].AccountNumber != AccountNumber ||
                    _dataService.Bank[0].SortCode != Sortcode ||
                    _dataService.Bank[0].IBAN != IBAN ||
                    _dataService.Bank[0].BIC != BIC
                    )
                {
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 0;
                        _state.messageBoxVM.HeadMessage = "Discard changes?";
                        _state.messageBoxVM.Message = Names.DiscardEsc;
                        _state.messageBoxVM.ButtonStyle = Names.NoYes;
                        await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }

                    // if clicked yes
                    if (_state.messageBoxVM.Output == true)
                    {
                        await TryCloseAsync();
                    }
                }
                else
                {
                    await TryCloseAsync();
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"BankInfoViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public View Variables
        public string BankName
        {
            get { return _bankName; }
            set
            {
                _bankName = value;
                NotifyOfPropertyChange(() => BankName);
            }
        }

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
