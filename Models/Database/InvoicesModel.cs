using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models.Database
{
    /// <summary>
    /// Represents an invoice, including billing details, amounts, status, and payment information.
    /// </summary>
    /// <remarks>The <see cref="InvoicesModel"/> class encapsulates all relevant data for a single invoice,
    /// such as identifiers, billing information, monetary amounts, and payment status. This model is typically used for
    /// transferring invoice data between application layers or for serialization.</remarks>
    public class InvoicesModel
    {
        public int InvoiceId { get; set; } = 0;
        public int JobId { get; set; } = 0;
        public int InvoiceNumber { get; set; } = 0;
        public decimal Subtotal { get; set; } = 0.0m;
        public decimal TaxAmount { get; set; } = 0.0m;
        public decimal TotalAmount { get; set; } = 0.0m;
        public DateOnly IssueDate { get; set; } = new DateOnly();
        public DateOnly DueDate { get; set; } = new DateOnly();
        public string BillingName { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingPostcode { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string InvoiceName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateOnly PaidDate { get; set; } = new DateOnly();
        public string PaymentMethod { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}