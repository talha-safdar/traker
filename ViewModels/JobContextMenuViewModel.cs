using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.ViewModels
{
    using Database;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows;
    using Traker.Data;
    using Traker.Events;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Models.Database;
    using Traker.Services;
    using Traker.States;
    using Traker.ViewModels.Edit;

    public class JobContextMenuViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private ObservableCollection<double> _activeDoneOpacity;
        private ObservableCollection<bool> _activeDoneEnable;
        private DashboardModel _selectedJob; // used in the XAML
        #endregion

        #region Private Field Vairables
        private CreateInvoiceViewModel _createInvoice;
        private EditInvoiceViewModel _editInvoiceViewModel;
        #endregion


        public JobContextMenuViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _selectedJob = new DashboardModel();
            _activeDoneOpacity = new ObservableCollection<double>();
            _activeDoneEnable = new ObservableCollection<bool>();

            _createInvoice = new CreateInvoiceViewModel(_events, _windowManager, _dataService, _state);
            _editInvoiceViewModel = new EditInvoiceViewModel(_events, _windowManager, _dataService, _state);
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                // 0=active, 1=done
                ActiveDoneEnable = new ObservableCollection<bool>() { false, false };
                ActiveDoneOpacity = new ObservableCollection<double>() { 0.5, 0.5 };

                if (SelectedJob.JobStatus == Names.Active)
                {
                    UIHelper.RadioOptionChanged(0, ActiveDoneEnable, ActiveDoneOpacity);
                }
                else if (SelectedJob.JobStatus == Names.Done)
                {
                    UIHelper.RadioOptionChanged(1, ActiveDoneEnable, ActiveDoneOpacity);
                }
                else if (SelectedJob.JobStatus == Names.New)
                {
                    ActiveDoneEnable[0] = true;
                    ActiveDoneEnable[1] = true;
                    ActiveDoneOpacity[0] = 1.0;
                    ActiveDoneOpacity[1] = 1.0;
                }
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
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            return base.OnInitializedAsync(cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task SetStatus(string status)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await Database.SetJobStatus(status, SelectedJob.ClientId, SelectedJob.JobId);
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
                    await _dataService.RefreshDatabase();
                    await TryCloseAsync();
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Set Status";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: SetStatus() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OpenFolder()
        {
            try
            {
                await Task.Run(async () =>
                {
                    // get list of jobs under the client ID
                    List<JobsModel> jobDetails = new List<JobsModel>();
                    jobDetails = _dataService.Jobs.Where(j => j.ClientId == SelectedJob.ClientId).ToList();

                    await FileStore.LocateJobFolder(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName, SelectedJob.JobTitle);
                    await TryCloseAsync();
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Open Folder";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: OpenFolder() FAIL\n\t{ex.Message}");
            }
        }

        public async Task EditInvoice()
        {
            try
            {
                await TryCloseAsync();
                _editInvoiceViewModel = new EditInvoiceViewModel(_events, _windowManager, _dataService, _state);
                _editInvoiceViewModel.SelectedJob = SelectedJob;
                await _windowManager.ShowDialogAsync(_editInvoiceViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Edit Invoice";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: EditInvoice() FAIL\n\t{ex.Message}");
            }
        }

        public async Task CreateInvoice()
        {
            try
            {
                await TryCloseAsync();
                _createInvoice = new CreateInvoiceViewModel(_events, _windowManager, _dataService, _state);
                _createInvoice.SelectedJob = SelectedJob;
                _windowManager.ShowWindowAsync(_createInvoice, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Create Invoice";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: CreateInvoice() FAIL\n\t{ex.Message}");
            }
        }

        public async Task EditClient()
        {
            try
            {
                await TryCloseAsync();
                await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = "EditClient" });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: EditClient() FAIL\n\t{ex.Message}");
            }
        }

        public async Task DeleteJob()
        {
            try
            {
                await TryCloseAsync();

                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 0;
                    _state.messageBoxVM.HeadMessage = "Delete Job";
                    _state.messageBoxVM.Message = Names.DeleteJobConfirmation;
                    _state.messageBoxVM.ButtonStyle = Names.NoYes;
                    await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }

                // if clicked yes
                if (_state.messageBoxVM.Output == true)
                {
                    await Task.Run(async () =>
                    {
                        // delete job folder (if only one job then delete whole client folder)
                        // it deletes the current job folder then it checks if it was the last one if true, then delete client folder
                        if (await FileStore.DeleteJobFolder(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName, SelectedJob.JobTitle) == true)
                        {
                            // delete entire client folder too
                            await FileStore.DeleteClientFolder(SelectedJob.ClientId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName);

                            // delete client from database
                            await Database.DeleteClient(SelectedJob.ClientId);
                        }
                        else if (await FileStore.DeleteJobFolder(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName, SelectedJob.JobTitle) == false)
                        {
                            // delete job from database
                            await Database.DeleteJob(SelectedJob.JobId);
                        }
                        await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
                    });
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Delete Job";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: DeleteJob() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public View Variables
        public ObservableCollection<double> ActiveDoneOpacity
        {
            get { return _activeDoneOpacity; }
            set
            {
                _activeDoneOpacity = value;
                NotifyOfPropertyChange(() => ActiveDoneOpacity);
            }
        }

        public ObservableCollection<bool> ActiveDoneEnable
        {
            get { return _activeDoneEnable; }
            set
            {
                _activeDoneEnable = value;
                NotifyOfPropertyChange(() => ActiveDoneEnable);
            }
        }

        public DashboardModel SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                NotifyOfPropertyChange(() => SelectedJob);
            }
        }
        #endregion
    }
}