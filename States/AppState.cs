using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Traker.ViewModels;
using Traker.ViewModels.Add;
using Traker.ViewModels.Edit;
using Traker.ViewModels.User;

namespace Traker.States
{
    /// <summary>
    /// It contains variables that are shared accross
    /// the project
    /// </summary>
    public class AppState : PropertyChangedBase
    {
        // window form background overlay
        private bool _windowFormOpen = false;

        // splash screen
        public string _splashText = string.Empty;

        // loading animation 
        public bool _isBusy = false; // true = shows loading animation, false = hide
        public string _loadingMessage = string.Empty; // text below the loading animation 

        // filter sort
        public string currentSortOption = string.Empty; // useful when resetting filter but sort was on to restore it on filter reset
        public string currentFilterOption = string.Empty;
        public bool _isSortInUse = false;
        public bool _isFilterInUse = false;

        // messagebox
        public MessageBoxViewModel messageBoxVM = new MessageBoxViewModel();
        public bool allowProceed = false; // used for confirmation dialogs

        // VMs allow access from shell view model
        public JobContextMenuViewModel? JobContextMenuViewModel;
        public AddClientViewModel? AddClientViewModel;
        public AddJobViewModel? AddJobViewModel;
        public EditClientViewModel? EditClientViewModel;
        public EditJobViewModel? EditJobViewModel;
        public UserContextMenuViewModel? UserContextMenuViewModel;
        public SortJobsViewModel? SortJobsViewModel;
        public FilterJobsViewModel? FilterJobsViewModel;
        public EditInvoiceViewModel? EditInvoiceViewModel;

        public string SplashText
        {
            get => _splashText;
            set
            {
                _splashText = value;
                NotifyOfPropertyChange(() => SplashText);
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                NotifyOfPropertyChange(() => LoadingMessage);
            }
        }

        public bool IsSortInUse
        {
            get => _isSortInUse;
            set
            {
                _isSortInUse = value;
                NotifyOfPropertyChange(() => IsSortInUse);
            }
        }

        public bool IsFilterInUse
        {
            get => _isFilterInUse;
            set
            {
                _isFilterInUse = value;
                NotifyOfPropertyChange(() => IsFilterInUse);
            }
        }

        public bool WindowFormOpen
        {
            get => _windowFormOpen;
            set
            {
                _windowFormOpen = value;
                NotifyOfPropertyChange(() => WindowFormOpen);
            }
        }
    }
}