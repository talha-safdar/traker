using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models.Database
{
    public class BankModel
    {
        public int BankId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string BankName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string SortCode { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BIC { get; set; } = string.Empty;
    }
}