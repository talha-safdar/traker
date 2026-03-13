using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.ViewModels;

namespace Traker.States
{
    /// <summary>
    /// Represents the current state of the application, including view-related properties that notify listeners of
    /// changes.
    /// </summary>
    /// <remarks><see cref="AppState"/> provides observable properties for tracking application state, such as
    /// UI element visibility.  Properties raise change notifications to support data binding scenarios, enabling the
    /// user interface to respond to state changes.</remarks>
    public class AppState : PropertyChangedBase
    {
        #region Private View State Variables
        private bool _isWindowOpen = false;
        #endregion

        #region Public View State Variables
        public bool IsWindowOpen
        {
            get { return _isWindowOpen; }
            set
            {
                _isWindowOpen = value;
                NotifyOfPropertyChange(() => IsWindowOpen);
            }
        }
        #endregion
    }
}