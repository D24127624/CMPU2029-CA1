
namespace kms.Services
{

    using kms.Models;

    /// <summary>
    /// Interface for booking management operations.
    /// </summary>
    public interface IBookingService
    {

        Task<bool> CancelBookingsAsync(string groupId);
        Task<List<Booking>> CreateBookingsAsync(BookingGroup group);
        Task<List<BookingGroup>> SearchBookingsScheduledAsync(DateTime startDate, DateTime? endDate = null);
        Task<List<BookingGroup>> SearchBookingsByOwnerAsync(string? owner, string? phone);
        Task<List<BookingGroup>> SearchBookingsByPetAsync(string name, PetType type);

    }

}
