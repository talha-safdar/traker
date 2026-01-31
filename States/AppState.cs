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
        private bool _isAddRowEntryOpen = false;
        #endregion

        #region Public View State Variables
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
