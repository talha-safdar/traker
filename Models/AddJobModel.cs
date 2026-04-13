namespace Traker.Models
{
    /// <summary>
    /// Represents the data required to add a new job for a client.
    /// </summary>
    /// <remarks>This model is typically used to capture user input when creating a new job entry, such as in
    /// a form submission scenario.</remarks>
    public class AddJobModel
    {
        public int ClientId { get; set; } = 0;
        public string ClientType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
    }
}