
namespace kms.Services.Impl
{

    using kms.Models;
    using kms.Storage;

    /// <summary>
    /// Service for managing pets.
    /// </summary>
    public class PetService(IPersistedStorage storage) : IPetService
    {

        public Task<List<Pet>> GetPetsByOwnerAsync(int ownerId) =>
            _storage.GetPetsByOwnerAsync(ownerId);

        private readonly IPersistedStorage _storage = storage;

        public Task<Pet> RegisterPetAsync(Pet pet) =>
            _storage.RegisterPetAsync(pet);

        public Task<bool> RemovePetAsync(int petId) =>
            _storage.RemovePetAsync(petId);

        public Task<List<Pet>> SearchPetsAsync(string name, PetType type) =>
            _storage.SearchPetsAsync(name, type);

        public Task<List<Pet>> SearchPetsByOwnerAsync(string? owner, string? phone) =>
            _storage.SearchPetsByOwnerAsync(owner, phone);

        public Task<bool> UpdatePetAsync(Pet pet) =>
            _storage.UpdatePetAsync(pet);

    }

}
