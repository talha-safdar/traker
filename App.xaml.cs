using QuestPDF.Infrastructure;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Traker.Services;

namespace Traker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                base.OnStartup(e);

                // This forces WPF to use the local Windows culture for all Bindings
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                Logger.LogActivity(Logger.INFO, "####################################### NEW EXECUTION #######################################");
            }
            catch (Exception ex)
            {
                // This will tell you exactly which XAML line is broken
                MessageBox.Show(ex.ToString());
                Logger.LogActivity(Logger.ERROR, "FAIL");
            }
        }

        public App()
        {
            // Global exception handling
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender,
            UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            MessageBox.Show(ex.Message);
        }
    }
}
