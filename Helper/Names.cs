using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Helper
{
    public static class Names
    {
        // job status
        public const string New = "New";
        public const string Active = "Active";
        public const string Done = "Done";
        public const string Invoiced = "Invoiced";
        public const string Overdue = "Overdue";
        public const string Paid = "Paid";

        // business type
        public const string Individual = "Individual";
        public const string Company = "Company";
        // events
        public const string SetupCompleted = "SetupCompleted"; // for the user setup completion

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
    }
}
