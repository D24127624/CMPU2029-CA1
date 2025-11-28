
namespace kms.Util
{

    using System.ComponentModel;

    using log4net;

    using kms.Models;

    /// <summary>
    /// Model object utilities.
    /// </summary>
    public static class ModelUtils
    {

        private static ILog _log => LogManager.GetLogger(typeof(ModelUtils));

        public static Booking CreateBooking(string groupId, Pet pet, Kennel kennel, DateTime date, int? id = null)
        {
            _log.Info($"Create booking: groupId='{groupId}' / pet='{pet.Name}' / kennel='{kennel.Name}' / date='{date}' / id={id}");
            Booking booking = new()
            {
                GroupId = groupId,
                Pet = pet,
                Kennel = kennel,
                Date = date
            };
            if (id != null) booking.Id = (int)id;
            return booking;
        }

        public static BookingGroup CreateBookingGroup(Pet pet, Kennel kennel, DateTime startDate, DateTime endDate, string? groupId = null)
        {
            groupId ??= StringUtils.RandomString(10);
            _log.Info($"Create booking=group: groupId='{groupId}' / pet='{pet.Name}' / kennel='{kennel.Name}' /  start='{startDate}' / end='{endDate}'");
            return new BookingGroup
            {
                GroupId = groupId,
                Pet = pet,
                Kennel = kennel,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        public static Kennel? CreateKennel(string name, string size, string suitableFor, bool isOutOfService, string? outOfServiceComment, int? id = null)
        {
            _log.Info($"Create kennel: name='{name}' / size='{size}' / suitableFor='{suitableFor}' / isOutOfService='{isOutOfService}' / outOfServiceComment='{outOfServiceComment}' / id={id}");
            // validate inputs
            if (StringUtils.AnyNullOrWhiteSpace([name, size, suitableFor]))
            {
                _log.Error($"Failed to create kennel object, missing required values!");
                return null;
            }
            Kennel kennel = new()
            {
                Name = name,
                Size = EnumUtils.FromString<Size>(size),
                SuitableFor = EnumUtils.FromString<PetType>(suitableFor),
                IsOutOfService = isOutOfService,
                OutOfServiceComment = StringUtils.ValueOrNull(outOfServiceComment)
            };
            if (id != null) kennel.Id = (int)id;
            return kennel;
        }

        public static Owner? CreateOwner(string name, string address, string phone, int? id = null)
        {
            _log.Info($"Create owner: name='{name}' / address='{address}' / phone='{phone}' / id={id}");
            // validate inputs
            if (StringUtils.AnyNullOrWhiteSpace([name, address, phone]))
            {
                _log.Error($"Failed to create kennel object, missing required values!");
                return null;
            }
            Owner owner = new()
            {
                Name = name,
                Address = address,
                PhoneNumber = phone
            };
            if (id != null) owner.Id = (int)id;
            return owner;
        }

        public static Pet? CreatePet(Owner owner, string type, string name, int age, string breed, string foodPref, string? size = null, string? walkingPref = null, int? id = null)
        {
            _log.Info($"Create pet: type='{type}' / name='{name}' / age='{age}' / breed='{breed}' / foodPref='{foodPref}' / size='{size}' / walkingPref='{walkingPref}' / id={id}");
            // validate inputs
            if (StringUtils.AnyNullOrWhiteSpace([type, name, breed, foodPref]))
            {
                _log.Error($"Failed to create pet object, missing required values!");
                return null;
            }
            PetType petType = EnumUtils.FromString<PetType>(type);
            if (petType == PetType.DOG && StringUtils.AnyNullOrWhiteSpace([size, walkingPref]))
            {
                _log.Error($"Failed to create dog object, missing required values!");
                return null;
            }
            Pet pet = petType switch
            {
                PetType.CAT => new Cat
                {
                    Name = name,
                    Age = age,
                    Breed = breed,
                    Owner = owner,
                    FoodPreferences = foodPref
                },
                PetType.DOG => new Dog
                {
                    Name = name,
                    Age = age,
                    Breed = breed,
                    Owner = owner,
                    FoodPreferences = foodPref,
                    Size = EnumUtils.FromString<Size>(size),
                    WalkingPreference = EnumUtils.FromString<WalkingPreference>(walkingPref)
                },
                _ => throw new InvalidEnumArgumentException($"No supported handler for pet type {type}!"),
            };
            if (id != null) pet.Id = (int)id;
            return pet;
        }

    }

}
