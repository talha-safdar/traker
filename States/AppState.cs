using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.ViewModels;

namespace Traker.States
{
    public class AppState : PropertyChangedBase
    {
        #region Private View State Variables
        private Object _popUpMenu = new JobDetailsViewModel(); // for any view model as it must be showing up one at a time
        private bool _isAddRowEntryOpen = false;
        #endregion

        #region Public View State Variables
        public Object PopUpMenu
        {
            get => _popUpMenu;
            set
            {
                _popUpMenu = value;
                NotifyOfPropertyChange(() => PopUpMenu);
            }
        }

        public bool IsAddRowEntryOpen
        {
            get { return _isAddRowEntryOpen; }
            set
            {
                _isAddRowEntryOpen = value;
                NotifyOfPropertyChange(() => IsAddRowEntryOpen);
            }
        }
        #endregion
    }
}
