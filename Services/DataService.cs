using Caliburn.Micro;

namespace Traker.Services
{
    using Database;
    using System.Windows;
    using Traker.Helper;
    using Traker.Models.Database;
    using Traker.States;

    public class DataService : PropertyChangedBase
    {
        /// <summary>
        /// A data service to locally store all the tables and data
        /// localy. It also provides methods to retrieve, manipulate, and store data
        /// </summary>
        #region Private Data Variables
        private List<ClientsModel> _clients = new List<ClientsModel>();
        private List<JobsModel> _jobs = new List<JobsModel>();
        private List<InvoicesModel> _invoices = new List<InvoicesModel>();
        private List<UserModel> _user = new List<UserModel>();
        private List<BusinessModel> _business = new List<BusinessModel>();
        private List<BankModel> _bank = new List<BankModel>();
        #endregion

        #region Public Data Functions

        public async Task FetchDatabase()
        {
            await Task.Run(async() =>
            {
                try
                {
                    Clients = Database.FetchClientsTable(); // clients
                    Jobs = Database.FetchJobsTable(); // jobs
                    Invoices = Database.FetchInvoicesTable(); // invoices
                    User = Database.FetchUserTable(); // user
                    Business = Database.FetchBusinessTable(); // business
                    Bank = Database.FetchBankTable(); // bank
                }
                catch (Exception ex)
                {
                    await Execute.OnUIThreadAsync(async() =>
                    {
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Fetch Database Error";
                            state.messageBoxVM.Message = $"DataService\n\n{ex.Message}";
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            await windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                    });
                    Logger.LogActivity(Logger.ERROR, $"DataService: FetchDatabase() FAIL\n\t{ex.Message}");
                }
            });
        }

        public async Task ClearDataVariables()
        {
            await Task.Run(async () =>
            {
                try
                {
                    Clients.Clear();
                    Jobs.Clear();
                    Invoices.Clear();
                }
                catch (Exception ex)
                {
                    await Execute.OnUIThreadAsync(async () =>
                    {
                        AppState state = IoC.Get<AppState>();
                        IWindowManager windowManager = IoC.Get<IWindowManager>();
                        if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                        {
                            state.messageBoxVM.Symbol = 2;
                            state.messageBoxVM.HeadMessage = "Clear Data Variables Error";
                            state.messageBoxVM.Message = $"DataService\n\n{ex.Message}";
                            state.messageBoxVM.ButtonStyle = Names.OK;
                            await windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                        }
                    });
                    Logger.LogActivity(Logger.ERROR, $"DataService: ClearDataVariables() FAIL\n\t{ex.Message}");
                }
            });
        }

        public async Task RefreshDatabase()
        {
            try
            {
                await ClearDataVariables();
                await FetchDatabase();
            }
            catch (Exception ex)
            {
                await Execute.OnUIThreadAsync(async () =>
                {
                    AppState state = IoC.Get<AppState>();
                    IWindowManager windowManager = IoC.Get<IWindowManager>();
                    if (Application.Current.Windows.OfType<Window>().Any(w => w.DataContext == state.messageBoxVM) == false)
                    {
                        state.messageBoxVM.Symbol = 2;
                        state.messageBoxVM.HeadMessage = "Refresh Database Error";
                        state.messageBoxVM.Message = $"DataService\n\n{ex.Message}";
                        state.messageBoxVM.ButtonStyle = Names.OK;
                        await windowManager.ShowDialogAsync(state.messageBoxVM, null, CustomWindow.SettingsForDialog(450, 250, false));
                    }
                });
                Logger.LogActivity(Logger.ERROR, $"DataService: RefreshDatabase() FAIL\n\t{ex.Message}");
            }
        }
        #endregion

        #region Public Data Variables
        public List<ClientsModel> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                NotifyOfPropertyChange(() => Clients);
            }
        }

        public List<JobsModel> Jobs
        {
            get { return _jobs; }
            set
            {
                _jobs = value;
                NotifyOfPropertyChange(() => Jobs);
            }
        }

        public List<InvoicesModel> Invoices
        {
            get { return _invoices; }
            set
            {
                _invoices = value;
                NotifyOfPropertyChange(() => Invoices);
            }
        }

        public List<UserModel> User
        {
            get { return _user; }
            set
            {
                _user = value;
                NotifyOfPropertyChange(() => User);
            }
        }

        public List<BusinessModel> Business
        {
            get { return _business; }
            set
            {
                _business = value;
                NotifyOfPropertyChange(() => Business);
            }
        }

        public List<BankModel> Bank
        {
            get { return _bank; }
            set
            {
                _bank = value;
                NotifyOfPropertyChange(() => Bank);
            }
        }
        #endregion
    }
}