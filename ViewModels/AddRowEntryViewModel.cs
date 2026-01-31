using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    public class AddRowEntryViewModel : Screen
    {
        #region Private View Variables
        private decimal _price;
        #endregion

        public AddRowEntryViewModel()
        {
            Price = 0.00M;
        }

        #region Public View Variables
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }
        #endregion
    }
}
