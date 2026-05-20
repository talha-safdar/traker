using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Traker.Events;
using Traker.Events.DashboardVM;
using Traker.Events.ShellVM;
using Traker.Helper;
using Traker.Services;
using Traker.States;

namespace Traker.ViewModels
{
    public class FilterJobsViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
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

        private bool _isInitialized; // This stays 'true' because the VM is a Singleton
        #endregion

        public FilterJobsViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _optionsStatus = new List<bool>();
            _opacityStatus = new ObservableCollection<double>();
            _clientType = new ObservableCollection<bool>();
            _opacityClientType = new ObservableCollection<double>();
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // If already done this once, stop here!
                if (_isInitialized == true)
                {
                    return base.OnInitializedAsync(cancellationToken);
                }

                /*
                 * 0 = new
                 * 1 = active
                 * 2 = done
                 * 3 = invoiced
                 * 4 = overdue
                 * 5 = paid
                 */
                _optionsStatus = new List<bool>() { false, false, false, false, false, false };
                _opacityStatus = new ObservableCollection<double>() { _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity };

                _clientType = new ObservableCollection<bool>() { false, false }; // 0=individual, 1=company
                _opacityClientType = new ObservableCollection<double>() { _fullOpacity, _fullOpacity };
                _isInitialized = true; // Mark as done
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Initialise Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FilterJobsViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnActivatedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // if dashbaord refreshed and notified to reset
                if (_state.IsFilterToClear == true)
                {
                    DisbaleOptions();
                    _state.IsFilterToClear = false;
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "OnActivatedAsync";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FilterJobsViewModel: OnActivatedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnActivatedAsync(cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task FilterStatusOption(int option)
        {
            try
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
                    UIHelper.SetOptionTrue(option, _optionsStatus);
                    //_optionsStatus[option] = true;
                    UIHelper.InverseRadioOptionChangedOpacity(option, _opacityStatus);
                    await SelectedOption(option);
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Filter Status Option";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FilterJobsViewModel: FilterStatusOption() FAIL\n\t{ex.Message}");
            }
        }

        public async Task FilterClientType(int option)
        {
            try
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
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Filter Client Type";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FilterJobsViewModel: FilterClientType() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        private Task DisbaleOptions()
        {
            try
            {
                for (int i = 0; i < _optionsStatus.Count; i++)
                {
                    _optionsStatus[i] = false;
                }
                UIHelper.SetOpacityFull(_opacityStatus);

                for (int i = 0; i < _clientType.Count; i++)
                {
                    _clientType[i] = false;
                }
                UIHelper.SetOpacityFull(_opacityClientType);
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Disable Filter";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FilterJobsViewModel: DisbaleOptions() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task SelectedOption(int option)
        {
            try
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
                else if (option == 4) // overdue jobs
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusOverdue });
                }
                else if (option == 5) // paid jobs
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusPaid });
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Filter Selected";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"FilterJobsViewModel: SelectedOption() FAIL\n\t{ex.Message}");
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