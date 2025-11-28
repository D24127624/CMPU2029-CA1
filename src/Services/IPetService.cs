
namespace kms.Services
{

    using kms.Models;

    /// <summary>
    /// Interface for pet management operations.
    /// </summary>
    public interface IPetService
    {

        Task<List<Pet>> GetPetsByOwnerAsync(int ownerId);
        Task<Pet> RegisterPetAsync(Pet pet);
        Task<bool> RemovePetAsync(int petId);
        Task<List<Pet>> SearchPetsAsync(string name, PetType type);
        Task<List<Pet>> SearchPetsByOwnerAsync(string? owner, string? phone);
        Task<bool> UpdatePetAsync(Pet pet);

    }

}
