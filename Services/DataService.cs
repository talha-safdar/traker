using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Models;

namespace Traker.Services
{
    using Database;

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
        #endregion

        #region Public Data Functions

        public async Task FetchDatabase()
        {
                await Task.Run(() =>
                {
                    Clients = Database.FetchClientsTable(); // clients
                    Jobs = Database.FetchJobsTable(); // jobs
                    Invoices = Database.FetchInvoiceTable(); // invoices
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
        #endregion
    }
}