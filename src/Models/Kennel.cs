
namespace kms.Models
{

    /// <summary>
    /// Represents a kennel resource available for a booking.
    /// </summary>
    public class Kennel
    {

        public int Id { get; set; }
        public required string Name { get; set; }
        public required Size Size { get; set; }
        public required PetType SuitableFor { get; set; }
        public required bool IsOutOfService { get; set; }
        public string? OutOfServiceComment { get; set; }

    }

}
