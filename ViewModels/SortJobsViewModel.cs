using Caliburn.Micro;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Traker.Events;
using Traker.Events.DashboardVM;
using Traker.Helper;
using Traker.Services;
using Traker.States;

namespace Traker.ViewModels
{
    public class SortJobsViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private ObservableCollection<double> _opacityOrderStyle; // ascending, descending
        private ObservableCollection<double> _opacityOptions; // the rest of the buttons
        private ObservableCollection<bool> _orderStyleList;
        #endregion

        #region Private Field Variables
        private ObservableCollection<bool> _sortOptions;
        private int _selectedOption = -1;
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        private bool _isInitialized; // This stays 'true' because the VM is a Singleton
        private int _currentOption = -1; // it tracks the last option selected e.g. if 1, then click again 1 to disable it it knows
        #endregion

        public SortJobsViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _opacityOptions = new ObservableCollection<double>();
            _orderStyleList = new ObservableCollection<bool>();
            _opacityOrderStyle = new ObservableCollection<double>();

            _sortOptions = new ObservableCollection<bool>();
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
                 * 0 = client name
                 * 1 = job title
                 * 2 = job status
                 * 3 = job price
                 * 4 = due date
                 * 5 = created date
                 * 6 = client type
                 */
                _sortOptions = new ObservableCollection<bool>() { false, false, false, false, false, false, false };
                OpacityOptions = new ObservableCollection<double>() { _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity, _fullOpacity };

                OrderStyleList = new ObservableCollection<bool>() { false, false }; // 0=ascending, 1=descedning
                OpacityOrderStyle = new ObservableCollection<double>() { _halfOpacity, _halfOpacity }; // 0=ascending, 1=descedning
                _isInitialized = true; // Mark as done
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Initialise";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"SortJobsViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }        
        #endregion

        #region Public View Functions
        public async Task SortOption(int option)
        {
            try
            {
                _selectedOption = option;
                if (SortOptions[option] == true) // deselect
                {
                    for (int i = 0; i < SortOptions.Count; i++)
                    {
                        SortOptions[i] = false;
                    }
                    UIHelper.SetOpacityFull(_opacityOptions);
                    OrderStyleList[0] = false;
                    OrderStyleList[1] = false;
                    _opacityOrderStyle[0] = _halfOpacity;
                    _opacityOrderStyle[1] = _halfOpacity;
                    _selectedOption = -1;
                    _state.IsSortInUse = false;
                }
                else // select
                {
                    SortOptions[option] = true;
                    UIHelper.InverseRadioOptionChangedOpacity(option, _opacityOptions);
                    for(int i =0; i < SortOptions.Count(); i++) // disable the rest of the buttons in the list
                    {
                        if (i != option)
                        {
                            SortOptions[i] = false;
                        }
                    }

                    if (OrderStyleList[0] == false && OrderStyleList[1] == false) // if no order style is selected, default to ascending
                    {
                        OrderStyleList[0] = true;
                        _opacityOrderStyle[0] = _fullOpacity;
                    }
                    _state.IsSortInUse = true;
                }

                // do logics
                await SelectedOption(option);
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Sort Options";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"SortJobsViewModel: SortOption() FAIL\n\t{ex.Message}");
            }
        }

        public Task OrderStyle(int orderStyle)
        {
            try
            {
                /*
                 * 0 = ascending 
                 * 1 = descending
                 */
                if (_selectedOption != -1)
                {
                    if (orderStyle == 0)
                    {
                        OrderStyleList[1] = false;
                        _opacityOrderStyle[1] = _halfOpacity;
                        OrderStyleList[0] = true;
                        _opacityOrderStyle[0] = _fullOpacity;
                    }
                    else if (orderStyle == 1)
                    {
                        OrderStyleList[0] = false;
                        _opacityOrderStyle[0] = _halfOpacity;
                        OrderStyleList[1] = true;
                        _opacityOrderStyle[1] = _fullOpacity;
                    }
                    SelectedOption(_selectedOption);
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Order Style";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"SortJobsViewModel: OrderStyle() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Private Functions
        private Task DisbaleOptions()
        {
            try
            {
                for (int i = 0; i < SortOptions.Count; i++)
                {
                    SortOptions[i] = false;
                }

                for (int y = 0; y < OpacityOptions.Count; y++)
                {
                    OpacityOptions[y] = 0.5;
                }
                UIHelper.SetOpacityFull(_opacityOptions);
                OrderStyleList[0] = false;
                OrderStyleList[1] = false;
                OpacityOrderStyle[0] = _halfOpacity;
                OpacityOrderStyle[1] = _halfOpacity;
                _selectedOption = -1;
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Disable Sort";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"SortJobsViewModel: DisbaleOptions() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task SelectedOption(int option)
        {
            // OrderStyleList: 0=ascending, 1=descedning
            try
            {
                // if resetitng
                if (_currentOption == option)
                {
                    _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ResetSort });
                    _state.currentSortOption = string.Empty;
                }

                if (option == 0)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientNameAsc });
                        _state.currentSortOption = Names.ClientNameAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientNameDesc });
                        _state.currentSortOption = Names.ClientNameDesc;
                    }
                }
                else if (option == 1)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobTitleAsc });
                        _state.currentSortOption = Names.JobTitleAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobTitleDesc });
                        _state.currentSortOption = Names.JobTitleDesc;
                    }
                }
                else if (option == 2)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusAsc });
                        _state.currentSortOption = Names.JobStatusAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobStatusDesc });
                        _state.currentSortOption = Names.JobStatusDesc;
                    }
                }
                else if (option == 3)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobPriceAsc });
                        _state.currentSortOption = Names.JobPriceAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.JobPriceDesc });
                        _state.currentSortOption = Names.JobPriceDesc;
                    }
                }
                else if (option == 4)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.DueDateAsc });
                        _state.currentSortOption = Names.DueDateAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.DueDateDesc });
                        _state.currentSortOption = Names.DueDateDesc;
                    }
                }
                else if (option == 5)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.CreatedDateAsc });
                        _state.currentSortOption = Names.CreatedDateAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.CreatedDateDesc });
                        _state.currentSortOption = Names.CreatedDateDesc;
                    }
                }
                else if (option == 6)
                {
                    if (OrderStyleList[0] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientTypeAsc });
                        _state.currentSortOption = Names.ClientTypeAsc;
                    }
                    else if (OrderStyleList[1] == true)
                    {
                        _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = Names.ClientTypeDesc });
                        _state.currentSortOption = Names.ClientTypeDesc;
                    }
                }
                _currentOption = option; // track th last selection
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Option Selection";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"SortJobsViewModel: SelectedOption() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Public View variables
        public ObservableCollection<bool> SortOptions
        {
            get { return _sortOptions; }
            set
            {
                _sortOptions = value;
                NotifyOfPropertyChange(() => SortOptions);
            }
        }

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

        public ObservableCollection<bool> OrderStyleList
        {
            get { return _orderStyleList; }
            set
            {
                _orderStyleList = value;
                NotifyOfPropertyChange(() => OrderStyleList);
            }
        }
        #endregion
    }
}