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
        private JobDetailsViewModel _popUpMenu = new JobDetailsViewModel();
        #endregion

        #region Public View State Variables
        public JobDetailsViewModel PopUpMenu
        {
            get => _popUpMenu;
            set
            {
                _popUpMenu = value;
                NotifyOfPropertyChange(() => PopUpMenu);
            }
        }
        #endregion
    }
}
