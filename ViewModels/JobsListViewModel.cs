using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Traker.Helper;
using Traker.Models;
using Traker.Models.Database;
using Traker.Services;
using Traker.States;
using Traker.ViewModels.Edit;

namespace Traker.ViewModels
{
    using Database;
    using System.Net;
    using System.Windows.Controls;
    using Traker.Events.DashboardVM;

    public class JobsListViewModel : Screen,
    #region Interfaces
        IHandle<RefreshDatabase>
    #endregion
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private ObservableCollection<DashboardModel> _jobsList;
        private string _businessName;
        private string _clientType;
        #endregion

        #region Public State Variable
        public DashboardModel SelectedJob; // data passed by EdiClientVM
        #endregion

        #region Private Class Field Variables
        private EditJobViewModel _editJobViewModel;
        private int _clientId;
        #endregion

        public JobsListViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _jobsList = new ObservableCollection<DashboardModel>();
            _businessName = string.Empty;
            _clientType = string.Empty;

            SelectedJob = new DashboardModel();

            _editJobViewModel = new EditJobViewModel(_events, _windowManager, _state);
            _clientId = -1;

            _events.SubscribeOnPublishedThread(this);
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                _clientId = SelectedJob.ClientId;
                _businessName = SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName;
                _clientType = SelectedJob.TypeIcon;

                await RefreshJobsList();

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
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            await base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion

        #region Public View FUnctions
        public async Task OpenContextMenu(DashboardModel selectedJob)
        {
            try
            {
                if (SelectedJob != null)
                {
                    selectedJob.ClientName = SelectedJob.ClientName;
                    selectedJob.ClientEmail = SelectedJob.ClientEmail;
                    selectedJob.ClientPhone = SelectedJob.ClientPhone;
                    selectedJob.Address = SelectedJob.Address;
                    selectedJob.City = SelectedJob.City;
                    selectedJob.Postcode = SelectedJob.Postcode;
                    selectedJob.Country = SelectedJob.Country;
                    selectedJob.CreatedDate = SelectedJob.CreatedDate;
                    selectedJob.ClientType = SelectedJob.ClientType; // pass the type from maun SelectedJob as it is not included in the jobs list cards

                    if (SelectedJob.ClientId == selectedJob.ClientId)
                    {
                        _state.JobContextMenuViewModel = new JobContextMenuViewModel(_events, _windowManager, _dataService, _state);
                        _state.JobContextMenuViewModel.SelectedJob = selectedJob; // pass job selected data
                        await _windowManager.ShowPopupAsync(_state.JobContextMenuViewModel, null, CustomWindow.SettingsForDialog(310, 335, false)); // vertical, horizontal
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Open Options Menu";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: OpenContextMenu() FAIL\n\t{ex.Message}");
            }
        }

        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    await TryCloseAsync();
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit List";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task EditJob(DashboardModel jobSelected)
        {
            try
            {
                _editJobViewModel = new EditJobViewModel(_events, _windowManager, _state);
                _editJobViewModel.SelectedJob = jobSelected; // pass selected row to EditJobViewModel
                _editJobViewModel.IsOpenFromEditClient = true;
                await _windowManager.ShowWindowAsync(_editJobViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Edit Job";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: EditJob() FAIL\n\t{ex.Message}");
            }
        }

        public void SelectJob(DashboardModel selectedJob)
        {
            try
            {
                if (selectedJob == null)
                {
                    return;
                }
                SelectedJob = selectedJob;
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Select Job";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: SelectJob() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                await TryCloseAsync();
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Exit";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // disable all context menus on click away
                    if (_state.JobContextMenuViewModel != null)
                    {
                        await _state.JobContextMenuViewModel.TryCloseAsync(false);
                        _state.JobContextMenuViewModel = null;
                    }
                }
                catch (Exception ex)
                {
                    await Execute.OnUIThreadAsync(async () =>
                    {
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                        {
                            _state.messageBoxVM.Symbol = 2;
                            _state.messageBoxVM.HeadMessage = "Close Window";
                            _state.messageBoxVM.Message = ex.Message;
                            _state.messageBoxVM.ButtonStyle = Names.OK;
                            await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                    });
                    Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OnMouseDownEvent() FAIL\n\t{ex.Message}");
                }
            });
        }
        #endregion

        #region Private Functions
        private async Task RefreshJobsList(bool showLoading = true)
        {
            try
            {
                _state.IsBusy = showLoading == true ? true : false;
                _state.LoadingMessage = showLoading == true ? "P L E A S E   W A I T " : string.Empty;
                await Task.Delay(50); // let rendering breath

                JobsList.Clear();

                var data = await SetJobsList(); // get new data

                JobsList = new ObservableCollection<DashboardModel>(data); // refresh the UI with new data
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Refresh Dashboard";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: RefreshDashboard() FAIL\n\t{ex.Message}");
            }
            finally
            {
                _state.IsBusy = false;
                _state.LoadingMessage = string.Empty;
            }
        }
        
        private async Task<List<DashboardModel>> SetJobsList()
        {
            try
            {
                List<JobsModel> jobsModel = await Database.FetchJobsByClientId(_clientId);

                List<DashboardModel> cards = new List<DashboardModel>();

                foreach (var job in jobsModel)
                {
                    cards.Add(new DashboardModel
                    {
                        ClientId = job.ClientId,
                        ClientType = SelectedJob.ClientType,
                        ClientName = SelectedJob.ClientName,
                        ClientEmail = SelectedJob.ClientEmail,
                        ClientPhone = SelectedJob.ClientPhone,
                        Address = SelectedJob.Address,
                        City = SelectedJob.City,
                        Postcode = SelectedJob.Postcode,
                        Country = SelectedJob.Country,
                        CreatedDate = SelectedJob.CreatedDate,

                        JobId = job.JobId,
                        JobTitle = job.Title,
                        JobDescription = job.Description,
                        Price = job.FinalPrice,
                        AmountReceived = job.AmountReceived,
                        JobStatus = job.Status.ToString(),
                        StartDate = job.StartDate,
                        DueDate = job.DueDate,

                        HasInvoice = await Database.CheckIfJobHasInvoice(job.JobId),
                        InvoiceStatus = await Database.GetInvoiceStatusByJobId(job.JobId) ?? Names.NotInvoiced
                    });
                }
                return cards;
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(async () =>
                {
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 2;
                        _state.messageBoxVM.HeadMessage = "Setup Dashboard";
                        _state.messageBoxVM.Message = ex.Message;
                        _state.messageBoxVM.ButtonStyle = Names.OK;
                        await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                });
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: SetupDashboardData() FAIL\n\t{ex.Message}");
                return new List<DashboardModel>();
            }
        }
        #endregion

        #region Event Handlers
        public async Task HandleAsync(RefreshDatabase message, CancellationToken cancellationToken)
        {
            await RefreshJobsList();
        }
        #endregion

        #region Public View Variables 
        public ObservableCollection<DashboardModel> JobsList
        {
            get { return _jobsList; }
            set
            {
                _jobsList = value;
                NotifyOfPropertyChange(() => JobsList);
            }
        }

        public string BusinessName
        {
            get { return _businessName; }
            set
            {
                _businessName = value;
                NotifyOfPropertyChange(() => BusinessName);
            }
        }

        public string ClientType
        {
            get { return _clientType; }
            set
            {
                _clientType = value;
                NotifyOfPropertyChange(() => ClientType);
            }
        }
        #endregion
    }
}