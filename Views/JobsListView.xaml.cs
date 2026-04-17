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
    /// Interaction logic for JobsListView.xaml
    /// </summary>
    public partial class JobsListView : UserControl
    {
        public JobsListView()
        {
            InitializeComponent();
        }

        private void EditJob(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement fe && fe.DataContext is DashboardModel jobSelected)
            {
                if (DataContext is JobsListViewModel vm)
                {
                    using var _ = vm.EditJob(jobSelected);
                }
            }
        }
    }
}
