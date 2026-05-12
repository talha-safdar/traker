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

namespace Traker.Views
{
    /// <summary>
    /// Interaction logic for MessageBoxView.xaml
    /// </summary>
    public partial class MessageBoxView : UserControl
    {
        public MessageBoxView()
        {
            InitializeComponent();
        }

        #region left button animation
        private void LeftBorder_MouseLeftButtonDown( object sender,  MouseButtonEventArgs e)
        {
            LeftBorder.Opacity = 0.5;
        }

        private void LeftBorder_MouseLeftButtonUp( object sender, MouseButtonEventArgs e)
        {
            LeftBorder.Opacity = 1.0;
        }

        private void LeftBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            LeftBorder.Opacity = 1.0;
        }
        #endregion

        #region right button animation
        private void RightBorder_MouseLeftButtonDown( object sender,  MouseButtonEventArgs e)
        {
            RightBorder.Opacity = 0.5;
        }

        private void RightBorder_MouseLeftButtonUp( object sender, MouseButtonEventArgs e)
        {
            RightBorder.Opacity = 1.0;
        }

        private void RightBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            RightBorder.Opacity = 1.0;
        }
        #endregion
    }
}
