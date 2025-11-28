
namespace kms.Services
{

    using kms.Models;

    /// <summary>
    /// Interface for kennel management operations.
    /// </summary>
    public interface IKennelService
    {

        Task<Kennel> AddKennelAsync(Kennel kennel);
        Task<List<Kennel>> GetKennelsAsync();
        Task<List<Kennel>> FindAvailableKennelsForDateRange(Pet pet, DateTime startDate, DateTime endDate);
        Task<bool> RemoveKennelAsync(int kennelId);
        Task<bool> UpdateKennelAsync(Kennel kennel);

    }

}
