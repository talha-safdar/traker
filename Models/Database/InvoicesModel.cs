using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models.Database
{
    /// <summary>
    /// Invocies table model
    /// </summary>
    public class InvoicesModel
    {
        public int InvoiceId { get; set; } = 0;
        public int JobId { get; set; } = 0;
        public int InvoiceNumber { get; set; } = 0;
        public decimal Subtotal { get; set; } = 0.0m;
        public decimal TaxAmount { get; set; } = 0.0m;
        public decimal TotalAmount { get; set; } = 0.0m;
        public DateTime IssueDate { get; set; } = new DateTime();
        public DateOnly DueDate { get; set; } = new DateOnly();
        public string BillingName { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingPostcode { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateOnly PaidDate { get; set; } = new DateOnly();
        public string PaymentMethod { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string InvoiceName { get; set; } = string.Empty;
    }
}