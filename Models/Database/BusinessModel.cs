using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models.Database
{
    public class BusinessModel
    {
        public int BusinessId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string VatNumber { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
    }
}
