using Caliburn.Micro;
using Traker.Services;

namespace Traker.ViewModels.User
{
    using Database;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.States;

    public class UserInfoViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private string _fullName;
        private string _email;
        private string _phone;
        private string _businessType;

        private ObservableCollection<bool> _toggleButtons;
        private ObservableCollection<Brush> _backgroundButtons; // active=#333333. inactive=#1A1A1A
        private ObservableCollection<Brush> _foregroundText; // active=#FFFFFF. inactive=#888888
        #endregion

        #region Private Class Field Variables
        private string _activeButonColour = "#333333";
        private string _inactiveButonColour = "#1A1A1A";
        private string _activeTextColour = "#FFFFFF";
        private string _inactiveTextColour = "#888888";
        #endregion

        public UserInfoViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _fullName = string.Empty;
            _email = string.Empty;
            _phone = string.Empty;
            _businessType = string.Empty;
            _toggleButtons = new ObservableCollection<bool>();
            _backgroundButtons = new ObservableCollection<Brush>();
            _foregroundText = new ObservableCollection<Brush>();
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                FullName = _dataService.User[0].FullName;
                Email = _dataService.User[0].Email;
                Phone = _dataService.User[0].Phone;
                BusinessType = _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).BusinessType;

                // 0=individual, 1=company
                if (BusinessType == Names.Individual)
                {
                    ToggleButtons = new ObservableCollection<bool>() { false, true };
                    BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)) };
                    ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)) };
                }
                else if (BusinessType == Names.Company)
                {
                    ToggleButtons = new ObservableCollection<bool>() { true, false };
                    BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeButonColour)) };
                    ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeTextColour)) };
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
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (_dataService.User[0].FullName != FullName ||
                        _dataService.User[0].Email != Email ||
                        _dataService.User[0].Phone != Phone ||
                        _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).BusinessType != BusinessType
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
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task ConfirmUserInfoChanges()
        {
            try
            {
                await Task.Run(async () =>
                {
                    await Database.EditUser(_dataService.User[0].UserId, FullName.Trim(), Email.Trim(), Phone.Trim(), BusinessType.Trim());
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                    await TryCloseAsync();
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
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: ConfirmUserInfoChanges() FAIL\n\t{ex.Message}");
            }
        }

        public Task SetBusinessType(string businessType)
        {
            try
            {
                BusinessType = businessType;
                if (BusinessType == Names.Individual)
                {
                    ToggleButtons = new ObservableCollection<bool>() { false, true };
                    BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)) };
                    ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)) };
                }
                else if (BusinessType == Names.Company)
                {
                    ToggleButtons = new ObservableCollection<bool>() { true, false };
                    BackgroundButtons = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveButonColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeButonColour)) };
                    ForegroundText = new ObservableCollection<Brush>() { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inactiveTextColour)), new SolidColorBrush((Color)ColorConverter.ConvertFromString(_activeTextColour)) };
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
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: SetBusinessType() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        public async Task Exit()
        {
            try
            {
                if (_dataService.User[0].FullName != FullName ||
                    _dataService.User[0].Email != Email ||
                    _dataService.User[0].Phone != Phone ||
                    _dataService.Business.First(b => b.UserId == _dataService.User[0].UserId).BusinessType != BusinessType
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
                Logger.LogActivity(Logger.ERROR, $"UserInfoViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public View Variables
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyOfPropertyChange(() => Phone);
            }
        }

        public string BusinessType
        {
            get { return _businessType; }
            set
            {
                _businessType = value;
                NotifyOfPropertyChange(() => BusinessType);
            }
        }

        public ObservableCollection<bool> ToggleButtons
        {
            get { return _toggleButtons; }
            set
            {
                _toggleButtons = value;
                NotifyOfPropertyChange(() => ToggleButtons);
            }
        }

        public ObservableCollection<Brush> BackgroundButtons
        {
            get { return _backgroundButtons; }
            set
            {
                _backgroundButtons = value;
                NotifyOfPropertyChange(() => BackgroundButtons);
            }
        }

        public ObservableCollection<Brush> ForegroundText
        {
            get { return _foregroundText; }
            set
            {
                _foregroundText = value;
                NotifyOfPropertyChange(() => ForegroundText);
            }
        }
        #endregion
    }
}