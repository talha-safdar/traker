using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Traker.Models;
using Traker.ViewModels;

namespace Traker.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On double click trigger EditJob() function.
        /// 
        /// Caliburn doesn't work...
        /// </summary>
        private void EditJob(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement fe && fe.DataContext is DashboardModel job)
            {
                if (DataContext is DashboardViewModel vm)
                {
                    using var _ = vm.EditJob();
                }
            }
        }
    }
}