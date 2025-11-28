
namespace kms.Services.Impl
{

    using System.Text.Json;

    using kms.Models;
    using kms.Storage;
    using kms.Storage.Impl;
    using kms.Util;

    /// <summary>
    /// Service used to load the application configuration used by the Kennel Management System.
    /// </summary>
    public class ConfigurationService
    {

        private readonly Configuration _configuration;
        private readonly string _configFilePath;

        private IBookingService? _bookingService;
        private IKennelService? _kennelService;
        private IOwnerService? _ownerService;
        private IPetService? _petService;
        private IPersistedStorage? _storage;

        public ConfigurationService(string? configPath)
        {
            _configFilePath = configPath ?? $"{Environment.CurrentDirectory}/{ResourceUtils.CONFIG_FILE}";
            // create new configuration file does not exists
            if (!File.Exists(_configFilePath))
            {
                // ensure configuration file directory exists
                string? directory = Directory.GetParent(_configFilePath)?.FullName;
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(_configFilePath, JsonSerializer.Serialize(ResourceUtils.CONFIG_TEMPLATE));
            }
            _configuration = JsonSerializer.Deserialize<Configuration>(new StreamReader(_configFilePath).ReadToEnd())
                ?? throw new InvalidOperationException($"Failed to deserialize configuration located at {_configFilePath}!");
        }

        public IBookingService GetBookingService()
        {
            if (_bookingService == null)
            {
                _bookingService = new BookingService(GetStorage());
            }
            return _bookingService;
        }

        public IKennelService GetKennelService()
        {
            if (_kennelService == null)
            {
                _kennelService = new KennelService(GetStorage());
            }
            return _kennelService;
        }

        public IOwnerService GetOwnerService()
        {
            if (_ownerService == null)
            {
                _ownerService = new OwnerService(GetStorage());
            }
            return _ownerService;
        }

        public IPetService GetPetService()
        {
            if (_petService == null)
            {
                _petService = new PetService(GetStorage());
            }
            return _petService;
        }

        public IPersistedStorage GetStorage()
        {
            if (_storage == null)
            {
                switch (_configuration.StorageType)
                {
                    case StorageType.MEMORY:
                        {
                            _storage = new MemoryStorage();
                            break;
                        }
                    case StorageType.SQLITE:
                        {
                            _storage = new SqliteStorage(_configuration.ConnectionString);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Error: StorageType ({_configuration.StorageType}) is not supported!");
                        }
                }
            }
            return _storage;
        }

    }

}