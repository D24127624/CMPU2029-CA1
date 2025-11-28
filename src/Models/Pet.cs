
namespace kms.Models
{

    /// <summary>
    /// Base class for a pet.
    /// </summary>
    public abstract class Pet
    {

        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Age { get; set; }
        public required string Breed { get; set; }
        public required Owner Owner { get; set; }
        public string? FoodPreferences { get; set; }
        public abstract PetType PetType { get; }

    }

    /// <summary>
    /// Represents a cat.
    /// </summary>
    public class Cat : Pet
    {

        public override PetType PetType => PetType.CAT;
        // Additional cat-specific properties can be added here

    }

    /// <summary>
    /// Represents a dog.
    /// </summary>
    public class Dog : Pet
    {

        public override PetType PetType => PetType.DOG;
        public required Size Size { get; set; }
        public required WalkingPreference WalkingPreference { get; set; }

    }

}
