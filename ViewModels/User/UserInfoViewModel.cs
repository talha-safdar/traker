using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Services;

namespace Traker.ViewModels.User
{
    using Database;
    using System.Collections.ObjectModel;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Traker.Events;
    using Traker.Helper;

    public class UserInfoViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly DataService _dataService;
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

        public UserInfoViewModel(IEventAggregator events, DataService dataService)
        {
            _events = events;
            _dataService = dataService;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
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

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task ConfirmUserInfoChanges()
        {
            await Database.EditUser(_dataService.User[0].UserId, FullName, Email, Phone, BusinessType);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
            await TryCloseAsync();
        }

        public Task SetBusinessType(string businessType)
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
            return Task.CompletedTask;
        }

        public async Task Exit()
        {
            await TryCloseAsync();
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
