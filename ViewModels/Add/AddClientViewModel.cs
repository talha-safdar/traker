using Caliburn.Micro;
using Traker.States;

namespace Traker.ViewModels.Add
{
    using Database;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Traker.Data;
    using Traker.Events.DashboardVM;
    using Traker.Helper;
    using Traker.Services;

    public class AddClientViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly DataService _dataService;
        private readonly AppState _state;
        #endregion

        #region Private View Variables
        private string _clientType;
        private string _businessName; // generalised for both client or company name
        private string _jobTitle;
        private string _price;
        private string _dueDate;
        private string _businessNameText; // individual=client name, company=company name

        // submit button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field Variables
        private string _clientNameTxt = "Client Name";
        private string _companyNameTxt = "Company Name";
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;
        #endregion

        public AddClientViewModel(IEventAggregator events, IWindowManager windowManager, DataService dataService, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _dataService = dataService;
            _state = state;

            _clientType = string.Empty;
            _businessName = string.Empty;
            _jobTitle = string.Empty;
            _price = string.Empty;
            _dueDate = string.Empty;
            _businessNameText = string.Empty;
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                ClientType = Names.Individual; // default start with individual
                BusinessNameText = _clientNameTxt;

                // submit button
                EnableSubmitBtn = false;
                OpacitySubmitBtn = _halfOpacity;



                BusinessName = "ABC";
                JobTitle = "Count";
                Price = "12.45";
                DueDate = "12/12/2026";
            }
            catch (Exception ex)
            {
                // not already open?
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Initialise Form";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
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
        /// <summary>
        /// Esc button to exit the window
        /// </summary>
        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (
                        string.IsNullOrEmpty(BusinessName) == false ||
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
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        /// <summary>
        /// Exit button to exit the window
        /// </summary>
        public async Task Exit()
        {
            try
            {
                if (
                    string.IsNullOrEmpty(BusinessName) == false ||
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
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }

        /// <summary>
        /// Add client confirmation
        /// </summary>
        public async Task AddClient()
        {
            try
            {
                await Task.Run(async() => {
                    await TryCloseAsync();
                    var dueDate = DateOnly.MinValue;
                    decimal amount = 0;

                    // due date conversion
                    if (DueDate != string.Empty)
                    {
                        dueDate = DateOnly.ParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    // money conversion
                    if (Price != string.Empty)
                    {
                        amount = decimal.Parse(Price, CultureInfo.InvariantCulture);
                    }

                    /*
                     * 0 = clientId
                     * 1 = jobId
                     */
                    List<int> clientJobIds = new List<int>();

                    if (ClientType == Names.Individual)
                    {
                        clientJobIds = await Database.AddIndividualClient(BusinessName.Trim(), ClientType.Trim(), JobTitle.Trim(), amount, dueDate);
                    }
                    else if (ClientType == Names.Company)
                    {
                        clientJobIds = await Database.AddCompanyClient(BusinessName.Trim(), ClientType.Trim(), JobTitle.Trim(), amount, dueDate);
                    }

                    // refresh database
                    //await _dataService.RefreshDatabase();

                    // create Client folder in file store and get its name
                    string clientFolderName = await FileStore.CreateClientFolder(await Database.GetLastClientlastRowId(), BusinessName.Trim());

                    // create Job folder in file store and get its name
                    string jobFolderName = await FileStore.CreateJobFolder(clientJobIds[0], clientJobIds[1], BusinessName.Trim(), JobTitle.Trim());

                    // update client and job databases with their fodler names
                    await Database.SetClientFolderName(clientJobIds[0], clientFolderName.Trim());
                    await Database.SetJobFolderName(clientJobIds[1], jobFolderName.Trim());

                    // refresh database
                    await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                });
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
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: AddClient() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Based on client type change label text
        /// </summary>
        private Task ToggleClientType()
        {
            Task.Run(() =>
            {
                if (ClientType == Names.Individual)
                {
                    BusinessNameText = _clientNameTxt;
                }
                else if (ClientType == Names.Company)
                {
                    BusinessNameText = _companyNameTxt;
                }
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Check date
        /// </summary>
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

        /// <summary>
        /// Check if all textboxes are filled to enable submit button
        /// </summary>
        /// <returns></returns>
        private Task CanSubmit()
        {
            try
            {
                if (string.IsNullOrEmpty(ClientType) == false && string.IsNullOrEmpty(BusinessName) == false && string.IsNullOrEmpty(JobTitle) == false && string.IsNullOrEmpty(Price) == false && ValidateDate() == true)
                {
                    EnableSubmitBtn = true;
                    OpacitySubmitBtn = _fullOpacity;
                }
                else
                {
                    EnableSubmitBtn = false;
                    OpacitySubmitBtn = _halfOpacity;
                }
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
                Logger.LogActivity(Logger.ERROR, $"AddClientViewModel: CanSubmit() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Public View Variables
        public string ClientType
        {
            get { return _clientType; }
            set
            {
                _clientType = value;
                NotifyOfPropertyChange(() => ClientType);
                ToggleClientType();
                CanSubmit();
            }
        }

        public string BusinessName
        {
            get { return _businessName; }
            set
            {
                _businessName = value;
                NotifyOfPropertyChange(() => BusinessName);
                CanSubmit();
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

        public string BusinessNameText
        {
            get { return _businessNameText; }
            set
            {
                _businessNameText = value;
                NotifyOfPropertyChange(() => BusinessNameText);
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