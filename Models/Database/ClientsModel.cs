namespace Traker.Models.Database
{
    /// <summary>
    /// Represents a client, including personal or company details, contact information, and account status.
    /// </summary>
    /// <remarks>The <see cref="ClientsModel"/> class is used to store and transfer client information within
    /// the application. It supports both individual and company clients by providing relevant fields for each
    /// type.</remarks>
    public class ClientsModel
    {
        /// <summary>
        /// Business table model
        /// </summary>
        public int ClientId { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateOnly CreatedDate { get; set; } = new DateOnly();
        public string FolderName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}