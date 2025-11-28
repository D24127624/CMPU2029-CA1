
namespace kms.Models
{
    /// <summary>
    /// Represents a pet owner.
    /// </summary>
    public class Owner
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
