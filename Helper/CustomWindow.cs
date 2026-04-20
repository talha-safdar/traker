using System.Dynamic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Traker.Services;

namespace Traker.Helper
{
    /// <summary>
    /// Handles the settings for custom windows, such as dialogs, by providing a method that returns a dynamic object with the necessary properties set for the desired window configuration.
    /// </summary>
    public static class CustomWindow
    {
        /// <summary>
        /// Sets the properties for a custom dialog window, such as title, startup location, style, transparency, background, resize mode, and minimum dimensions, and returns these settings as a dynamic object that can be applied to a Window instance when creating a dialog. This allows for consistent styling and behavior across all dialogs in the application by centralizing the configuration in one method.
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

                Logger.LogActivity(Logger.INFO, $"CustomWindow: SettingsForDialog() OK");
                return settings;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while opening window. Please try again.\n\n{ex.Message}",
                    "Open Window",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Logger.LogActivity(Logger.ERROR, $"CustomWindow: SettingsForDialog() FAIL");
                throw; // necessary otherwsie cries about no return value
            }
        }
    }
}
