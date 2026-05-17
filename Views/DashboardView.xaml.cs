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

        #region submit filter button animation
        private void SubmitFilterBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitFilterBorder.Opacity = 0.7;
        }

        private void SubmitFilterBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitFilterBorder.Opacity = 1.0;
        }

        private void SubmitFilterBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitFilterBorder.Opacity = 1.0;
        }
        #endregion

        #region submit sort button animation
        private void SubmitSortBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitSortBorder.Opacity = 0.7;
        }

        private void SubmitSortBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitSortBorder.Opacity = 1.0;
        }

        private void SubmitSortBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitSortBorder.Opacity = 1.0;
        }
        #endregion

        #region submit add job button animation
        private void SubmitAddJobBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitAddJobBorder.Opacity = 0.8;
        }

        private void SubmitAddJobBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitAddJobBorder.Opacity = 1.0;
        }

        private void SubmitAddJobBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitAddJobBorder.Opacity = 1.0;
        }
        #endregion

        #region submit add client button animation
        private void SubmitAddClientBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitAddClientBorder.Opacity = 0.8;
        }

        private void SubmitAddClientBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitAddClientBorder.Opacity = 1.0;
        }

        private void SubmitAddClientBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitAddClientBorder.Opacity = 1.0;
        }
        #endregion

        #region submit user button animation
        private void SubmitUserBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitUserBorder.Opacity = 0.7;
        }

        private void SubmitUserBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitUserBorder.Opacity = 1.0;
        }

        private void SubmitUserBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitUserBorder.Opacity = 1.0;
        }
        #endregion

        /// <summary>
        /// On double click trigger EditJob() function.
        /// 
        /// Caliburn doesn't work...
        /// </summary>
        private void EditJob(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement fe && fe.DataContext is DashboardModel jobSelected)
            {
                if (DataContext is DashboardViewModel vm)
                {
                    using var _ = vm.EditJob(jobSelected);
                }
            }
        }
    }
}