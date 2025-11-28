
namespace kms.Models
{

    /// <summary>
    /// Represents a booking entry for a pet to stay.
    /// </summary>
    public class Booking
    {

        public int Id { get; set; }
        public required string GroupId { get; set; }
        public required Pet Pet { get; set; }
        public required Kennel Kennel { get; set; }
        public required DateTime Date { get; set; }

    }


    /// <summary>
    /// Represents a booking group for a pet to stay.
    /// </summary>
    public class BookingGroup
    {

        public required string GroupId { get; set; }
        public required Pet Pet { get; set; }
        public required Kennel Kennel { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }

    }
}
