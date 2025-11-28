
namespace kms.Services.Impl
{

    using kms.Models;
    using kms.Storage;

    /// <summary>
    /// Service for managing kennels.
    /// </summary>
    public class KennelService(IPersistedStorage storage) : IKennelService
    {

        private readonly IPersistedStorage _storage = storage;

        public Task<Kennel> AddKennelAsync(Kennel kennel) =>
            _storage.AddKennelAsync(kennel);

        public Task<bool> UpdateKennelAsync(Kennel kennel) =>
            _storage.UpdateKennelAsync(kennel);

        public Task<bool> RemoveKennelAsync(int kennelId) =>
            _storage.RemoveKennelAsync(kennelId);
        public Task<List<Kennel>> FindAvailableKennelsForDateRange(Pet pet, DateTime startDate, DateTime endDate) =>
            _storage.FindAvailableKennelsForDateRange(pet, startDate, endDate);

        public Task<List<Kennel>> GetKennelsAsync() =>
            _storage.GetKennelsAsync();

    }

}
