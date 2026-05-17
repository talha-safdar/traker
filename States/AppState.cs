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
        // splash screen
        public string _splashText = string.Empty;

        // filter sort
        public bool IsSortToClear = false; // set from dashboard to clear the sort on opening its menu
        public bool IsFilterToClear = false; // set from dashboard to clear the filter on opening its menu
        public string currentSortOption = string.Empty; // useful when resetting filter but sort was on to restore it on filter reset
        public string currentFilterOption = string.Empty;

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
    }
}