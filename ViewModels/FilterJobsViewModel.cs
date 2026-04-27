using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Events.DashboardVM;
using Traker.Helper;

namespace Traker.ViewModels
{
    public class FilterJobsViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        #endregion

        #region Private View Variables
        private ObservableCollection<double> _opacityStatus;
        private ObservableCollection<bool> _clientType;
        private ObservableCollection<double> _opacityClientType;
        #endregion

        #region Private Field Variables
        private List<bool> _optionsStatus;
        private int _selectedOption = -1;
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;

        private bool _isInitialized; // This stays 'true' because the VM is a Singleton
        #endregion

        public FilterJobsViewModel(IEventAggregator events)
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
             * 0 = new
             * 1 = active
             * 2 = done
             * 3 = invoiced
             * 4 = paid // to be added?
             */
            _optionsStatus = new List<bool>() { false, false, false, false };
            _opacityStatus = new ObservableCollection<double>() { _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity };

            _clientType = new ObservableCollection<bool>() { false, false }; // 0=individual, 1=company
            _opacityClientType = new ObservableCollection<double>() { _fullOpacity, _fullOpacity };
            _isInitialized = true; // Mark as done
            return base.OnInitializedAsync(cancellationToken);
        }

        #region Public View Functions
        public async Task FilterStatusOption(int option)
        {
            _selectedOption = option;
            if (_optionsStatus[option] == true) // deselect
            {
                for (int i = 0; i < _optionsStatus.Count; i++)
                {
                    _optionsStatus[i] = false;
                }
                UIHelper.SetOpacityFull(_opacityStatus);
                await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.AllJobStatus });
            }
            else // select
            {
                _optionsStatus[option] = true;
                UIHelper.InverseRadioOptionChangedOpacity(option, _opacityStatus);
                await SelectedOption(option);
            }
        }

        public async Task FilterClientType(int option)
        {
            if (_clientType[option] == true) // deselect
            {
                for (int i = 0; i < _clientType.Count; i++)
                {
                    _clientType[i] = false;
                }
                UIHelper.SetOpacityFull(_opacityClientType);
                await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.UnfilterClientType });
            }
            else // select
            {
                UIHelper.SetOptionTrue(option, _clientType);
                UIHelper.InverseRadioOptionChangedOpacity(option, _opacityClientType);
                if (option == 0) // individual
                {
                    await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.FilterIndividual });
                }
                else if (option == 1) // company
                {
                    await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.FilterComapny });
                }
            }
        }
        #endregion

        #region Private Functions
        private Task SelectedOption(int option)
        {
            if (option == 0) // new jobs
            {
                _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusNew });
            }
            else if (option == 1) // active jobs
            {
                _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusActive });
            }
            else if (option == 2) // done jobs
            {
                _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusDone });
            }
            else if (option == 3) // invoiced jobs
            {
                _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusInvoiced });
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Public View Variables
        public ObservableCollection<double> OpacityStatus
        {
            get { return _opacityStatus; }
            set
            {
                _opacityStatus = value;
                NotifyOfPropertyChange(() => OpacityStatus);
            }
        }

        public ObservableCollection<bool> ClientType
        {
            get { return _clientType; }
            set
            {
                _clientType = value;
                NotifyOfPropertyChange(() => ClientType);
            }
        }

        public ObservableCollection<double> OpacityClientType
        {
            get { return _opacityClientType; }
            set
            {
                _opacityClientType = value;
                NotifyOfPropertyChange(() => OpacityClientType);
            }
        }
        #endregion
    }
}