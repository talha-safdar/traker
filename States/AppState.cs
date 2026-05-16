using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Traker.ViewModels;

namespace Traker.States
{
    /// <summary>
    /// It contains variables that are shared accross
    /// the project
    /// </summary>
    public class AppState : PropertyChangedBase
    {        
        public bool IsSortToClear = false; // set from dashboard to clear the sort on opening its menu
        public bool IsFilterToClear = false; // set from dashboard to clear the filter on opening its menu

        // messagebox
        public MessageBoxViewModel messageBoxVM = new MessageBoxViewModel();
        public bool allowProceed = false; // used for confirmation dialogs
    }
}