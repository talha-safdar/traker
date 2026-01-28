using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class Invoices
    {
        public int InvoiceId { get; set; } = 0;
        public int JobId { get; set; } = 0;
        public decimal Amount { get; set; } = 0.0m;
        public bool IsPaid { get; set; } = false;
    }
}