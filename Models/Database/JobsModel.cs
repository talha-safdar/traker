using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.Models.Database
{
    /// <summary>
    /// Represents a job record, including details such as client association, status, pricing, scheduling, and archival
    /// state.
    /// </summary>
    /// <remarks>The <c>JobsModel</c> class encapsulates information about a job, including identifiers,
    /// descriptive fields, pricing estimates, important dates, and status indicators. It is typically used to transfer
    /// job-related data between application layers or to persist job information in storage.</remarks>
    public class JobsModel
    {
        public int JobId { get; set; } = 0;
        public int ClientId { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; } = 0.0m;
        public decimal FinalPrice { get; set; } = 0.0m;
        public decimal AmountReceived { get; set; } = 0.0m;
        public DateOnly CreatedDate { get; set; } = new DateOnly();
        public DateOnly StartDate { get; set; } = new DateOnly();
        public DateOnly CompletedDate { get; set; } = new DateOnly();
        public DateOnly DueDate { get; set; } = new DateOnly();
        public string FolderPath { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsArchived { get; set; } = false;
    }
}