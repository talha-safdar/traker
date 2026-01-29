using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models
{
    public class JobModel
    {
        public int JobId { get; set; } = 0;
        public int ClientId { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; } = 0.0m;
        public decimal FinalPrice { get; set; } = 0.0m;
        public DateTime CreatedDate { get; set; } = new DateTime();
        public DateTime StartDate { get; set; } = new DateTime();
        public DateTime CompletedDate { get; set; } = new DateTime();
        public DateTime DueDate { get; set; } = new DateTime();
        public string FolderPath { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsArchived { get; set; } = false;
    }
}