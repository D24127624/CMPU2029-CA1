
DELETE FROM Bookings WHERE PetId IN (SELECT Id FROM Pets WHERE OwnerId = @ownerId);
DELETE FROM Pets WHERE OwnerId = @ownerId;
DELETE FROM Owners WHERE Id = @ownerId;
