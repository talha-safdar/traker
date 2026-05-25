using Caliburn.Micro;
using Traker.Services;

namespace Traker.ViewModels.User
{
    using Database;
    using System.Net;
    using System.Windows;
    using System.Windows.Input;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Models.Database;
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

        private BankModel _bankModel;

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field variables
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
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

            _bankModel = new BankModel();
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // submit button
                EnableSubmitBtn = false;
                OpacitySubmitBtn = _halfOpacity;

                _bankModel = await Database.FetchBank();

                BankName = _bankModel.BankName;
                AccountName = _bankModel.AccountName;
                AccountNumber = _bankModel.AccountNumber;
                Sortcode = _bankModel.SortCode;
                IBAN = _bankModel.IBAN;
                BIC = _bankModel.BIC;
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
            await base.OnInitializedAsync(cancellationToken);
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
                    await Exit();
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
                await TryCloseAsync();
                _state.WindowFormOpen = false;
                await Task.Run(async() =>
                {
                    await Database.EditBank(await Database.GetUserId(), BankName.Trim(), AccountName.Trim(), AccountNumber.Trim(), Sortcode.Trim(), IBAN.Trim(), BIC.Trim());
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
                if (_bankModel.BankName != BankName ||
                    _bankModel.AccountName != AccountName ||
                    _bankModel.AccountNumber != AccountNumber ||
                    _bankModel.SortCode != Sortcode ||
                    _bankModel.IBAN != IBAN ||
                    _bankModel.BIC != BIC
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
                        _state.WindowFormOpen = false;
                    }
                }
                else
                {
                    await TryCloseAsync();
                    _state.WindowFormOpen = false;
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

        #region Private Functions
        private Task CanSubmit()
        {
            try
            {
                if (BankName != _bankModel.BankName ||
                AccountName != _bankModel.AccountName ||
                AccountNumber != _bankModel.AccountNumber ||
                Sortcode != _bankModel.SortCode ||
                IBAN != _bankModel.IBAN ||
                BIC != _bankModel.BIC)
                {
                    EnableSubmitBtn = true;
                    OpacitySubmitBtn = _fullOpacity;
                }
                else
                {
                    EnableSubmitBtn = false;
                    OpacitySubmitBtn = _halfOpacity;
                }               
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Edit User";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: CanSubmit() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
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
                CanSubmit();
            }
        }

        public string AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                NotifyOfPropertyChange(() => AccountName);
                CanSubmit();
            }
        }

        public string AccountNumber
        {
            get { return _accountNumber; }
            set
            {
                _accountNumber = value;
                NotifyOfPropertyChange(() => AccountNumber);
                CanSubmit();
            }
        }

        public string Sortcode
        {
            get { return _sortcode; }
            set
            {
                _sortcode = value;
                NotifyOfPropertyChange(() => Sortcode);
                CanSubmit();
            }
        }

        public string IBAN
        {
            get { return _IBAN; }
            set
            {
                _IBAN = value;
                NotifyOfPropertyChange(() => IBAN);
                CanSubmit();
            }
        }

        public string BIC
        {
            get { return _BIC; }
            set
            {
                _BIC = value;
                NotifyOfPropertyChange(() => BIC);
                CanSubmit();
            }
        }

        public bool EnableSubmitBtn
        {
            get { return _enableSubmitBtn; }
            set
            {
                _enableSubmitBtn = value;
                NotifyOfPropertyChange(() => EnableSubmitBtn);
            }
        }

        public double OpacitySubmitBtn
        {
            get { return _opacitySubmitBtn; }
            set
            {
                _opacitySubmitBtn = value;
                NotifyOfPropertyChange(() => OpacitySubmitBtn);
            }
        }
        #endregion
    }
}
