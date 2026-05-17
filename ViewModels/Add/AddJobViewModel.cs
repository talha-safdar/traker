using Caliburn.Micro;
using System.Collections.ObjectModel;
using Traker.Models;

namespace Traker.ViewModels.Add
{
    using Database;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using Traker.Data;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Services;
    using Traker.States;

    // try use inheritance or a design pattern to avoid repetition

    public class AddJobViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly AppState _state;
        #endregion

        #region Public Variables
        public ObservableCollection<DashboardModel> dashboardData; // to obtain the current dashboard clients list
        #endregion

        #region Private View Variables
        private AddJobModel _selectedClient;
        private string _jobTitle;
        private string _price;
        private string _dueDate;

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field Variables
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        private ObservableCollection<AddJobModel> _addJob;
        #endregion

        public AddJobViewModel(IEventAggregator events, IWindowManager windowManager, AppState appState)
        {
            _events = events;
            _windowManager = windowManager;
            _state = appState;

            dashboardData = new ObservableCollection<DashboardModel>();

            _addJob = new ObservableCollection<AddJobModel>();
            _selectedClient = new AddJobModel();
            _jobTitle = string.Empty;
            _price = string.Empty;
            _dueDate = string.Empty;
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                EnableSubmitBtn = false;
                OpacitySubmitBtn = _halfOpacity;

                foreach (var client in dashboardData.DistinctBy(x => x.ClientId))
                {
                    AddJob.Add(new AddJobModel
                    {
                        ClientId = client.ClientId,
                        CreatedDate = client.CreatedDate.ToString(),
                        BusinessName = client.ClientType == Names.Individual ? client.ClientName : client.CompanyName,
                    });
                }

                SelectedClient = AddJob.First(); // pre-select the first item in the list
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
                Logger.LogActivity(Logger.ERROR, $"AddJobViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }

            return base.OnInitializedAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _events.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion

        #region Public View Functions
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (
                        string.IsNullOrEmpty(JobTitle) == false ||
                        string.IsNullOrEmpty(Price) == false ||
                        string.IsNullOrEmpty(DueDate) == false
                        )
                    {
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                        {
                            _state.messageBoxVM.Symbol = 0;
                            _state.messageBoxVM.HeadMessage = "Discard changes?";
                            _state.messageBoxVM.Message = Names.DiscardEsc;
                            _state.messageBoxVM.ButtonStyle = Names.NoYes;
                            await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }

                        // if clicked yes
                        if (_state.messageBoxVM.Output == true)
                        {
                            await TryCloseAsync();
                        }
                    }
                    else
                    {
                        await TryCloseAsync();
                    }
                }
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
                Logger.LogActivity(Logger.ERROR, $"AddJobViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task AddJobToClient()
        {
            try
            {
                var dueDate = DateOnly.MinValue;
                decimal amount = 0;

                if (DueDate != string.Empty)
                {
                    dueDate = DateOnly.ParseExact(
                    DueDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture
                    );
                }

                if (Price != string.Empty)
                {
                    amount = decimal.Parse(
                        Price,
                        CultureInfo.InvariantCulture
                    );
                }

                // add job under the client's id
                int jobId = await Database.AddNewJobToClient(SelectedClient.ClientId, JobTitle.Trim(), amount, dueDate);

                // add job folder
                await FileStore.CreateJobFolder(SelectedClient.ClientId, jobId, SelectedClient.BusinessName.Trim(), JobTitle.Trim());

                // refresh database
                await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                await TryCloseAsync();
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Add Job";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"AddJobViewModel: AddJobToClient() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                if (
                    string.IsNullOrEmpty(JobTitle) == false ||
                    string.IsNullOrEmpty(Price) == false ||
                    string.IsNullOrEmpty(DueDate) == false
                    )
                {
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                    {
                        _state.messageBoxVM.Symbol = 0;
                        _state.messageBoxVM.HeadMessage = "Discard changes?";
                        _state.messageBoxVM.Message = Names.DiscardEsc;
                        _state.messageBoxVM.ButtonStyle = Names.NoYes;
                        await _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }

                    // if clicked yes
                    if (_state.messageBoxVM.Output == true)
                    {
                        await TryCloseAsync();
                    }
                }
                else
                {
                    await TryCloseAsync();
                }
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
                Logger.LogActivity(Logger.ERROR, $"AddJobViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        private bool ValidateDate()
        {
            return DateTime.TryParseExact(
                DueDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate)
                && parsedDate.Date >= DateTime.Today;
        }

        private Task CanSubmit()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedClient.BusinessName) == false && string.IsNullOrEmpty(JobTitle) == false && string.IsNullOrEmpty(Price) == false && ValidateDate() == true)
                {

                    EnableSubmitBtn = true;
                    OpacitySubmitBtn = _fullOpacity;
                }
                else
                {
                    EnableSubmitBtn = false;
                    OpacitySubmitBtn = _halfOpacity;
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Add Client";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"AddJobViewModel: CanSubmit() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Public View Variables
        public AddJobModel SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                NotifyOfPropertyChange(() => SelectedClient);
            }
        }

        public ObservableCollection<AddJobModel> AddJob
        {
            get { return _addJob; }
            set
            {
                _addJob = value;
                NotifyOfPropertyChange(() => AddJob);
            }
        }

        public string JobTitle
        {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                NotifyOfPropertyChange(() => JobTitle);
                CanSubmit();
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
                CanSubmit();
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
                CanSubmit();
            }
        }

        public bool EnableSubmitBtn
        {
            get { return _enableSubmitBtn; }
            set
            {
                _enableSubmitBtn = value;
                NotifyOfPropertyChange(() => EnableSubmitBtn);
            }
        }

        public double OpacitySubmitBtn
        {
            get { return _opacitySubmitBtn; }
            set
            {
                _opacitySubmitBtn = value;
                NotifyOfPropertyChange(() => OpacitySubmitBtn);
            }
        }
        #endregion
    }
}
