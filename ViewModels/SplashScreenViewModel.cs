using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.States;

namespace Traker.ViewModels
{
    class SplashScreenViewModel : Screen
    {
        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        #endregion

        public SplashScreenViewModel(AppState appState)
        {
            State = appState;
        }
    }
}
