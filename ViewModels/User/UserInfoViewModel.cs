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
    using Traker.Events;

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

            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task ConfirmUserInfoChanges()
        {
            await Database.EditUser(_dataService.User[0].UserId, FullName, Email, Phone, BusinessType);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
        }

        public Task SetBusinessType(string businessType)
        {
            BusinessType = businessType;
            return Task.CompletedTask;
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
        #endregion
    }
}
