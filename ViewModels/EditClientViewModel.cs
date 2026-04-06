using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    class EditClientViewModel : Screen
    {
        public EditClientViewModel()
        {

        }
        public async Task Exit()
        {
            await TryCloseAsync();
        }
    }
}
