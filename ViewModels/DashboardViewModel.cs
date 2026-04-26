using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Traker.ViewModels
{
    using Database;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Traker.Events;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Models;
    using Traker.Services;
    using Traker.States;
    using Traker.ViewModels.User;

    public class DashboardViewModel : Screen, IHandle<RefreshDatabase>, IHandle<DashboardVMEvents>
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        #endregion

        #region Public State Variable
        public AppState State { get; } // state binding variable accessible from other viewmodels
        public DataService Data { get; } // data from the database
        #endregion

        #region Public View Variables
        public List<string> JobStatusEdit { get; set; } = new List<string> { "New", "Active", "Done" };
        public List<string> InvoiceStatusEdit { get; set; } = new List<string> { "Created", "Sent", "Paid", "Overdue" };
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
        private ObservableCollection<DashboardModel> _dashboardData; // listo of data shown on the data grid
        public DashboardModel _selectedJob; // selected data row automatically filled on click
        #endregion

        #region Private Field Variables
        private JobContextMenuViewModel? _contextMenuVM;
        private AddClientViewModel _addClientViewModel;
        private AddJobViewModel _addJobViewModel;
        private EditClientViewModel _editClientViewModel;
        private EditJobViewModel _editJobViewModel;
        private UserContextMenuViewModel _userContextMenuViewModel;
        private SortJobsViewModel _sortJobsViewModel;
        private FilterJobsViewModel _filterJobsViewModel;
        #endregion

        public DashboardViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState, DataService dataService)
        {
            _events = events;
            _windowManager = windowManager;

            State = appState;
            Data = dataService;

            _receivedAmount = "0";
            _outstandingAmount = "0";
            _overdueAmount = "0";
            _grossAmount = "0";
            _newJobsCount = "0";
            _activeJobsCount = "0";
            _doneJobsCount = "0";
            _invoicedJobsCount = "0";

            _dashboardData = new ObservableCollection<DashboardModel>();
            _selectedJob = new DashboardModel();

            _contextMenuVM = new JobContextMenuViewModel(_events, _windowManager, Data);
            _addClientViewModel = new AddClientViewModel(_events, State);
            _addJobViewModel = new AddJobViewModel(_events, State);
            _editClientViewModel = new EditClientViewModel(_events, _windowManager, Data);
            _userContextMenuViewModel = new UserContextMenuViewModel(_events, _windowManager, Data);
            _sortJobsViewModel = new SortJobsViewModel(_events);
            _filterJobsViewModel = new FilterJobsViewModel();

            _events.SubscribeOnPublishedThread(this);
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SetupDashboardData();

                //return base.OnInitializedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while while loading Dashboard. Please try again.\n\n{ex.Message}",
                    "Dashboard",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                System.Environment.Exit(1); // kill process
            }
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion

        #region Public View Functions
        public async Task OpenFilterContextMenu(FrameworkElement anchorElement)
        {
            // if add menu open do nothing
            if (State.IsWindowOpen == true)
            {
                return;
            }

            if (_filterJobsViewModel != null)
            {
                await _filterJobsViewModel.TryCloseAsync(false);
            }

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
                double popupTop = (locationFromScreen.Y / dpiY) - 20; // positive=down, negative=up
                double popupLeft = (locationFromScreen.X / dpiX) - 315; // psotiive=right, negative=left

                _filterJobsViewModel = new FilterJobsViewModel();
                await _windowManager.ShowPopupAsync(_filterJobsViewModel, null, CustomWindow.SettingsForDialog(310, 335, true, popupTop, popupLeft)); // vertical, horizontal
            }
        }

        public async Task OpenSortContextMenu(FrameworkElement anchorElement)
        {
            // if add menu open do nothing
            if (State.IsWindowOpen == true)
            {
                return;
            }

            if (_sortJobsViewModel != null)
            {
                await _sortJobsViewModel.TryCloseAsync(false);
            }

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
                double popupTop = (locationFromScreen.Y / dpiY) - 5; // positive=down, negative=up
                double popupLeft = (locationFromScreen.X / dpiX) - 315; // psotiive=right, negative=left

                //_sortJobsViewModel = new SortJobsViewModel();
                // for singleton use use IoC
                await _windowManager.ShowPopupAsync(IoC.Get<SortJobsViewModel>(), null, CustomWindow.SettingsForDialog(310, 335, true, popupTop, popupLeft)); // vertical, horizontal
            }
        }

        public async Task OpenUserContenxtMenu(FrameworkElement anchorElement)
        {
            // if add menu open do nothing
            if (State.IsWindowOpen == true)
            {
                return;
            }

            if (_userContextMenuViewModel != null)
            {
                await _userContextMenuViewModel.TryCloseAsync(false);
            }

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

                _userContextMenuViewModel = new UserContextMenuViewModel(_events, _windowManager, Data);
                await _windowManager.ShowPopupAsync(_userContextMenuViewModel, null, CustomWindow.SettingsForDialog(310, 335, true, popupTop, popupLeft)); // vertical, horizontal
            }
        }

        public Task SelectJob(DashboardModel selectedJob)
        {
            if (selectedJob != null)
            {
                SelectedJob = selectedJob;
            }
            else
            {
                // do something and show error message
            }
            return Task.CompletedTask;
        }

        public async Task OpenContextMenu(DashboardModel selectedJob)
        {
            // if add menu open do nothing
            if (State.IsWindowOpen == true)
            {
                return;
            }

            if (_contextMenuVM != null)
            {
                await _contextMenuVM.TryCloseAsync(false);
            }

            // if using client name then it must be required at all the time

            /*
             * there are two variables to get the data from the row
             * 1 - on the right click (datagrid approach)
             * 2 - on the left click (to pass row's current object 'selectedJob')
             * to open the popup the datagrid selection object must match with the right-clicked onwe
             * we use the clientName for matching for now, but might use unique id in the future.
             * and the latter must become required hence Id seems perfect.
             * In fact we just swapped to clientId :)
             */
            if (SelectedJob.ClientId == selectedJob.ClientId)
            {
                _contextMenuVM = new JobContextMenuViewModel(_events, _windowManager, Data);
                _contextMenuVM.SelectedJob = selectedJob; // pass row selected
                await _windowManager.ShowPopupAsync(_contextMenuVM, null, CustomWindow.SettingsForDialog(310, 335, false)); // vertical, horizontal
            }
        }

        public async Task EditJob(DashboardModel jobSelected)
        {
            _editJobViewModel = new EditJobViewModel(_events);

            if (jobSelected != null)
            {
                _editJobViewModel.SelectedJob = jobSelected; // pass selected   row to EditJobViewModel
            }
            else
            {
                _editJobViewModel.SelectedJob = SelectedJob;

            }
            await _windowManager.ShowWindowAsync(_editJobViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
        }

        public async Task EditClient()
        {
            _editClientViewModel = new EditClientViewModel(_events, _windowManager, Data);
            _editClientViewModel.SelectedRow = SelectedJob; // pass selected row to EditClientViewModel
            await _windowManager.ShowWindowAsync(_editClientViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
        }

        public async Task AddClient()
        {
            Debug.WriteLine("ADDING client..");

            if (State.IsWindowOpen == false)
            {
                // if is not free then report it with a message box
                // move this to a private function later
                if (_contextMenuVM != null)
                {
                    await _contextMenuVM.TryCloseAsync(false);
                    _contextMenuVM = null;
                }
                if (_addJobViewModel != null)
                {
                    await _addJobViewModel.TryCloseAsync(false);
                    _addJobViewModel = null;
                }

                _addClientViewModel = new AddClientViewModel(_events, State);
                await _windowManager.ShowWindowAsync(_addClientViewModel, null, CustomWindow.SettingsForDialog(600, 500, false));
                State.IsWindowOpen = true; // flag as open accross the project
            }

            // the state State.IsAddRowEntryOpen will be false again from addrowentryVM when user closes the window
            else if (State.IsWindowOpen == true)
            {
                return; // do nothing
            }
        }

        public async Task AddJob()
        {
            //Debug.WriteLine("ADDING job..");

            if (State.IsWindowOpen == false)
            {
                // if State.popup is not free then report it with a message box
                if (_contextMenuVM != null)
                {
                    await _contextMenuVM.TryCloseAsync(false);
                    _contextMenuVM = null;
                }
                if (_addClientViewModel != null)
                {
                    await _addClientViewModel.TryCloseAsync(false);
                    _addClientViewModel = null;
                }

                _addJobViewModel = new AddJobViewModel(_events, State);
                _addJobViewModel.dashboardData = _dashboardData; // pass dashboard data to AddJob
                await _windowManager.ShowWindowAsync(_addJobViewModel, null, CustomWindow.SettingsForDialog(600, 500, false));
                State.IsWindowOpen = true; // flag as open accross the project
            }

            // the state State.IsAddRowEntryOpen will be false again from addrowentryVM when user closes the window
            else if (State.IsWindowOpen == true)
            {
                return; // do nothing
            }
        }

        public async Task OnMouseDownEvent(Grid gridSource)
        {
            if (_contextMenuVM != null)
            {
                await _contextMenuVM.TryCloseAsync(false);
                _contextMenuVM = null;
            }
            if (_userContextMenuViewModel != null)
            {
                await _userContextMenuViewModel.TryCloseAsync(false);
                _userContextMenuViewModel = null;
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
        #endregion

        #region Private Functions
        private Task SortJobs(string command)
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
                DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderBy(j => Decimal.Parse(j.Price, NumberStyles.Currency)));
            }
            else if (command == Names.JobPriceDesc)
            {
                DashboardData = new ObservableCollection<DashboardModel>(DashboardData.OrderByDescending(j => Decimal.Parse(j.Price, NumberStyles.Currency)));
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
            return Task.CompletedTask;
        }

        private Task SetupDashboardData()
        {
            // row data
            _dashboardData.Clear();
            int index = 0;
            foreach (var job in Data.Jobs)
            {
                var client = Data.Clients.First(c => c.ClientId == job.ClientId);

                DashboardModel dashboardEntry = new DashboardModel
                {
                    ClientId = client.ClientId,
                    ClientType = client.Type,
                    TypeIcon = (client.Type == "Individual") ? "/Resources/Media/Images/Icons/Lucide/user-round.svg" : "/Resources/Media/Images/Icons/Lucide/building.svg",
                    ClientName = client.FullName,
                    ClientEmail = client.Email,
                    ClientPhone = client.PhoneNumber,
                    CompanyName = client.CompanyName,
                    BillingAddress = client.BillingAddress,
                    City = client.City,
                    Postcode = client.Postcode,
                    Country = client.Country,
                    CreatedDate = client.CreatedDate,
                    IsActive = client.IsActive,

                    JobId = job.JobId,
                    JobTitle = job.Title,
                    JobDescription = job.Description,
                    Price = job.FinalPrice.ToString("C"), // use toString("C") only for ui side
                    AmountReceived = job.AmountReceived.ToString("C"),
                    JobStatus = job.Status.ToString(),
                    StartDate = job.StartDate,
                    DueDate = job.DueDate,

                    HasInvoice = Data.Invoices.Any(i => i.JobId == job.JobId && i.IsDeleted == false),
                    InvoiceStatus = Data.Invoices.Where(i => i.JobId == job.JobId).Select(i => i.Status).FirstOrDefault() ?? "Not invoiced"
                };
                _dashboardData.Add(dashboardEntry);
                index++;
            }

            // ascending order job title
            //_dashboardData = new ObservableCollection<DashboardModel>(_dashboardData.OrderBy(j => j.JobTitle));

            // job status order
            _dashboardData = new ObservableCollection<DashboardModel>(_dashboardData.OrderBy(
                j =>
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

            /*
             * sortb by:
             * - client name (up/down)
             * - job name (up/down)
             * - job status (up/down)
             * - job price (up/down)
             * - due date
             * - created date?
             * - company type
             * 
             * filter by:
             * - job status (e.g. only Active)
             * - company type
             */

            NewJobsCount = Data.Jobs.Where(j => j.Status == Names.New).Count().ToString(); // new jobs count
            DoneJobsCount = Data.Jobs.Where(j => j.Status == Names.Done).Count().ToString(); // done jobs count
            ActiveJobsCount = Data.Jobs.Where(j => j.Status == Names.Active).Count().ToString(); // active jobs count
            InvoicedJobsCount = Data.Jobs.Where(j => j.Status == Names.Invoiced).Count().ToString(); // invoiced jobs count
            GrossAmount = Data.Jobs.Sum(gross => gross.FinalPrice).ToString("C"); // gross amount
            ReceivedAmount = Data.Jobs.Where(j => j.Status == Names.Paid).Sum(j => j.FinalPrice).ToString("C");
            OutstandingAmount = Data.Jobs
                .Where(j => j.Status == Names.Done || (j.Status == Names.Invoiced && Data.Invoices.Any(i => i.JobId == j.JobId && i.DueDate > DateOnly.FromDateTime(DateTime.Now))))
                .Sum(j => j.FinalPrice).ToString("C");

            OverdueAmount = Data.Jobs
                                .Where(j => j.Status == Names.Invoiced)
                                .Join(Data.Invoices, 
                                      job => job.JobId, 
                                      inv => inv.JobId, (job, inv) => new { job.FinalPrice, inv.DueDate })
                                .Where(x => x.DueDate < DateOnly.FromDateTime(DateTime.Now))
                                .Sum(x => x.FinalPrice).ToString("C"); // overdue
            return Task.CompletedTask;
        }
        #endregion

        #region Event Handlers
        public async Task HandleAsync(DashboardVMEvents message, CancellationToken cancellationToken)
        {
            if (message != null)
            {
                if (message.Command == "EditClient")
                {
                    await EditClient();
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
            }
        }

        public async Task HandleAsync(RefreshDatabase message, CancellationToken cancellationToken)
        {
            await Data.RefreshDatabase();
            await SetupDashboardData();
        }
        #endregion

        // TO CHECK
        public Task UpdateJobStatus(DashboardModel row)
        {
            // update DB here
            return Task.CompletedTask;
        }

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

        public ObservableCollection<DashboardModel> DashboardData
        {
            get { return _dashboardData; }
            set
            {
                _dashboardData = value;
                NotifyOfPropertyChange(() => DashboardData);
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
