
namespace kms.Storage.Impl
{

    using log4net;
    using System.ComponentModel;

    using kms.Models;
    using kms.Util;

    using Microsoft.Data.Sqlite;


    /// <summary>
    /// SQLite implementation of the persisted storage interface.
    /// </summary>
    public class SqliteStorage : IPersistedStorage
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof(SqliteStorage));
        private readonly string _connectionString;

        public SqliteStorage(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            (SqliteConnection connection, SqliteCommand command) = PrepareCommand(GetQuery("initialize_database"));
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        // Owners
        public async Task<List<Owner>> GetAllOwnersAsync()
        {
            return ExecuteCommand(GetQuery("get_owner"), (command) =>
            {
                using SqliteDataReader dr = command.ExecuteReader();
                List<Owner> results = [];
                while (dr.Read())
                {
                    Owner owner = ModelUtils.CreateOwner(dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetInt32(0))
                        ?? throw new Exception("No able to parse query to create OWNER object");
                    results.Add(owner);
                }
                dr.Close();
                return results;
            });
        }

        public async Task<Owner> RegisterOwnerAsync(Owner owner)
        {
            return ExecuteCommand(GetQuery("create_owner"), (command) =>
            {
                command.Parameters.AddWithValue("@name", owner.Name);
                command.Parameters.AddWithValue("@address", owner.Address);
                command.Parameters.AddWithValue("@phone", owner.PhoneNumber);

                owner.Id = Convert.ToInt32(command.ExecuteScalar());
                return owner;
            });
        }

        public async Task<bool> RemoveOwnerAsync(int ownerId)
        {
            return ExecuteCommand(GetQuery("remove_owner"), (command) =>
              {
                  command.Parameters.AddWithValue("@ownerId", ownerId);
                  return command.ExecuteNonQuery() > 0;
              });
        }

        public async Task<List<Owner>> SearchOwnersAsync(string? owner, string? phone)
        {
            return ExecuteCommand(
                GetQuery("get_owner", "(@owner IS NOT NULL AND LOWER(Name) LIKE @owner || '%') OR (@phone IS NOT NULL AND LOWER(PhoneNumber) LIKE @phone || '%')"),
                (command) =>
                {
                    command.Parameters.AddWithValue("@owner", (object?)owner?.ToLower() ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", (object?)phone?.ToLower() ?? DBNull.Value);
                    using SqliteDataReader dr = command.ExecuteReader();
                    List<Owner> results = [];
                    while (dr.Read())
                    {
                        Owner owner = ModelUtils.CreateOwner(dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetInt32(0))
                            ?? throw new Exception("No able to parse query to create OWNER object");
                        results.Add(owner);
                    }
                    dr.Close();
                    return results;
                }
            );
        }

        public async Task<bool> UpdateOwnerAsync(Owner owner)
        {
            return ExecuteCommand(GetQuery("update_owner", "Id = @id"), (command) =>
            {
                command.Parameters.AddWithValue("@id", owner.Id);
                command.Parameters.AddWithValue("@name", owner.Name);
                command.Parameters.AddWithValue("@address", owner.Address);
                command.Parameters.AddWithValue("@phone", owner.PhoneNumber);

                return command.ExecuteNonQuery() > 0;
            });
        }

        // Pets
        public async Task<List<Pet>> GetPetsByOwnerAsync(int ownerId)
        {
            return ExecuteCommand(GetQuery("get_pet", "o.Id = @id"), (command) =>
            {
                command.Parameters.AddWithValue("@id", ownerId);
                return ParsePetQueryResults(command);
            });
        }

        public async Task<Pet> RegisterPetAsync(Pet pet)
        {
            return ExecuteCommand(GetQuery("create_pet"), (command) =>
            {
                command.Parameters.AddWithValue("@name", pet.Name);
                command.Parameters.AddWithValue("@age", pet.Age);
                command.Parameters.AddWithValue("@breed", pet.Breed);
                command.Parameters.AddWithValue("@ownerId", pet.Owner.Id);
                command.Parameters.AddWithValue("@food", pet.FoodPreferences);
                command.Parameters.AddWithValue("@type", $"{pet.PetType}");
                if (pet is Dog dog)
                {
                    command.Parameters.AddWithValue("@size", $"{dog.Size}");
                    command.Parameters.AddWithValue("@walking", $"{dog.WalkingPreference}");
                }
                else
                {
                    command.Parameters.AddWithValue("@size", DBNull.Value);
                    command.Parameters.AddWithValue("@walking", DBNull.Value);
                }
                pet.Id = Convert.ToInt32(command.ExecuteScalar());
                return pet;
            });
        }

        public async Task<bool> RemovePetAsync(int petId)
        {
            return ExecuteCommand(GetQuery("remove_pet"), (command) =>
              {
                  command.Parameters.AddWithValue("@petId", petId);
                  return command.ExecuteNonQuery() > 0;
              });
        }

        public async Task<List<Pet>> SearchPetsAsync(string name, PetType type)
        {
            return ExecuteCommand(GetQuery("get_pet", "LOWER(p.Name) LIKE @name || '%' AND p.Type = @type"), (command) =>
            {
                command.Parameters.AddWithValue("@name", name.ToLower());
                command.Parameters.AddWithValue("@type", $"{type}");
                return ParsePetQueryResults(command);
            });
        }

        public async Task<List<Pet>> SearchPetsByOwnerAsync(string? owner, string? phone)
        {
            return ExecuteCommand(
                GetQuery("get_pet", "(@owner IS NOT NULL AND LOWER(o.Name) LIKE @owner || '%') OR (@phone IS NOT NULL AND LOWER(o.PhoneNumber) LIKE @phone || '%')"),
                (command) =>
                {
                    command.Parameters.AddWithValue("@owner", (object?)owner?.ToLower() ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", (object?)phone?.ToLower() ?? DBNull.Value);
                    return ParsePetQueryResults(command);
                }
            );
        }

        public async Task<bool> UpdatePetAsync(Pet pet)
        {
            return ExecuteCommand(GetQuery("update_pet", "Id = @id"), (command) =>
            {
                command.Parameters.AddWithValue("@id", pet.Id);
                command.Parameters.AddWithValue("@name", pet.Name);
                command.Parameters.AddWithValue("@age", pet.Age);
                command.Parameters.AddWithValue("@breed", pet.Breed);
                command.Parameters.AddWithValue("@ownerId", pet.Owner.Id);
                command.Parameters.AddWithValue("@food", pet.FoodPreferences);
                command.Parameters.AddWithValue("@type", $"{pet.PetType}");
                if (pet is Dog dog)
                {
                    command.Parameters.AddWithValue("@size", $"{dog.Size}");
                    command.Parameters.AddWithValue("@walking", $"{dog.WalkingPreference}");
                }
                else
                {
                    command.Parameters.AddWithValue("@size", DBNull.Value);
                    command.Parameters.AddWithValue("@walking", DBNull.Value);
                }
                return command.ExecuteNonQuery() > 0;
            });
        }

        // Bookings
        public async Task<bool> CancelBookingsAsync(string groupId)
        {
            return ExecuteCommand(GetQuery("cancel_booking", "GroupId = @groupId"), (command) =>
              {
                  command.Parameters.AddWithValue("@groupId", groupId);
                  return command.ExecuteNonQuery() > 0;
              });
        }

        public async Task<List<Booking>> CreateBookingsAsync(List<Booking> bookings)
        {
            List<Booking> results = [];
            foreach (Booking booking in bookings)
            {
                results.Add(CreateBookingAsync(booking));
            }
            return results;
        }

        public async Task<List<BookingGroup>> SearchBookingsScheduledAsync(DateTime startDate, DateTime? endDate = null)
        {
            return ExecuteCommand(
                GetQuery("get_booking_group", "((@end IS NULL AND StartDate = @start) OR (@end IS NOT NULL AND StartDate >= @start AND StartDate <= @end))"),
                (command) =>
                {
                    command.Parameters.AddWithValue("@start", ToTimeStamp(startDate));
                    command.Parameters.AddWithValue("@end", ToTimeStamp(endDate));
                    return ParseBookingGroupQueryResults(command);
                }
            );
        }

        public async Task<List<BookingGroup>> SearchBookingsByOwnerAsync(string? owner, string? phone)
        {
            return ExecuteCommand(
                GetQuery("get_booking_group", "(@owner IS NOT NULL AND LOWER(Owner) LIKE @owner || '%') OR (@phone IS NOT NULL AND LOWER(PhoneNumber) LIKE @phone || '%')"),
                (command) =>
                {
                    command.Parameters.AddWithValue("@owner", (object?)owner ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", (object?)phone ?? DBNull.Value);
                    return ParseBookingGroupQueryResults(command);
                }
            );
        }

        public async Task<List<BookingGroup>> SearchBookingsByPetAsync(string name, PetType type)
        {
            return ExecuteCommand(GetQuery("get_booking_group", "LOWER(Name) LIKE @name || '%' AND Type = @type"), (command) =>
            {
                command.Parameters.AddWithValue("@name", name.ToLower());
                command.Parameters.AddWithValue("@type", $"{type}");
                return ParseBookingGroupQueryResults(command);
            });
        }

        private Booking CreateBookingAsync(Booking booking)
        {
            return ExecuteCommand(GetQuery("create_booking"), (command) =>
            {
                command.Parameters.AddWithValue("@groupId", booking.GroupId);
                command.Parameters.AddWithValue("@petId", booking.Pet.Id);
                command.Parameters.AddWithValue("@kennelId", booking.Kennel.Id);
                command.Parameters.AddWithValue("@date", ToTimeStamp(booking.Date));

                booking.Id = Convert.ToInt32(command.ExecuteScalar());
                return booking;
            });
        }

        // Kennels
        public async Task<Kennel> AddKennelAsync(Kennel kennel)
        {
            return ExecuteCommand(GetQuery("create_kennel"), (command) =>
            {
                command.Parameters.AddWithValue("@name", kennel.Name);
                command.Parameters.AddWithValue("@size", $"{kennel.Size}");
                command.Parameters.AddWithValue("@suitableFor", $"{kennel.SuitableFor}");
                command.Parameters.AddWithValue("@outOfService", kennel.IsOutOfService);
                command.Parameters.AddWithValue("@serviceComment", (object?)kennel.OutOfServiceComment ?? DBNull.Value);

                kennel.Id = Convert.ToInt32(command.ExecuteScalar());
                return kennel;
            });
        }

        public async Task<List<Kennel>> GetKennelsAsync()
        {
            return ExecuteCommand(GetQuery("get_kennel"), (command) =>
            {
                using SqliteDataReader dr = command.ExecuteReader();
                List<Kennel> results = [];
                while (dr.Read())
                {
                    Kennel kennel = ModelUtils.CreateKennel(dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetBoolean(4),
                                GetStringOrNull(dr, 5), dr.GetInt32(0))
                        ?? throw new Exception("No able to parse query to create KENNEL object");
                    results.Add(kennel);
                }
                dr.Close();
                return results;
            });
        }

        public async Task<List<Kennel>> FindAvailableKennelsForDateRange(Pet pet, DateTime startDate, DateTime endDate)
        {
            return ExecuteCommand(GetQuery("find_kennel", "BookingCount = 0"), (command) =>
            {
                command.Parameters.AddWithValue("@size", pet.PetType == PetType.CAT ? DBNull.Value : $"{((Dog)pet).Size}");
                command.Parameters.AddWithValue("@type", $"{pet.PetType}");
                command.Parameters.AddWithValue("@start", ToTimeStamp(startDate));
                command.Parameters.AddWithValue("@end", ToTimeStamp(endDate));

                using SqliteDataReader dr = command.ExecuteReader();
                List<Kennel> results = [];
                while (dr.Read())
                {
                    Kennel kennel = ModelUtils.CreateKennel(dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetBoolean(4),
                                GetStringOrNull(dr, 5), dr.GetInt32(0))
                        ?? throw new Exception("No able to parse query to create KENNEL object");
                    results.Add(kennel);
                }
                dr.Close();
                return results;
            });
        }

        public async Task<bool> RemoveKennelAsync(int kennelId)
        {
            // fail if there are future scheduled bookings
            if (ExecuteCommand(
                GetQuery("get_booking", "k.Id = @kennelId AND b.Date >= @today"),
                (command) =>
                {
                    command.Parameters.AddWithValue("@kennelId", kennelId);
                    command.Parameters.AddWithValue("@today", ToTimeStamp(DateTime.Today));
                    using SqliteDataReader dr = command.ExecuteReader();
                    return dr.HasRows;
                }
            )) return false;
            // remove specified kennel
            return ExecuteCommand(GetQuery("remove_kennel"), (command) =>
            {
                command.Parameters.AddWithValue("@kennelId", kennelId);
                return command.ExecuteNonQuery() > 0;
            });
        }

        public async Task<bool> UpdateKennelAsync(Kennel kennel)
        {
            return ExecuteCommand(GetQuery("update_kennel", "Id = @id"), (command) =>
            {
                command.Parameters.AddWithValue("@id", kennel.Id);
                command.Parameters.AddWithValue("@name", kennel.Name);
                command.Parameters.AddWithValue("@size", $"{kennel.Size}");
                command.Parameters.AddWithValue("@suitableFor", $"{kennel.SuitableFor}");
                command.Parameters.AddWithValue("@outOfService", kennel.IsOutOfService);
                command.Parameters.AddWithValue("@serviceComment", (object?)kennel.OutOfServiceComment ?? DBNull.Value);

                return command.ExecuteNonQuery() > 0;
            });
        }

        // shared\utility functions
        private List<BookingGroup> ParseBookingGroupQueryResults(SqliteCommand command)
        {
            List<BookingGroup> results = [];
            using SqliteDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                Owner owner = ModelUtils.CreateOwner(dr.GetString(18), dr.GetString(19), dr.GetString(20), dr.GetInt32(17))
                    ?? throw new Exception("No able to parse query to create OWNER object");
                Pet pet = ModelUtils.CreatePet(owner, dr.GetString(10), dr.GetString(11), dr.GetInt32(12), dr.GetString(13),
                            dr.GetString(14), GetStringOrNull(dr, 15), GetStringOrNull(dr, 16), dr.GetInt32(9))
                    ?? throw new Exception("No able to parse query to create PET object");
                Kennel kennel = ModelUtils.CreateKennel(dr.GetString(4), dr.GetString(5), dr.GetString(6), dr.GetBoolean(7),
                            GetStringOrNull(dr, 8), dr.GetInt32(3))
                    ?? throw new Exception("No able to parse query to create KENNEL object");
                results.Add(ModelUtils.CreateBookingGroup(pet, kennel, FromTimeStamp(dr.GetInt64(1)), FromTimeStamp(dr.GetInt64(2)), dr.GetString(0)));
            }
            dr.Close();
            return results;
        }

        private List<Pet> ParsePetQueryResults(SqliteCommand command)
        {
            List<Pet> results = [];
            using SqliteDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                Owner owner = ModelUtils.CreateOwner(dr.GetString(9), dr.GetString(10), dr.GetString(11), dr.GetInt32(8))
                    ?? throw new Exception("No able to parse query to create OWNER object");
                Pet pet = ModelUtils.CreatePet(owner, dr.GetString(1), dr.GetString(2), dr.GetInt32(3), dr.GetString(4),
                            dr.GetString(5), GetStringOrNull(dr, 6), GetStringOrNull(dr, 7), dr.GetInt32(0))
                    ?? throw new Exception("No able to parse query to create PET object");
                results.Add(pet);
            }
            dr.Close();
            return results;
        }

        private T ExecuteCommand<T>(string commandText, Func<SqliteCommand, T> func)
        {
            (SqliteConnection connection, SqliteCommand command) = PrepareCommand(commandText);
            connection.Open();
            T result = func(command);
            connection.Close();
            return result;
        }

        private (SqliteConnection, SqliteCommand) PrepareCommand(string commandText)
        {
            using SqliteConnection connection = new(_connectionString);
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            return (connection, command);
        }

        private string GetQuery(string queryRef, string? where = null) =>
            ResourceUtils.GetSqlResource(StorageType.SQLITE, queryRef, where);

        private string? GetStringOrNull(SqliteDataReader dr, int col) =>
            dr.IsDBNull(col) ? null : dr.GetString(col);

        private DateTime FromTimeStamp(long timeStamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).DateTime;
        }

        private object ToTimeStamp(DateTime? dateTime)
        {
            if (dateTime == null) return DBNull.Value;
            return new DateTimeOffset(((DateTime)dateTime).ToUniversalTime()).ToUnixTimeMilliseconds();
        }

    }
}
