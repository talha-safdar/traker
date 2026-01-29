using System.Configuration;
using System.Data;
using System.Windows;

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
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                // This will tell you exactly which XAML line is broken
                MessageBox.Show(ex.ToString());
            }
        }
    }

}
