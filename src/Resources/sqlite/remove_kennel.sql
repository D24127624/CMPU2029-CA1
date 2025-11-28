
DELETE FROM Bookings WHERE KennelId = @kennelId;
DELETE FROM Kennels WHERE Id = @kennelId;
