using QuestPDF.Infrastructure;
using System.Configuration;
using System.Data;
using System.Windows;
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
                Logger.LogActivity(Logger.INFO, "####################################### NEW EXECUTION #######################################");
            }
            catch (Exception ex)
            {
                // This will tell you exactly which XAML line is broken
                MessageBox.Show(ex.ToString());
                Logger.LogActivity(Logger.ERROR, "FAIL");
            }
        }
    }

}
