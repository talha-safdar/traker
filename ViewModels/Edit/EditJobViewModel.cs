using Caliburn.Micro;
using Traker.Models;

namespace Traker.ViewModels.Edit
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

    public class EditJobViewModel : Screen
    {
        #region Caliburn Variables
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;
        private readonly AppState _state;
        #endregion

        #region Public Variables
        public DashboardModel SelectedJob;
        #endregion

        #region Private View Variables
        private string _businessName;
        private DateOnly _createdDate;
        private string _jobTitle;
        private string _jobDescription;
        private string _status;
        private decimal _price;
        private decimal _amountReceived;
        private string _startDate;
        private string _dueDate;
        private string _remainingAmount;

        // confirm button
        private bool _enableSubmitBtn;
        private double _opacitySubmitBtn;
        #endregion

        #region Private Class Field Variables
        private double _fullOpacity = 1.0;
        private double _halfOpacity = 0.5;

        private string _priceText;
        private string _amountReceivedText;
        private bool _isEditingPrice;
        private bool _isEditingReceived;
        #endregion

        public EditJobViewModel(IEventAggregator events, IWindowManager windowManager, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            _state = state;

            SelectedJob = new DashboardModel();

            _businessName = string.Empty;
            _createdDate = DateOnly.MinValue;
            _jobTitle = string.Empty;
            _jobDescription = string.Empty;
            _status = string.Empty;
            _price = 0.0m;
            _amountReceived = 0.0m;
            _startDate = string.Empty;
            _dueDate = string.Empty;
            _remainingAmount = string.Empty;

            // confirm button
            _enableSubmitBtn = false;
            _opacitySubmitBtn = 0.0;

            _priceText = string.Empty;
            _amountReceivedText = string.Empty;
            _isEditingPrice = false;
            _isEditingReceived = false;
        }

        #region Caliburn Functions
        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                BusinessName = SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName;
                CreatedDate = SelectedJob.CreatedDate;
                JobTitle = SelectedJob.JobTitle;
                JobDescription = SelectedJob.JobDescription;
                Status = SelectedJob.JobStatus;
                Price = SelectedJob.Price.ToString();
                AmountReceived = SelectedJob.AmountReceived.ToString();
                StartDate = SelectedJob.StartDate == DateOnly.FromDateTime(DateTime.MinValue) ? string.Empty : SelectedJob.StartDate.ToString();
                DueDate = SelectedJob.DueDate.ToString();
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
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
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
        // focus/unfocus behaviours for price edit
        public void PriceFocused()
        {
            _isEditingPrice = true;
            NotifyOfPropertyChange(nameof(Price));
        }

        public void PriceUnfocused()
        {
            _isEditingPrice = false;
            NotifyOfPropertyChange(nameof(Price));
        }

        public void ReceivedFocused()
        {
            _isEditingReceived = true;
            NotifyOfPropertyChange(nameof(AmountReceived));
        }

        public void ReceivedUnfocused()
        {
            _isEditingReceived = false;
            NotifyOfPropertyChange(nameof(AmountReceived));
        }
        // focus/unfocus behaviours for price edit

        public async Task ConfirmJobChanges()
        {
            try
            {
                // price
                decimal.TryParse(Price,
                NumberStyles.Currency,
                CultureInfo.CurrentCulture,
                out var priceFormatted);

                // amount received
                decimal.TryParse(AmountReceived,
                NumberStyles.Currency,
                CultureInfo.CurrentCulture,
                out var AmountReceivedFormatted);

                var startDate = DateOnly.MinValue;
                if (StartDate != String.Empty)
                {
                    startDate = DateOnly.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                var dueDate = DateOnly.MinValue;
                if (DueDate != String.Empty)
                {
                    dueDate = DateOnly.ParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                // check if anything changed first
                string startDateCheck = SelectedJob.StartDate.ToString() == "01/01/0001" ? string.Empty : SelectedJob.StartDate.ToString();
                if (SelectedJob.JobTitle != JobTitle ||
                    SelectedJob.JobDescription != JobDescription ||
                    SelectedJob.JobStatus != Status ||
                    SelectedJob.Price.ToString("C") != Price.ToString() ||
                    SelectedJob.AmountReceived.ToString("C") != AmountReceived.ToString() ||
                    (SelectedJob.StartDate.ToString() == "01/01/0001" ? string.Empty : SelectedJob.StartDate.ToString()) != startDateCheck.ToString() ||
                    SelectedJob.DueDate.ToString() != DueDate.ToString())
                {
                    // check if job title changed 
                    if (SelectedJob.JobTitle != JobTitle)
                    {
                        if (await FileStore.UpdateJobFolderName(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName.Trim() : SelectedJob.CompanyName.Trim(), SelectedJob.JobTitle.Trim(), JobTitle.Trim()) == true)
                        {
                            await Task.Run(async () =>
                            {
                                await Database.EditJob(SelectedJob.JobId, JobTitle.Trim(), JobDescription.Trim(), Status.Trim(), priceFormatted.ToString().Trim(), AmountReceivedFormatted.ToString().Trim(), startDate, dueDate);
                                await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                            });
                            await TryCloseAsync();
                        }
                    }
                    else
                    {
                        await Task.Run(async () =>
                        {
                            await Database.EditJob(SelectedJob.JobId, JobTitle.Trim(), JobDescription.Trim(), Status.Trim(), priceFormatted.ToString().Trim(), AmountReceivedFormatted.ToString().Trim(), startDate, dueDate);
                            await _events.PublishOnUIThreadAsync(new RefreshDatabase());
                        });
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
                    _state.messageBoxVM.HeadMessage = "Confirm Changes";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: ConfirmJobChanges() FAIL\n\t{ex.Message}");
            }
        }

        public async Task HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    string startDate = SelectedJob.StartDate.ToString() == "01/01/0001" ? string.Empty : SelectedJob.StartDate.ToString();
                    if (SelectedJob.JobTitle != JobTitle ||
                        SelectedJob.JobDescription != JobDescription ||
                        SelectedJob.JobStatus != Status ||
                        SelectedJob.Price.ToString("C") != Price.ToString() ||
                        SelectedJob.AmountReceived.ToString("C") != AmountReceived.ToString() ||
                        (SelectedJob.StartDate.ToString() == "01/01/0001" ? string.Empty : SelectedJob.StartDate.ToString()) != StartDate.ToString() ||
                        SelectedJob.DueDate.ToString() != DueDate.ToString())
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
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: HandleKeyPress() FAIL\n\t{ex.Message}");
            }
        }

        public async Task DeleteJob()
        {
            try
            {
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
                    await Task.Run(async() => { 
                        // delete job folder (if only one job then delete whole client folder)
                        // it deletes the current job folder then it checks if it was the last one if true, then delete client folder
                        if (await FileStore.DeleteJobFolder(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName.Trim() : SelectedJob.CompanyName.Trim(), SelectedJob.JobTitle.Trim()) == true)
                        {
                            // delete entire client folder too
                            await FileStore.DeleteClientFolder(SelectedJob.ClientId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName.Trim() : SelectedJob.CompanyName.Trim());

                            // delete client from database
                            await Database.DeleteClient(SelectedJob.ClientId);
                        }
                        else if (await FileStore.DeleteJobFolder(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName.Trim() : SelectedJob.CompanyName.Trim(), SelectedJob.JobTitle.Trim()) == false)
                        {
                            // delete job from database
                            await Database.DeleteJob(SelectedJob.JobId);
                        }

                        await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
                        await TryCloseAsync();                   
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
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: DeleteJob() FAIL\n\t{ex.Message}");
            }
        }

        public async Task Exit()
        {
            try
            {
                string startDate = SelectedJob.StartDate.ToString() == "01/01/0001" ? string.Empty : SelectedJob.StartDate.ToString();
                if (SelectedJob.JobTitle != JobTitle ||
                    SelectedJob.JobDescription != JobDescription ||
                    SelectedJob.JobStatus != Status ||
                    SelectedJob.Price.ToString("C") != Price.ToString() ||
                    SelectedJob.AmountReceived.ToString("C") != AmountReceived.ToString() ||
                    (SelectedJob.StartDate.ToString() == "01/01/0001" ? string.Empty : SelectedJob.StartDate.ToString()) != StartDate.ToString() ||
                    SelectedJob.DueDate.ToString() != DueDate.ToString())
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
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: Exit() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Private Functions
        private Task ValidateDate()
        {
            try
            {
                if (string.IsNullOrEmpty(StartDate) == false)
                {
                    bool startDate = DateTime.TryParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStartDate);
                    bool dueDate = DateTime.TryParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDueDate);

                    if (startDate == true && dueDate == true)
                    {
                        if (DateOnly.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) < DateOnly.ParseExact(DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))
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
                    else
                    {
                        EnableSubmitBtn = false;
                        OpacitySubmitBtn = _halfOpacity;
                    }
                }
                else
                {
                    EnableSubmitBtn = true;
                    OpacitySubmitBtn = _fullOpacity;
                }
            }
            catch (Exception ex)
            {
                if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == _state.messageBoxVM) == false)
                {
                    _state.messageBoxVM.Symbol = 2;
                    _state.messageBoxVM.HeadMessage = "Validate Date";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: ValidateDate() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task ValidateAmountDifference()
        {
            try
            {
                if (string.IsNullOrEmpty(Price) == false && string.IsNullOrEmpty(AmountReceived) == false)
                {
                    if (decimal.TryParse(Price, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal PriceRes) == true && decimal.TryParse(AmountReceived, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal AmountReceivedRes) == true)
                    {
                        if (decimal.Parse(AmountReceived, NumberStyles.Currency, CultureInfo.CurrentCulture) <= decimal.Parse(Price, NumberStyles.Currency, CultureInfo.CurrentCulture))
                        {
                            EnableSubmitBtn = true;
                            OpacitySubmitBtn = _fullOpacity;
                        }
                        else
                        {
                            // cap remaining amount to 0
                            RemainingAmount = "0.00";

                            EnableSubmitBtn = false;
                            OpacitySubmitBtn = _halfOpacity;
                        }
                    }
                    else
                    {
                        EnableSubmitBtn = false;
                        OpacitySubmitBtn = _halfOpacity;
                    }
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
                    _state.messageBoxVM.HeadMessage = "Validate Amount";
                    _state.messageBoxVM.Message = ex.Message;
                    _state.messageBoxVM.ButtonStyle = Names.OK;
                    _windowManager.ShowDialogAsync(_state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                }
                Logger.LogActivity(Logger.ERROR, $"EditJobViewModel: ValidateAmountDifference() FAIL\n\t{ex.Message}");
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Public View Variables
        public string BusinessName
        {
            get { return _businessName; }
            set
            {
                _businessName = value;
                NotifyOfPropertyChange(() => BusinessName);
            }
        }

        public DateOnly CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                NotifyOfPropertyChange(() => CreatedDate);
            }
        }

        public string JobTitle
        {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                NotifyOfPropertyChange(() => JobTitle);
            }
        }

        public string JobDescription
        {
            get { return _jobDescription; }
            set
            {
                _jobDescription = value;
                NotifyOfPropertyChange(() => JobDescription);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        public string Price
        {
            get
            {
                if (_isEditingPrice == true)
                {
                    return _priceText;
                }

                return _price.ToString("C", CultureInfo.CurrentCulture);
            }
            set
            {
                _priceText = value;

                decimal parsed;

                bool success = decimal.TryParse(
                    value,
                    NumberStyles.Any,
                    CultureInfo.CurrentCulture,
                    out parsed);

                if (success)
                {
                    _price = parsed;

                    RemainingAmount =
                        (_price - _amountReceived)
                        .ToString("C", CultureInfo.CurrentCulture);

                    NotifyOfPropertyChange(nameof(RemainingAmount));
                }

                NotifyOfPropertyChange(nameof(Price));
                ValidateAmountDifference();
            }
        }

        public string AmountReceived
        {
            get
            {
                if (_isEditingReceived == true)
                {
                    return _amountReceivedText;
                }

                return _amountReceived.ToString("C", CultureInfo.CurrentCulture);
            }

            set
            {
                _amountReceivedText = value;

                decimal parsed;

                bool success = decimal.TryParse(
                    value,
                    NumberStyles.Any,
                    CultureInfo.CurrentCulture,
                    out parsed);

                if (success)
                {
                    _amountReceived = parsed;
                }

                RemainingAmount =
                    (_price - _amountReceived)
                    .ToString("C", CultureInfo.CurrentCulture);

                NotifyOfPropertyChange(nameof(AmountReceived));
                NotifyOfPropertyChange(nameof(RemainingAmount));
                ValidateAmountDifference();
            }
        }

        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyOfPropertyChange(() => StartDate);
                ValidateDate();
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
                ValidateDate();
            }
        }

        public string RemainingAmount
        {
            get { return _remainingAmount; }
            set
            {
                if (decimal.Parse(value, NumberStyles.Currency, CultureInfo.CurrentCulture) >= 0.0m)
                {
                    _remainingAmount = value;
                    NotifyOfPropertyChange(() => RemainingAmount);
                }
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