
namespace kms.Services.Impl
{

    using kms.Models;
    using kms.Storage;
    using kms.Util;

    /// <summary>
    /// Service for managing bookings.
    /// </summary>
    public class BookingService(IPersistedStorage storage) : IBookingService
    {

        private readonly IPersistedStorage _storage = storage;

        public Task<bool> CancelBookingsAsync(string groupId) =>
            _storage.CancelBookingsAsync(groupId);

        public Task<List<Booking>> CreateBookingsAsync(BookingGroup group)
        {
            List<Booking> bookings = [];

            DateTime startDate = group.StartDate;
            while (startDate <= group.EndDate)
            {
                bookings.Add(ModelUtils.CreateBooking(group.GroupId, group.Pet, group.Kennel, startDate));
                startDate = startDate.AddDays(1);
            }
            return _storage.CreateBookingsAsync(bookings);
        }

        public Task<List<BookingGroup>> SearchBookingsScheduledAsync(DateTime startDate, DateTime? endDate = null) =>
            _storage.SearchBookingsScheduledAsync(startDate, endDate);

        public Task<List<BookingGroup>> SearchBookingsByOwnerAsync(string? owner, string? phone) =>
            _storage.SearchBookingsByOwnerAsync(owner, phone);

        public Task<List<BookingGroup>> SearchBookingsByPetAsync(string name, PetType type) =>
            _storage.SearchBookingsByPetAsync(name, type);

    }
}
