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
        #endregion

        #region Private View Variables
        private ObservableCollection<double> _activeDoneOpacity;
        private ObservableCollection<bool> _activeDoneEnable;
        #endregion

        public DashboardModel SelectedJob { get; set; } // data passed by DashboardVM
        public DataService Data { get; } // data from the database
        private AppState State { get; }

        #region Private Field Vairables
        private CreateInvoiceViewModel _createInvoice;
        private EditInvoiceViewModel _editInvoiceViewModel;
        #endregion


        public JobContextMenuViewModel(IEventAggregator events, IWindowManager windowManager, DataService data, AppState state)
        {
            _events = events;
            _windowManager = windowManager;
            Data = data;
            State = state;

            SelectedJob = new DashboardModel();

            _createInvoice = new CreateInvoiceViewModel(_events, Data, State, _windowManager);
            _editInvoiceViewModel = new EditInvoiceViewModel(Data, _events, _windowManager, State);
        }

        protected override Task OnInitializedAsync(CancellationToken cancellationToken)
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

                return base.OnInitializedAsync(cancellationToken);
        }

        public async Task SetStatus(string status)
        {
            await Database.SetJobStatus(status, SelectedJob.ClientId, SelectedJob.JobId);
            await _events.PublishOnUIThreadAsync(new RefreshDatabase()); // report back to dashboard for refresh
            await Data.RefreshDatabase();
            await TryCloseAsync();
        }

        public async Task OpenFolder()
        {
            // get list of jobs under the client ID
            List<JobsModel> jobDetails = new List<JobsModel>();
            jobDetails = Data.Jobs.Where(j => j.ClientId == SelectedJob.ClientId).ToList();

            await FileStore.LocateJobFolder(SelectedJob.ClientId, SelectedJob.JobId, SelectedJob.ClientType == Names.Individual ? SelectedJob.ClientName : SelectedJob.CompanyName, SelectedJob.JobTitle);
            await TryCloseAsync();;
        }

        public async Task EditInvoice()
        {
            await TryCloseAsync();
            _editInvoiceViewModel = new EditInvoiceViewModel(Data, _events, _windowManager, State);
            _editInvoiceViewModel.SelectedJob = SelectedJob;
            await _windowManager.ShowDialogAsync(_editInvoiceViewModel, null, CustomWindow.SettingsForDialog(800, 1000, false));
        }

        public async Task CreateInvoice()
        {
            await TryCloseAsync();
            _createInvoice = new CreateInvoiceViewModel(_events, Data, State, _windowManager);
            _createInvoice.SelectedJob = SelectedJob;
            _windowManager.ShowWindowAsync(_createInvoice, null, CustomWindow.SettingsForDialog(800, 1000, false));
        }

        public async Task EditClient()
        {
            await TryCloseAsync();
            await _events.PublishOnUIThreadAsync(new DashboardVMEvents() { Command = "EditClient" });
        }

        public async Task DeleteJob()
        {
            await TryCloseAsync();

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
            }
        }

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
        #endregion
    }
}