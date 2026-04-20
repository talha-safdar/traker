using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Helper;
using Traker.Services;

namespace Traker.ViewModels.User
{
    public class UserContextMenuViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        #endregion

        #region Private Class Field Variables
        private UserInfoViewModel _userInfoViewModel;
        private BusinessInfoViewModel _businessInfoViewModel;
        private BankInfoViewModel _bankInfoViewModel;
        #endregion

        public UserContextMenuViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
        }

        #region Public View Functions
        public async Task OpenOption(string option)
        {
            if (option == "user")
            {
                _userInfoViewModel = new UserInfoViewModel(_events, _dataService);
                await _windowManager.ShowWindowAsync(_userInfoViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            else if (option == "business")
            {
                _businessInfoViewModel = new BusinessInfoViewModel(_events, _dataService);
                await _windowManager.ShowWindowAsync(_businessInfoViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            else if (option == "bank")
            {
                _bankInfoViewModel = new BankInfoViewModel(_events, _dataService);
                await _windowManager.ShowWindowAsync(_bankInfoViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
        }
        #endregion
    }
}