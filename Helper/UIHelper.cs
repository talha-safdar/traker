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

        /// <summary>
        /// Inverse Radio Style:
        /// Full opacity: in use
        /// Half opacity: not in use
        /// </summary>
        public static void InverseRadioOptionChanged(int option, ObservableCollection<bool> optionsIsEnabled, ObservableCollection<double> optionsOpacity)
        {
            double fullOpacity = 1.0;
            double halfOpacity = 0.5;

            for (int i = 0; i < optionsIsEnabled.Count; i++)
            {
                if (i == option) // if clicked option set it to half opacity
                {
                    optionsOpacity[i] = fullOpacity;
                    optionsIsEnabled[i] = false;
                }
                else // if not clicked option set it to full opacity
                {
                    optionsOpacity[i] = halfOpacity;
                    optionsIsEnabled[i] = true;
                }
            }
        }

        /// <summary>
        /// Inverse Radio Style:
        /// Full opacity: in use
        /// Half opacity: not in use
        /// </summary>
        public static void InverseRadioOptionChangedOpacity(int option, ObservableCollection<double> optionsOpacity)
        {
            double fullOpacity = 1.0;
            double halfOpacity = 0.5;

            for (int i = 0; i < optionsOpacity.Count; i++)
            {
                if (i == option) // if clicked option set it to half opacity
                {
                    optionsOpacity[i] = fullOpacity;
                }
                else // if not clicked option set it to full opacity
                {
                    optionsOpacity[i] = halfOpacity;
                }
            }
        }

        /// <summary>
        /// Reset the opacity to 1.0
        /// in a list
        /// </summary>
        public static void SetOpacityFull(ObservableCollection<double> optionsOpacity)
        {
            for (int i = 0; i < optionsOpacity.Count; i++)
            {
                if (optionsOpacity[i] != 1.0)
                {
                    optionsOpacity[i] = 1.0;
                }
            }
        }

        /// <summary>
        /// Set the opacity to 0.5
        /// in a list
        /// </summary>
        public static void SetOpacityHalf(ObservableCollection<double> optionsOpacity)
        {
            for (int i = 0; i < optionsOpacity.Count; i++)
            {
                if (optionsOpacity[i] != 0.5)
                {
                    optionsOpacity[i] = 0.5;
                }
            }
        }

        /// <summary>
        /// Pass a selected item in a list
        /// to set to true.
        /// The rest of the items will be set to false
        /// </summary>
        public static void SetOptionTrue(int option, ObservableCollection<bool> optionsList)
        {
            for (int i = 0; i < optionsList.Count(); i++)
            {
                if (i == option)
                {
                    optionsList[option] = true;
                }
                else
                {
                    optionsList[i] = false;
                }
            }
        }
    }
}
