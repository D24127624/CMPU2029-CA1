
namespace kms.Services
{

    using kms.Models;

    /// <summary>
    /// Interface for owner management operations.
    /// </summary>
    public interface IOwnerService
    {

        Task<List<Owner>> GetAllOwnersAsync();
        Task<Owner> RegisterOwnerAsync(Owner owner);
        Task<bool> RemoveOwnerAsync(int ownerId);
        Task<List<Owner>> SearchOwnersAsync(string? owner, string? phone);
        Task<bool> UpdateOwnerAsync(Owner owner);

    }

}
