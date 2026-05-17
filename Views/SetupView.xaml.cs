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
    /// Interaction logic for SetupView.xaml
    /// </summary>
    public partial class SetupView : UserControl
    {
        public SetupView()
        {
            InitializeComponent();
        }

        #region submit button user animation
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

        #region submit button business animation
        // individual
        private void SubmitBorder2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder2.Opacity = 0.7;
        }

        private void SubmitBorder2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder2.Opacity = 1.0;
        }

        private void SubmitBorder2_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitBorder2.Opacity = 1.0;
        }

        // company
        private void SubmitBorder22_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder22.Opacity = 0.7;
        }

        private void SubmitBorder22_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder22.Opacity = 1.0;
        }

        private void SubmitBorder22_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitBorder22.Opacity = 1.0;
        }
        #endregion

        #region submit button bank animation
        private void SubmitBorder3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder3.Opacity = 0.7;
        }

        private void SubmitBorder3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SubmitBorder3.Opacity = 1.0;
        }

        private void SubmitBorder3_MouseLeave(object sender, MouseEventArgs e)
        {
            SubmitBorder3.Opacity = 1.0;
        }
        #endregion
    }
}
