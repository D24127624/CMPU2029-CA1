
namespace kms.Storage
{

    using kms.Models;

    /// <summary>
    /// Interface for persisted storage operations.
    /// </summary>
    public interface IPersistedStorage
    {

        // Owners
        Task<List<Owner>> GetAllOwnersAsync();
        Task<Owner> RegisterOwnerAsync(Owner owner);
        Task<bool> RemoveOwnerAsync(int ownerId);
        Task<List<Owner>> SearchOwnersAsync(string? owner, string? phone);
        Task<bool> UpdateOwnerAsync(Owner owner);

        // Pets
        Task<List<Pet>> GetPetsByOwnerAsync(int ownerId);
        Task<Pet> RegisterPetAsync(Pet pet);
        Task<bool> RemovePetAsync(int petId);
        Task<List<Pet>> SearchPetsAsync(string name, PetType type);
        Task<List<Pet>> SearchPetsByOwnerAsync(string? owner, string? phone);
        Task<bool> UpdatePetAsync(Pet pet);

        // Bookings
        Task<bool> CancelBookingsAsync(string groupId);
        Task<List<Booking>> CreateBookingsAsync(List<Booking> bookings);
        Task<List<BookingGroup>> SearchBookingsScheduledAsync(DateTime startDate, DateTime? endDate = null);
        Task<List<BookingGroup>> SearchBookingsByOwnerAsync(string? owner, string? phone);
        Task<List<BookingGroup>> SearchBookingsByPetAsync(string name, PetType type);

        // Kennels
        Task<Kennel> AddKennelAsync(Kennel kennel);
        Task<List<Kennel>> GetKennelsAsync();
        Task<List<Kennel>> FindAvailableKennelsForDateRange(Pet pet, DateTime startDate, DateTime endDate);
        Task<bool> RemoveKennelAsync(int kennelId);
        Task<bool> UpdateKennelAsync(Kennel kennel);

    }
}
