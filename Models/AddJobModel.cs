using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class AddJobModel
    {
        public int ClientId { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
    }
}
