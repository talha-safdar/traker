using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Helper
{
    /// <summary>
    /// Anything that deals with UI logics should be
    /// placed in here for simplicity and reuse
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// For VMs with multiple sections in the XAML
        /// that needs to be toggled in sequence or based
        /// on conditions.
        /// 
        /// pass list of items and the number of the view
        /// to be shown
        /// 
        /// Usefulness: it saved space on making new VMs everytime
        /// </summary>
        /// <returns></returns>
        public static Task SwitchBetweenViews(ObservableCollection<bool> listItems, int showItem)
        {
            for (int i = 0; i < listItems.Count(); i++)
            {
                if (showItem == i)
                {
                    listItems[i] = true;
                }
                else
                {
                    listItems[i] = false;
                }
            }

            return Task.CompletedTask;
        }
    }
}
