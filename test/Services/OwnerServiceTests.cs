
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

    public class OwnerServiceTests
    {
        private readonly Mock<IPersistedStorage> _mockStorage;
        private readonly IOwnerService _ownerService;

        public OwnerServiceTests()
        {
            _mockStorage = new Mock<IPersistedStorage>();
            _ownerService = new OwnerService(_mockStorage.Object);
        }

        [Fact]
        public async Task RegisterOwnerAsync_ShouldReturnOwner_WhenOwnerIsRegistered()
        {
            // prepare objects
            Owner owner = new()
            {
                Id = 1,
                Name = "Joe Jones",
                PhoneNumber = "086 282-0173"
            };
            _mockStorage.Setup(s => s.RegisterOwnerAsync(It.IsAny<Owner>())).ReturnsAsync(owner);

            // execute test
            Owner result = await _ownerService.RegisterOwnerAsync(owner);

            // verify results
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAllOwnersAsync_ShouldReturnAllOwners()
        {
            // prepare objects
            List<Owner> owners = [
                new Owner {
                    Id = 1,
                    Name = "Joe Jones",
                    PhoneNumber = "086 282-0173"
                },
                new Owner {
                    Id = 5,
                    Name = "Sally Smith",
                    PhoneNumber = "091 357-8264"
}
            ];
            _mockStorage.Setup(s => s.GetAllOwnersAsync()).ReturnsAsync(owners);

            // execute test
            List<Owner> result = await _ownerService.GetAllOwnersAsync();

            // verify results
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

    }

}
