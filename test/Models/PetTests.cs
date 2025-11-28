
namespace kms.Test.Models
{

    using Xunit;
    using kms.Models;

    public class PetTests
    {

        private readonly Owner _owner;

        public PetTests()
        {
            _owner = new Owner
            {
                Id = 1,
                Name = "Paula Potter",
                PhoneNumber = "073 836-1054"
            };
        }

        [Fact]
        public void Cat_Creation_Should_Set_Properties_Correctly()
        {
            // prepare objects / execute test
            Cat cat = new()
            {
                Id = 1,
                Name = "Whiskers",
                Age = 5,
                Breed = "Siamese",
                Owner = _owner,
                FoodPreferences = "Tuna"
            };

            // verify results
            Assert.Equal(1, cat.Id);
            Assert.Equal("Whiskers", cat.Name);
            Assert.Equal(5, cat.Age);
            Assert.Equal("Siamese", cat.Breed);
            Assert.Equal(_owner, cat.Owner);
            Assert.Equal("Tuna", cat.FoodPreferences);
            Assert.Equal(PetType.CAT, cat.PetType);
        }

        [Fact]
        public void Dog_Creation_Should_Set_Properties_Correctly()
        {
            // prepare objects / execute test
            Dog dog = new()
            {
                Id = 2,
                Name = "Buddy",
                Age = 3,
                Breed = "Golden Retriever",
                Owner = _owner,
                FoodPreferences = "Pedigree Chunks",
                Size = Size.LARGE,
                WalkingPreference = WalkingPreference.SOCIAL
            };

            // verify results
            Assert.Equal(2, dog.Id);
            Assert.Equal("Buddy", dog.Name);
            Assert.Equal(3, dog.Age);
            Assert.Equal("Golden Retriever", dog.Breed);
            Assert.Equal(_owner, dog.Owner);
            Assert.Equal("Pedigree Chunks", dog.FoodPreferences);
            Assert.Equal(PetType.DOG, dog.PetType);
            Assert.Equal(Size.LARGE, dog.Size);
            Assert.Equal(WalkingPreference.SOCIAL, dog.WalkingPreference);
        }

    }

}
