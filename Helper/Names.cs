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
    }
}
