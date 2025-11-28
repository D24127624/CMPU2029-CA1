
namespace kms.Models
{

    /// <summary>
    /// Represents available configuration options for the Kennel Management System.
    /// </summary>
    public class Configuration
    {

        public StorageType StorageType { get; set; }
        public required string ConnectionString { get; set; }

    }

}
