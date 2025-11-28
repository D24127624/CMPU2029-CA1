
namespace kms.Models
{

    using System.Text.Json.Serialization;

    /// <summary>
    /// Enum for dog or kennel size.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Size { SMALL, MEDIUM, LARGE }

    /// <summary>
    /// Enum for dog walking preferences.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WalkingPreference { SOLO, SOCIAL, NO }

    /// <summary>
    /// Enum of supported storage-types
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StorageType { MEMORY, SQLITE }

    /// <summary>
    /// Enum for pet type.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PetType { DOG, CAT }

}
