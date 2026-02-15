using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Traker.Helper
{
    public static class CustomWindow
    {
        public static dynamic SettingsForDialog(int height, int width)
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
            return settings;
        }
    }
}
