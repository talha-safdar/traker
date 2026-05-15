using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Traker.Helper;
using Traker.Services;
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
        public string ButtonStyle { get; set; } = string.Empty; // OK, CancelOK, NoYes
        public bool Output { get; set; } = false; // for Yes or No
        public Visibility IsVisible { get; set; } = Visibility.Collapsed; // for showing the window elements all at the same time
        #endregion


        public MessageBoxViewModel() { }

        #region Caliburn Functions
        protected override async Task OnActivatedAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Yield();

                Output = false;
                Action = string.Empty; // reset action

                IsVisible = Visibility.Visible;
                NotifyOfPropertyChange(() => IsVisible);

                await base.OnActivatedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                $"{ex.Message}",
                "Activation Form",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
                Logger.LogActivity(Logger.ERROR, $"MessageBoxViewModel: OnActivatedAsync() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public Functions
        public async Task RightButton()
        {
            try
            {
                if (Action == Names.Close) // close app
                {
                    //Application.Current.Shutdown();
                    Environment.Exit(1); // kill process
                }
                else if (ButtonStyle == Names.NoYes)
                {
                    Output = true;
                    await TryCloseAsync();
                }
                else if (Action == Names.LeaveForm)
                {
                    Output = true;
                    await TryCloseAsync();
                }
                else
                {
                    await TryCloseAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                $"{ex.Message}",
                "Right Button",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
                Logger.LogActivity(Logger.ERROR, $"MessageBoxViewModel: RightButton() FAIL\n\t{ex.Message}");
            }
        }

        public async Task LeftButton()
        {
            try
            {
                await TryCloseAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                $"{ex.Message}",
                "Left Button",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
                Logger.LogActivity(Logger.ERROR, $"MessageBoxViewModel: LeftButton() FAIL\n\t{ex.Message}");
            }
        }
        #endregion
    }
}