using Caliburn.Micro;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Traker.Services;
using Traker.States;

namespace Traker.Helper
{
    /// <summary>
    /// Handles the settings for custom windows, such as dialogs, by providing a method that returns a dynamic object with the necessary properties set for the desired window configuration.
    /// </summary>
    public static class CustomWindow
    {
        /// <summary>
        /// Set the window in the centre of the parent window.
        /// You can customise the positioning by setting the "isAnchored" parameter 
        /// to true and providing the desired vertical and horizontal offsets.
        /// </summary>
        public static dynamic SettingsForDialog(int height, int width, bool isAnchored, double verticalOffset = 0, double horizontalOffset = 0)
        {
            try
            {
                dynamic settings = new ExpandoObject();
                settings.Title = string.Empty;
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.WindowStyle = WindowStyle.None;
                settings.AllowsTransparency = true;
                settings.Background = Brushes.Transparent;
                settings.ResizeMode = ResizeMode.CanResize;
                settings.MinHeight = height;
                settings.MinWidth = width;

                if (isAnchored == true)
                {
                    // --- Required Context Settings ---
                    settings.Placement = PlacementMode.Relative;
                    settings.VerticalOffset = verticalOffset; // negative=up, positive=down
                    settings.HorizontalOffset = horizontalOffset; // negative=left, positive=right
                }
                return settings;
            }
            catch (Exception ex)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Open Window";
                        state.messageBoxVM.Message = ex.Message;
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                    return Task.CompletedTask;
                });
                Logger.LogActivity(Logger.ERROR, $"CustomWindow: SettingsForDialog() FAIL\n\t{ex.Message}");
                throw;
            }
        }
    }
}
