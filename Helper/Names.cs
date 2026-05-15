using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Helper
{
    /// <summary>
    /// Repetitive general terms should be palced here
    /// to prevnet typos
    /// </summary>
    public static class Names
    {
        // job status
        public const string New = "New";
        public const string Active = "Active";
        public const string Done = "Done";
        public const string Invoiced = "Invoiced";
        public const string Overdue = "Overdue";
        public const string Paid = "Paid";
        public const string NotInvoiced = "Not Invoiced";

        // business type
        public const string Individual = "Individual";
        public const string Company = "Company";

        // events
        public const string SetupCompleted = "SetupCompleted"; // for the user setup completion
        public const string ShowInvoice = "ShowInvoice"; // after invoice created open edit invoice

        // sort/filter
        public const string ClientNameAsc = "ClientNameAsc";
        public const string ClientNameDesc = "ClientNameDesc";
        public const string JobTitleAsc = "JobTitleAsc";
        public const string JobTitleDesc = "JobTitleDesc";
        public const string JobStatusAsc = "JobStatusAsc";
        public const string JobStatusDesc = "JobStatusDesc";
        public const string JobPriceAsc = "JobPriceAsc";
        public const string JobPriceDesc = "JobPriceDesc";
        public const string DueDateAsc = "DueDateAsc";
        public const string DueDateDesc = "DueDateDesc";
        public const string CreatedDateAsc = "CreatedDateAsc";
        public const string CreatedDateDesc = "CreatedDateDesc";
        public const string ClientTypeAsc = "ClientTypeAsc";
        public const string ClientTypeDesc = "ClientTypeDesc";
        public const string JobStatusNew = "JobStatusNew";
        public const string JobStatusActive = "JobStatusActive";
        public const string JobStatusDone = "JobStatusDone";
        public const string JobStatusInvoiced = "JobStatusInvoiced";
        public const string FilterIndividual = "FilterIndividual";
        public const string FilterComapny = "FilterComapny";
        public const string AllJobStatus = "AllJobStatus";
        public const string UnfilterClientType = "UnfilterClientType";

        // message box
        public const string Close = "Close"; // cloe entire app
        public const string ConfirmProceed = "ConfirmProceed"; // cloe entire app
        public const string DeleteClientConfirmation = "Are you sure you want to delete this client?";
        public const string DeleteJobConfirmation = "Are you sure you want to delete this job?";
        public const string DeleteInvoiceConfirmation = "Are you sure you want to delete this invoice?";
        public const string DiscardEsc = "You have unsaved progress.\n\nAre you sure you want to exit?";
        public const string OK = "OK";
        public const string CancelOK = "CancelOK";
        public const string NoYes = "NoYes";
        public const string LeaveForm = "LeaveForm";


        public const string Invoice = "Invoice";
    }
}
