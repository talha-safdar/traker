using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Traker.Helper;
using Traker.States;

namespace Traker.ViewModels
{
    public class MessageBoxViewModel : Screen
    {
        #region Public View Variables
        public int Symbol { get; set; } = 0; // 0=info, 1=warning, 2=error
        public string HeadMessage { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // Close=kill app, else just close message box
        public int ButtonStyle { get; set; } = 0; // 0=OK, 1=CancelOK
        #endregion

        public AppState State { get; set; }

        public Visibility IsVisible { get; set; } = Visibility.Collapsed; // for showing the window elements all at the same time

        public MessageBoxViewModel() 
        {
            //State = appState;
        }

        protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            IsVisible = Visibility.Visible;
            NotifyOfPropertyChange(() => IsVisible);

            await base.OnActivatedAsync(cancellationToken);
        }

        #region Public Functions
        public async Task OK()
        {
            if (Action == Names.Close) // close app
            {
                //Application.Current.Shutdown();
                Environment.Exit(1); // kill process
            }
            else if (Action == Names.ConfirmProceed) // confirmation
            {
                State.allowProceed = false;
                Action = string.Empty; // reset action
                await TryCloseAsync();
            }
        }
        #endregion
    }
}
