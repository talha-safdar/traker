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
    public class JobsListViewModel : Screen
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
        }

        #region Caliburn Functions
        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            try
            {
                List<JobsModel> jobsModel = await Database.getJobsByClientId(SelectedJob.ClientId);

                _businessName = SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName;
                _clientType = SelectedJob.TypeIcon;

                foreach (var job in jobsModel)
                {
                    DashboardModel currentJob = new DashboardModel
                    {
                        ClientId = job.ClientId,
                        TypeIcon = SelectedJob.TypeIcon,
                        ClientName = SelectedJob.ClientName,
                        ClientEmail = SelectedJob.ClientEmail,
                        ClientPhone = SelectedJob.ClientPhone,
                        CompanyName = SelectedJob.CompanyName,
                        Address = SelectedJob.Address,
                        City = SelectedJob.City,
                        Postcode = SelectedJob.Postcode,
                        Country = SelectedJob.Country,
                        CreatedDate = SelectedJob.CreatedDate,
                        IsActive = SelectedJob.IsActive,

                        JobId = job.JobId,
                        JobTitle = job.Title,
                        JobDescription = job.Description,
                        Price = job.FinalPrice,
                        AmountReceived = job.AmountReceived,
                        JobStatus = job.Status.ToString(),
                        StartDate = job.StartDate,
                        DueDate = job.DueDate,

                        HasInvoice = await Database.CheckIfInvoicedByJobId(job.JobId),
                        InvoiceStatus = await Database.GetInvoiceStatusByJobId(job.JobId) ?? Names.NotInvoiced
                    };
                    _jobsList.Add(currentJob);
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
                Logger.LogActivity(Logger.ERROR, $"JobsListViewModel: OnInitializedAsync() FAIL\n\t{ex.Message}");
            }
            await base.OnInitializedAsync(cancellationToken);
        }
        #endregion

        #region Public View FUnctions
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