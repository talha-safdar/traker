using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traker.Helper;
using Traker.Services;

namespace Traker.Models
{
    public class MoneyInfoModel
    {
        public string NewJobsCount { get; set; } = string.Empty;
        public string DoneJobsCount { get; set; } = string.Empty;
        public string ActiveJobsCount { get; set; } = string.Empty;
        public string InvoicedJobsCount { get; set; } = string.Empty;
        public decimal GrossAmount { get; set; } = 0.0m;
        public decimal ReceivedAmount { get; set; } = 0.0m;
        public decimal OutstandingAmount { get; set; } = 0.0m;
        public decimal OverdueAmount { get; set; } = 0.0m;
    }
}
