
SELECT b.Id, b.GroupId, b.Date,
       k.Id As KennelId, k.Name AS Kennel, k.Size AS KennelSize, k.SuitableFor, k.IsOutOfService, k.OutOfServiceComment,
       p.Id AS PetId, p.Type, p.Name, p.Age, p.Breed, p.FoodPreferences, p.Size, p.WalkingPreference,
       o.Id AS OwnerId, o.Name AS Owner, o.Address, o.PhoneNumber
  FROM Bookings b
  JOIN Kennels k
    ON k.Id = b.KennelId
  JOIN Pets p
    ON p.Id = b.PetId
  JOIN Owners o
    ON o.Id = p.OwnerId
