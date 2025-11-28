
namespace kms.test.Storage
{

    using Xunit;
    using kms.Storage.Impl;
    using kms.Models;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
    using kms.Util;

    public class MemoryStorageTests
    {

        private readonly MemoryStorage _storage;

        public MemoryStorageTests()
        {
            _storage = new MemoryStorage();
        }

        [Fact]
        public async Task RegisterOwnerAsync_ShouldAddOwner()
        {
            // prepare objects
            Owner owner = new()
            {
                Name = "Paula Potter",
                PhoneNumber = "073 836-1054"
            };

            // execute test
            Owner result = await _storage.RegisterOwnerAsync(owner);

            // verify results
            Assert.NotEqual(0, result.Id);
            List<Owner> allOwners = await _storage.GetAllOwnersAsync();
            Assert.Contains(result, allOwners);
        }

        [Fact]
        public async Task RegisterPetAsync_ShouldAddPet()
        {
            // prepare objects
            Owner owner = await _storage.RegisterOwnerAsync(new Owner
            {
                Name = "Joe Jones",
                PhoneNumber = "086 282-0173"
            });
            Dog dog = new()
            {
                Name = "Max",
                Age = 2,
                Breed = "Jack-Russel",
                Owner = owner,
                Size = Size.MEDIUM,
                WalkingPreference = WalkingPreference.SOCIAL
            };

            // execute test
            Pet result = await _storage.RegisterPetAsync(dog);

            // verify results
            Assert.NotEqual(0, result.Id);
            List<Pet> pets = await _storage.GetPetsByOwnerAsync(owner.Id);
            Assert.Contains(result, pets);
        }

        [Fact]
        public async Task AddKennelAsync_ShouldAddKennel()
        {
            // prepare objects
            Kennel kennel = new()
            {
                Name = "Test Unit 001",
                Size = Size.MEDIUM,
                SuitableFor = PetType.DOG,
                IsOutOfService = false
            };

            // execute test
            Kennel result = await _storage.AddKennelAsync(kennel);

            // verify results
            Assert.NotEqual(0, result.Id);
            List<Kennel> kennels = await _storage.GetKennelsAsync();
            Assert.Contains(result, kennels);
        }

        [Fact]
        public async Task CreateBookingsAsync_ShouldAddBookings()
        {
            // prepare objects
            Owner owner = await _storage.RegisterOwnerAsync(
                new Owner
                {
                    Name = "Sally Smith",
                    PhoneNumber = "091 357-8264"
                });
            Pet pet = await _storage.RegisterPetAsync(new Cat()
            {
                Name = "Lucy",
                Age = 3,
                Breed = "Siamese",
                Owner = owner
            });
            Kennel kennel = await _storage.AddKennelAsync(new Kennel
            {
                Name = "Test Unit 002",
                Size = Size.SMALL,
                SuitableFor = PetType.CAT,
                IsOutOfService = false
            });
            string groupId = StringUtils.RandomString(10);
            List<Booking> bookings = [
                new Booking {
                    GroupId = groupId,
                    Pet = pet,
                    Kennel = kennel,
                    Date = DateTime.Today.AddDays(1)
                },
                new Booking {
                    GroupId = groupId,
                    Pet = pet,
                    Kennel = kennel,
                    Date = DateTime.Today.AddDays(2)
                }
            ];

            // execute test
            List<Booking> result = await _storage.CreateBookingsAsync(bookings);

            // verify results
            Assert.Equal(2, result.Count);
            List<BookingGroup> searchResult = await _storage.SearchBookingsByPetAsync(pet.Name, pet.PetType);
            Assert.Single(searchResult);
        }

    }

}
