
INSERT INTO Pets (Name, Age, Breed, OwnerId, FoodPreferences, Type, Size, WalkingPreference)
VALUES (@name, @age, @breed, @ownerId, @food, @type, @size, @walking);
SELECT last_insert_rowid();
