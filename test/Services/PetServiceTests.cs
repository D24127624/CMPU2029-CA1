
namespace kms.test.Services
{

    using Xunit;
    using Moq;
    using kms.Services;
    using kms.Services.Impl;
    using kms.Storage;
    using kms.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PetServiceTests
    {

        private readonly Mock<IPersistedStorage> _mockStorage;
        private readonly IPetService _petService;

        public PetServiceTests()
        {
            _mockStorage = new Mock<IPersistedStorage>();
            _petService = new PetService(_mockStorage.Object);
        }

        [Fact]
        public async Task RegisterPetAsync_ShouldReturnPet_WhenPetIsRegistered()
        {
            // prepare objects
            Owner owner = new()
            {
                Id = 1,
                Name = "Joe Jones",
                PhoneNumber = "086 282-0173"
            };
            Dog dog = new()
            {
                Id = 2,
                Name = "Max",
                Age = 2,
                Breed = "Jack-Russel",
                Owner = owner,
                Size = Size.MEDIUM,
                WalkingPreference = WalkingPreference.SOCIAL
            };
            _mockStorage.Setup(s => s.RegisterPetAsync(It.IsAny<Pet>())).ReturnsAsync(dog);

            // execute test
            Pet result = await _petService.RegisterPetAsync(dog);

            // verify results
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
        }

        [Fact]
        public async Task GetPetsByOwnerAsync_ShouldReturnPetsForOwner()
        {
            // prepare objects
            Owner owner = new()
            {
                Id = 5,
                Name = "Sally Smith",
                PhoneNumber = "091 357-8264"
            };
            List<Pet> pets = [
                new Cat()
                {
                    Id = 3,
                    Name = "Lucy",
                    Age = 3,
                    Breed = "Siamese",
                    Owner = owner
                },
                new Dog {
                    Id = 4,
                    Name = "Buddy",
                    Age = 5,
                    Breed = "Golden Retriever",
                    Owner = owner,
                    Size = Size.LARGE,
                    WalkingPreference = WalkingPreference.SOCIAL
                }
            ];
            _mockStorage.Setup(s => s.GetPetsByOwnerAsync(1)).ReturnsAsync(pets);

            // execute test
            List<Pet> result = await _petService.GetPetsByOwnerAsync(1);

            // verify results
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

    }

}
