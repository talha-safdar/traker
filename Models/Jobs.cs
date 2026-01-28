using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class Jobs
    {
        public int JobId { get; set; } = 0;
        public int ClientId { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
    }
}