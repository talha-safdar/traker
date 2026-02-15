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
        public string InvoiceNumber { get; set; } = String.Empty;
        public decimal Subtotal { get; set; } = 0.0m;
        public decimal TaxAmount { get; set; } = 0.0m;
        public decimal TotalAmount { get; set; } = 0.0m;
        public DateTime IssueDate { get; set; } = new DateTime();
        public DateTime DueDate { get; set; } = new DateTime();
        public string Status { get; set; } = String.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime PaidDate { get; set; } = new DateTime();
        public string PaymentMethod { get; set; } = String.Empty;
        public string Notes { get; set; } = String.Empty;
    }
}