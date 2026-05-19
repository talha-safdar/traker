using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Traker.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            Loader.FileName = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "Media",
            "Images",
            "Icons",
            "Loading",
            "spinner.json");

            ApplyWindowStateFix();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            ApplyWindowStateFix();
        }

        private void ApplyWindowStateFix()
        {
            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new Thickness(8);
                MaxHeight = SystemParameters.WorkArea.Height + 16;
            }
            else
            {
                BorderThickness = new Thickness(0);
                MaxHeight = double.PositiveInfinity;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out System.Drawing.Point pt);

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow((DependencyObject)sender);

            // Double-click → maximize / restore
            if (e.ClickCount == 2)
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
                return;
            }

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    // 🔑 Get real cursor position (screen coords, not WPF)
                    GetCursorPos(out var cursor);

                    // Get current screen (monitor)
                    var screen = System.Windows.SystemParameters.WorkArea;

                    // Calculate relative X inside window
                    double percentX = e.GetPosition(window).X / window.ActualWidth;

                    // Restore window
                    window.WindowState = WindowState.Normal;

                    // Position window correctly on current monitor
                    window.Left = cursor.X - (window.Width * percentX);
                    window.Top = cursor.Y - 10; // small offset like Windows
                }

                window.DragMove();
            }
        }
    }
}
