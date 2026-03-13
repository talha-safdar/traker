using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
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
        public string InvoiceNumber { get; set; } = String.Empty;
        public decimal Subtotal { get; set; } = 0.0m;
        public decimal TaxAmount { get; set; } = 0.0m;
        public decimal TotalAmount { get; set; } = 0.0m;
        public DateTime IssueDate { get; set; } = new DateTime();
        public DateTime DueDate { get; set; } = new DateTime();
        public string BillingName { get; set; } = String.Empty;
        public string BillingAddress { get; set; } = String.Empty;
        public string BillingCity { get; set; } = String.Empty;
        public string BillingPostcode { get; set; } = String.Empty;
        public string BillingCountry { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime PaidDate { get; set; } = new DateTime();
        public string PaymentMethod { get; set; } = String.Empty;
        public string Notes { get; set; } = String.Empty;
    }
}