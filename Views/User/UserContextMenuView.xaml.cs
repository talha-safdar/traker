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

namespace Traker.Views.User
{
    /// <summary>
    /// Interaction logic for UserContextMenuView.xaml
    /// </summary>
    public partial class UserContextMenuView : UserControl
    {
        public UserContextMenuView()
        {
            InitializeComponent();
        }

        #region user button animation
        private void UserBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserBorder.Opacity = 0.7;
        }

        private void UserBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UserBorder.Opacity = 1.0;
        }

        private void UserBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            UserBorder.Opacity = 1.0;
        }
        #endregion

        #region business button animation
        private void BusinessBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BusinessBorder.Opacity = 0.7;
        }

        private void BusinessBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BusinessBorder.Opacity = 1.0;
        }

        private void BusinessBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            BusinessBorder.Opacity = 1.0;
        }
        #endregion

        #region bank button animation
        private void BankBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BankBorder.Opacity = 0.7;
        }

        private void BankBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BankBorder.Opacity = 1.0;
        }

        private void BankBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            BankBorder.Opacity = 1.0;
        }
        #endregion
    }
}
