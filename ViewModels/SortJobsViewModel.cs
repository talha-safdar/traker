using Caliburn.Micro;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Traker.Events;
using Traker.Events.DashboardVM;
using Traker.Helper;

namespace Traker.ViewModels
{
    public class SortJobsViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private ObservableCollection<double> _opacityOptions;
        private ObservableCollection<bool> _orderStyle;
        private ObservableCollection<double> _opacityOrderStyle;
        #endregion

        #region Private Field Variables
        private List<bool> _sortOptions;
        private int _selectedOption = -1;
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.7;

        private bool _isInitialized; // This stays 'true' because the VM is a Singleton
        #endregion

        public SortJobsViewModel(IEventAggregator events)
        {
            _events = events;
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            // If we've already done this once, stop here!
            if (_isInitialized == true)
            {
                return base.OnInitializedAsync(cancellationToken);
            }

            /*
             * 0 = client name
             * 1 = job title
             * 2 = job status
             * 3 = job price
             * 4 = due date
             * 5 = created date
             * 6 = client type
             */
            _sortOptions = new List<bool>() { false, false, false, false, false, false, false };
            _opacityOptions = new ObservableCollection<double>() { _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity };
            
            _orderStyle = new ObservableCollection<bool>() { false, false }; // 0=ascending, 1=descedning
            _opacityOrderStyle = new ObservableCollection<double>() { _halfOpacity, _halfOpacity }; // 0=ascending, 1=descedning
            _isInitialized = true; // Mark as done
            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task SortOption(int option)
        {
            _selectedOption = option;
            if (_sortOptions[option] == true) // deselect
            {
                for (int i = 0; i < _sortOptions.Count; i++)
                {
                    _sortOptions[i] = false;
                }
                UIHelper.SetOpacityFull(_opacityOptions);
                _orderStyle[0] = false;
                _orderStyle[1] = false;
                _opacityOrderStyle[0] = _halfOpacity;
                _opacityOrderStyle[1] = _halfOpacity;
                _selectedOption = -1;
            }
            else // select
            {
                _sortOptions[option] = true;
                UIHelper.InverseRadioOptionChangedOpacity(option, _opacityOptions);
                if (_orderStyle[0] == false && _orderStyle[1] == false) // if no order style is selected, default to ascending
                {
                    _orderStyle[0] = true;
                    _opacityOrderStyle[0] = _fullOpacity;
                }
            }

            // do logics
            await SelectedOption(option);
        }

        public Task OrderStyle(int orderStyle)
        {
            /*
             * 0 = ascending 
             * 1 = descending
             */
            if (_selectedOption != -1)
            {
                if (orderStyle == 0)
                {
                    _orderStyle[1] = false;
                    _opacityOrderStyle[1] = _halfOpacity;
                    _orderStyle[0] = true;
                    _opacityOrderStyle[0] = _fullOpacity;
                }
                else if (orderStyle == 1)
                {
                    _orderStyle[0] = false;
                    _opacityOrderStyle[0] = _halfOpacity;
                    _orderStyle[1] = true;
                    _opacityOrderStyle[1] = _fullOpacity;
                }
                SelectedOption(_selectedOption);
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Private Functions
        private Task SelectedOption(int option)
        {
            if (option == 0)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientNameAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientNameDesc });
                }
            }
            if (option == 1)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobTitleAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobTitleDesc });
                }
            }
            if (option == 2)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusDesc });
                }
            }
            if (option == 3)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobPriceAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobPriceDesc });
                }
            }
            if (option == 4)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.DueDateAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.DueDateDesc });
                }
            }
            if (option == 5)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.CreatedDateAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.CreatedDateDesc });
                }
            }
            if (option == 6)
            {
                if (_orderStyle[0] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientTypeAsc });
                }
                else if (_orderStyle[1] == true)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientTypeDesc });
                }
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Public View variables
        public ObservableCollection<double> OpacityOptions
        {
            get { return _opacityOptions; }
            set
            {
                _opacityOptions = value;
                NotifyOfPropertyChange(() => OpacityOptions);
            }
        }

        public ObservableCollection<double> OpacityOrderStyle
        {
            get { return _opacityOrderStyle; }
            set
            {
                _opacityOrderStyle = value;
                NotifyOfPropertyChange(() => OpacityOrderStyle);
            }
        }
        #endregion
    }
}