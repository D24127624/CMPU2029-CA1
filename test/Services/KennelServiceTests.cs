
namespace kms.test.Services
{

    using Xunit;
    using Moq;
    using kms.Services.Impl;
    using kms.Storage;
    using kms.Models;

    public class KennelServiceTests
    {

        private readonly Mock<IPersistedStorage> _mockStorage;
        private readonly KennelService _kennelService;

        public KennelServiceTests()
        {
            _mockStorage = new Mock<IPersistedStorage>();
            _kennelService = new KennelService(_mockStorage.Object);
        }

        [Fact]
        public async Task AddKennelAsync_ShouldReturnKennel_WhenKennelIsAdded()
        {
            // prepare objects
            Kennel kennel = new()
            {
                Id = 1,
                Name = "Test Unit 001",
                Size = Size.MEDIUM,
                SuitableFor = PetType.DOG,
                IsOutOfService = false
            };
            _mockStorage.Setup(s => s.AddKennelAsync(It.IsAny<Kennel>())).ReturnsAsync(kennel);

            // execute test
            Kennel result = await _kennelService.AddKennelAsync(kennel);

            // verify results
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetKennelsAsync_ShouldReturnAllKennels()
        {
            // prepare objects
            List<Kennel> kennels = [
                new Kennel {
                    Id = 1,
                    Name = "Test Unit 001",
                    Size = Size.MEDIUM,
                    SuitableFor = PetType.DOG,
                    IsOutOfService = false
                },
                new Kennel {
                    Id = 2,
                    Name = "Test Unit 002",
                    Size = Size.LARGE,
                    SuitableFor = PetType.DOG,
                    IsOutOfService = true
                }
            ];
            _mockStorage.Setup(s => s.GetKennelsAsync()).ReturnsAsync(kennels);

            // execute test
            List<Kennel> result = await _kennelService.GetKennelsAsync();

            // verify results
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

    }

}
