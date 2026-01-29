using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class InvoicesModel
    {
        public int InvoiceId { get; set; } = 0;
        public int JobId { get; set; } = 0;
        public decimal Amount { get; set; } = 0.0m;
        public DateTime IssueDate { get; set; } = new DateTime();
        public DateTime DueDate { get; set; } = new DateTime();
        public bool IsPaid { get; set; } = false;
    }
}