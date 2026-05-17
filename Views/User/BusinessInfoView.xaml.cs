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
    /// Interaction logic for BusinessInfoView.xaml
    /// </summary>
    public partial class BusinessInfoView : UserControl
    {
        public BusinessInfoView()
        {
            InitializeComponent();
        }

        #region submit individual button animation
        private void SubmitBorder1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder1.Opacity = 0.7;
        }

        private void SubmitBorder1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder1.Opacity = 1.0;
        }

        private void SubmitBorder1_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitBorder1.Opacity = 1.0;
        }
        #endregion

        #region submit company button animation
        private void SubmitBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder.Opacity = 0.7;
        }

        private void SubmitBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder.Opacity = 1.0;
        }

        private void SubmitBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitBorder.Opacity = 1.0;
        }
        #endregion
    }
}