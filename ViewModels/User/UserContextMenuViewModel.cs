using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Traker.Helper;
using Traker.Services;
using Traker.States;

namespace Traker.ViewModels.User
{
    public class UserContextMenuViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private Class Field Variables
        private UserInfoViewModel _userInfoViewModel;
        private BusinessInfoViewModel _businessInfoViewModel;
        private BankInfoViewModel _bankInfoViewModel;
        #endregion

        public UserContextMenuViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _userInfoViewModel = new UserInfoViewModel(_events, _windowManager, _dataService, _state);
            _businessInfoViewModel = new BusinessInfoViewModel(_events, _windowManager, _dataService, _state);
            _bankInfoViewModel = new BankInfoViewModel(_events, _windowManager, _dataService, _state);
        }

        #region Public View Functions
        public async Task OpenOption(string option)
        {
            try
            {
                //bool isOpen = Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is BusinessInfoViewModel);
                //Window window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is BusinessInfoViewModel)!;
                //window?.Close();
                //window?.Activate(); // bring to front
                await TryCloseAsync();

                if (option == "user")
                {
                    // close other windows
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is BusinessInfoViewModel) == true)
                    {
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is BusinessInfoViewModel)!.Close();
                    }
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is BankInfoViewModel) == true)
                    {
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is BankInfoViewModel)!.Close();
                    }

                    _userInfoViewModel = new UserInfoViewModel(_events, _windowManager, _dataService, _state);
                    await _windowManager.ShowDialogAsync(_userInfoViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
                else if (option == "business")
                {
                    // close other windows
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is UserInfoViewModel) == true)
                    {
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is UserInfoViewModel)!.Close();
                    }
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is BankInfoViewModel) == true)
                    {
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is BankInfoViewModel)!.Close();
                    }

                    _businessInfoViewModel = new BusinessInfoViewModel(_events, _windowManager, _dataService, _state);
                    await _windowManager.ShowDialogAsync(_businessInfoViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
                else if (option == "bank")
                {
                    // close other windows
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is UserInfoViewModel) == true)
                    {
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is UserInfoViewModel)!.Close();
                    }
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext is BusinessInfoViewModel) == true)
                    {
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext is BusinessInfoViewModel)!.Close();
                    }

                    _bankInfoViewModel = new BankInfoViewModel(_events, _windowManager, _dataService, _state);
                    await _windowManager.ShowDialogAsync(_bankInfoViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Open Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"UserContextMenuViewModel: OpenOption() FAIL\n\t{ex.Message}");
            }
        }
        #endregion
    }
}