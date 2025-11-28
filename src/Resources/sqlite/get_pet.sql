
SELECT p.Id, p.Type, p.Name, p.Age, p.Breed, p.FoodPreferences, p.Size, p.WalkingPreference,
       o.Id AS OwnerId, o.Name AS OwnerName, o.Address, o.PhoneNumber
  FROM Pets p
  JOIN Owners o
    ON o.Id = p.OwnerId
