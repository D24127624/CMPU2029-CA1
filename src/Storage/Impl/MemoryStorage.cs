
namespace kms.Storage.Impl
{

    using log4net;

    using kms.Models;
    using kms.Util;

    /// <summary>
    /// SQLite implementation of the persisted storage interface.
    /// </summary>
    public class MemoryStorage : IPersistedStorage
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof(MemoryStorage));

        private readonly Dictionary<int, Booking> _bookingsTable = [];
        private readonly Dictionary<int, Kennel> _kennelsTable = [];
        private readonly Dictionary<int, Owner> _ownersTable = [];
        private readonly Dictionary<int, Pet> _petsTable = [];

        // Owners
        public Task<List<Owner>> GetAllOwnersAsync() =>
            Task.FromResult<List<Owner>>([.. _ownersTable.Values]);

        public Task<Owner> RegisterOwnerAsync(Owner owner)
        {
            int ownerId = _ownersTable.Keys.Count + 1;
            owner.Id = ownerId;
            _ownersTable[ownerId] = owner;
            return Task.FromResult<Owner>(owner);
        }

        public Task<bool> RemoveOwnerAsync(int ownerId)
        {
            _ownersTable.Remove(ownerId);
            foreach (var entry in _petsTable)
            {
                if (entry.Value.Owner.Id == ownerId)
                {
                    RemovePetAsync(entry.Key);
                }
            }
            return Task.FromResult<bool>(true);
        }

        public Task<List<Owner>> SearchOwnersAsync(string? owner, string? phone)
        {
            List<Owner> results = [];
            foreach (var entry in _ownersTable)
            {
                if ((owner != null && entry.Value.Name.Contains(owner, StringComparison.CurrentCultureIgnoreCase)) ||
                    (phone != null && entry.Value.PhoneNumber.Contains(phone, StringComparison.CurrentCultureIgnoreCase)))
                {
                    results.Add(entry.Value);
                }
            }
            return Task.FromResult<List<Owner>>(results);
        }

        public Task<bool> UpdateOwnerAsync(Owner owner)
        {
            _ownersTable[owner.Id] = owner;
            return Task.FromResult<bool>(true);
        }

        // Pets
        public Task<List<Pet>> GetPetsByOwnerAsync(int ownerId)
        {
            List<Pet> results = [];
            foreach (var entry in _petsTable)
            {
                if (entry.Value.Owner.Id == ownerId)
                {
                    results.Add(entry.Value);
                }
            }
            return Task.FromResult<List<Pet>>(results);
        }

        public Task<Pet> RegisterPetAsync(Pet pet)
        {
            int petId = _petsTable.Keys.Count + 1;
            pet.Id = petId;
            _petsTable[petId] = pet;
            return Task.FromResult<Pet>(pet);
        }

        public Task<bool> RemovePetAsync(int petId)
        {
            _petsTable.Remove(petId);
            foreach (var entry in _bookingsTable)
            {
                if (entry.Value.Pet.Id == petId)
                {
                    _bookingsTable.Remove(entry.Key);
                }
            }
            return Task.FromResult<bool>(true);
        }

        public Task<List<Pet>> SearchPetsAsync(string name, PetType type)
        {
            List<Pet> results = [];
            foreach (var entry in _petsTable)
            {
                if (entry.Value.PetType == type && entry.Value.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    results.Add(entry.Value);
                }
            }
            return Task.FromResult<List<Pet>>(results);
        }

        public Task<List<Pet>> SearchPetsByOwnerAsync(string? owner, string? phone)
        {
            List<Pet> results = [];
            foreach (var entry in _petsTable)
            {
                if ((owner != null && entry.Value.Owner.Name.Contains(owner, StringComparison.CurrentCultureIgnoreCase)) ||
                    (phone != null && entry.Value.Owner.PhoneNumber.Contains(phone, StringComparison.CurrentCultureIgnoreCase)))
                {
                    results.Add(entry.Value);
                }
            }
            return Task.FromResult<List<Pet>>(results);
        }

        public Task<bool> UpdatePetAsync(Pet pet)
        {
            _petsTable[pet.Id] = pet;
            return Task.FromResult<bool>(true);
        }

        // Bookings
        public Task<bool> CancelBookingsAsync(string groupId)
        {
            foreach (var entry in _bookingsTable)
            {
                if (entry.Value.GroupId == groupId)
                {
                    _bookingsTable.Remove(entry.Key);
                }
            }
            return Task.FromResult<bool>(true);
        }

        public Task<List<Booking>> CreateBookingsAsync(List<Booking> bookings)
        {
            List<Booking> results = [];
            foreach (Booking booking in bookings)
            {
                int bookingId = _bookingsTable.Keys.Count + 1;
                booking.Id = bookingId;
                _bookingsTable[bookingId] = booking;
                results.Add(booking);
            }
            return Task.FromResult<List<Booking>>(results);
        }

        public Task<List<BookingGroup>> SearchBookingsScheduledAsync(DateTime startDate, DateTime? endDate = null)
        {
            List<BookingGroup> results = [];
            foreach (var entry in GetBookingGroups())
            {
                if (endDate == null && entry.Value.StartDate == startDate)
                {
                    results.Add(entry.Value);
                }
                else if (endDate != null && entry.Value.StartDate >= startDate && entry.Value.StartDate <= endDate)
                {
                    results.Add(entry.Value);
                }

            }
            return Task.FromResult<List<BookingGroup>>(results);
        }

        public Task<List<BookingGroup>> SearchBookingsByOwnerAsync(string? owner, string? phone)
        {
            List<BookingGroup> results = [];
            foreach (var entry in GetBookingGroups())
            {
                Pet pet = entry.Value.Pet;
                if ((owner != null && pet.Owner.Name.Contains(owner, StringComparison.CurrentCultureIgnoreCase)) ||
                    (phone != null && pet.Owner.PhoneNumber.Contains(phone, StringComparison.CurrentCultureIgnoreCase)))
                {
                    results.Add(entry.Value);
                }

            }
            return Task.FromResult<List<BookingGroup>>(results);
        }

        public Task<List<BookingGroup>> SearchBookingsByPetAsync(string name, PetType type)
        {
            List<BookingGroup> results = [];
            foreach (var entry in GetBookingGroups())
            {
                Pet pet = entry.Value.Pet;
                if (pet.PetType == type && pet.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    results.Add(entry.Value);
                }
            }
            return Task.FromResult<List<BookingGroup>>(results);
        }

        // Kennels
        public Task<Kennel> AddKennelAsync(Kennel kennel)
        {
            int kennelId = _kennelsTable.Keys.Count + 1;
            kennel.Id = kennelId;
            _kennelsTable[kennelId] = kennel;
            return Task.FromResult<Kennel>(kennel);
        }

        public Task<List<Kennel>> GetKennelsAsync() =>
            Task.FromResult<List<Kennel>>([.. _kennelsTable.Values]);

        public Task<List<Kennel>> FindAvailableKennelsForDateRange(Pet pet, DateTime startDate, DateTime endDate)
        {
            // get all occupied kennels
            List<int> occupiedIds = [];
            foreach (var entry in GetBookingGroups())
            {
                if (entry.Value.StartDate <= startDate && entry.Value.EndDate >= endDate)
                {
                    occupiedIds.Add(entry.Value.Kennel.Id);
                }
            }
            List<Kennel> results = [];
            foreach (var entry in _kennelsTable)
            {
                if (!occupiedIds.Contains(entry.Key) && entry.Value.SuitableFor == pet.PetType && (pet.PetType == PetType.CAT || entry.Value.Size == ((Dog)pet).Size))
                {
                    results.Add(entry.Value);
                }
            }
            return Task.FromResult<List<Kennel>>(results);
        }

        public Task<bool> RemoveKennelAsync(int kennelId)
        {
            foreach (var entry in _bookingsTable)
            {
                if (entry.Value.Kennel.Id == kennelId)
                {
                    if (entry.Value.Date > DateTime.Today)
                    {
                        return Task.FromResult<bool>(false);
                    }
                    _bookingsTable.Remove(entry.Key);
                }
            }
            _kennelsTable.Remove(kennelId);
            return Task.FromResult<bool>(true);
        }

        public Task<bool> UpdateKennelAsync(Kennel kennel)
        {
            _kennelsTable[kennel.Id] = kennel;
            return Task.FromResult<bool>(true);
        }

        // shared\utility functions
        private Dictionary<string, BookingGroup> GetBookingGroups()
        {
            Dictionary<string, BookingGroup> results = [];
            foreach (var entry in _bookingsTable)
            {
                string groupId = entry.Value.GroupId;
                BookingGroup? group = results.GetValueOrDefault(groupId);
                // new entry
                if (group == null)
                {
                    group = ModelUtils.CreateBookingGroup(entry.Value.Pet, entry.Value.Kennel, entry.Value.Date, entry.Value.Date, groupId);
                    results[groupId] = group;
                }
                // update existing entry
                else
                {
                    // find first(start) date
                    if (group.StartDate >= entry.Value.Date)
                    {
                        group.StartDate = entry.Value.Date;
                    }

                    // find last(end) date
                    else if (group.EndDate <= entry.Value.Date)
                    {
                        group.EndDate = entry.Value.Date;
                    }
                }
            }
            return results;
        }

    }

}
