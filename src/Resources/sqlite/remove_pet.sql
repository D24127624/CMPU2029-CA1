
DELETE FROM Bookings WHERE PetId = @petId;
DELETE FROM Pets WHERE Id = @petId;
