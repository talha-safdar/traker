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
        public static string New { get; } = "New";
        public static string Active { get; } = "Active";
        public static string Done { get; } = "Done";
        public static string Invoiced { get; } = "Invoiced";
        public static string Paid { get; } = "Paid";

        // business type
        public static string Individual { get; } = "Individual";
        public static string Company { get; } = "Company";

        // events
        public static string SetupCompleted { get; } = "SetupCompleted"; // for the user setup completion

        // sort/filter
        public static string ClientNameAsc { get; } = "ClientNameAsc";
        public static string ClientNameDesc { get; } = "ClientNameDesc";
        public static string JobTitleAsc { get; } = "JobTitleAsc";
        public static string JobTitleDesc { get; } = "JobTitleDesc";
        public static string JobStatusAsc { get; } = "JobStatusAsc";
        public static string JobStatusDesc { get; } = "JobStatusDesc";
        public static string JobPriceAsc { get; } = "JobPriceAsc";
        public static string JobPriceDesc { get; } = "JobPriceDesc";
        public static string DueDateAsc { get; } = "DueDateAsc";
        public static string DueDateDesc { get; } = "DueDateDesc";
        public static string CreatedDateAsc { get; } = "CreatedDateAsc";
        public static string CreatedDateDesc { get; } = "CreatedDateDesc";
        public static string ClientTypeAsc { get; } = "ClientTypeAsc";
        public static string ClientTypeDesc { get; } = "ClientTypeDesc";
    }
}
