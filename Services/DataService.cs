using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Services
{
    using Database;
    using Traker.Models.Database;

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
                await Task.Run(() =>
                {
                    Clients = Database.FetchClientsTable(); // clients
                    Jobs = Database.FetchJobsTable(); // jobs
                    Invoices = Database.FetchInvoiceTable(); // invoices
                    User = Database.FetchUserTable(); // user
                    Business = Database.FetchBusinessTable(); // business
                    Bank = Database.FetchBankTable(); // bank
                });
        }

        public async Task ClearDataVariables()
        {
            await Task.Run(() =>
            {
                Clients.Clear();
                Jobs.Clear();
                Invoices.Clear();
            });
        }

        //public async Task DeleteJob(int clientId)
        //{
        //    await Task.Run(async() =>
        //    {
        //        await Database.DeleteRow(clientId);
        //    });
        //}
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