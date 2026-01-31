using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class ClientsModel
    {
        public int ClientId { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = new DateTime();
        public bool IsActive { get; set; } = false;
    }
}