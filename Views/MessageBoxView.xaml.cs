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

        #region cancel button animation
        private void CancelBorder_MouseLeftButtonDown( object sender,  MouseButtonEventArgs e)
        {
            CancelBorder.Opacity = 0.5;
        }

        private void CancelBorder_MouseLeftButtonUp( object sender, MouseButtonEventArgs e)
        {
            CancelBorder.Opacity = 1.0;
        }

        private void CancelBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            CancelBorder.Opacity = 1.0;
        }
        #endregion

        #region ok button animation
        private void OKBorder_MouseLeftButtonDown( object sender,  MouseButtonEventArgs e)
        {
            OKBorder.Opacity = 0.5;
        }

        private void OKBorder_MouseLeftButtonUp( object sender, MouseButtonEventArgs e)
        {
            OKBorder.Opacity = 1.0;
        }

        private void OKBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            OKBorder.Opacity = 1.0;
        }
        #endregion
    }
}
