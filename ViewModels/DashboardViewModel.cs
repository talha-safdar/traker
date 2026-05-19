using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Traker.ViewModels
{
    using Database;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using Traker.Data;
    using Traker.Events;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Models;
    using Traker.Services;
    using Traker.States;
    using Traker.ViewModels.Add;
    using Traker.ViewModels.Edit;
    using Traker.ViewModels.User;

    public class DashboardViewModel : Screen, IHandle<RefreshDatabase>, IHandle<DashboardVMEvents>
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        
        #endregion
        public DataService DataService { get; }

        #region Public View Variables
        public List<string> JobStatusEdit { get; set; } = new List<string> { "New", "Active", "Done" };
        public List<string> InvoiceStatusEdit { get; set; } = new List<string> { "Created", "Sent", "Paid", "Overdue" };
        public AppState State { set; get; }
        #endregion

        #region Private View Variables
        private string _receivedAmount; // money received value shown total
        private string _outstandingAmount; // money outstanding value shown total
        private string _overdueAmount; // money overdue value shown total
        private string _grossAmount; // money price = the final amount for a job (not invoiced)
        private string _newJobsCount;
        private string _activeJobsCount;
        private string _doneJobsCount;
        private string _invoicedJobsCount;
        private int _totalJobsCount;
        private ObservableCollection<DashboardModel> _dashboardData; // listo of data shown on the data grid
        public DashboardModel _selectedJob; // selected data row automatically filled on click
        private string _outstandingStatusBorder; // grey=normal, pink=overdue

        /*
         * for buttons:
         * edit client, edit job, add job, filter, sort
         */
        private bool _enableBtns;
        private double _opacityBtns;
        #endregion

        #region Private Field Variables
        // for filter purpose
        private ObservableCollection<DashboardModel> _dashboardDataBackup; // backup when using filter mode
        private ObservableCollection<DashboardModel> _dashboardDataStatusFiltered; // current status filtered list status
        private ObservableCollection<DashboardModel> _dashboardDataTypeFiltered; // current type filtered list status
        private bool _isFilterJobStatusOn = false; // to flag wether the job status filter is on
        private bool _isFilterClientTypeOn = false; // to flag wether the client type filter is on
        private bool _isFilterClientTypeIndividual = false;
        private bool _isFilterClientTypeCompany = false;

        private CancellationTokenSource _cts; // for checking overude
        private ManualResetEventSlim _pauseEvent; // true = start open

        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        #endregion

        public DashboardViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            DataService = dataService;
            State = state;

            // private view variable
            _receivedAmount = "0";
            _outstandingAmount = "0";
            _overdueAmount = "0";
            _grossAmount = "0";
            _newJobsCount = "0";
            _activeJobsCount = "0";
            _doneJobsCount = "0";
            _invoicedJobsCount = "0";
            _outstandingStatusBorder = string.Empty;
            _dashboardData = new ObservableCollection<DashboardModel>();
            _selectedJob = new DashboardModel();

            // private variables
            State.EditJobViewModel = new EditJobViewModel(_events, _windowManager, State);
            State.AddJobViewModel = new AddJobViewModel(_events, _windowManager, State);
            State.SortJobsViewModel = new SortJobsViewModel(_events, _windowManager, DataService, State);
            State.FilterJobsViewModel = new FilterJobsViewModel(_events, _windowManager, DataService, State);
            State.JobContextMenuViewModel = new JobContextMenuViewModel(_events, _windowManager, DataService, State);
            State.AddClientViewModel = new AddClientViewModel(_events, _windowManager, DataService, State);
            State.EditClientViewModel = new EditClientViewModel(_events, _windowManager, DataService, State);
            State.UserContextMenuViewModel = new UserContextMenuViewModel(_events, _windowManager, DataService, State);
            State.EditInvoiceViewModel = new EditInvoiceViewModel(_events, _windowManager, DataService, State);
            _dashboardDataBackup = new ObservableCollection<DashboardModel>();
            _dashboardDataStatusFiltered = new ObservableCollection<DashboardModel>();
            _dashboardDataTypeFiltered = new ObservableCollection<DashboardModel>();
            _cts = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true); // true = start open

            _events.SubscribeOnPublishedThread(this);
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                EnableBtns = false;
                OpacityBtns = _halfOpacity;

                await SetupDashboardData();

                //return base.OnInitializedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Initialise Form";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            _cts?.Cancel();
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion

        #region Public View Functions
        public async Task OpenFilterContextMenu(FrameworkElement anchorElement)
        {
            try
            {
                if (anchorElement != null)
                {
                    var window = Window.GetWindow(anchorElement);

                    // 1. Get the absolute position of the button on the screen
                    Point locationFromScreen = anchorElement.PointToScreen(new Point(anchorElement.ActualWidth, 0));
                    Point windowScreenPos = window.PointToScreen(new Point(0, 0));

                    // 2. Adjust for DPI (High-res screens) - Very important for proper alignment
                    var source = PresentationSource.FromVisual(anchorElement);
                    if (source == null)
                    {
                        return; // Element isn't rendered yet, math will be 0,0
                    }

                    double dpiY = source.CompositionTarget.TransformToDevice.M22;
                    double dpiX = source.CompositionTarget.TransformToDevice.M11;

                    // 3. Calculate "Above": 
                    double popupTop = (locationFromScreen.Y / dpiY) - 12.5; // positive=down, negative=up
                    double popupLeft = (locationFromScreen.X / dpiX) - 295; // psotiive=right, negative=left

                    await _windowManager.ShowPopupAsync(IoC.Get<FilterJobsViewModel>(), null, CustomWindow.SettingsForDialog(295, 160, true, popupTop, popupLeft)); // vertical, horizontal
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Open FIlter Menu";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OpenFilterContextMenu() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OpenSortContextMenu(FrameworkElement anchorElement)
        {
            try
            {
                if (anchorElement != null)
                {
                    var window = Window.GetWindow(anchorElement);

                    // 1. Get the absolute position of the button on the screen
                    Point locationFromScreen = anchorElement.PointToScreen(new Point(anchorElement.ActualWidth, 0));
                    Point windowScreenPos = window.PointToScreen(new Point(0, 0));

                    // 2. Adjust for DPI (High-res screens) - Very important for proper alignment
                    var source = PresentationSource.FromVisual(anchorElement);
                    if (source == null)
                    {
                        return; // Element isn't rendered yet, math will be 0,0
                    }

                    double dpiY = source.CompositionTarget.TransformToDevice.M22;
                    double dpiX = source.CompositionTarget.TransformToDevice.M11;

                    // 3. Calculate "Above": 
                    double popupTop = (locationFromScreen.Y / dpiY) + 4; // positive=down, negative=up
                    double popupLeft = (locationFromScreen.X / dpiX) - 295; // psotiive=right, negative=left

                    //_sortJobsViewModel = new SortJobsViewModel();
                    // for singleton use use IoC
                    await _windowManager.ShowPopupAsync(IoC.Get<SortJobsViewModel>(), null, CustomWindow.SettingsForDialog(295, 205, true, popupTop, popupLeft)); // vertical, horizontal
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Open Sort Menu";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OpenSortContextMenu() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OpenUserContenxtMenu(FrameworkElement anchorElement)
        {
            try
            {
                if (anchorElement != null)
                {
                    // 1. Get the absolute position of the button on the screen
                    Point locationFromScreen = anchorElement.PointToScreen(new Point(0, 0));

                    // 2. Adjust for DPI (High-res screens) - Very important for proper alignment
                    var source = PresentationSource.FromVisual(anchorElement);
                    double dpiY = source.CompositionTarget.TransformToDevice.M22;
                    double dpiX = source.CompositionTarget.TransformToDevice.M11;

                    // 3. Calculate "Above": 
                    // Top = ButtonTop - PopupHeight
                    // Left = ButtonLeft (aligned to the left of the button)
                    double popupTop = (locationFromScreen.Y / dpiY) - 260;
                    double popupLeft = (locationFromScreen.X / dpiX) - 20;

                    State.UserContextMenuViewModel = new UserContextMenuViewModel(_events, _windowManager, DataService, State);
                    await _windowManager.ShowPopupAsync(State.UserContextMenuViewModel, null, CustomWindow.SettingsForDialog(310, 335, true, popupTop, popupLeft)); // vertical, horizontal
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Open User Menu";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OpenUserContenxtMenu() FAIL\n\t{ex.Message}");
            }
        }

        public Task SelectJob(DashboardModel selectedJob)
        {
            try
            {
                if (selectedJob != null)
                {
                    SelectedJob = selectedJob;
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Exit Form";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: SelectJob() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        public async Task OpenContextMenu(DashboardModel selectedJob)
        {
            try
            {
                if (SelectedJob != null)
                {
                    if (SelectedJob.ClientId == selectedJob.ClientId)
                    {
                        State.JobContextMenuViewModel = new JobContextMenuViewModel(_events, _windowManager, DataService, State);
                        State.JobContextMenuViewModel.SelectedJob = selectedJob; // pass job selected data
                        await _windowManager.ShowPopupAsync(State.JobContextMenuViewModel, null, CustomWindow.SettingsForDialog(310, 335, false)); // vertical, horizontal
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Open Options Menu";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OpenContextMenu() FAIL\n\t{ex.Message}");
            }
        }

        public async Task EditJob(DashboardModel jobSelected)
        {
            try
            {
                 // if already invoiced
                 // double-click
                if (jobSelected != null && DataService.Invoices.Any(i => i.JobId == jobSelected.JobId && i.IsDeleted == false) == true)
                {
                    State.EditInvoiceViewModel = new EditInvoiceViewModel(_events, _windowManager, DataService, State);
                    State.EditInvoiceViewModel.SelectedJob = jobSelected;
                    await _windowManager.ShowDialogAsync(State.EditInvoiceViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
                // icon click
                else if (SelectedJob != null && DataService.Invoices.Any(i => i.JobId == SelectedJob.JobId && i.IsDeleted == false) == true)
                {
                    State.EditInvoiceViewModel = new EditInvoiceViewModel(_events, _windowManager, DataService, State);
                    State.EditInvoiceViewModel.SelectedJob = SelectedJob;
                    await _windowManager.ShowDialogAsync(State.EditInvoiceViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
                else
                {
                    State.EditJobViewModel = new EditJobViewModel(_events, _windowManager, State);

                    if (jobSelected != null)
                    {
                        State.EditJobViewModel.SelectedJob = jobSelected; // pass selected   row to EditJobViewModel
                    }
                    else
                    {
                        State.EditJobViewModel.SelectedJob = SelectedJob!;
                    }
                    await _windowManager.ShowDialogAsync(State.EditJobViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Exit Form";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: EditJob() FAIL\n\t{ex.Message}");
            }
        }

        public async Task EditClient()
        {
            try
            {
                State.EditClientViewModel = new EditClientViewModel(_events, _windowManager, DataService, State);
                State.EditClientViewModel.SelectedJob = SelectedJob; // pass selected row to EditClientViewModel
                await _windowManager.ShowDialogAsync(State.EditClientViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Edit Client";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: EditClient() FAIL\n\t{ex.Message}");
            }
        }

        public async Task DeleteJob()
        {
            try
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 0;
                    State.messageBoxVM.HeadMessage = "Delete Job";
                    State.messageBoxVM.Message = Names.DeleteJobConfirmation;
                    State.messageBoxVM.ButtonStyle = Names.NoYes;
                    await _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }

                // if clicked yes
                if (State.messageBoxVM.Output == true)
                {
                    await Task.Run(async () =>
                    {
                        // delete job folder (if only one job then delete whole client folder)
                        // it deletes the current job folder then it checks if it was the last one if true, then delete client folder
                        if (await FileStore.DeleteJobFolder(SelectedJob!.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName, SelectedJob.JobTitle) == true)
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
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Delete Job";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"JobContextMenuViewModel: DeleteJob() FAIL\n\t{ex.Message}");
            }
        }

        public async Task AddClient()
        {
            try
            {
                State.AddClientViewModel = new AddClientViewModel(_events, _windowManager, DataService, State);
                await _windowManager.ShowDialogAsync(State.AddClientViewModel, null, CustomWindow.SettingsForDialog(790, 600, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Add Client";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: AddClient() FAIL\n\t{ex.Message}");
            }
        }

        public async Task AddJob()
        {
            try
            {
                State.AddJobViewModel = new AddJobViewModel(_events, _windowManager, State);
                State.AddJobViewModel.dashboardData = _dashboardData; // pass dashboard data to AddJob
                await _windowManager.ShowDialogAsync(State.AddJobViewModel, null, CustomWindow.SettingsForDialog(700, 550, false));
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Add Job";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: AddJob() FAIL\n\t{ex.Message}");
            }
        }

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // disable all context menus on click away
                    if (State.JobContextMenuViewModel != null)
                    {
                        await State.JobContextMenuViewModel.TryCloseAsync(false);
                        State.JobContextMenuViewModel = null;
                    }
                    if (State.UserContextMenuViewModel != null)
                    {
                        await State.UserContextMenuViewModel.TryCloseAsync(false);
                        State.UserContextMenuViewModel = null;
                    }
                    if (IoC.Get<FilterJobsViewModel>().IsActive == true)
                    {
                        await IoC.Get<FilterJobsViewModel>().TryCloseAsync(false);
                    }
                    if (IoC.Get<SortJobsViewModel>().IsActive == true)
                    {
                        await IoC.Get<SortJobsViewModel>().TryCloseAsync(false);
                    }
                }
                catch (Exception ex)
                {
                    await Execute.OnUIThreadAsync(async() =>
                    {
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                        {
                            State.messageBoxVM.Symbol = 2;
                            State.messageBoxVM.HeadMessage = "Close Window";
                            State.messageBoxVM.Message = ex.Message;
                            State.messageBoxVM.ButtonStyle = Names.OK;
                            await _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                    });
                    Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: OnMouseDownEvent() FAIL\n\t{ex.Message}");
                }
            });
        }
        #endregion

        #region Private Functions
        private Task SetupDashboardData()
        {
            try
            {
                PauseLoop();

                // row data
                _dashboardData.Clear();
                int index = 0;
                foreach (var job in DataService.Jobs)
                {
                    var client = DataService.Clients.First(c => c.ClientId == job.ClientId);

                    DashboardModel dashboardEntry = new DashboardModel
                    {
                        // client
                        ClientId = client.ClientId,
                        ClientType = client.Type,
                        TypeIcon = (client.Type == "Individual") ? "/Resources/Media/Images/Icons/Lucide/user-round.svg" : "/Resources/Media/Images/Icons/Lucide/building.svg",
                        ClientName = client.FullName,
                        ClientEmail = client.Email,
                        ClientPhone = client.PhoneNumber,
                        CompanyName = client.CompanyName,
                        Address = client.BillingAddress,
                        City = client.City,
                        Postcode = client.Postcode,
                        Country = client.Country,
                        CreatedDate = client.CreatedDate,
                        IsActive = client.IsActive,

                        // job
                        JobId = job.JobId,
                        JobTitle = job.Title,
                        JobDescription = job.Description,
                        Price = job.FinalPrice, // use toString("C") only for ui side
                        AmountReceived = job.AmountReceived,
                        JobStatus = DataService.Invoices.Where(i => i.JobId == job.JobId && string.IsNullOrEmpty(i.Status) == false).Select(i => i.Status).FirstOrDefault() ?? job.Status.ToString(),
                        StartDate = job.StartDate,
                        DueDate = job.DueDate,

                        // invoice
                        HasInvoice = DataService.Invoices.Any(i => i.JobId == job.JobId && i.IsDeleted == false),
                        InvoiceStatus = DataService.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.Status).FirstOrDefault() ?? "Not invoiced", // if left side not null return that else right side
                        InvoiceDueDate = DataService.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.DueDate).FirstOrDefault(),
                        PaidDate = DataService.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.PaidDate).FirstOrDefault(),
                    };
                    DashboardData.Add(dashboardEntry);
                    index++;
                }

                Task.Run(() =>
                {
                    _dashboardDataBackup = DashboardData; // backup for filtering
                    _dashboardDataStatusFiltered = DashboardData;
                    _dashboardDataTypeFiltered = DashboardData;
                });

                // disable sort and filter
                State.IsSortToClear = true;
                State.IsFilterToClear = true;

                NewJobsCount = DataService.Jobs.Where(j => j.Status == Names.New).Count().ToString(); // new jobs count
                DoneJobsCount = DataService.Jobs.Where(j => j.Status == Names.Done).Count().ToString(); // done jobs count
                ActiveJobsCount = DataService.Jobs.Where(j => j.Status == Names.Active).Count().ToString(); // active jobs count
                InvoicedJobsCount = DataService.Jobs.Where(j => j.Status == Names.Invoiced && DataService.Invoices.Any(i => i.JobId == j.JobId && i.Status != Names.Paid)).Count().ToString(); // invoiced jobs count
                GrossAmount = DataService.Jobs.Sum(gross => gross.FinalPrice).ToString("C"); // gross amount
                ReceivedAmount = DataService.Jobs
                    .Join(DataService.Invoices,
                    j => j.JobId,
                    i => i.JobId,
                    (j, i) => new { j, i }).Where(combined => combined.i.Status == Names.Paid)
                    .Sum(combined => combined.j.FinalPrice).ToString("C");

                OutstandingAmount = DataService.Jobs
                    .Where(
                        j => j.Status == Names.Done || // job = done
                        (j.Status == Names.Invoiced && DataService.Invoices.Any(i => i.JobId == j.JobId && i.Status != Names.Paid)) || // job = invoiced
                        DataService.Invoices.Any(i => i.JobId == j.JobId && i.DueDate < DateOnly.FromDateTime(DateTime.Now) && i.Status == Names.Overdue) // invoice = overdue
                    ).Sum(j => j.FinalPrice).ToString("C");

                OverdueAmount = DataService.Jobs
                                    .Where(j => j.Status == Names.Invoiced)
                                    .Join(DataService.Invoices,
                                          job => job.JobId,
                                          inv => inv.JobId, (job, inv) => new { job.FinalPrice, inv.DueDate, inv.Status })
                                    .Where(x => x.DueDate < DateOnly.FromDateTime(DateTime.Now) && x.Status != Names.Paid)
                                    .Sum(x => x.FinalPrice).ToString("C"); // overdue

                TotalJobsCount = DataService.Jobs.Count();

                // if job count is zero then disable edit buttons and add jobs
                if (TotalJobsCount == 0)
                {
                    EnableBtns = false;
                    OpacityBtns = _halfOpacity;
                    SelectedJob = null;
                }
                else if (TotalJobsCount > 0)
                {
                    EnableBtns = true;
                    OpacityBtns = _fullOpacity;
                }

                // if there is any overdue
                if (decimal.Parse(OverdueAmount, NumberStyles.Currency, CultureInfo.CurrentCulture) > 0.0m)
                {
                    OutstandingStatusBorder = "Overdue";
                }
                else
                {
                    OutstandingStatusBorder = string.Empty;
                }

                StartLoop();
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Setup Dashboard";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: SetupDashboardData() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        
        private void StartLoop()
        {
            try
            {
                _cts = new CancellationTokenSource();
                _pauseEvent.Set(); // Ensure gate is open
                _ = RunLoopAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Start Loop Check";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: StartLoop() FAIL\n\t{ex.Message}");
            }
        }

        /// <summary>
        /// Pause the loop call
        /// </summary>
        private void PauseLoop()
        {
            try
            {
                if (_pauseEvent != null)
                {
                    _pauseEvent.Reset(); // Close the gate
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Pause Loop Check";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: PauseLoop() FAIL\n\t{ex.Message}");
            }
        }

        /// <summary>
        /// Resume the loop call
        /// </summary>
        private void ResumeLoop()
        {
            try
            {
                if (_pauseEvent != null)
                {
                    _pauseEvent.Set(); // Open the gate
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Resume Loop Check";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: ResumeLoop() FAIL\n\t{ex.Message}");
            }
        }

        /// <summary>
        /// Stop the loop call
        /// </summary>
        private void StopLoop()
        {
            try
            {
                _cts?.Cancel();
                _cts = null;
                _pauseEvent.Set(); // Release the gate so the loop can exit
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Stop Loop Check";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: StopLoop() FAIL\n\t{ex.Message}");
            }
        }

        /// <summary>
        /// Allows to loop through a function every 1 second
        /// </summary>
        private async Task RunLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 1. Wait here if paused (non-blocking for the UI)
                    await Task.Run(() => _pauseEvent.Wait(token), token);

                    // 2. Perform work
                    await CheckOverDuePay();

                    // 3. Wait for the next interval
                    await Task.Delay(1000, token);
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: RunLoopAsync() FAIL\n\t{ex.Message}");
            }
        }

        private async Task CheckOverDuePay()
        {
            try
            {
                await Task.Run(async () =>
                {
                    foreach (var job in DashboardData.ToList())
                    {
                        if (job.JobStatus == Names.Invoiced && DateOnly.FromDateTime(DateTime.Now) > job.InvoiceDueDate)
                        {
                            await Database.SetInvoiceStatus(DataService.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.InvoiceId).First(), Names.Overdue, null);
                            await DataService.RefreshDatabase();
                            await Execute.OnUIThreadAsync(async () => { await SetupDashboardData(); });
                        }
                        else if (job.JobStatus == Names.Overdue && DateOnly.FromDateTime(DateTime.Now) < job.InvoiceDueDate)
                        {
                            await Database.SetInvoiceStatus(DataService.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.InvoiceId).First(), Names.Invoiced, null);
                            await DataService.RefreshDatabase();
                            await Execute.OnUIThreadAsync(async () => { await SetupDashboardData(); });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Check Overdue";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: CheckOverDuePay() FAIL\n\t{ex.Message}");
            }
        }

        private Task SortJobs(string command)
        {
            try
            {
                if (command == Names.ClientNameAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => j.ClientName));
                }
                else if (command == Names.ClientNameDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => j.ClientName));
                }

                else if (command == Names.JobTitleAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => j.JobTitle));
                }
                else if (command == Names.JobTitleDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => j.JobTitle));
                }
                else if (command == Names.JobStatusAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(_dashboardData.OrderBy(j =>
                    {
                        switch (j.JobStatus.ToLower())
                        {
                            case "new": return 1;
                            case "active": return 2;
                            case "done": return 3;
                            case "invoiced": return 4;
                            default: return 5; // Anything else goes to the bottom
                        }
                    }));
                }
                else if (command == Names.JobStatusDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(_dashboardData.OrderByDescending(j =>
                    {
                        switch (j.JobStatus.ToLower())
                        {
                            case "new": return 1;
                            case "active": return 2;
                            case "done": return 3;
                            case "invoiced": return 4;
                            default: return 5; // Anything else goes to the bottom
                        }
                    }));
                }
                else if (command == Names.JobPriceAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => Decimal.Parse(j.Price.ToString(), NumberStyles.Currency)));
                }
                else if (command == Names.JobPriceDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => Decimal.Parse(j.Price.ToString(), NumberStyles.Currency)));
                }

                else if (command == Names.DueDateAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => j.DueDate));
                }
                else if (command == Names.DueDateDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => j.DueDate));
                }

                else if (command == Names.CreatedDateAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => j.CreatedDate));
                }
                else if (command == Names.CreatedDateDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => j.CreatedDate));
                }

                else if (command == Names.ClientTypeAsc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => j.ClientType));
                }
                else if (command == Names.ClientTypeDesc)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => j.ClientType));
                }
                else if (command == Names.ResetSort)
                {
                    DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => j.ClientId));
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Sort Jobs";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: SortJobs() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task FilterJobs(string command)
        {
            try
            {
                /*
                 * DashboardBackUp = supreme backup
                 * DashboardFiltered = backup of current filter
                 * Dashboard = Normal UI list
                 */

                if (command != Names.FilterIndividual && command != Names.FilterComapny && command != Names.AllJobStatus && command != Names.UnfilterClientType) // status
                {
                    if (_isFilterClientTypeOn == true)
                    {
                        if (_isFilterClientTypeIndividual == true)
                        {
                            _dashboardDataTypeFiltered = new ObservableCollection<DashboardModel>(_dashboardDataBackup.Where(j => j.ClientType == "Individual").ToList());
                        }
                        else if (_isFilterClientTypeCompany == true)
                        {
                            _dashboardDataTypeFiltered = new ObservableCollection<DashboardModel>(_dashboardDataBackup.Where(j => j.ClientType == "Company").ToList());
                        }
                        DashboardData = _dashboardDataTypeFiltered;
                    }
                    else
                    {
                        DashboardData = _dashboardDataBackup;
                    }

                    if (command == Names.JobStatusNew)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.JobStatus == "New").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataStatusFiltered = DashboardData; // backup to filtered list
                        }
                    }
                    else if (command == Names.JobStatusActive)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.JobStatus == "Active").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataStatusFiltered = DashboardData; // backup to filtered list
                        }
                    }
                    else if (command == Names.JobStatusDone)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.JobStatus == "Done").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataStatusFiltered = DashboardData; // backup to filtered list
                        }
                    }
                    else if (command == Names.JobStatusInvoiced)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.JobStatus == "Invoiced").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataStatusFiltered = DashboardData; // backup to filtered list
                        }
                    }
                    else if (command == Names.JobStatusOverdue)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.InvoiceStatus == "Overdue").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataStatusFiltered = DashboardData; // backup to filtered list
                        }
                    }
                    else if (command == Names.JobStatusPaid)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.InvoiceStatus == "Paid").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataStatusFiltered = DashboardData; // backup to filtered list
                        }
                    }
                    _isFilterJobStatusOn = true;
                }
                else if (command == Names.FilterIndividual || command == Names.FilterComapny) // cllient type
                {
                    if (_isFilterJobStatusOn == true)
                    {
                        DashboardData = _dashboardDataStatusFiltered;
                    }
                    else
                    {
                        DashboardData = _dashboardDataBackup;
                    }

                    if (command == Names.FilterIndividual)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.ClientType == "Individual").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataTypeFiltered = DashboardData;
                        }
                        _isFilterClientTypeCompany = false;
                        _isFilterClientTypeIndividual = true;
                    }
                    else if (command == Names.FilterComapny)
                    {
                        DashboardData = new ObservableCollection<DashboardModel>(DashboardData.Where(j => j.ClientType == "Company").ToList());

                        if (DashboardData.Count > 0)
                        {
                            _dashboardDataTypeFiltered = DashboardData;
                        }
                        _isFilterClientTypeIndividual = false;
                        _isFilterClientTypeCompany = true;
                    }
                    _isFilterClientTypeOn = true;
                }
                else if (command == Names.AllJobStatus)
                {
                    if (_isFilterJobStatusOn == false)
                    {
                        DashboardData = _dashboardDataBackup; // reset list
                        _dashboardDataStatusFiltered = DashboardData;
                    }
                    else if (_isFilterJobStatusOn == true) // deselect status (new, active, done, invoiced)
                    {
                        DashboardData = _dashboardDataBackup;
                        if (_isFilterClientTypeOn == true)
                        {
                            DashboardData = _dashboardDataTypeFiltered;
                        }
                        _isFilterJobStatusOn = false;

                        // check if sort was enabled
                        if (string.IsNullOrEmpty(State.currentSortOption) == false)
                        {
                            SortJobs(State.currentSortOption);
                        }
                    }
                }
                else if (command == Names.UnfilterClientType) // reset client type
                {
                    if (_isFilterClientTypeOn == false)
                    {
                        DashboardData = _dashboardDataBackup; // reset list
                        _dashboardDataTypeFiltered = DashboardData;
                    }
                    else if (_isFilterClientTypeOn == true) // deselect type (individual, company)
                    {
                        DashboardData = _dashboardDataBackup;
                        if (_isFilterJobStatusOn == true)
                        {
                            DashboardData = _dashboardDataStatusFiltered;
                        }
                        _isFilterClientTypeOn = false;

                        // check if sort was enabled
                        if (string.IsNullOrEmpty(State.currentSortOption) == false)
                        {
                            SortJobs(State.currentSortOption);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Exit Form";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Event Handlers
        public async Task HandleAsync(DashboardVMEvents message, CancellationToken cancellationToken)
        {
            try
            {
                if (message != null)
                {
                    if (message.Command == "EditClient")
                    {
                        await EditClient();
                    }
                    else if (message.Command == Names.ResetSort)
                    {
                        await SortJobs(Names.ResetSort);
                    }
                    else if (message.Command == Names.ClientNameAsc)
                    {
                        await SortJobs(Names.ClientNameAsc);
                    }
                    else if (message.Command == Names.ClientNameDesc)
                    {
                        await SortJobs(Names.ClientNameDesc);
                    }
                    else if (message.Command == Names.JobTitleAsc)
                    {
                        await SortJobs(Names.JobTitleAsc);
                    }
                    else if (message.Command == Names.JobTitleDesc)
                    {
                        await SortJobs(Names.JobTitleDesc);
                    }
                    else if (message.Command == Names.JobStatusAsc)
                    {
                        await SortJobs(Names.JobStatusAsc);
                    }
                    else if (message.Command == Names.JobStatusDesc)
                    {
                        await SortJobs(Names.JobStatusDesc);
                    }
                    else if (message.Command == Names.JobPriceAsc)
                    {
                        await SortJobs(Names.JobPriceAsc);
                    }
                    else if (message.Command == Names.JobPriceDesc)
                    {
                        await SortJobs(Names.JobPriceDesc);
                    }
                    else if (message.Command == Names.DueDateAsc)
                    {
                        await SortJobs(Names.DueDateAsc);
                    }
                    else if (message.Command == Names.DueDateDesc)
                    {
                        await SortJobs(Names.DueDateDesc);
                    }
                    else if (message.Command == Names.CreatedDateAsc)
                    {
                        await SortJobs(Names.CreatedDateAsc);
                    }
                    else if (message.Command == Names.CreatedDateDesc)
                    {
                        await SortJobs(Names.CreatedDateDesc);
                    }
                    else if (message.Command == Names.ClientTypeAsc)
                    {
                        await SortJobs(Names.ClientTypeAsc);
                    }
                    else if (message.Command == Names.ClientTypeDesc)
                    {
                        await SortJobs(Names.ClientTypeDesc);
                    }
                    else if (message.Command == Names.JobStatusNew)
                    {
                        await FilterJobs(Names.JobStatusNew);
                    }
                    else if (message.Command == Names.JobStatusActive)
                    {
                        await FilterJobs(Names.JobStatusActive);
                    }
                    else if (message.Command == Names.JobStatusDone)
                    {
                        await FilterJobs(Names.JobStatusDone);
                    }
                    else if (message.Command == Names.JobStatusInvoiced)
                    {
                        await FilterJobs(Names.JobStatusInvoiced);
                    }
                    else if (message.Command == Names.JobStatusOverdue)
                    {
                        await FilterJobs(Names.JobStatusOverdue);
                    }
                    else if (message.Command == Names.JobStatusPaid)
                    {
                        await FilterJobs(Names.JobStatusPaid);
                    }
                    else if (message.Command == Names.FilterIndividual)
                    {
                        await FilterJobs(Names.FilterIndividual);
                    }
                    else if (message.Command == Names.FilterComapny)
                    {
                        await FilterJobs(Names.FilterComapny);
                    }
                    else if (message.Command == Names.AllJobStatus)
                    {
                        await FilterJobs(Names.AllJobStatus);
                    }
                    else if (message.Command == Names.UnfilterClientType)
                    {
                        await FilterJobs(Names.UnfilterClientType);
                    }
                    else if (message.Command == Names.ShowInvoice)
                    {
                        State.EditInvoiceViewModel = new EditInvoiceViewModel(_events, _windowManager, DataService, State);
                        State.EditInvoiceViewModel.SelectedJob = SelectedJob!;
                        await _windowManager.ShowWindowAsync(State.EditInvoiceViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Dashboard Events";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: HandleAsync(DashboardVMEvents) FAIL\n\t{ex.Message}");
            }
        }

        public async Task HandleAsync(RefreshDatabase message, CancellationToken cancellationToken)
        {
            try
            {
                State.IsBusy = true;
                State.LoadingMessage = "P L E A S E   W A I T ";
                await Task.Delay(50);

                await DataService.RefreshDatabase();

                await SetupDashboardData();

                if (message != null)
                {
                    if (message.Command != Names.Invoice) // skip for invoice creation as no data has been amneded
                    {
                        SelectedJob = null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == State.messageBoxVM) == false)
                {
                    State.messageBoxVM.Symbol = 2;
                    State.messageBoxVM.HeadMessage = "Refresh Database";
                    State.messageBoxVM.Message = ex.Message;
                    State.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(State.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"DashboardViewModel: HandleAsync(RefreshDatabase) FAIL\n\t{ex.Message}");
            }
            finally
            {
                State.IsBusy = false;
                State.LoadingMessage = string.Empty;
            }
        }
        #endregion

        #region Public View Variables     
        public String ReceivedAmount
        {
            get { return _receivedAmount; }
            set
            {
                _receivedAmount = value;
                NotifyOfPropertyChange(() => ReceivedAmount);
            }
        }

        public String OutstandingAmount
        {
            get { return _outstandingAmount; }
            set
            {
                _outstandingAmount = value;
                NotifyOfPropertyChange(() => OutstandingAmount);
            }
        }

        public String OverdueAmount
        {
            get { return _overdueAmount; }
            set
            {
                _overdueAmount = value;
                NotifyOfPropertyChange(() => OverdueAmount);
            }
        }

        public String GrossAmount
        {
            get { return _grossAmount; }
            set
            {
                _grossAmount = value;
                NotifyOfPropertyChange(() => GrossAmount);
            }
        }

        public String NewJobsCount
        {
            get { return _newJobsCount; }
            set
            {
                _newJobsCount = value;
                NotifyOfPropertyChange(() => NewJobsCount);
            }
        }

        public String ActiveJobsCount
        {
            get { return _activeJobsCount; }
            set
            {
                _activeJobsCount = value;
                NotifyOfPropertyChange(() => ActiveJobsCount);
            }
        }

        public String DoneJobsCount
        {
            get { return _doneJobsCount; }
            set
            {
                _doneJobsCount = value;
                NotifyOfPropertyChange(() => DoneJobsCount);
            }
        }

        public String InvoicedJobsCount
        {
            get { return _invoicedJobsCount; }
            set
            {
                _invoicedJobsCount = value;
                NotifyOfPropertyChange(() => InvoicedJobsCount);
            }
        }

        public int TotalJobsCount
        {
            get { return _totalJobsCount; }
            set
            {
                _totalJobsCount = value;
                NotifyOfPropertyChange(() => TotalJobsCount);
            }
        }

        public ObservableCollection<DashboardModel> DashboardData
        {
            get { return _dashboardData; }
            set
            {
                _dashboardData = value;
                NotifyOfPropertyChange(() => DashboardData);
            }
        }

        public DashboardModel? SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                NotifyOfPropertyChange(() => SelectedJob);
            }
        }

        public bool EnableBtns
        {
            get { return _enableBtns; }
            set
            {
                _enableBtns = value;
                NotifyOfPropertyChange(() => EnableBtns);
            }
        }

        public double OpacityBtns
        {
            get { return _opacityBtns; }
            set
            {
                _opacityBtns = value;
                NotifyOfPropertyChange(() => OpacityBtns);
            }
        }

        public string OutstandingStatusBorder
        {
            get { return _outstandingStatusBorder; }
            set
            {
                _outstandingStatusBorder = value;
                NotifyOfPropertyChange(() => OutstandingStatusBorder);
            }
        }
        #endregion
    }
}
