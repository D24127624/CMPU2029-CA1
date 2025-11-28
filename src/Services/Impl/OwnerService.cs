
namespace kms.Services.Impl
{

    using kms.Models;
    using kms.Storage;

    /// <summary>
    /// Service for managing pets.
    /// </summary>
    public class OwnerService(IPersistedStorage storage) : IOwnerService
    {

        private readonly IPersistedStorage _storage = storage;

        public Task<List<Owner>> GetAllOwnersAsync() =>
            _storage.GetAllOwnersAsync();

        public Task<Owner> RegisterOwnerAsync(Owner owner) =>
            _storage.RegisterOwnerAsync(owner);

        public Task<bool> RemoveOwnerAsync(int ownerId) =>
            _storage.RemoveOwnerAsync(ownerId);

        public Task<List<Owner>> SearchOwnersAsync(string? owner, string? phone) =>
            _storage.SearchOwnersAsync(owner, phone);

        public Task<bool> UpdateOwnerAsync(Owner owner) =>
            _storage.UpdateOwnerAsync(owner);

    }

}
