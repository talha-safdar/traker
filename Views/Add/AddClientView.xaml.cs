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

namespace Traker.Views.Add
{
    /// <summary>
    /// Interaction logic for AddRowEntryView.xaml
    /// </summary>
    public partial class AddClientView : UserControl
    {
        public AddClientView()
        {
            InitializeComponent();
        }

        #region submit button animation
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