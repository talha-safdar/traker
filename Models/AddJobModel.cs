namespace Traker.Models
{
    /// <summary>
    /// For the Jobs list ViewModel, hold only
    /// potentially useful data, though it can expand
    /// </summary>
    public class AddJobModel
    {
        public int ClientId { get; set; } = 0;
        public string CreatedDate { get; set; } = string.Empty;
        public string ClientType { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
    }
}