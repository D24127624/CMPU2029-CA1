
namespace kms.test.Models
{

    using Xunit;
    using kms.Models;
    using kms.Util;
    using System.Text.RegularExpressions;

    public class BookingTests
    {

        [Fact]
        public void Booking_ShouldSetPropertiesCorrectly()
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
            Kennel kennel = new()
            {
                Id = 3,
                Name = "Test Unit 001",
                Size = Size.MEDIUM,
                SuitableFor = PetType.DOG,
                IsOutOfService = false
            };
            DateTime date = DateTime.Today.AddDays(5);

            // execute test
            string groupId = StringUtils.RandomString(10);
            Booking booking = new()
            {
                Id = 4,
                GroupId = groupId,
                Pet = dog,
                Kennel = kennel,
                Date = date
            };

            // verify results
            Assert.Equal(4, booking.Id);
            Assert.Equal(groupId, booking.GroupId);
            Assert.Equal(dog, booking.Pet);
            Assert.Equal(kennel, booking.Kennel);
            Assert.Equal(date, booking.Date);
        }

        [Fact]
        public void BookingGroup_ShouldSetPropertiesCorrectly()
        {
            // prepare objects
            Owner owner = new()
            {
                Id = 5,
                Name = "Sally Smith",
                PhoneNumber = "091 357-8264"
            };
            Cat cat = new()
            {
                Id = 6,
                Name = "Chariman Meow",
                Age = 2,
                Breed = "Common House Cat",
                Owner = owner
            };
            Kennel kennel = new()
            {
                Id = 7,
                Name = "Test Unit 011",
                Size = Size.SMALL,
                SuitableFor = PetType.CAT,
                IsOutOfService = false
            };
            DateTime startDate = DateTime.Today.AddDays(5);
            DateTime endDate = DateTime.Today.AddDays(10);

            // execute test
            string groupId = StringUtils.RandomString(10);
            BookingGroup bookingGroup = new()
            {
                GroupId = groupId,
                Pet = cat,
                Kennel = kennel,
                StartDate = startDate,
                EndDate = endDate
            };

            // verify results
            Assert.Equal(groupId, bookingGroup.GroupId);
            Assert.Equal(cat, bookingGroup.Pet);
            Assert.Equal(kennel, bookingGroup.Kennel);
            Assert.Equal(startDate, bookingGroup.StartDate);
            Assert.Equal(endDate, bookingGroup.EndDate);
        }

    }

}
